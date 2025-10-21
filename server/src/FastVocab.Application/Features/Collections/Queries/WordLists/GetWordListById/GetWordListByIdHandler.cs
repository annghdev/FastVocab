using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.WordLists.GetWordListById;

public class GetWordListByIdHandler : IRequestHandler<GetWordListByIdQuery, Result<WordListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWordListByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WordListDto>> Handle(GetWordListByIdQuery request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.GetWithWordListsAsync(request.CollectionId);

        if (collection == null || !collection.WordLists!.Any(wl => wl.Id == request.WordListId))
        {
            return Result<WordListDto>.Failure(Error.NotFound);
        }

        var wordList = await _unitOfWork.Collections.GetWordListAsync(request.WordListId);

        if (wordList == null)
        {
            return Result<WordListDto>.Failure(Error.NotFound);
        }

        var data = _mapper.Map<WordListDto>(wordList);

        return Result<WordListDto>.Success(data);
    }
}
