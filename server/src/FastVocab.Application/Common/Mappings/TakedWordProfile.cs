using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.Practice;

namespace FastVocab.Application.Common.Mappings;

public class TakedWordProfile : Profile
{
    public TakedWordProfile()
    {
        // Entity to DTO
        CreateMap<TakedWord, TakedWordDto>()
            .ForMember(dest => dest.WordText, opt => opt.MapFrom(src => src.Word != null ? src.Word.Text : string.Empty))
            .ForMember(dest => dest.IsDueForReview, opt => opt.MapFrom(src => 
                src.NextReview.HasValue && src.NextReview.Value <= DateTimeOffset.UtcNow));
    }
}

