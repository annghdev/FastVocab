using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Topics.Commands.DeleteTopic;

/// <summary>
/// Handler for DeleteTopicCommand
/// </summary>
public class DeleteTopicHandler : IRequestHandler<DeleteTopicCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTopicHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTopicCommand request, CancellationToken cancellationToken)
    {
        // Find topic
        var topic = await _unitOfWork.Topics.FindAsync(request.TopicId);
        if (topic == null)
        {
            return Result.Failure(Error.NotFound);
        }

        // Check if already deleted
        if (topic.IsDeleted)
        {
            return Result.Failure(Error.Deleted);
        }

        // Soft delete (set IsDeleted flag)
        topic.IsDeleted = true;
        _unitOfWork.Topics.Update(topic);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

