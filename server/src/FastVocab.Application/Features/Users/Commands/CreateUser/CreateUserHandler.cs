using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Users.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.AccountId.HasValue)
        {
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.AccountId == request.Request.AccountId.Value);
            if (existingUser != null)
            {
                return Result<UserDto>.Failure(Error.Conflict);
            }
        }
        
        var user = _mapper.Map<AppUser>(request.Request);
        
        _unitOfWork.Users.Add(user);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var userDto = _mapper.Map<UserDto>(user);
        
        return Result<UserDto>.Success(userDto);
    }
}
