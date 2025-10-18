using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.Topics;

namespace FastVocab.Application.Common.Mapping;

public class TopicProfile : Profile
{
    public TopicProfile()
    {
        // Entity to DTO
        CreateMap<Topic, TopicDto>();

        // Request to Entity
        CreateMap<CreateTopicRequest, Topic>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Words, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());

        CreateMap<UpdateTopicRequest, Topic>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Words, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());
    }
}

