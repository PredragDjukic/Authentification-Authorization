using Authentication_Authorization.BLL.Contracts.Enums;
using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.DTOs.UserDTOs;
using Authentication_Authorization.Presentation.Attributes;
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


        [IsAuthorized(Roles.Admin)]
        [HttpGet]
        public ActionResult<IEnumerable<UserConfirmationDTO>> GetUsers()
        {
            IEnumerable<UserResponseDTO> users =  _service.GetAllUsers();

            return Ok(users);
        }

        [IsAuthorized(Roles.All)]
        [HttpGet("{id}")]
        public ActionResult<UserConfirmationDTO> GetUserById(int id)
        {
            UserResponseDTO userById = _service.GetByIdUser(id, HttpContext);

            return Ok(userById);
        }

        [HttpPost]
        public ActionResult<UserConfirmationDTO> PostUser([FromBody] UserRequestBodyDTO newUserBody)
        {
            UserConfirmationDTO createdUserResponse = _service.AddUser(newUserBody);

            return Created(Request.Path, createdUserResponse);
        }

        [IsAuthorized(Roles.All)]
        [HttpPut("{id}")]
        public  ActionResult<UserConfirmationDTO> PutUser(int id, [FromBody] UserRequestBodyDTO updatedUserBody)
        {
            UserConfirmationDTO updatedUserResponse =  _service.UpdateUser(id, updatedUserBody, HttpContext);

            return Ok(updatedUserResponse);
        }

        [IsAuthorized(Roles.Admin)]
        [HttpDelete("{id}")]
        public  IActionResult DeleteUser(int id)
        {
            _service.DeleteUser(id);

            return NoContent();
        }
    }
}
