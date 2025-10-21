using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Collections.Commands.DeleteCollection;

/// <summary>
/// Command to delete a Collection (soft delete)
/// </summary>
public record DeleteCollectionCommand(int Id) : IRequest<Result>;
