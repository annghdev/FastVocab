using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.RemoveWordFromList;

public class RemoveWordFromListHandler : IRequestHandler<RemoveWordFromListCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWordFromListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveWordFromListCommand request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.GetWithFullDetailsAsync(request.CollectionId);

        if(collection == null)
        {
            return Result.Failure(Error.NotFound);
        }

        var wordList = collection.WordLists?.FirstOrDefault(wl=>wl.Id== request.WordListId);

        if (wordList == null)
        {
            return Result.Failure(Error.NotFound);
        }

        var word = await _unitOfWork.Words.FindAsync(request.WordId);

        if (word == null)
        {
            return Result.Failure(Error.NotFound);
        }

        var detail = wordList.Words.FirstOrDefault(dt=>dt.WordId== request.WordId);

        if (detail == null)
        {
            return Result.Failure(Error.NotFound);
        }

        wordList.Words.Remove(detail);

        _unitOfWork.Collections.Update(collection);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
