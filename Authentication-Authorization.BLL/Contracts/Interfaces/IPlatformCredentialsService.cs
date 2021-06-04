using Authentication_Authorization.BLL.DTOs.PlatformCredentialsDTOs;
using Authentication_Authorization.BLL.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Authentication_Authorization.BLL.Contracts.Interfaces
{
    public interface IPlatformCredentialsService
    {
        IEnumerable<PlatformCredentialsResponseDTO> GetAllCredentials(int userId);
        PlatformCredentialsResponseDTO GetCredentialsById(int userId, int id);
        PlatformCredentialsConfirmationDTO AddCredentials(int userId, PlatformCredentialsPostBodyDTO newCredentials);
        PlatformCredentialsConfirmationDTO UpdateCredentials(int userId, int id, PlatformCredentialsPutBodyDTO updatedCredentials);
        void DeleteCredentials(int userId, int id);
        string GetPlatformPassword(int id, int userId, string passwordToCheck);
        byte[] GeneratePlatformCredentialsPdf(int userId, PdfRequestModel pdfRequestModel);
        void AddImage(int id, int userId, IFormFile image);
        void UpdateImage(int id, int userId, IFormFile image);
    }
}
