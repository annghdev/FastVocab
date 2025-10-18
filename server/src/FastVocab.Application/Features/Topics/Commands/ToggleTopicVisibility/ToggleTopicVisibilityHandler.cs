using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.ToggleTopicVisibility;

/// <summary>
/// Handler for ToggleTopicVisibilityCommand
/// </summary>
public class ToggleTopicVisibilityHandler : IRequestHandler<ToggleTopicVisibilityCommand, Result<TopicDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ToggleTopicVisibilityHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TopicDto>> Handle(ToggleTopicVisibilityCommand request, CancellationToken cancellationToken)
    {
        // Find topic
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

        // Toggle visibility
        topic.IsHiding = !topic.IsHiding;
        _unitOfWork.Topics.Update(topic);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO and return success
        var topicDto = _mapper.Map<TopicDto>(topic);
        return Result<TopicDto>.Success(topicDto);
    }
}

