using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetAllTopics;

/// <summary>
/// Handler for GetAllTopicsQuery
/// </summary>
public class GetAllTopicsHandler : IRequestHandler<GetAllTopicsQuery, IEnumerable<TopicDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTopicsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TopicDto>> Handle(GetAllTopicsQuery request, CancellationToken cancellationToken)
    {
        // Get all topics that are not deleted
        var topics = await _unitOfWork.Topics.GetAllAsync(cancellationToken: cancellationToken);

        // Map to DTOs
        var topicDtos = _mapper.Map<IEnumerable<TopicDto>>(topics);

        return topicDtos;
    }
}

