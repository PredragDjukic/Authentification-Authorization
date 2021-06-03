using Authentication_Authorization.BLL.DTOs.UserDTOs;
using Authentication_Authorization.DAL.Entities;
using AutoMapper;

namespace Authentication_Authorization.BLL.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponseDTO>();
            CreateMap<User, UserConfirmationDTO>();
            CreateMap<User, UserForTokenDTO>();
        }
    }
}
