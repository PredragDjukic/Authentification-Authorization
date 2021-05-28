using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorization.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthentificationService _service;

        public AuthController(IAuthentificationService service)
        {
            _service = service;
        }

        [HttpPost("auth")]
        public ActionResult AutorizeUser([FromBody] PrincipalModel principal)
        {
            var token = _service.ValidatePrincipalAndGenerateToken(principal, Response);

            return Ok(new { token });
        }


        [HttpPost("refresh")]
        public ActionResult RefreshToken([FromBody] JwtModel token)
        {
            string newJwt = _service.Refresh(token);
            return Ok(new { token = newJwt });

        }

    }
}
