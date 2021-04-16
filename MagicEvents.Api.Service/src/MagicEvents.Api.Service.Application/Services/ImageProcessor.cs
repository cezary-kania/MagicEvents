using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicEvents.Api.Service.Application.Services
{
    public class ImageProcessor : IImageProcessor
    {
        private static readonly List<string> _allowedContentTypes = new() 
        {
            "image/jpeg",
            "image/png"
        };
        public async Task<byte[]> CreateThumbnail(byte[] fileData)
        {
            try {
                using var image = Image.Load(fileData);
                image.Mutate(x => x.Resize(200,200));
                using var memoryStream = new MemoryStream();
                await image.SaveAsPngAsync(memoryStream);
                return memoryStream.ToArray();
            } catch(Exception ex)
            {
                throw new ServiceException(ExceptionMessage.Org.UknownError);
            }
        }

        public bool IsValidImage(byte[] fileData)
        {
            return IsFileSizeValid(fileData.Length) && IsImage(fileData);
        }

        private bool IsImage(byte[] fileBytes)
        {
            try 
            {
                using var image = Image.Load(fileBytes);
                var x = Image.Load(fileBytes);
                return true;
            } 
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool IsFileSizeValid(long length)
        {
            // 5MB
            return length <= 5 * Math.Pow(1024,2);
        }
    }
}