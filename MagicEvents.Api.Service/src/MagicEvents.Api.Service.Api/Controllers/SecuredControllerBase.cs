using System;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.Api.Service.Api.Controllers
{
    public class SecuredControllerBase : ControllerBase
    {
        protected Guid UserId 
            => User?.Identity.IsAuthenticated == true ? 
                Guid.Parse(User.Identity.Name) : Guid.Empty;
    }
}