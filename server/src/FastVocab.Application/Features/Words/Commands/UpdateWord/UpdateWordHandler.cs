using AutoMapper;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.UpdateWord;

/// <summary>
/// Handler for UpdateWordCommand
/// </summary>
public class UpdateWordHandler : IRequestHandler<UpdateWordCommand, Result<WordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateWordHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WordDto>> Handle(UpdateWordCommand request, CancellationToken cancellationToken)
    {
        // Find existing word
        var word = await _unitOfWork.Words.GetWithInfoAsync(
            w => w.Id == request.Request.Id,
            w => w.Topics!);

        if (word == null)
        {
            return Result<WordDto>.Failure(Error.NotFound);
        }

        // Check if soft deleted
        if (word.IsDeleted)
        {
            return Result<WordDto>.Failure(Error.Deleted);
        }

        // Check if text already exists (excluding current word)
        var existingWord = await _unitOfWork.Words.FindAsync(w => 
            w.Text == request.Request.Text && w.Id != request.Request.Id);
        if (existingWord != null)
        {
            return Result<WordDto>.Failure(Error.Duplicate);
        }

        // Update properties
        word.Text = request.Request.Text;
        word.Meaning = request.Request.Meaning;
        word.Definition = request.Request.Definition;
        word.Type = request.Request.Type;
        word.Level = request.Request.Level;
        word.Example1 = request.Request.Example1;
        word.Example2 = request.Request.Example2;
        word.Example3 = request.Request.Example3;
        word.ImageUrl = request.Request.ImageUrl;
        word.AudioUrl = request.Request.AudioUrl;

        // Update topic associations if provided
        if (request.Request.TopicIds != null)
        {
            // Clear existing topics
            word.Topics?.Clear();

            // Add new topics
            foreach (var topicId in request.Request.TopicIds)
            {
                // Verify topic exists
                var topic = await _unitOfWork.Topics.FindAsync(topicId);
                if (topic == null || topic.IsDeleted)
                {
                    return Result<WordDto>.Failure(Error.NotFound);
                }

                word.Topics ??= new List<WordTopic>();
                word.Topics.Add(new WordTopic
                {
                    WordId = word.Id,
                    TopicId = topicId
                });
            }
        }

        // Update in repository
        _unitOfWork.Words.Update(word);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO and return success
        var wordDto = _mapper.Map<WordDto>(word);
        return Result<WordDto>.Success(wordDto);
    }
}

