using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MagicEvents.Api.Service.Api.Common
{
    public class FileConverter
    {
        public static async Task<byte[]> ConvertToByteArray(IFormFile imageFile) 
        {
            using (var memoryStream  = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}