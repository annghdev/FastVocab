using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Practice;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Queries.GetPracticSessionById;

public class GetPracticeSessionByIdHandler : IRequestHandler<GetPracticeSessionByIdQuery, Result<PracticeSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPracticeSessionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PracticeSessionDto>> Handle(GetPracticeSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var practiceSession = await _unitOfWork.PracticeSessions.FindAsync(request.Id);
        if (practiceSession == null)
        {
            return Result<PracticeSessionDto>.Failure(Error.NotFound);
        }

        var dto = _mapper.Map<PracticeSessionDto>(practiceSession);

        return Result<PracticeSessionDto>.Success(dto);
    }
}
