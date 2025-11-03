using FastVocab.Application.Features.PracticeSessions.Commands.CreatePracticeSession;
using FastVocab.Application.Features.PracticeSessions.Commands.SubmitPracticeSession;
using FastVocab.Application.Features.PracticeSessions.Queries.GetAllPracticeSessions;
using FastVocab.Application.Features.PracticeSessions.Queries.GetPracticeSessionsByUserId;
using FastVocab.Application.Features.PracticeSessions.Queries.GetPracticSessionById;
using FastVocab.Shared.DTOs.Practice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastVocab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PracticeSessionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PracticeSessionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPracticeSessionQuery());

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _mediator.Send(new GetPracticeSessionsByUserIdQuery(userId));

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetPracticeSessionByIdQuery(id));

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return NotFound(result.Errors?.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePracticeSessionRequest request)
        {
            var result = await _mediator.Send(new CreatePracticeSessionCommand(request));

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Errors?.FirstOrDefault());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Submit(int id, [FromBody] Guid userId)
        {
            var result = await _mediator.Send(new SubmitPracticeSessionCommand(id, userId));

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Errors?.FirstOrDefault());
        }
    }
}
