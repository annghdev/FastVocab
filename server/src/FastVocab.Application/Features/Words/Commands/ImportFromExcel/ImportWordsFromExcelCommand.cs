using FastVocab.Application.Common.Interfaces;
using FastVocab.Shared.DTOs.Words;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastVocab.Application.Features.Words.Commands.ImportFromExcel;

public record ImportWordsFromExcelCommand(IFormFile File) :IRequest<ImportExcelResult<WordDto>> , ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => ["words_all"];

    public string? Prefix => "words:";
}
