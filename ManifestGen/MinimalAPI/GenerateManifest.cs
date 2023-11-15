using ManifestGen.Data;
using ManifestGen.State;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace ManifestGen.MinimalAPI
{
    public static class GenerateManifest
    {
        public static RouteGroupBuilder MapGenerateApi(this RouteGroupBuilder group)
        {

            group.MapPost("/", async Task<Results<FileContentHttpResult, BadRequest<string>>> ([FromForm] FileUpload upload, HttpContext context, ApplicationDbContext dbContext) =>
            {
                try
                {
                    AppState json = JsonSerializer.Deserialize<AppState>(upload.JsonContent)!;
                    var generate = new ManifestGenerationLogic(json, upload.Image!);
                    context.Items[nameof(ManifestGenerationLogic)] = generate;
                    await generate.ExecuteProcess();
                    var zipFile = System.IO.File.ReadAllBytes(generate.ZipPath);
                    if (context.User is not null)
                    {
                        var userEmail = context.User.FindFirstValue(ClaimTypes.Email);
                        var user = dbContext.Users.FirstOrDefault(u => u.Email == userEmail);

                        var userFiles = dbContext.UserFiles
                            .Where(uf => uf.ApplicationUserId == user.Id)
                            .OrderBy(uf => uf.CreatedAt)
                            .ToList();
                        if (userFiles.Count >= 10)
                        {
                            var earliestFile = userFiles.First();
                            dbContext.UserFiles.Remove(earliestFile);
                        }
                        var userFile = new UserFile
                        {
                            UserFileId = Guid.NewGuid().ToString(),
                            FileName = $"manifest-file {DateTime.Now}",
                            ManifestData = zipFile,
                            ApplicationUserId = user.Id
                        };

                        dbContext.UserFiles.Add(userFile);
                        dbContext.SaveChanges();

                    }
                    return TypedResults.File(zipFile);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException);
                    return TypedResults.BadRequest("Bad Request");
                }
            }).RequireAuthorization().DisableAntiforgery()
            .AddEndpointFilter(async (context, next) =>
            {
                var result = await next(context);
                var generate = (ManifestGenerationLogic)context.HttpContext.Items[nameof(ManifestGenerationLogic)]!;
                generate.DeleteGeneratedFolder();
                return result;
            });

            group.MapGet("/list", async Task<IResult> (HttpContext context, ApplicationDbContext dbContext) =>
            {
                var userEmail = context.User?.FindFirstValue(ClaimTypes.Email);
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                var userFiles = await dbContext.UserFiles
               .Where(uf => uf.ApplicationUserId == user.Id)
               .Select(uf => new FileDetails
               {
                   FileId = uf.UserFileId,
                   FileName = uf.FileName
               })
               .ToListAsync();
                return TypedResults.Ok(userFiles);
            }).RequireAuthorization();

            group.MapGet("/{id}", async Task<Results<FileContentHttpResult, NotFound>> (HttpContext context, string id, ApplicationDbContext dbContext) =>
            {
                var userFile = await dbContext.UserFiles.FirstOrDefaultAsync(uf => uf.UserFileId == id);
                if (userFile is not null)
                {
                    return TypedResults.File(userFile.ManifestData!);
                }
                return TypedResults.NotFound();
            }).RequireAuthorization();

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (HttpContext context, string id, ApplicationDbContext dbContext) =>
            {
                var userFile = await dbContext.UserFiles
       .FirstOrDefaultAsync(uf => uf.UserFileId == id);

                if (userFile is not null)
                {
                    dbContext.UserFiles.Remove(userFile);
                    await dbContext.SaveChangesAsync();
                    return TypedResults.Ok();
                }

                return TypedResults.NotFound();

            }).RequireAuthorization();

            return group;
        }
    }
    public class FileUpload
    {
        public string JsonContent { get; set; } = default!;
        public IFormFile? Image { get; set; }
    }
    public class FileDetails
    {
        public string? FileId { get; set; }
        public string? FileName { get; set; }
    }
}
