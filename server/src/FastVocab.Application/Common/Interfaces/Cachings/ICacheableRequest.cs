namespace FastVocab.Application.Common.Interfaces;

public interface ICacheableRequest
{
    string? CacheKey { get; }
    TimeSpan? ExpirationSliding { get; }
}
