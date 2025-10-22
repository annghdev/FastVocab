using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.Practice;

namespace FastVocab.Application.Common.Mappings;

public class PracticeSessionProfile : Profile
{
    public PracticeSessionProfile()
    {
        // Entity to DTO
        CreateMap<PracticeSesssion, PracticeSessionDto>()
            .ForMember(dest => dest.ListName, opt => opt.MapFrom(src => src.List != null ? src.List.Name : string.Empty))
            .ForMember(dest => dest.IsDueForReview, opt => opt.MapFrom(src => 
                src.NextReview.HasValue && src.NextReview.Value <= DateTimeOffset.UtcNow));
    }
}

