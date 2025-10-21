using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.AddWordToList;

public record AddWordToListCommand(int CollectionId,AddWordToListRequest Request) : IRequest<Result>;
