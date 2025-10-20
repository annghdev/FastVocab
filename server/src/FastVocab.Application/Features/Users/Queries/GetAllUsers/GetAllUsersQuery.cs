using FastVocab.Shared.DTOs.Users;
using MediatR;

namespace FastVocab.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
