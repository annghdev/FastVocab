using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(request.Request.Id);

        if (user == null)
        {
            return Result<UserDto>.Failure(Error.NotFound);
        }

        if (user.IsDeleted)
        {
            return Result<UserDto>.Failure(Error.Deleted);
        }
        
        if (request.Request.AccountId.HasValue)
        {
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.AccountId == request.Request.AccountId.Value && u.Id != request.Request.Id);
            if (existingUser != null)
            {
                return Result<UserDto>.Failure(Error.Conflict);
            }
        }

        _mapper.Map(request.Request, user);

        _unitOfWork.Users.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(userDto);
    }
}
