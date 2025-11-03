using FastVocab.BlazorWebApp.ApiServices;

namespace FastVocab.BlazorWebApp;

public static class RegistrationExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddAntDesign();
        services.AddScoped<WordService>();
    }
}