using Authentication_Authorization.BLL.Contracts.Enums;
using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.DatabaseAccess;
using Authentication_Authorization.DAL.Entities;
using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication_Authorization.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _database;
        private readonly IMapper _mapper;
        private readonly IOptions<DatabaseConnectionStringModel> _connectionStringOptions;


        public UserService(IMapper mapper, IOptions<DatabaseConnectionStringModel> connectionStringOptions)
        {
            _database = new UserRepository();
            _mapper = mapper;
            _connectionStringOptions = connectionStringOptions;
        }

        public ICollection<UserResponseDTO> BrowseUsers()
        {
            ICollection<User> allUsers = _database.GetAllUsers(_connectionStringOptions.Value.ConnectionString);

            return (_mapper.Map<ICollection<UserResponseDTO>>(allUsers));
        }

        public UserResponseDTO FindUserById(int id)
        {
            User userById = _database.GetUserById(id, _connectionStringOptions.Value.ConnectionString);
            
            if (userById.Id == 0)
                throw new BussinesException("User doesn't exist", 400);

            return (_mapper.Map<UserResponseDTO>(userById));
        }

        public UserForTokenDTO FindUserByUsername(string username)
        {
            User userByUsername = _database.GetUserByUsername(
                username,
                _connectionStringOptions.Value.ConnectionString
            );

            return (_mapper.Map<UserForTokenDTO>(userByUsername));
        }

        public UserConfirmationDTO CreateUser(UserRequestBodyDTO newUserBody)
        {
            bool isRoleValid = Enum.IsDefined(typeof(Roles), newUserBody.Role);

            if (!isRoleValid)
                throw new BussinesException("Given Role doesn't exist", 400);

            User newUser = this.CreateUserAndAddToDatabase(newUserBody);

            return (_mapper.Map<UserConfirmationDTO>(newUser));

        }

        private User CreateUserAndAddToDatabase(UserRequestBodyDTO newUserBody)
        {
            User newUser = new()
            {
                FullName = newUserBody.FullName,
                Username = newUserBody.Username,
                Email = newUserBody.Email,
                Password = PasswordHashHelper.Hash(newUserBody.Password),
                Role = newUserBody.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _database.AddUser(newUser, _connectionStringOptions.Value.ConnectionString);

            return newUser;

        }

        public UserConfirmationDTO UpdateUser(int id, UserRequestBodyDTO updatedUser)
        {
            User userToUpdate = _database.GetUserById(id, _connectionStringOptions.Value.ConnectionString);
            bool isRoleValid = Enum.IsDefined(typeof(Roles), updatedUser.Role);

            if(userToUpdate.Id == 0)
                throw new BussinesException("User with given Id doesn't exist", 400);

            else if(!isRoleValid)
                throw new BussinesException("Given Role doesn't exist", 400);

            this.UpdateUserAndAddToDatabase(userToUpdate, updatedUser);                
            return (_mapper.Map<UserConfirmationDTO>(userToUpdate));
        }

        private void UpdateUserAndAddToDatabase(User userToUpdate, UserRequestBodyDTO updatedUser)
        {
            userToUpdate.FullName = updatedUser.FullName;
            userToUpdate.Username = updatedUser.Username;
            userToUpdate.Email = updatedUser.Email;
            userToUpdate.Password = PasswordHashHelper.Hash(updatedUser.Password);
            userToUpdate.Role = updatedUser.Role;
            userToUpdate.UpdatedAt = DateTime.UtcNow;
            _database.UpdateUser(userToUpdate, _connectionStringOptions.Value.ConnectionString);
        }

        public void RemoveUser(int id)
        {
            User userToDelete = _database.GetUserById(id, _connectionStringOptions.Value.ConnectionString);

            if (userToDelete.Id == 0)
                throw new BussinesException("User with that Id doesn't exist", 400);

            _database.DeleteUser(id, _connectionStringOptions.Value.ConnectionString);
        }
    }
}
