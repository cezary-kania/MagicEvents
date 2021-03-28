using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MagicEvents.Api.Service.Api.Common
{
    public class FileConverter
    {
        public static async Task<byte[]> ConvertToByteArray(IFormFile thFile) 
        {
            using (var memoryStream  = new MemoryStream())
            {
                await thFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}