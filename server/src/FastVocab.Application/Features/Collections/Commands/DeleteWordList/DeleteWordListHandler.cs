using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.DeleteWordList;

public class DeleteWordListHandler : IRequestHandler<DeleteWordListCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWordListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteWordListCommand request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.GetWithWordListsAsync(request.CollectionId);
        if (collection == null)
        {
            return Result.Failure(Error.NotFound);
        }
        var wordList = collection.WordLists?.FirstOrDefault(l=>l.Id==request.WordListId);
        if (wordList == null)
        {
            return Result.Failure(Error.NotFound);
        }
        collection.WordLists?.Remove(wordList);
        _unitOfWork.Collections.Update(collection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
