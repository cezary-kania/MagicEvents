using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicEvents.CRUD.Service.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.ModelState.IsValid)
            {
                 var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage))
                    .ToArray();
                var errors = new List<KeyValuePair<string, string>>();  
                foreach(var error in errorsInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new KeyValuePair<string, string>(error.Key, subError);
                        errors.Add(errorModel);
                    }
                }
                context.Result = new BadRequestObjectResult(new {Errors = errors});
                return;
            }
            await next();
        }
    }
}