using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.GetCollectionWithLists;

/// <summary>
/// Handler for GetCollectionWithListsQuery
/// </summary>
public class GetCollectionWithListsHandler : IRequestHandler<GetCollectionWithListsQuery, Result<CollectionDto>>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IMapper _mapper;

    public GetCollectionWithListsHandler(ICollectionRepository collectionRepository, IMapper mapper)
    {
        _collectionRepository = collectionRepository;
        _mapper = mapper;
    }

    public async Task<Result<CollectionDto>> Handle(GetCollectionWithListsQuery request, CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.GetWithWordListsAsync(request.Id, cancellationToken);
        if (collection == null)
        {
            return Result<CollectionDto>.Failure(Error.NotFound);
        }

        var collectionDto = _mapper.Map<CollectionDto>(collection);
        return Result<CollectionDto>.Success(collectionDto);
    }
}
