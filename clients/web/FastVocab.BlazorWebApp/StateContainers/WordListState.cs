using FastVocab.Shared.DTOs.Words;

namespace FastVocab.BlazorWebApp.StateContainers;

public class WordListState
{
    public List<WordDto> Data { get; set; } = [];
}
