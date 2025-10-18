using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Queries.GetTopicById;

/// <summary>
/// Handler for GetTopicByIdQuery
/// </summary>
public class GetTopicByIdHandler : IRequestHandler<GetTopicByIdQuery, Result<TopicDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTopicByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TopicDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        // Find topic by ID
        var topic = await _unitOfWork.Topics.FindAsync(request.TopicId);

        if (topic == null)
        {
            return Result<TopicDto>.Failure(Error.NotFound);
        }

        // Check if deleted
        if (topic.IsDeleted)
        {
            return Result<TopicDto>.Failure(Error.Deleted);
        }

        // Map to DTO
        var topicDto = _mapper.Map<TopicDto>(topic);

        return Result<TopicDto>.Success(topicDto);
    }
}

