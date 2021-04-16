using System.IO;
using System.Threading.Tasks;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IImageProcessor
    {
        bool IsValidImage(byte[] fileBytes);
        Task<byte[]> CreateThumbnailAsync(byte[] fileBytes);
        bool IsFileSizeValid(long length);
    }
}