namespace FastVocab.Shared.Utils;

public abstract class QueryRequestBase
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public string? SearchBy { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public IEnumerable<SortOption>? SortExtends { get; set; }
}
