using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.DTOs.PlatformCredentialsDTOs;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Authentication_Authorization.BLL.Services
{
    public class PlatformCredentialsService : IPlatformCredentialsService
    {
        private readonly IPlatformCredentialsRepository _credentialsRepository;
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly IOptions<EncryptionSecretModel> _encryptionSecretModel;

        private readonly IWebHostEnvironment env;


        public PlatformCredentialsService(
            IPlatformCredentialsRepository credentialsRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IOptions<EncryptionSecretModel> encryptionSecretModel,
            IWebHostEnvironment env)
        {
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _encryptionSecretModel = encryptionSecretModel;
            this.env = env;
        }


        public IEnumerable<PlatformCredentialsResponseDTO> GetAllCredentials(int userId)
        {
            IEnumerable<PlatformCredentials> result = 
                _credentialsRepository.GetAllPlatformCredentials(userId);

            return _mapper.Map<IEnumerable<PlatformCredentialsResponseDTO>>(result);
        }

        public PlatformCredentialsResponseDTO GetCredentialsById(int userId, int id)
        {
            PlatformCredentials found = 
                _credentialsRepository.GetPlatformCredentialsById(userId, id);

            if (found == null)
                throw new BussinesException("Platform Credentials doesn't exist", 400);

            return _mapper.Map<PlatformCredentialsResponseDTO>(found);
        }

        public PlatformCredentialsConfirmationDTO AddCredentials(
            int userId,
            PlatformCredentialsPostBodyDTO newCredentials
        )
        {
            PlatformCredentials newPlatformCredentials = 
                this.CreatePlatformCredentialsObject(userId, newCredentials);

            _credentialsRepository.AddPlatformCredentials(newPlatformCredentials);

            return _mapper.Map<PlatformCredentialsConfirmationDTO>(newPlatformCredentials);
        }

        private PlatformCredentials CreatePlatformCredentialsObject(
            int userId, 
            PlatformCredentialsPostBodyDTO newCredentials
        )
        {
            User user = _userRepository.GetUserById(userId);

            PlatformCredentials newPlatformCredentials = new()
            {
                Username = newCredentials.Username,
                Name = newCredentials.Name,
                Password = EncryptionHelper.Encrypt(
                    newCredentials.Password, user.SecretId, _encryptionSecretModel.Value.Secret
                ),
                UserId = userId
            };

            return newPlatformCredentials;
        }

        public PlatformCredentialsConfirmationDTO UpdateCredentials(
            int userId,
            int id,
            PlatformCredentialsPutBodyDTO updatedCredentials
        )
        {
            PlatformCredentials credentialsToUpdate = 
                _credentialsRepository.GetPlatformCredentialsById(userId, id);

            if (credentialsToUpdate == null)
                throw new BussinesException("Platform Credentials doesn't exist", 400);

            User user = _userRepository.GetUserById(userId);
            PasswordHashHelper.VerifyPassword(updatedCredentials.UserPassword, user.Password);

            this.UpdatePlatformCredentialsAndAddToDatabase(
                credentialsToUpdate, updatedCredentials, user.SecretId
            );

            return _mapper.Map<PlatformCredentialsConfirmationDTO>(credentialsToUpdate);
        }

        private void UpdatePlatformCredentialsAndAddToDatabase(
            PlatformCredentials credentialsToUpdate,
            PlatformCredentialsPutBodyDTO updatedCredentials,
            string secretId
        )
        {
            credentialsToUpdate.Username = updatedCredentials.Username;
            credentialsToUpdate.Name = updatedCredentials.Name;
            credentialsToUpdate.Password = EncryptionHelper.Encrypt(
                updatedCredentials.Password, secretId, _encryptionSecretModel.Value.Secret
            );

            _credentialsRepository.UpdatePlatformCredentials(credentialsToUpdate);
        }

        public void DeleteCredentials(int userId, int id)
        {
            PlatformCredentials credentialsToDelete = 
                _credentialsRepository.GetPlatformCredentialsById(userId, id);

            if (credentialsToDelete == null)
                throw new BussinesException("Platform Credentials doesn't exist", 400);

            _credentialsRepository.DeletePlatformCredentials(userId, id);
        }

        public string GetPlatformPassword(int id, int userId, string passwordToCheck)
        {
            User user = _userRepository.GetUserById(userId);

            if (user == null)
                throw new BussinesException("User doesn't exist", 400);

            PasswordHashHelper.VerifyPassword(passwordToCheck, user.Password);

            string passwordEncrypted = _credentialsRepository.GetPlatformCredentialsHashedPassword(userId, id);

            if (passwordEncrypted == null)
                throw new BussinesException("Platform Credentials doesn't exist", 400);

            string passwordDecrypted = EncryptionHelper.Decrypt(
                passwordEncrypted, user.SecretId, _encryptionSecretModel.Value.Secret
            );

            return passwordDecrypted;
        }

        public void CheckIfPlatformCredentialsMatchId(int[] ids, int userId)
        {
            foreach(int id in ids)
            {
                PlatformCredentials found = _credentialsRepository.GetPlatformCredentialsById(userId, id);

                if (found == null)
                    throw new BussinesException("IDs are not valid", 400);
            }
        }

        public byte[] GeneratePlatformCredentialsPdf(int userId, PdfRequestModel pdfRequestModel)
        {   
            User user = _userRepository.GetUserById(userId);
            PasswordHashHelper.VerifyPassword(pdfRequestModel.Password, user.Password);

            string path = env.ContentRootPath + "\\Templates\\PlatformCredentialsTemplate.html";
            string html = File.ReadAllText(path);
            string content = this.GenerateHtmlContent(user, pdfRequestModel.PlatformCredentialsForPdf);

            string fullHtml = String.Format(html, content);
            byte[] pdf = GeneratePdfHelper.GeneratePdf(fullHtml);

            return pdf;
        }

        private string GenerateHtmlContent(User user, int[] credentialsIds)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int id in credentialsIds)
            {
                PlatformCredentials credentials = this.GetPlatformCredentialsWithPassword(user.Id, id);

                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                  </tr>",
                                  credentials.Name,
                                  credentials.Username,
                                  EncryptionHelper.Decrypt(
                                      credentials.Password,
                                      user.SecretId,
                                      _encryptionSecretModel.Value.Secret
                                  )
                );
            }

            return sb.ToString();
        }

        private PlatformCredentials GetPlatformCredentialsWithPassword(int userId, int id)
        {
            PlatformCredentials found = _credentialsRepository.GetPlatformCredentialsById(userId, id);

            if (found == null)
                throw new BussinesException("IDs are not valid", 400);

            found.Password = _credentialsRepository.GetPlatformCredentialsHashedPassword(userId, id);

            return found;
        }
    }
}
