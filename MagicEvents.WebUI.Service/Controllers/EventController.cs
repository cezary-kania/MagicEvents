using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.WebUI.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.WebUI.Service.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var paginatedEventList = await _eventService.GetEvents(0, 10);
            return View(paginatedEventList.Items);
        }
    }
}