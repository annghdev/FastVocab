using FastVocab.Shared.Utils;

namespace FastVocab.Shared.DTOs.Words;

public class ImportExcelResult<T>
{
    public List<T> Data { get; set; } = [];
    public int SuccessItemCount => Data.Count;
    public Error? Error { get; set; }
    public bool IsSuccess => Error != Error.None;
    public List<string> ErrorDetails { get; set; } = [];
    public int ErrorItemCount => ErrorDetails.Count;
}
