namespace FastVocab.Application.Common.Interfaces;

public interface ICacheInvalidatorRequest
{
    public IEnumerable<string> CacheKeysToInvalidate { get; }
    public IEnumerable<string>? CacheKeysPattern { get; }
}
