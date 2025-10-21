using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.CreateWordList;

/// <summary>
/// Handler for CreateWordListCommand
/// </summary>
public class CreateWordListHandler : IRequestHandler<CreateWordListCommand, Result<WordListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWordListHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WordListDto>> Handle(CreateWordListCommand request, CancellationToken cancellationToken)
    {
        // Check if collection exists
        var collection = await _unitOfWork.Collections.GetWithWordListsAsync(request.Request.CollectionId);
        if (collection == null)
        {
            return Result<WordListDto>.Failure(Error.NotFound);
        }

        // Check if name is unique within collection
        if (collection.WordLists.Any(l=> l.Name == request.Request.Name))
        {
            return Result<WordListDto>.Failure(Error.Duplicate);
        }

        // Map to entity
        var wordList = _mapper.Map<WordList>(request.Request);
        collection.WordLists?.Add(wordList);
        _unitOfWork.Collections.Update(collection);

        // Save
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var wordListDto = _mapper.Map<WordListDto>(wordList);
        return Result<WordListDto>.Success(wordListDto);
    }
}
