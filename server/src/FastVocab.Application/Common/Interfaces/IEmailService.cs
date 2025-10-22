using FastVocab.Application.Common.Models;

namespace FastVocab.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(SendEmailRequest request);
}
