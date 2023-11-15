using ManifestGen.State;
using System.IO.Compression;
using System.Text.Json;
namespace ManifestGen.MinimalAPI
{
    public class ManifestGenerationLogic
    {
        public string FolderId { get; }

        private IFormFile UploadedImage;
        private AppState manifestContent;

        public ManifestGenerationLogic(AppState jsonContent, IFormFile file)
        {

            FolderId = Guid.NewGuid().ToString();
            UploadedImage = file;
            manifestContent = jsonContent;
            Directory.CreateDirectory(GenerationFolder);
            RootPath = Path.Combine(GenerationFolder, FolderId);
            ContentPath = Path.Combine(RootPath, "content");
            Directory.CreateDirectory(ContentPath);
            RootIconFile = Path.Combine(RootPath, "icon.png");
            IconPath = Path.Combine(ContentPath, "images", "icons");
            ZipPath = Path.Combine(RootPath, "app-image.zip");
            Directory.CreateDirectory(IconPath);

        }
        public async Task ExecuteProcess()
        {
            await CreateFolders();
            CreateImages();
            WriteManifest();
            ZipContent();
        }


        public static string GenerationFolder = nameof(GenerationFolder);
        public string IconPath { get; }
        public string ZipPath { get; }
        public string RootPath { get; }
        public string ContentPath { get; }
        public string RootIconFile { get; }
        public async Task CreateFolders()
        {
            using var rootIconStream = File.OpenWrite(RootIconFile);
            await UploadedImage.CopyToAsync(rootIconStream);

        }
        public void CreateImages()
        {
            using (var input = Image.Load(RootIconFile))
            {
                int[] sizes = { 72, 96, 128, 144, 152, 192, 384, 512 };
                manifestContent.Icons = new();
                foreach (int width in sizes)
                {
                    input.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(width, 0), Mode = ResizeMode.Max }));
                    string outputPath = Path.Combine(IconPath, $"icon-{width}x{width}.png");
                    input.Save(outputPath);
                    var dic = new Dictionary<string, string>()
                    {
                        {"src", $"images/icons/icon-{width}x{width}.png" },
                        {"sizes", $"{width}x{width}" },
                        {"type", "image/png" }
                    };
                    manifestContent.Icons.Add(dic);
                }
            }

        }
        public void DeleteGeneratedFolder()
        {
            Directory.Delete(RootPath, true);
        }
        public void WriteManifest()
        {
            var manifestFile = Path.Combine(ContentPath, "manifest.json");
            var json = JsonSerializer.Serialize(manifestContent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(manifestFile, json);
        }
        public void ZipContent()
        {
            ZipFile.CreateFromDirectory(ContentPath, ZipPath);
        }
    }
}
