using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.CreateWord;

/// <summary>
/// Handler for CreateWordCommand
/// </summary>
public class CreateWordHandler : IRequestHandler<CreateWordCommand, Result<WordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWordHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WordDto>> Handle(CreateWordCommand request, CancellationToken cancellationToken)
    {
        // Check if word with same text already exists
        var existingWord = await _unitOfWork.Words.FindAsync(w => w.Text == request.Request.Text);
        if (existingWord != null)
        {
            return Result<WordDto>.Failure(Error.NameExists);
        }

        // Map request to entity
        var word = _mapper.Map<Word>(request.Request);

        // Add to repository
        _unitOfWork.Words.Add(word);

        // Save to get the Word ID
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Associate with topics if provided
        if (request.Request.TopicIds != null && request.Request.TopicIds.Any())
        {
            foreach (var topicId in request.Request.TopicIds)
            {
                // Verify topic exists
                var topic = await _unitOfWork.Topics.FindAsync(topicId);
                if (topic == null || topic.IsDeleted)
                {
                    return Result<WordDto>.Failure(Error.NotFound);
                }

                // Create WordTopic association
                var wordTopic = new WordTopic
                {
                    WordId = word.Id,
                    TopicId = topicId
                };
                
                // Add through DbContext (assume we have access or use repository pattern)
                // For now, we'll need to add this to the word's navigation property
                word.Topics ??= new List<WordTopic>();
                word.Topics.Add(wordTopic);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Map to DTO and return success
        var wordDto = _mapper.Map<WordDto>(word);
        return Result<WordDto>.Success(wordDto);
    }
}

