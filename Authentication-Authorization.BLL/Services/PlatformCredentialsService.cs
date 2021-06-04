using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.DTOs.PlatformCredentialsDTOs;
using Authentication_Authorization.BLL.Exceptions;
using Authentication_Authorization.BLL.Helpers;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Authentication_Authorization.BLL.Services
{
    public class PlatformCredentialsService : IPlatformCredentialsService
    {
        private readonly IPlatformCredentialsRepository _credentialsRepository;
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly IOptions<EncryptionSecretModel> _encryptionSecretModel;

        private readonly IWebHostEnvironment _env;

        private const string HTML_TEMPLATE_PATH = "/Templates/PlatformCredentialsTemplate.html";


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
            _env = env;
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
                this.GetPlatformCredentials(id, userId);

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
                this.GetPlatformCredentials(id, userId);

            User user = ValidateUser(updatedCredentials.UserPassword, userId);

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
            PlatformCredentials credentials = this.GetPlatformCredentials(id, userId);
            this.DeleteImage(credentials.ImageName);

            _credentialsRepository.DeletePlatformCredentials(userId, id);
        }

        public string GetPlatformPassword(int id, int userId, string passwordToCheck)
        {
            User user = this.ValidateUser(passwordToCheck, userId);
            string passwordDecrypted = this.GetDecryptedPassword(userId, id, user.SecretId);

            return passwordDecrypted;
        }

        private string GetDecryptedPassword(int userId, int id, string secretId)
        {
            string passwordEncrypted = _credentialsRepository.GetPlatformCredentialsHashedPassword(userId, id);

            if (passwordEncrypted == null)
                throw new BussinesException("Platform Credentials doesn't exist", 400);

            string passwordDecrypted = EncryptionHelper.Decrypt(
                passwordEncrypted, secretId, _encryptionSecretModel.Value.Secret
            );

            return passwordDecrypted;
        }

        public byte[] GeneratePlatformCredentialsPdf(int userId, PdfRequestModel pdfRequestModel)
        {
            if (pdfRequestModel.PlatformCredentialsForPdf.Length == 0)
                throw new BussinesException("No Platform Credentials to print", 400);

            User user = this.ValidateUser(pdfRequestModel.Password, userId);
            byte[] pdf = this.PdfFileContent(user, pdfRequestModel.PlatformCredentialsForPdf);

            return pdf;
        }

        private byte[] PdfFileContent(User user, int[] credentialsForPdf)
        {
            string path = _env.ContentRootPath + HTML_TEMPLATE_PATH;
            string html = File.ReadAllText(path);
            string content = this.GenerateHtmlContent(user, credentialsForPdf);

            string fullHtml = String.Format(html, content);
            byte[] pdf = GeneratePdfHelper.GeneratePdf(fullHtml);

            return pdf;
        }

        private User ValidateUser(string password, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            if (user == null)
                throw new BussinesException("User doesn't exist", 400);

            HashHelper.VerifyValue(password, user.Password);

            return user;
        }

        private string GenerateHtmlContent(User user, int[] credentialsIds)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int id in credentialsIds)
            {
                PlatformCredentials credentials = this.GetPlatformCredentialsWithPassword(user.Id, id);
                string path = _env.WebRootFileProvider.GetFileInfo("Images/" + credentials.ImageName)?.PhysicalPath;

                sb.AppendFormat(@"<tr>
                                    <img src=""{0}"">
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>",
                                  path,
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
            PlatformCredentials found = this.GetPlatformCredentials(id, userId);
            found.Password = _credentialsRepository.GetPlatformCredentialsHashedPassword(userId, id);

            return found;
        }

        public void AddImage(int id, int userId, IFormFile image)
        {
            this.CheckIfPlatformCredentialsExists(id, userId);
            this.CheckIfPlatformCredentialsHasAImage(id, userId);
            this.AddNewImageToStorage(id, userId, image);
        }

        private void CheckIfPlatformCredentialsExists(int id, int userId)
        {
            PlatformCredentials credentials = _credentialsRepository.GetPlatformCredentialsById(userId, id);

            if (credentials == null)
                throw new BussinesException("Platform Credentials doesn't exist", 400);
        }

        private void CheckIfPlatformCredentialsHasAImage(int id, int userId)
        {
            string alreadyExistCheck = _credentialsRepository.CheckIfPlatformCredentialsContainImage(id, userId);

            if (!string.IsNullOrEmpty(alreadyExistCheck))
                throw new BussinesException("Platform Credentials already has a image", 400);
        }

        public void UpdateImage(int id, int userId, IFormFile image)
        {
            PlatformCredentials credentials = this.GetPlatformCredentials(id, userId);

            this.DeleteImage(credentials.ImageName);
            this.AddNewImageToStorage(id, userId, image);
        }

        private PlatformCredentials GetPlatformCredentials(int id, int userId)
        {
            PlatformCredentials credentials = _credentialsRepository.GetPlatformCredentialsById(userId, id);

            if (credentials == null)
                throw new BussinesException("Platform Credentials doesn't exist", 400);

            return credentials;
        }

        private void DeleteImage(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                string pathToDelete = _env.WebRootFileProvider.GetFileInfo("Images/" + imageName)?.PhysicalPath;
                File.Delete(pathToDelete);
            }
        }

        private void AddNewImageToStorage(int id, int userId, IFormFile image)
        {
            string folderPath = this.CreateFolderPath();
            string fullFileName = this.CreateFullFileName(image);
            string path = Path.Combine(folderPath, fullFileName);

            this.WriteFileToWWWRoot(path, image);

            _credentialsRepository.AddImage(id, userId, fullFileName);
        }
       
        private string CreateFolderPath()
        {
            string webRootPath = this._env.WebRootPath;
            string folderName = "Images";
            string folderPath = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        private string CreateFullFileName(IFormFile image)
        {
            string fileName = Path.GetFileNameWithoutExtension(image.FileName);
            string extension = Path.GetExtension(image.FileName);
            string hashedFileName = HashHelper.Hash(fileName);

            hashedFileName = new string (hashedFileName //Slika zasto nije koriscen string.Replace()
                .Where(e => char.IsLetterOrDigit(e))
                .ToArray()
            );

            string fullFileName = hashedFileName + extension;

            return fullFileName;
        }

        private void WriteFileToWWWRoot(string path, IFormFile image)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }
        }
    }
}
