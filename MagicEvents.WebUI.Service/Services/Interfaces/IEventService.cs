using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.WebUI.Service.Models.Events;

namespace MagicEvents.WebUI.Service.Services.Interfaces
{
    public interface IEventService
    {
        Task<PaginatedListViewModel<EventViewModel>> GetEvents(int pageIndex, int pageSize);
    }
}