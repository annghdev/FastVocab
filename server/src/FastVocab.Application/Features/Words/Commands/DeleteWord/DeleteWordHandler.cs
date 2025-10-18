using FastVocab.Domain.Repositories;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.DeleteWord;

/// <summary>
/// Handler for DeleteWordCommand
/// </summary>
public class DeleteWordHandler : IRequestHandler<DeleteWordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWordHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteWordCommand request, CancellationToken cancellationToken)
    {
        // Find word
        var word = await _unitOfWork.Words.FindAsync(request.WordId);
        if (word == null)
        {
            return Result.Failure(Error.NotFound);
        }

        // Check if already deleted
        if (word.IsDeleted)
        {
            return Result.Failure(Error.Deleted);
        }

        // Soft delete (set IsDeleted flag)
        word.IsDeleted = true;
        _unitOfWork.Words.Update(word);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

