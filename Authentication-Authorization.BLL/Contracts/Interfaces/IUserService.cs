using Authentication_Authorization.BLL.Models;
using System.Collections.Generic;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IUserService
    {
        ICollection<UserResponseDTO> BrowseUsers();
        UserResponseDTO FindUserById(int id);
        UserForTokenDTO FindUserByUsername(string username);
        UserConfirmationDTO CreateUser(UserRequestBodyDTO newUserBody);
        UserConfirmationDTO UpdateUser(int id, UserRequestBodyDTO updatedUserBody);
        void RemoveUser(int id);
    }
}
