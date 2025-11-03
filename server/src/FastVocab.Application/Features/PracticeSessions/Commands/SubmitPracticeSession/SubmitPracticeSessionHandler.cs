using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Practice;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Commands.SubmitPracticeSession;

public class SubmitPracticeSessionHandler : IRequestHandler<SubmitPracticeSessionCommand, Result<PracticeSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubmitPracticeSessionHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PracticeSessionDto>> Handle(SubmitPracticeSessionCommand request, CancellationToken cancellationToken)
    {
        var practiceSession = await _unitOfWork.PracticeSessions.FindAsync(request.Id);
        if (practiceSession == null)
        {
            return Result<PracticeSessionDto>.Failure(Error.NotFound);
        }
        practiceSession.Submit();

        _unitOfWork.PracticeSessions.Update(practiceSession);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PracticeSessionDto>(practiceSession);

        return Result<PracticeSessionDto>.Success(dto);
    }
}