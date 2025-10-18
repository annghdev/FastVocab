using FastVocab.Application.Features.Topics.Commands.CreateTopic;
using FastVocab.Application.Features.Topics.Commands.DeleteTopic;
using FastVocab.Application.Features.Topics.Commands.ToggleTopicVisibility;
using FastVocab.Application.Features.Topics.Commands.UpdateTopic;
using FastVocab.Application.Features.Topics.Queries.GetAllTopics;
using FastVocab.Application.Features.Topics.Queries.GetTopicById;
using FastVocab.Application.Features.Topics.Queries.GetVisibleTopics;
using FastVocab.Shared.DTOs.Topics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastVocab.API.Controllers;

/// <summary>
/// Controller for managing Topics
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TopicsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TopicsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all topics (including hidden ones - for admin)
    /// </summary>
    /// <returns>List of all topics</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllTopicsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get only visible topics (for students)
    /// </summary>
    /// <returns>List of visible topics</returns>
    [HttpGet("visible")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVisible(CancellationToken cancellationToken)
    {
        var query = new GetVisibleTopicsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title });
    }

    /// <summary>
    /// Get topic by ID
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <returns>Topic details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetTopicByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title });
    }

    /// <summary>
    /// Create a new topic
    /// </summary>
    /// <param name="request">Topic creation data</param>
    /// <returns>Created topic</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTopicRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTopicCommand(request);
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
    /// Update an existing topic
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <param name="request">Topic update data</param>
    /// <returns>Updated topic</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTopicRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest("ID in URL does not match ID in request body.");
        }

        var command = new UpdateTopicCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    /// <summary>
    /// Delete a topic (soft delete)
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <returns>Success or error message</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteTopicCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    /// <summary>
    /// Toggle topic visibility (show/hide)
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <returns>Updated topic with new visibility status</returns>
    [HttpPatch("{id}/visibility")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleVisibility(int id, CancellationToken cancellationToken)
    {
        var command = new ToggleTopicVisibilityCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }
}

