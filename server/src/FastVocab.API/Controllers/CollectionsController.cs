using FastVocab.Application.Features.Collections.Commands.CreateCollection;
using FastVocab.Application.Features.Collections.Commands.UpdateCollection;
using FastVocab.Application.Features.Collections.Commands.DeleteCollection;
using FastVocab.Application.Features.Collections.Commands.ToggleCollectionVisibility;
using FastVocab.Application.Features.Collections.Commands.CreateWordList;
using FastVocab.Application.Features.Collections.Queries.GetAllCollections;
using FastVocab.Application.Features.Collections.Queries.GetCollectionById;
using FastVocab.Application.Features.Collections.Queries.GetCollectionWithLists;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.DTOs.WordLists;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FastVocab.Application.Features.Collections.Commands.UpdateWordList;
using FastVocab.Application.Features.Collections.Commands.AddWordToList;
using FastVocab.Application.Features.Collections.Commands.RemoveWordFromList;
using FastVocab.Application.Features.Collections.Commands.DeleteWordList;
using FastVocab.Application.Features.Collections.Queries.WordLists.GetWordListById;

namespace FastVocab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CollectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    #region collection info base
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllCollectionsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetCollectionByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title });
    }

    [HttpGet("{id}/lists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWithLists(int id, CancellationToken cancellationToken)
    {
        var query = new GetCollectionWithListsQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCollectionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCollectionCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCollectionRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest("ID in URL does not match ID in request body.");
        }

        var command = new UpdateCollectionCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteCollectionCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    [HttpPatch("{id}/visibility")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleVisibility(int id, CancellationToken cancellationToken)
    {
        var command = new ToggleCollectionVisibilityCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }
    #endregion

    #region word lists

    [HttpGet("{id}/lists/{listId}")]
    public async Task<IActionResult> GetWordListWithDetails(int id, int listId)
    {
        var result = await _mediator.Send(new GetWordListByIdQuery(id, listId));
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return NotFound(result.Errors);
    }


    [HttpPost("{id}/lists")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWordList(int id, [FromBody] CreateWordListRequest request, CancellationToken cancellationToken)
    {
        if (id != request.CollectionId)
        {
            return BadRequest("Collection ID in URL does not match request.");
        }

        var command = new CreateWordListCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(CreateWordList), new { listId = result.Data.Id }, result.Data);
        }

        return BadRequest(new { message = result.Errors?.FirstOrDefault()?.Title, errors = result.Errors });
    }

    [HttpPut("{id}/lists/{listId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWordList(int id, int listId, [FromBody] UpdateWordListRequest request, CancellationToken cancellationToken)
    {
        if (listId != request.Id)
        {
            return BadRequest("ID mismatch");
        }

        var command = new UpdateWordListCommand(id, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("{id}/lists/{listId}/words")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddWordToList(int id, int listId, [FromBody] AddWordToListRequest request, CancellationToken cancellationToken)
    {
        if (listId != request.WordListId)
        {
            return BadRequest("ID mismatch");
        }

        var command = new AddWordToListCommand(id, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}/lists/{listId}/words/{wordId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveWordFromList(int id, int listId, int wordId, CancellationToken cancellationToken)
    {
        var command = new RemoveWordFromListCommand(listId, wordId, wordId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return NotFound(result.Errors);
    }

    [HttpDelete("{id}/lists/{listId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWordList(int id, int listId, CancellationToken cancellationToken)
    {
        var command = new DeleteWordListCommand(id, listId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return NotFound(result.Errors);
    }

    #endregion
}
