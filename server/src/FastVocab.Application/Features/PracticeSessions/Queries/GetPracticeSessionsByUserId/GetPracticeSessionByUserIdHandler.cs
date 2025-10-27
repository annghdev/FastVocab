using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Practice;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Queries.GetPracticeSessionsByUserId;

public class GetPracticeSessionByUserIdHandler : IRequestHandler<GetPracticeSessionsByUserIdQuery, IEnumerable<PracticeSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPracticeSessionByUserIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PracticeSessionDto>> Handle(GetPracticeSessionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var practices = await _unitOfWork.PracticeSessions.GetAllAsync(p => p.UserId == request.UserId, cancellationToken);

        return _mapper.Map<IEnumerable<PracticeSessionDto>>(practices);
    }
}
