using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.CreateCollection;

/// <summary>
/// Handler for CreateCollectionCommand
/// </summary>
public class CreateCollectionHandler : IRequestHandler<CreateCollectionCommand, Result<CollectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCollectionHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CollectionDto>> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
    {
        // Check if collection with same name already exists
        var existingCollection = await _unitOfWork.Collections.FindAsync(c => c.Name == request.Request.Name);
        if (existingCollection != null)
        {
            return Result<CollectionDto>.Failure(Error.Duplicate);
        }

        // Map request to entity
        var collection = _mapper.Map<Collection>(request.Request);
        collection.IsHiding = false; // Default to visible

        // Add to repository
        _unitOfWork.Collections.Add(collection);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO and return success
        var collectionDto = _mapper.Map<CollectionDto>(collection);
        return Result<CollectionDto>.Success(collectionDto);
    }
}
