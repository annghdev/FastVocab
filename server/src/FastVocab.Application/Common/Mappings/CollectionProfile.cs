using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.Collections;

namespace FastVocab.Application.Common.Mappings;

public class CollectionProfile : Profile
{
    public CollectionProfile()
    {
        CreateMap<Collection, CollectionDto>()
            .ForMember(dest => dest.WordLists, opt => opt.MapFrom(src => src.WordLists));
        CreateMap<CreateCollectionRequest, Collection>();
        CreateMap<UpdateCollectionRequest, Collection>();
    }
}
