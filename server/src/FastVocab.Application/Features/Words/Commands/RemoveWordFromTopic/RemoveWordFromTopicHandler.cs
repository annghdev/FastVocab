using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.RemoveWordFromTopic;

/// <summary>
/// Handler for RemoveWordFromTopicCommand
/// </summary>
public class RemoveWordFromTopicHandler : IRequestHandler<RemoveWordFromTopicCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWordFromTopicHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveWordFromTopicCommand request, CancellationToken cancellationToken)
    {
        // Verify word exists
        var word = await _unitOfWork.Words.GetWithInfoAsync(
            w => w.Id == request.WordId,
            w => w.Topics!);

        if (word == null || word.IsDeleted)
        {
            return Result.Failure(Error.NotFound);
        }

        // Verify topic exists
        var topic = await _unitOfWork.Topics.FindAsync(request.TopicId);
        if (topic == null || topic.IsDeleted)
        {
            return Result.Failure(Error.NotFound);
        }

        // Check if association exists
        var wordTopic = word.Topics?.FirstOrDefault(wt => wt.TopicId == request.TopicId);
        if (wordTopic == null)
        {
            return Result.Failure(Error.Conflict);
        }

        // Remove association
        word.Topics!.Remove(wordTopic);

        _unitOfWork.Words.Update(word);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

