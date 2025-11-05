using Microsoft.JSInterop;

namespace FastVocab.BlazorWebApp.JSHelpers;

public class TextToSpeechHepler
{
    private readonly IJSRuntime _js;

    public string Language { get; set; } = "en-US";
    public string? VoiceName { get; set; }
    public double Rate { get; set; } = 1.0;
    public double Pitch { get; set; } = 1.0;

    public TextToSpeechHepler(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SpeakAsync(string word, string lang = "en-US", double rate = 1.0, string? voiceName = null)
    {
        var options = new { lang, rate, voiceName, pitch = Pitch };
        await _js.InvokeVoidAsync("speechHelper.speakWord", word, options); 
    }

    public async Task<List<SpeechVoice>> GetVoicesAsync()
    {
        return await _js.InvokeAsync<List<SpeechVoice>>("speechHelper.getVoices");
    }

    public async Task PauseAsync()
    {
        await _js.InvokeVoidAsync("speechHelper.pause");
    }

    public async Task ResumeAsync()
    {
        await _js.InvokeVoidAsync("speechHelper.resume");
    }

    public async Task StopAsync()
    {
        await _js.InvokeVoidAsync("speechHelper.stop");
    }
}

public class SpeechVoice
{
    public string Name { get; set; } = "";
    public string Lang { get; set; } = "";
    public bool Default { get; set; }
}