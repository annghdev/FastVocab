using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetVisibleTopics;

/// <summary>
/// Handler for GetVisibleTopicsQuery
/// </summary>
public class GetVisibleTopicsHandler : IRequestHandler<GetVisibleTopicsQuery, Result<IEnumerable<TopicDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVisibleTopicsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<TopicDto>>> Handle(GetVisibleTopicsQuery request, CancellationToken cancellationToken)
    {
        // Get only visible topics (not hidden and not deleted)
        var topics = await _unitOfWork.Topics.GetAllAsync(
            predicate: t => !t.IsDeleted && !t.IsHiding,
            cancellationToken: cancellationToken);

        // Map to DTOs
        var topicDtos = _mapper.Map<IEnumerable<TopicDto>>(topics);

        return Result<IEnumerable<TopicDto>>.Success(topicDtos);
    }
}

