using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetWordById;

/// <summary>
/// Handler for GetWordByIdQuery
/// </summary>
public class GetWordByIdHandler : IRequestHandler<GetWordByIdQuery, Result<WordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWordByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WordDto>> Handle(GetWordByIdQuery request, CancellationToken cancellationToken)
    {
        // Find word by ID
        var word = await _unitOfWork.Words.FindAsync(request.WordId);

        if (word == null)
        {
            return Result<WordDto>.Failure(Error.NotFound);
        }

        // Check if deleted
        if (word.IsDeleted)
        {
            return Result<WordDto>.Failure(Error.Deleted);
        }

        // Map to DTO
        var wordDto = _mapper.Map<WordDto>(word);

        return Result<WordDto>.Success(wordDto);
    }
}

