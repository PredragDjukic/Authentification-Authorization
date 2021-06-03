using Authentication_Authorization.DAL.Entities;
using System.Collections.Generic;

namespace Authentication_Authorization.DAL.Interfaces
{
    public interface IPlatformCredentialsRepository
    {
        IEnumerable<PlatformCredentials> GetAllPlatformCredentials(int userId);
        PlatformCredentials GetPlatformCredentialsById(int userId, int id);
        void AddPlatformCredentials(PlatformCredentials newCredentials);
        void UpdatePlatformCredentials(PlatformCredentials updatedCredentials);
        void DeletePlatformCredentials(int userId, int deletedCredentialsId);
        string GetPlatformCredentialsHashedPassword(int userId, int id);
    }
}
