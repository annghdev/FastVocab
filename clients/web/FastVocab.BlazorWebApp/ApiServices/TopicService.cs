using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.DTOs.Topics;

namespace FastVocab.BlazorWebApp.ApiServices
{
    public class TopicService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public TopicService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            string apiUrl = configuration["ApiUrl"] ?? "https://localhost:5001/api";
            _baseUrl = $"{apiUrl}/topics";
        }

        #region READ
        public async Task<List<TopicDto>> GetAllAsync()
        {
            var res = await _httpClient.GetFromJsonAsync<List<TopicDto>>(_baseUrl);
            return res ?? [];
        }
        #endregion

        #region WRITE
        public async Task<TopicDto?> CreateAsync(CreateTopicRequest request)
        {
            var res = await _httpClient.PostAsJsonAsync(_baseUrl, request);
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<TopicDto>();
                return result;
            }
            var error = await res.Content.ReadAsStringAsync();
            Console.WriteLine(error);
            return null;
        }

        public async Task<bool> UpdateAsync(UpdateTopicRequest request)
        {
            var res = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{request.Id}", request);
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

        #endregion
    }
}
