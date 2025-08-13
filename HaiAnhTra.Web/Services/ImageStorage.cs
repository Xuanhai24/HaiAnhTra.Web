using Microsoft.AspNetCore.Hosting;

namespace HaiAnhTra.Web.Services
{
    public interface IImageStorage { Task<string?> SaveAsync(IFormFile? file, string subfolder = "products"); }

    public class ImageStorage : IImageStorage
    {
        private readonly IWebHostEnvironment _env;
        public ImageStorage(IWebHostEnvironment env) => _env = env;

        public async Task<string?> SaveAsync(IFormFile? file, string subfolder = "products")
        {
            if (file == null || file.Length == 0) return null;
            var folder = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName));
            using var fs = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(fs);
            return $"/uploads/{subfolder}/{Path.GetFileName(path)}";
        }
    }
}
