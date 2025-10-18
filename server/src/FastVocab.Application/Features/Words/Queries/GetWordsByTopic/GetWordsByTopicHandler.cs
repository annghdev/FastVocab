using AutoMapper;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Queries.GetWordsByTopic;

/// <summary>
/// Handler for GetWordsByTopicQuery
/// </summary>
public class GetWordsByTopicHandler : IRequestHandler<GetWordsByTopicQuery, Result<IEnumerable<WordDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWordsByTopicHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<WordDto>>> Handle(GetWordsByTopicQuery request, CancellationToken cancellationToken)
    {
        // Verify topic exists
        var topic = await _unitOfWork.Topics.FindAsync(request.TopicId);
        if (topic == null || topic.IsDeleted)
        {
            return Result<IEnumerable<WordDto>>.Failure(Error.NotFound);
        }

        // Get words by topic using repository method
        var words = await _unitOfWork.Words.GetByTopic(request.TopicId);

        // Filter out deleted words
        var activeWords = words?.Where(w => !w.IsDeleted) ?? Enumerable.Empty<Domain.Entities.CoreEntities.Word>();

        // Map to DTOs
        var wordDtos = _mapper.Map<IEnumerable<WordDto>>(activeWords);

        return Result<IEnumerable<WordDto>>.Success(wordDtos);
    }
}

