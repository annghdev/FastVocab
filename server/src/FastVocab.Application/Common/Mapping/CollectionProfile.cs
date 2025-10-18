using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.Collections;

namespace FastVocab.Application.Common.Mapping;

public class CollectionProfile : Profile
{
    public CollectionProfile()
    {
        // Entity to DTO
        CreateMap<Collection, CollectionDto>()
            .ForMember(dest => dest.WordListCount, opt => opt.MapFrom(src => src.WordLists != null ? src.WordLists.Count : 0));

        // Request to Entity
        CreateMap<CreateCollectionRequest, Collection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsHiding, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.WordLists, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());

        CreateMap<UpdateCollectionRequest, Collection>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.WordLists, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());
    }
}

