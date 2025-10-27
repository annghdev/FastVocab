using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Practice;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Queries.GetAllPracticeSessions;

public class GetAllPracticeSessionHandler : IRequestHandler<GetAllPracticeSessionQuery, IEnumerable<PracticeSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPracticeSessionHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PracticeSessionDto>> Handle(GetAllPracticeSessionQuery request, CancellationToken cancellationToken)
    {
        var practices = await _unitOfWork.PracticeSessions.GetAllAsync(cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<PracticeSessionDto>>(practices);
    }
}
