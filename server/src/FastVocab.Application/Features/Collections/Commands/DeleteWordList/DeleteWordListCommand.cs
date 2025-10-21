using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.DeleteWordList;

public record DeleteWordListCommand(int CollectionId, int WordListId) : IRequest<Result>;
