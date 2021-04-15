using System.IO;
using System.Threading.Tasks;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IImageProcessor
    {
        bool IsValidImage(byte[] fileBytes, string contentType);
        Task<byte[]> CreateThumbnail(byte[] fileBytes);
        bool IsFileSizeValid(long length);
    }
}