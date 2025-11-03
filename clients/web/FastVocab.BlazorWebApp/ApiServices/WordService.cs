using FastVocab.Shared.DTOs.Words;

namespace FastVocab.BlazorWebApp.ApiServices;

public class WordService
{
    private readonly string baseUrl;
    private readonly HttpClient _httpClient;

    public WordService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        string apiUrl = configuration["ApiUrl"] ?? "http://localhost:5001/api";
        baseUrl = $"{apiUrl}/words";
    }

    public async Task<List<WordDto>> GetAllAsync()
    {
        var res = await _httpClient.GetFromJsonAsync<List<WordDto>>(baseUrl);
        return res ?? [];
    }

    public async Task<WordDto?> CreateAsync(CreateWordRequest request)
    {
        var res = await _httpClient.PostAsJsonAsync(baseUrl, request);
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadFromJsonAsync<WordDto>();
            return result;
        }
        var error = await res.Content.ReadAsStringAsync();
        Console.WriteLine(error);
        return null;
    }

    public async Task<bool> UpdateAsync(UpdateWordRequest request)
    {
        var res = await _httpClient.PutAsJsonAsync($"{baseUrl}/{request.Id}", request);
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
        var res = await _httpClient.DeleteAsync($"{baseUrl}/{id}");
        if (res.IsSuccessStatusCode)
        {
            return true;
        }
        var error = await res.Content.ReadAsStringAsync();
        Console.WriteLine(error);
        return false;
    }
}
