using FastVocab.BlazorWebApp.ApiServices;
using FastVocab.BlazorWebApp.JSHelpers;
using FastVocab.BlazorWebApp.StateContainers;

namespace FastVocab.BlazorWebApp;

public static class RegistrationExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddAntDesign();

        // Api Services
        services.AddScoped<WordService>();
        services.AddScoped<TopicService>();

        // State Containers
        services.AddScoped<WordListState>();
        services.AddTransient<TextToSpeechHepler>();
        services.AddTransient<SpeechToTextHepler>();
    }
}