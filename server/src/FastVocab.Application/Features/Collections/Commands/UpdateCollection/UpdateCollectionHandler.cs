using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.UpdateCollection;

/// <summary>
/// Handler for UpdateCollectionCommand
/// </summary>
public class UpdateCollectionHandler : IRequestHandler<UpdateCollectionCommand, Result<CollectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCollectionHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CollectionDto>> Handle(UpdateCollectionCommand request, CancellationToken cancellationToken)
    {
        var existingCollection = await _unitOfWork.Collections.FindAsync(c => c.Id == request.Request.Id);
        if (existingCollection == null)
        {
            return Result<CollectionDto>.Failure(Error.NotFound);
        }

        // Check if name is changed and new name is unique
        if (existingCollection.Name != request.Request.Name)
        {
            var duplicate = await _unitOfWork.Collections.FindAsync(c => c.Name == request.Request.Name && c.Id != request.Request.Id);
            if (duplicate != null)
            {
                return Result<CollectionDto>.Failure(Error.Duplicate);
            }
        }

        // Map updates
        _mapper.Map(request.Request, existingCollection);

        // Update
        _unitOfWork.Collections.Update(existingCollection);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var collectionDto = _mapper.Map<CollectionDto>(existingCollection);
        return Result<CollectionDto>.Success(collectionDto);
    }
}
