using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Shared.DTOs.Words;

namespace FastVocab.Application.Common.Mappings;

public class WordProfile : Profile
{
    public WordProfile()
    {
        // Entity to DTO
        CreateMap<Word, WordDto>();

        // Junction entity to DTO - Map through Word navigation property
        CreateMap<WordListDetail, WordDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Word!.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Word!.Text))
            .ForMember(dest => dest.Meaning, opt => opt.MapFrom(src => src.Word!.Meaning))
            .ForMember(dest => dest.Definition, opt => opt.MapFrom(src => src.Word!.Definition))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Word!.Type))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Word!.Level))
            .ForMember(dest => dest.Example1, opt => opt.MapFrom(src => src.Word!.Example1))
            .ForMember(dest => dest.Example2, opt => opt.MapFrom(src => src.Word!.Example2))
            .ForMember(dest => dest.Example3, opt => opt.MapFrom(src => src.Word!.Example3))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Word!.ImageUrl))
            .ForMember(dest => dest.AudioUrl, opt => opt.MapFrom(src => src.Word!.AudioUrl))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Word!.CreatedAt));

        // Request to Entity
        CreateMap<CreateWordRequest, Word>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Topics, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());

        CreateMap<UpdateWordRequest, Word>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Topics, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());
    }
}

