using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.AddWordToList;

public class AddWordToListHandler : IRequestHandler<AddWordToListCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddWordToListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddWordToListCommand request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.GetWithFullDetailsAsync(request.CollectionId);

        var wordList = collection.WordLists.FirstOrDefault(l=>l.Id== request.Request.WordListId);
        if (wordList == null)
        {
            return Result.Failure(Error.NotFound);
        }

        var word = await _unitOfWork.Words.FindAsync(w => w.Id == request.Request.WordId);
        if (word == null)
        {
            return Result.Failure(Error.NotFound);
        }

        if (wordList.Words.Any(wld => wld.WordId == request.Request.WordId))
        {
            return Result.Failure(Error.Duplicate);
        }

        var detail = new WordListDetail { WordListId = request.Request.WordListId, WordId = request.Request.WordId };
        wordList.Words.Add(detail);

        _unitOfWork.Collections.Update(collection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
