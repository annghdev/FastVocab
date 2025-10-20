using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;
