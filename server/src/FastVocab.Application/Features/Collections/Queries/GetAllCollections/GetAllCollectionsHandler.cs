using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Collections;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.GetAllCollections;

/// <summary>
/// Handler for GetAllCollectionsQuery
/// </summary>
public class GetAllCollectionsHandler : IRequestHandler<GetAllCollectionsQuery, IEnumerable<CollectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCollectionsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CollectionDto>> Handle(GetAllCollectionsQuery request, CancellationToken cancellationToken)
    {
        // Get all collections that are not deleted
        var collections = await _unitOfWork.Collections.GetAllAsync(cancellationToken: cancellationToken);

        // Map to DTOs
        var collectionDtos = _mapper.Map<IEnumerable<CollectionDto>>(collections);

        return collectionDtos;
    }
}
