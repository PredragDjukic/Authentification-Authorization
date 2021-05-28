using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.Presentation.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Authentication_Authorization.Presentation.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService userService)
        {
            _service = userService;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstatnts.Admin)]
        public ActionResult<ICollection<UserConfirmationDTO>> GetUsers()
        {
            ICollection<UserResponseDTO> users =  _service.BrowseUsers();

            return Ok(users);
        }

        [Authorize(Roles = RoleConstatnts.All)]
        [HttpGet("{id}")]
        public ActionResult<UserConfirmationDTO> GetUserById(int id)
        {
            TokenValidationHelper.CheckTokenForServerId(id, HttpContext);
            UserResponseDTO userById = _service.FindUserById(id);

            return Ok(userById);
        }

        [HttpPost]
        public ActionResult<UserConfirmationDTO> PostUser([FromBody] UserRequestBodyDTO newUserBody)
        {
            UserConfirmationDTO createdUserResponse = _service.CreateUser(newUserBody);

            return Created(Request.Path, createdUserResponse);
        }

        [Authorize(Roles = RoleConstatnts.All)]
        [HttpPut("{id}")]
        public  ActionResult<UserConfirmationDTO> PutUser(int id, [FromBody] UserRequestBodyDTO updatedUserBody)
        {
            TokenValidationHelper.CheckTokenForServerId(id, HttpContext);
            UserConfirmationDTO updatedUserResponse =  _service.UpdateUser(id, updatedUserBody);

            return Ok(updatedUserResponse);
        }

        [Authorize(Roles = RoleConstatnts.Admin)]
        [HttpDelete("{id}")]
        public  IActionResult DeleteUser(int id)
        {
            _service.RemoveUser(id);

            return NoContent();
        }
    }
}
