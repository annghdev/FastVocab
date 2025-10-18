namespace FastVocab.Shared.Utils;

public class PagedList<T>(int page, int size, IEnumerable<T> items, int totalCount)
{
    public int Page { get; } = page;
    public int Size { get; } = size;
    public IEnumerable<T> Items { get; } = items;
    public int TotalCount { get; } = totalCount;
    public int TotalPage => (int)Math.Ceiling((double)TotalCount / Size);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPage;
}
