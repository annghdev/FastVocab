using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.DTOs.Users.Requests;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(CreateUserRequest Request) : IRequest<Result<UserDto>>;
