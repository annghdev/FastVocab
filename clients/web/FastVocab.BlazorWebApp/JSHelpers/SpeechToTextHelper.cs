using Microsoft.JSInterop;

// Đặt tên namespace phù hợp
namespace FastVocab.BlazorWebApp.JSHelpers;

public class SpeechToTextHepler : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<SpeechToTextHepler>? _objRef;

    // 1. Định nghĩa các Events để thông báo cho component
    public event Action<string>? OnResultReceived;
    public event Action<bool>? OnStateChanged;
    public event Action<string>? OnError;

    public SpeechToTextHepler(IJSRuntime js)
    {
        _js = js;
    }

    // Component sẽ gọi hàm này trước tiên
    public async Task InitializeAsync()
    {
        if (_objRef == null)
        {
            _objRef = DotNetObjectReference.Create(this);
            // Truyền tham chiếu của service này cho JS
            await _js.InvokeVoidAsync("speechRecognizer.init", _objRef);
        }
    }

    // 2. Các phương thức public cho component gọi
    public async Task StartAsync(string language)
    {
        await _js.InvokeVoidAsync("speechRecognizer.start", language);
    }

    public async Task StopAsync()
    {
        await _js.InvokeVoidAsync("speechRecognizer.stop");
    }

    // ==========================================================
    // CÁC HÀM [JSInvokable] (JS sẽ gọi các hàm này)
    // ==========================================================

    [JSInvokable]
    public void SetRecognitionResult(string text)
    {
        // 3. Khi nhận kết quả, kích hoạt event
        OnResultReceived?.Invoke(text);
    }

    [JSInvokable]
    public void SetListeningState(bool isListening)
    {
        // 3. Khi đổi trạng thái, kích hoạt event
        OnStateChanged?.Invoke(isListening);
    }

    [JSInvokable]
    public void RecognitionError(string error)
    {
        // 3. Khi có lỗi, kích hoạt event
        // (Xử lý thông điệp lỗi ngay tại đây cho sạch)
        string friendlyError = error switch
        {
            "no-speech" => "Không phát hiện thấy giọng nói. Vui lòng thử lại.",
            "audio-capture" => "Không tìm thấy micro hoặc không có quyền truy cập.",
            "not-allowed" => "Bạn đã chặn quyền truy cập micro.",
            "network" => "Đã xảy ra lỗi mạng.",
            _ => $"Đã xảy ra lỗi: {error}"
        };
        OnError?.Invoke(friendlyError);
        OnStateChanged?.Invoke(false); // Thường thì lỗi là kết thúc nghe
    }

    // Dọn dẹp
    public async ValueTask DisposeAsync()
    {
        if (_objRef != null)
        {
            _objRef.Dispose();
        }
        // Bạn cũng có thể thêm hàm JS để dọn dẹp (nếu cần)
    }
}