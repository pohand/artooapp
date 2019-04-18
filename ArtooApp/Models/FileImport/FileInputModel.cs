using Microsoft.AspNetCore.Http;

namespace Artoo.Models.FileInputModel
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
