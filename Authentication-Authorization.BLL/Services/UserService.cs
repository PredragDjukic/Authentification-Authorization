using Authentication_Authorization.BLL.Contracts.Enums;
using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.DTOs.UserDTOs;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authentication_Authorization.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;


        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public IEnumerable<UserResponseDTO> GetAllUsers()
        {
            IEnumerable<User> allUsers = _repository.GetAllUsers();

            return (_mapper.Map<IEnumerable<UserResponseDTO>>(allUsers));
        }

        public UserResponseDTO GetByIdUser(int id, HttpContext context)
        {
            this.CheckIfJwtUserIdIsValid(id, context);
            User userById = this.GetUser(id);

            return (_mapper.Map<UserResponseDTO>(userById));
        }

        public UserConfirmationDTO UpdateUser(int id, UserRequestBodyDTO updatedUser, HttpContext context)
        {
            this.CheckIfJwtUserIdIsValid(id, context);
            this.CheckIfRoleIsValid(updatedUser.Role);

            User userToUpdate = GetUser(id);
            this.UpdateUserObject(userToUpdate, updatedUser);
            _repository.UpdateUser(userToUpdate);

            return (_mapper.Map<UserConfirmationDTO>(userToUpdate));
        }

        private void CheckIfJwtUserIdIsValid(int id, HttpContext context)
        {
            var role = context.User.Claims.FirstOrDefault(e => e.Type == "titleRole").Value;

            if (Convert.ToInt32(role) == Convert.ToInt32(Roles.Admin))
            {
                return;
            }

            var userIdFromJwt = context.User.Claims.FirstOrDefault(e => e.Type == "id").Value;

            if (id != Convert.ToInt32(userIdFromJwt))
                throw new BussinesException("Forbidden", 403);
        }

        private void UpdateUserObject(User userToUpdate, UserRequestBodyDTO updatedUser)
        {
            userToUpdate.FullName = updatedUser.FullName;
            userToUpdate.Username = updatedUser.Username;
            userToUpdate.Email = updatedUser.Email;
            userToUpdate.Password = HashHelper.Hash(updatedUser.Password);
            userToUpdate.Role = updatedUser.Role;
        }

        public UserConfirmationDTO AddUser(UserRequestBodyDTO newUserBody)
        {
            this.CheckIfRoleIsValid(newUserBody.Role);

            User newUser = this.CreateUserObject(newUserBody);
            _repository.AddUser(newUser);

            return (_mapper.Map<UserConfirmationDTO>(newUser));
        }

        private void CheckIfRoleIsValid(int role)
        {
            bool isRoleValid = Enum.IsDefined(typeof(Roles), role);

            if (!isRoleValid)
                throw new BussinesException("Given Role doesn't exist", 400);
        }

        private User CreateUserObject(UserRequestBodyDTO newUserBody)
        {
            return new User()
            {
                FullName = newUserBody.FullName,
                Username = newUserBody.Username,
                Email = newUserBody.Email,
                SecretId = this.GenerateSecretId(),
                Password = HashHelper.Hash(newUserBody.Password),
                Role = newUserBody.Role
            };
        }

        private string GenerateSecretId()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 64)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void DeleteUser(int id)
        {
            User userToDelete = _repository.GetUserById(id);

            if (userToDelete == null)
                throw new BussinesException("User with that Id doesn't exist", 400);

            _repository.DeleteUser(id);
        }

        private User GetUser(int id)
        {
            User user = _repository.GetUserById(id);

            if (user == null)
                throw new BussinesException("User doesn't exist", 400);

            return user;
        }
    }
}
