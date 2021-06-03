using Authentication_Authorization.BLL.DTOs.PlatformCredentialsDTOs;
using Authentication_Authorization.DAL.Entities;
using AutoMapper;

namespace Authentication_Authorization.BLL.Mapper
{
    public class PlatformCredentialsProfile : Profile
    {
        public PlatformCredentialsProfile()
        {
            CreateMap<PlatformCredentials, PlatformCredentialsResponseDTO>();
            CreateMap<PlatformCredentials, PlatformCredentialsConfirmationDTO>();
        }
    }
}
