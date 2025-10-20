using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Queries.GetUserBySessionId;

public class GetUserBySessionIdHandler : IRequestHandler<GetUserBySessionIdQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserBySessionIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserBySessionIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(u => u.SessionId == request.SessionId);

        if (user == null)
        {
            return Result<UserDto>.Failure(Error.NotFound);
        }

        if (user.IsDeleted)
        {
            return Result<UserDto>.Failure(Error.Deleted);
        }

        var userDto = _mapper.Map<UserDto>(user);

        return Result<UserDto>.Success(userDto);
    }
}
