using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.CreateTopic;

/// <summary>
/// Handler for CreateTopicCommand
/// </summary>
public class CreateTopicHandler : IRequestHandler<CreateTopicCommand, Result<TopicDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTopicHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TopicDto>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        // Check if topic with same name already exists
        var existingTopic = await _unitOfWork.Topics.FindAsync(t => t.Name == request.Request.Name);
        if (existingTopic != null)
        {
            return Result<TopicDto>.Failure(Error.NameExists);
        }

        // Map request to entity
        var topic = _mapper.Map<Topic>(request.Request);
        topic.IsHiding = false; // Default to visible

        // Add to repository
        _unitOfWork.Topics.Add(topic);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO and return success
        var topicDto = _mapper.Map<TopicDto>(topic);
        return Result<TopicDto>.Success(topicDto);
    }
}

