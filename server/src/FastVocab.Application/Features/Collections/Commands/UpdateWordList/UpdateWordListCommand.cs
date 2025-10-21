using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.UpdateWordList;

public record UpdateWordListCommand(int CollectionId, UpdateWordListRequest Request) : IRequest<Result<WordListDto>>;
