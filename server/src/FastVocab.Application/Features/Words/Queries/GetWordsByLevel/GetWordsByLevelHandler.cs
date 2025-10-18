using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetWordsByLevel;

/// <summary>
/// Handler for GetWordsByLevelQuery
/// </summary>
public class GetWordsByLevelHandler : IRequestHandler<GetWordsByLevelQuery, Result<IEnumerable<WordDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWordsByLevelHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<WordDto>>> Handle(GetWordsByLevelQuery request, CancellationToken cancellationToken)
    {
        // Get words by level
        var words = await _unitOfWork.Words.GetAllAsync(
            predicate: w => !w.IsDeleted && w.Level == request.Level,
            cancellationToken: cancellationToken);

        // Map to DTOs
        var wordDtos = _mapper.Map<IEnumerable<WordDto>>(words);

        return Result<IEnumerable<WordDto>>.Success(wordDtos);
    }
}

