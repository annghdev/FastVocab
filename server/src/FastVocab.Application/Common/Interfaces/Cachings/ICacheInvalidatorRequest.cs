namespace FastVocab.Application.Common.Interfaces;

public interface ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate { get; }
    public string? Prefix { get; }
}
