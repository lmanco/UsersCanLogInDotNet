using AutoMapper;
using UsersCanLogIn.API.DAL.Models;

namespace UsersCanLogIn.API.Util
{
    public class AutoMapperConfig
    {
        public static MapperConfiguration GetConfig()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<UserRequestDTO, User>();
                config.CreateMap<User, UserResponseDTO>();
            });
        }
    }
}
