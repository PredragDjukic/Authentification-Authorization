using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Authentication_Authorization.Presentation.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthentificationService _authentificationService;

        public UserController(IUserService userService, IAuthentificationService authentificationService)
        {
            _userService = userService;
            _authentificationService = authentificationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<ICollection<UserConfirmationDTO>> GetUsers()
        {
            ICollection<UserResponseDTO> users =  _userService.BrowseUsers();

            return Ok(users);
        }

        [Authorize(Roles = "Admin, Server")]
        [HttpGet("{id}")]
        public ActionResult<UserConfirmationDTO> GetUserById(int id)
        {
            TokenValidationHelper.CheckTokenForServerId(id, HttpContext);
            UserResponseDTO userById = _userService.FindUserById(id);

            return Ok(userById);
        }

        [HttpPost]
        public ActionResult<UserConfirmationDTO> PostUser([FromBody] UserRequestBodyDTO newUserBody)
        {
            UserConfirmationDTO createdUserResponse = _userService.CreateUser(newUserBody);

            return Created(Request.Path, createdUserResponse);
        }

        [HttpPost("login")]
        public  ActionResult AutorizeUser([FromBody] PrincipalModel principal)
        {
            var token = _authentificationService.ValidatePrincipalAndGenerateToken(principal, Response);

            return Ok(new { token });

        }

        [HttpPost("refresh")]
        public  ActionResult RefreshToken([FromBody] JwtModel token)
        {
            string newJwt = _authentificationService.Refresh(token);
            return Ok(new { token = newJwt });

        }

        [Authorize(Roles = "Admin, Server")]
        [HttpPut("{id}")]
        public  ActionResult<UserConfirmationDTO> PutUser(int id, [FromBody] UserRequestBodyDTO updatedUserBody)
        {
            TokenValidationHelper.CheckTokenForServerId(id, HttpContext);
            UserConfirmationDTO updatedUserResponse =  _userService.UpdateUser(id, updatedUserBody);

            return Ok(updatedUserResponse);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public  IActionResult DeleteUser(int id)
        {
            _userService.RemoveUser(id);

            return NoContent();
        }
    }
}
