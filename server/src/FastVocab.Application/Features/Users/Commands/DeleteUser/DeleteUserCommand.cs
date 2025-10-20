using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<Result>;
