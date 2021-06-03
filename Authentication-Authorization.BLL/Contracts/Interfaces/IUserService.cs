using Authentication_Authorization.BLL.DTOs.UserDTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserResponseDTO> GetAllUsers();
        UserResponseDTO GetByIdUser(int id, HttpContext context);
        UserForTokenDTO FindUserByUsername(string username);
        UserConfirmationDTO AddUser(UserRequestBodyDTO newUserBody);
        UserConfirmationDTO UpdateUser(int id, UserRequestBodyDTO updatedUserBody, HttpContext context);
        void DeleteUser(int id);
    }
}
