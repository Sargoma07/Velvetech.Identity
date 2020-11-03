using AutoMapper;
using Velvetech.Identity.API.Controllers;
using Velvetech.Identity.API.Entities;
using Velvetech.Identity.API.Entities.Requests;
using Velvetech.Identity.API.Requests;

namespace Velvetech.Identity.API.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMapUserToUserIdentity();

            CreateMapSignInRequestToCreateUserRequest();

        }

        private void CreateMapUserToUserIdentity()
        {
            CreateMap<User, UserIdentity>()
                .ForMember(m => m.Login, mo => mo.MapFrom(e => e.Login));
        }

        private void CreateMapSignInRequestToCreateUserRequest()
        {
            CreateMap<SignInRequest, CreateUserRequest>()
                .ForMember(m => m.Login, mo => mo.MapFrom(e => e.Email))
                // TODO: для пароля добавить шифрование 
                .ForMember(m => m.Password, mo => mo.MapFrom(e => e.Password));
        }


    }
}