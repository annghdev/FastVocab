using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Queries.WordLists.GetWordListById;

public record GetWordListByIdQuery(int CollectionId, int WordListId) : IRequest<Result<WordListDto>>;
