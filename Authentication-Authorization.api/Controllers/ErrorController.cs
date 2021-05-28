using Authentication_Authorization.BLL.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorization.Presentation.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)] //Nakon dodavanja UseExceptionHandler potrebno da bi swagger radio, inace baca grešku
    public class ErrorController : ControllerBase
    {
        [Route("/error-development")]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = context.Error as BussinesException;

            string stackTrace = context.Error.StackTrace;
            string message = context.Error.Message;

            if (exception is BussinesException)
            {
                return Problem(
                detail: stackTrace,
                statusCode: exception.StatusCode,
                title: message);
            }

            return Problem(
                detail: stackTrace,
                title: message);
            
        }

        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = context.Error as BussinesException;

            if (exception is BussinesException)
            {
                return Problem(
                statusCode: exception.StatusCode,
                title: exception.Message);
            }

            return Problem(
                title: context.Error.Message,
                statusCode: 500);
        }
    }
}
