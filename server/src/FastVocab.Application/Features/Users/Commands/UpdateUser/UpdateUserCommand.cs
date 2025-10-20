using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.DTOs.Users.Requests;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(UpdateUserRequest Request) : IRequest<Result<UserDto>>;
