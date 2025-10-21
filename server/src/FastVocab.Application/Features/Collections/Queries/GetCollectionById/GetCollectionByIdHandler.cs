using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.GetCollectionById;

/// <summary>
/// Handler for GetCollectionByIdQuery
/// </summary>
public class GetCollectionByIdHandler : IRequestHandler<GetCollectionByIdQuery, Result<CollectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCollectionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CollectionDto>> Handle(GetCollectionByIdQuery request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.FindAsync(c => c.Id == request.Id);
        if (collection == null)
        {
            return Result<CollectionDto>.Failure(Error.NotFound);
        }

        var collectionDto = _mapper.Map<CollectionDto>(collection);
        return Result<CollectionDto>.Success(collectionDto);
    }
}
