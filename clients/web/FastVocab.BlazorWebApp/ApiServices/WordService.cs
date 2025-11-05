using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.DTOs.Words;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FastVocab.BlazorWebApp.ApiServices;

public class WordService
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public WordService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        string apiUrl = configuration["ApiUrl"] ?? "https://localhost:5001/api";
        _baseUrl = $"{apiUrl}/words";
    }

    #region READ

    public async Task<List<WordDto>> GetAllAsync()
    {
        var res = await _httpClient.GetFromJsonAsync<List<WordDto>>(_baseUrl);
        return res ?? [];
    }

    public async Task<List<WordDto>> GetByTopic(int topicId)
    {
        var res = await _httpClient.GetFromJsonAsync<List<WordDto>>($"{_baseUrl}/topic/{topicId}");
        return res ?? [];
    }

    #endregion

    #region WRITE

    public async Task<WordDto?> CreateAsync(CreateWordRequest word)
    {
        var res = await _httpClient.PostAsJsonAsync(_baseUrl, word);
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadFromJsonAsync<WordDto>();
            return result;
        }
        var error = await res.Content.ReadAsStringAsync();
        Console.WriteLine(error);
        return null;
    }

    public async Task<bool> UpdateAsync(WordDto word)
    {
        var req = new CreateWordRequest
        {
            Text = word.Text,
            Meaning = word.Meaning,
            Type = word.Type,
            Level = word.Level,
            Definition = word.Definition,
            Example1 = word.Example1,
            Example2 = word.Example2,
            Example3 = word.Example3,
        };
        var res = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{word.Id}", req);
        if (res.IsSuccessStatusCode)
        {
            return true;
        }
        var error = await res.Content.ReadAsStringAsync();
        Console.WriteLine(error);
        return false;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var res = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
        if (res.IsSuccessStatusCode)
        {
            return true;
        }
        var error = await res.Content.ReadAsStringAsync();
        Console.WriteLine(error);
        return false;
    }

    public async Task<ImportExcelResult<WordDto>?> ImportFromExcel(IBrowserFile file)
    {
        if (file == null)
            return null;

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var res = await _httpClient.PostAsync($"{_baseUrl}/import", content);

        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadFromJsonAsync<ImportExcelResult<WordDto>>();
            return result ?? null;
        }

        var error = await res.Content.ReadAsStringAsync();
        Console.WriteLine(error);
        return null;
    }

    public async Task<bool> AddToTopic(int id, int topicId)
    {
        var res = await _httpClient.PostAsync($"{_baseUrl}/{id}/topics/{topicId}", null);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveFromTopic(int id, int topicId)
    {
        var res = await _httpClient.PutAsync($"{_baseUrl}/{id}/topics/{topicId}", null);
        return res.IsSuccessStatusCode;
    }

    #endregion
}
