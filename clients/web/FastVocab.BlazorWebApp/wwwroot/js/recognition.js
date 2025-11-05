window.speechRecognizer = {
    recognition: null,
    dotnetHelper: null,
    isInitialized: false,

    init: function (dotnetHelper, lang) {
        this.dotnetHelper = dotnetHelper;

        // Kiểm tra trình duyệt hỗ trợ
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        if (!SpeechRecognition) {
            this.dotnetHelper.invokeMethodAsync('RecognitionError', 'Trình duyệt không hỗ trợ nhận dạng giọng nói.');
            return;
        }

        // Chỉ khởi tạo một lần
        if (!this.recognition) {
            this.recognition = new SpeechRecognition();

            // Event: Khi có kết quả
            this.recognition.onresult = (event) => {
                // Lấy kết quả cuối cùng (khi người dùng ngừng nói)
                const transcript = event.results[event.results.length - 1][0].transcript;
                this.dotnetHelper.invokeMethodAsync('SetRecognitionResult', transcript.trim());
            };

            // Event: Khi có lỗi
            this.recognition.onerror = (event) => {
                this.dotnetHelper.invokeMethodAsync('RecognitionError', event.error);
            };

            // Event: Khi bắt đầu nghe
            this.recognition.onstart = () => {
                this.dotnetHelper.invokeMethodAsync('SetListeningState', true);
            };

            // Event: Khi kết thúc (tự động khi ngừng nói, hoặc gọi stop())
            this.recognition.onend = () => {
                this.dotnetHelper.invokeMethodAsync('SetListeningState', false);
            };
        }

        // Cài đặt cho mỗi lần gọi
        this.recognition.lang = lang;
        this.recognition.interimResults = false; // Chỉ lấy kết quả cuối cùng
        this.recognition.maxAlternatives = 1;
        this.isInitialized = true;
    },

    start: function (lang) {
        if (!this.isInitialized) {
            this.dotnetHelper.invokeMethodAsync('RecognitionError', 'Chưa khởi tạo.');
            return;
        }

        // Cập nhật ngôn ngữ (nếu đổi)
        this.recognition.lang = lang;

        try {
            // Bắt đầu nghe
            this.recognition.start();
        } catch (e) {
            // Xử lý lỗi (ví dụ: đang nghe mà bấm start lần nữa)
            console.error("Lỗi khi bắt đầu nhận dạng:", e.message);
        }
    },

    stop: function () {
        if (this.isInitialized) {
            this.recognition.stop();
        }
    }
};