using Microsoft.AspNetCore.Http;

namespace NewsProject.Utils
{
    public class FileHelper
    {
        public static async Task<string> FileLoaderAsync(IFormFile formFile, string filePath = "img/")
        {
            if (formFile == null || formFile.Length == 0)
                return "";

            // Dosya adını ve yolu oluştur
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

            // Eğer klasör yoksa oluştur
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string path = Path.Combine(directory, fileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            // Geriye sadece dosya adını dönelim
            return fileName;
        }

        public static bool FileRemover(string fileName, string filePath = "img/")
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}
