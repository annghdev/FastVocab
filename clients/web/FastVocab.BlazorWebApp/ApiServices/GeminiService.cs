using GenerativeAI;
using GenerativeAI.Core;
using GenerativeAI.Types;

namespace FastVocab.BlazorWebApp.ApiServices;

public class GeminiService
{
    private readonly GenerativeModel _model;

    public GeminiService(IConfiguration configuration)
    {
        // Lấy key từ appsettings.json
        var apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;

        _model = new GenerativeModel(
            model: "gemini-2.5-flash",
            apiKey: apiKey
        );
    }
    public async Task<string> GenerateResponseAsync(string prompt)
    {
        try
        {
            // Hàm này vẫn gọi tương tự
            var response = await _model.GenerateContentAsync(prompt);

            // THAY ĐỔI NHỎ: Dùng .Text() thay vì .Text
            return response.Text() ?? "response error!";
        }
        catch (Exception ex)
        {
            return $"Lỗi: {ex.Message}";
        }
    }
}
