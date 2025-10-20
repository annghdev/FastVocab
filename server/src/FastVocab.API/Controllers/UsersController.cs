using FastVocab.Application.Features.Users.Commands.CreateUser;
using FastVocab.Application.Features.Users.Commands.DeleteUser;
using FastVocab.Application.Features.Users.Commands.UpdateUser;
using FastVocab.Application.Features.Users.Queries.GetAllUsers;
using FastVocab.Application.Features.Users.Queries.GetUserById;
using FastVocab.Application.Features.Users.Queries.GetUserBySessionId;
using FastVocab.Shared.DTOs.Users.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastVocab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : NotFound();
    }

    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetBySessionId(string sessionId, CancellationToken cancellationToken)
    {
        var query = new GetUserBySessionIdQuery(sessionId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(request);
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest("ID mismatch");
        }

        var command = new UpdateUserCommand(request);
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound();
    }
}
