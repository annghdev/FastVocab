using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Queries.GetUserBySessionId;

public record GetUserBySessionIdQuery(string SessionId) : IRequest<Result<UserDto>>;
