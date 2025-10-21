using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.ToggleCollectionVisibility;

/// <summary>
/// Handler for ToggleCollectionVisibilityCommand
/// </summary>
public class ToggleCollectionVisibilityHandler : IRequestHandler<ToggleCollectionVisibilityCommand, Result<CollectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ToggleCollectionVisibilityHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CollectionDto>> Handle(ToggleCollectionVisibilityCommand request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.FindAsync(c => c.Id == request.Id);
        if (collection == null)
        {
            return Result<CollectionDto>.Failure(Error.NotFound);
        }

        collection.IsHiding = !collection.IsHiding;

        _unitOfWork.Collections.Update(collection);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var collectionDto = _mapper.Map<CollectionDto>(collection);
        return Result<CollectionDto>.Success(collectionDto);
    }
}
