using Authentication_Authorization.BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<UserResponseDTO>> BrowseUsers();
        Task<UserResponseDTO> FindUserById(int id);
        Task<UserForTokenDTO> FindUserByUsername(string username);
        Task<UserConfirmationDTO> CreateUser(UserRequestBodyDTO newUserBody);
        Task<UserConfirmationDTO> UpdateUser(int id, UserRequestBodyDTO updatedUserBody);
        Task RemoveUser(int id);
    }
}
