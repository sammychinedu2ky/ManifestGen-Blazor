using ManifestGen.Data;
using ManifestGen.State;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
                    var zipFile = File.ReadAllBytes(generate.ZipPath);
                    if (context.User is not null)
                    {
                        var userFile = new UserFile
                        {
                            UserFileId = Guid.NewGuid().ToString(),
                            FileName = $"manifest-file-{DateTime.Now}",
                            ManifestData = zipFile,
                            ApplicationUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                        };
                        dbContext.UserFiles.Add(userFile);
                        dbContext.SaveChanges();

                    }
                    return TypedResults.File(zipFile);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return TypedResults.BadRequest("Bad Request");
                }
            }).DisableAntiforgery()
            .AddEndpointFilter(async (context, next) =>
            {
                var result = await next(context);
                var generate = (ManifestGenerationLogic)context.HttpContext.Items[nameof(ManifestGenerationLogic)]!;
                generate.DeleteGeneratedFolder();
                return result;
            });


            return group;
        }
    }
    public class FileUpload
    {
        public string JsonContent { get; set; } = default!;
        public IFormFile? Image { get; set; }
    }
}
