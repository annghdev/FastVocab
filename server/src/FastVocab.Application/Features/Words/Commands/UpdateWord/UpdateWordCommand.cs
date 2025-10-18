using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.UpdateWord;

/// <summary>
/// Command to update an existing Word
/// </summary>
public record UpdateWordCommand(UpdateWordRequest Request) : IRequest<Result<WordDto>>;

