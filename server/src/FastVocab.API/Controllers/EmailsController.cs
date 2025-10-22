using FastVocab.Application.Common.Interfaces;
using FastVocab.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastVocab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService _service;

        public EmailsController(IEmailService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> TestSendEmail()
        {
            var request = new SendEmailRequest();
            request.ToEmail = "annghdev@gmail.com";
            request.Template = "Welcome.cshtml";
            request.Model = new { Name = "An", ConfirmUrl = "chuacoconfirmurl" };
            request.Subject = "Test Send Email";
            await _service.SendEmailAsync(request);
            return Ok();
        }
    }
}
