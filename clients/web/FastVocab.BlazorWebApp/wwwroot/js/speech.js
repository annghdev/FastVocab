window.speechHelper = {
    speakWord: function (word, options) {
        if (!('speechSynthesis' in window)) {
            alert("Trình duyệt không hỗ trợ phát âm!");
            return;
        }

        if (speechSynthesis.speaking) {
            speechSynthesis.cancel();
        }

        const utterance = new SpeechSynthesisUtterance(word);
        if (options) {
            utterance.lang = options.lang || 'en-US';
            utterance.rate = options.rate || 1.0;
            utterance.pitch = options.pitch || 1.0;

            // ### THÊM MỚI START ###
            // Logic tìm và gán voice bằng tên
            if (options.voiceName) {
                const voices = speechSynthesis.getVoices();
                const selectedVoice = voices.find(v => v.name === options.voiceName);
                if (selectedVoice) {
                    utterance.voice = selectedVoice;
                    // Tùy chọn: ghi đè lang để khớp với voice
                    utterance.lang = selectedVoice.lang;
                }
            }
            // ### THÊM MỚI END ###
        }

        speechSynthesis.speak(utterance);
    },

    getVoices: function () {
        return new Promise(resolve => {
            function loadVoices() {
                const voices = speechSynthesis.getVoices();
                if (voices && voices.length > 0) {
                    resolve(voices.map(v => ({
                        name: v.name,
                        lang: v.lang,
                        default: v.default
                    })));
                } else {
                    // chờ rồi thử lại
                    setTimeout(loadVoices, 200);
                }
            }

            // gọi thử ngay
            loadVoices();

            // bắt sự kiện khi trình duyệt load xong voice
            if (typeof speechSynthesis.onvoiceschanged !== 'undefined') {
                speechSynthesis.onvoiceschanged = loadVoices;
            }
        });
    },

    pause: function () {
        if (speechSynthesis.speaking && !speechSynthesis.paused) {
            speechSynthesis.pause();
        }
    },

    resume: function () {
        if (speechSynthesis.paused) {
            speechSynthesis.resume();
        }
    },

    stop: function () {
        if (speechSynthesis.speaking || speechSynthesis.paused) {
            speechSynthesis.cancel();
        }
    }
};
