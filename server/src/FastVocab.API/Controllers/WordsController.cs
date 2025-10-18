using FastVocab.Application.Features.Words.Commands.AddWordToTopic;
using FastVocab.Application.Features.Words.Commands.CreateWord;
using FastVocab.Application.Features.Words.Commands.DeleteWord;
using FastVocab.Application.Features.Words.Commands.RemoveWordFromTopic;
using FastVocab.Application.Features.Words.Commands.UpdateWord;
using FastVocab.Application.Features.Words.Queries.GetAllWords;
using FastVocab.Application.Features.Words.Queries.GetWordById;
using FastVocab.Application.Features.Words.Queries.GetWordsByLevel;
using FastVocab.Application.Features.Words.Queries.GetWordsByTopic;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastVocab.API.Controllers;

/// <summary>
/// Controller for managing Words (Vocabulary)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WordsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WordsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all words
    /// </summary>
    /// <returns>List of all words</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllWordsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get word by ID
    /// </summary>
    /// <param name="id">Word ID</param>
    /// <returns>Word details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetWordByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title });
    }

    /// <summary>
    /// Get words by topic
    /// </summary>
    /// <param name="topicId">Topic ID</param>
    /// <returns>List of words in topic</returns>
    [HttpGet("topic/{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTopic(int topicId, CancellationToken cancellationToken)
    {
        var query = new GetWordsByTopicQuery(topicId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title });
    }

    /// <summary>
    /// Get words by difficulty level
    /// </summary>
    /// <param name="level">Difficulty level (A1, A2, B1, B2, C1, C2)</param>
    /// <returns>List of words at specified level</returns>
    [HttpGet("level/{level}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLevel(string level, CancellationToken cancellationToken)
    {
        var query = new GetWordsByLevelQuery(level);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title });
    }

    /// <summary>
    /// Create a new word
    /// </summary>
    /// <param name="request">Word creation data</param>
    /// <returns>Created word</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWordRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateWordCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.Id },
                result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    /// <summary>
    /// Update an existing word
    /// </summary>
    /// <param name="id">Word ID</param>
    /// <param name="request">Word update data</param>
    /// <returns>Updated word</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWordRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest("ID in URL does not match ID in request body.");
        }

        var command = new UpdateWordCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    /// <summary>
    /// Delete a word (soft delete)
    /// </summary>
    /// <param name="id">Word ID</param>
    /// <returns>Success or error message</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteWordCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    /// <summary>
    /// Add word to topic
    /// </summary>
    /// <param name="wordId">Word ID</param>
    /// <param name="topicId">Topic ID</param>
    /// <returns>Success or error message</returns>
    [HttpPost("{wordId}/topics/{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToTopic(int wordId, int topicId, CancellationToken cancellationToken)
    {
        var command = new AddWordToTopicCommand(wordId, topicId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { message = "Word added to topic successfully." });
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    /// <summary>
    /// Remove word from topic
    /// </summary>
    /// <param name="wordId">Word ID</param>
    /// <param name="topicId">Topic ID</param>
    /// <returns>Success or error message</returns>
    [HttpDelete("{wordId}/topics/{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromTopic(int wordId, int topicId, CancellationToken cancellationToken)
    {
        var command = new RemoveWordFromTopicCommand(wordId, topicId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { message = "Word removed from topic successfully." });
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }
}

