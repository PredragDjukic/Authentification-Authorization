using Authentication_Authorization.DAL.Entities;
using System.Collections.Generic;

namespace Authentication_Authorization.DAL.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetAllUsers();
        User GetUserById(int id);
        void AddUser(User newUser);
        void UpdateUser(User updatedUser);
        void DeleteUser(int deletedUserId);
        User GetUserByUsername(string username);
    }
}
