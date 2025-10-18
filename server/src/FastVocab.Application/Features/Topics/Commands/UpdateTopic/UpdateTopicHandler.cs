using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.UpdateTopic;

/// <summary>
/// Handler for UpdateTopicCommand
/// </summary>
public class UpdateTopicHandler : IRequestHandler<UpdateTopicCommand, Result<TopicDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateTopicHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TopicDto>> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        // Find existing topic
        var topic = await _unitOfWork.Topics.FindAsync(request.Request.Id);
        if (topic == null)
        {
            return Result<TopicDto>.Failure(Error.NotFound);
        }

        // Check if soft deleted
        if (topic.IsDeleted)
        {
            return Result<TopicDto>.Failure(Error.Deleted);
        }

        // Check if name already exists (excluding current topic)
        var existingTopic = await _unitOfWork.Topics.FindAsync(t => 
            t.Name == request.Request.Name && t.Id != request.Request.Id);
        if (existingTopic != null)
        {
            return Result<TopicDto>.Failure(Error.NameExists);
        }

        // Update properties
        topic.Name = request.Request.Name;
        topic.VnText = request.Request.VnText;
        topic.ImageUrl = request.Request.ImageUrl;
        topic.IsHiding = request.Request.IsHiding;

        // Update in repository
        _unitOfWork.Topics.Update(topic);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO and return success
        var topicDto = _mapper.Map<TopicDto>(topic);
        return Result<TopicDto>.Success(topicDto);
    }
}

