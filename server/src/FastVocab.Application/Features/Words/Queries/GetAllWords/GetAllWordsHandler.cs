using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetAllWords;

/// <summary>
/// Handler for GetAllWordsQuery
/// </summary>
public class GetAllWordsHandler : IRequestHandler<GetAllWordsQuery, IEnumerable<WordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWordsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WordDto>> Handle(GetAllWordsQuery request, CancellationToken cancellationToken)
    {
        // Get all words that are not deleted
        var words = await _unitOfWork.Words.GetAllAsync(cancellationToken: cancellationToken);

        // Map to DTOs
        var wordDtos = _mapper.Map<IEnumerable<WordDto>>(words);

        return wordDtos;
    }
}

