using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MagicEvents.Api.Service.Api.Common
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> ToByteArray(this IFormFile imageFile) 
        {
            using (var memoryStream  = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static bool IsLargeFile(this IFormFile formFile)
        {
            return formFile.Length > 10 * Math.Pow(1024,2);
        }

        public static bool ContainImage(this IFormFile formFile)
        {
            var allowedImageFormats = new List<string>() 
            {
                "image/jpeg",
                "image/png"
            };
            return allowedImageFormats.Contains(formFile.ContentType);
        }
    }
}