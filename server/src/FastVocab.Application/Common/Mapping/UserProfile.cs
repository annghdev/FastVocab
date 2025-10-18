using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.Users;

namespace FastVocab.Application.Common.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Entity to DTO
        CreateMap<AppUser, UserDto>();
    }
}

