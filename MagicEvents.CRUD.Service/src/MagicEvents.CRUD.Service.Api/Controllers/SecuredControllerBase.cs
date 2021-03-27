using System;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.CRUD.Service.Api.Controllers
{
    public class SecuredControllerBase : ControllerBase
    {
        protected Guid UserId 
            => User?.Identity.IsAuthenticated == true ? 
                Guid.Parse(User.Identity.Name) : Guid.Empty;
    }
}