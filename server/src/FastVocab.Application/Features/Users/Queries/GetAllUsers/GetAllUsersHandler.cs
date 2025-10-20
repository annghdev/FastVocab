using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Users;
using MediatR;

namespace FastVocab.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(predicate: null, cancellationToken);
        
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return userDtos;
    }
}
