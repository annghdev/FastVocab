using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.UpdateWordList;

public class UpdateWordListHandler : IRequestHandler<UpdateWordListCommand, Result<WordListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateWordListHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WordListDto>> Handle(UpdateWordListCommand request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Collections.GetWithWordListsAsync(request.CollectionId);
        if (collection == null)
        {
            return Result<WordListDto>.Failure(Error.NotFound);
        }
        var wordList = collection.WordLists?.FirstOrDefault(wl => wl.Id == request.Request.Id);
        if (wordList == null)
        {
            return Result<WordListDto>.Failure(Error.NotFound);
        }

        // Check name unique in collection
        if (wordList.Name != request.Request.Name)
        {
            var duplicate = collection.WordLists.Any(l=>l.Name == request.Request.Name);
            if (duplicate)
            {
                return Result<WordListDto>.Failure(Error.Duplicate);
            }
        }

        _mapper.Map(request.Request, wordList);

        _unitOfWork.Collections.Update(collection);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<WordListDto>(wordList);
        return Result<WordListDto>.Success(dto);
    }
}
