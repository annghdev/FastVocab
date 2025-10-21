using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.CreateWordList;

/// <summary>
/// Command to create a new WordList in a Collection
/// </summary>
public record CreateWordListCommand(CreateWordListRequest Request) : IRequest<Result<WordListDto>>;
