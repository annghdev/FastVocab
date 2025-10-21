using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.WordLists;

namespace FastVocab.Application.Common.Mapping;

public class WordListProfile : Profile
{
    public WordListProfile()
    {
        // Entity to DTO
        CreateMap<WordList, WordListDto>()
            .ForMember(dest => dest.WordCount, opt => opt.MapFrom(src => src.Words != null ? src.Words.Count : 0));

        // Request to Entity
        CreateMap<CreateWordListRequest, WordList>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Collection, opt => opt.Ignore())
            .ForMember(dest => dest.Words, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());

        CreateMap<UpdateWordListRequest, WordList>()
            .ForMember(dest => dest.CollectionId, opt => opt.Ignore())
            .ForMember(dest => dest.Collection, opt => opt.Ignore())
            .ForMember(dest => dest.Words, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());
    }
}

