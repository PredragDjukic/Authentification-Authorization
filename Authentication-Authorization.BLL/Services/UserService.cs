using Authentication_Authorization.BLL.Contracts.Enums;
using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace Authentication_Authorization.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;


        public UserService(IMapper mapper, IUserRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public ICollection<UserResponseDTO> BrowseUsers()
        {
            ICollection<User> allUsers = _repository.GetAllUsers();

            return (_mapper.Map<ICollection<UserResponseDTO>>(allUsers));
        }

        public UserResponseDTO FindUserById(int id)
        {
            User userById = _repository.GetUserById(id);
            
            if (userById.Id == 0)
                throw new BussinesException("User doesn't exist", 400);

            return (_mapper.Map<UserResponseDTO>(userById));
        }

        public UserForTokenDTO FindUserByUsername(string username)
        {
            User userByUsername = _repository.GetUserByUsername(username);

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
            _repository.AddUser(newUser);

            return newUser;
        }

        public UserConfirmationDTO UpdateUser(int id, UserRequestBodyDTO updatedUser)
        {
            User userToUpdate = _repository.GetUserById(id);
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
            _repository.UpdateUser(userToUpdate);
        }

        public void RemoveUser(int id)
        {
            User userToDelete = _repository.GetUserById(id);

            if (userToDelete.Id == 0)
                throw new BussinesException("User with that Id doesn't exist", 400);

            _repository.DeleteUser(id);
        }
    }
}
