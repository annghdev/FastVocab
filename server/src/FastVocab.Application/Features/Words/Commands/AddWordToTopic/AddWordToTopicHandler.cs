using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.AddWordToTopic;

/// <summary>
/// Handler for AddWordToTopicCommand
/// </summary>
public class AddWordToTopicHandler : IRequestHandler<AddWordToTopicCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddWordToTopicHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddWordToTopicCommand request, CancellationToken cancellationToken)
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

        // Check if association already exists
        if (word.Topics?.Any(wt => wt.TopicId == request.TopicId) == true)
        {
            return Result.Failure(Error.Conflict);
        }

        // Add association
        word.Topics ??= new List<WordTopic>();
        word.Topics.Add(new WordTopic
        {
            WordId = request.WordId,
            TopicId = request.TopicId
        });

        _unitOfWork.Words.Update(word);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

