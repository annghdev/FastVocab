using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.DeleteCollection;

/// <summary>
/// Handler for DeleteCollectionCommand
/// </summary>
public class DeleteCollectionHandler : IRequestHandler<DeleteCollectionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCollectionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.FindAsync(c => c.Id == request.Id);
        if (collection == null)
        {
            return Result.Failure(Error.NotFound);
        }

        collection.IsDeleted = true;

        _unitOfWork.Collections.Update(collection);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
