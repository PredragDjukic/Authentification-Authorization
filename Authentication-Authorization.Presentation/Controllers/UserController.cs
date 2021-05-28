using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<ICollection<UserConfirmationDTO>>> GetUsers()
        {
            ICollection<UserResponseDTO> users = await _userService.BrowseUsers();

            return Ok(users);
        }

        [Authorize(Roles = "Admin, Server")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserConfirmationDTO>> GetUserById(int id)
        {
            TokenValidationHelper.CheckTokenForServerId(id, HttpContext);
            UserResponseDTO userById = await _userService.FindUserById(id);

            return Ok(userById);
        }

        [HttpPost]
        public async Task<ActionResult<UserConfirmationDTO>> PostUser([FromBody] UserRequestBodyDTO newUserBody)
        {
            UserConfirmationDTO createdUserResponse = await _userService.CreateUser(newUserBody);

            return Created(Request.Path, createdUserResponse);
        }

        [HttpPost("login")]
        public async Task<ActionResult> AutorizeUser([FromBody] PrincipalModel principal)
        {
            var token = _authentificationService.ValidatePrincipalAndGenerateTokens(principal, Response);

            return Ok(new { token });

        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] JwtModel token)
        {
            string jwt = _authentificationService.Refresh(token, Request, Response);

            return Ok(new { token = jwt });

        }

        [Authorize(Roles = "Admin, Server")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserConfirmationDTO>> PutUser(int id, [FromBody] UserRequestBodyDTO updatedUserBody)
        {
            TokenValidationHelper.CheckTokenForServerId(id, HttpContext);
            UserConfirmationDTO updatedUserResponse = await _userService.UpdateUser(id, updatedUserBody);

            return Ok(updatedUserResponse);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.RemoveUser(id);

            return NoContent();
        }
    }
}
