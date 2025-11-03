using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Practice;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.PracticeSessions.Commands.CreatePracticeSession;

public class CreatePracticeSessionHandler : IRequestHandler<CreatePracticeSessionCommand, Result<PracticeSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePracticeSessionHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PracticeSessionDto>> Handle(CreatePracticeSessionCommand request, CancellationToken cancellationToken)
    {
        var practiceSession = await _unitOfWork.PracticeSessions
            .FindAsync(p => p.UserId == request.Request.UserId && p.ListId == request.Request.ListId);

        if (practiceSession != null)
        {
            return Result<PracticeSessionDto>.Failure(Error.Duplicate);
        }

        practiceSession = new PracticeSesssion
        {
            UserId = request.Request.UserId,
            ListId = request.Request.ListId,
        };

        var dto = _mapper.Map<PracticeSessionDto>(practiceSession);
        return Result<PracticeSessionDto>.Success(dto);
    }
}
