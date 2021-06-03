using Authentication_Authorization.BLL.DTOs.PlatformCredentialsDTOs;
using Authentication_Authorization.BLL.Models;
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
        void CheckIfPlatformCredentialsMatchId(int[] ids, int userId);
        byte[] GeneratePlatformCredentialsPdf(int userId, PdfRequestModel pdfRequestModel);
    }
}
