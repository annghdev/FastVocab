using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.RemoveWordFromList;

public record RemoveWordFromListCommand(int CollectionId, int WordListId, int WordId) : IRequest<Result>;
