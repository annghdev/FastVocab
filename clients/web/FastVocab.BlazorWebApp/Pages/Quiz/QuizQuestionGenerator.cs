using FastVocab.Shared.DTOs.Words;

namespace FastVocab.BlazorWebApp.Pages.Quiz;

public class QuizQuestionGenerator
{
    public static List<QuizQuestion> GenerateFromWordList(IEnumerable<WordDto> words, bool askForMeaning = true)
    {
        List<QuizQuestion> questions = [];
        // Chọn ngẫu nhiên numQuestions từ vựng để tạo câu hỏi
        var random = new Random();
        var selectedWords = words.OrderBy(x => random.Next()).ToList();

        foreach (var word in selectedWords)
        {
            var question = new QuizQuestion()
            {
                IsAskingForMeaning = askForMeaning,
            };
            string correctAnswer;
            if (askForMeaning)
            {
                // Hỏi nghĩa của từ
                question.Text = $"Chọn nghĩa của từ: '{word.Text}'";
                correctAnswer = word.Meaning;
            }
            else
            {
                // Hỏi từ của nghĩa
                question.Text = $"Từ nào có nghĩa là: '{word.Meaning}'";
                correctAnswer = word.Text;
            }


            // Lấy đáp án sai ngẫu nhiên (không trùng với đúng và không trùng nhau)
            var otherCandidates = words.Where(w => askForMeaning ? w.Meaning != correctAnswer : w.Text != correctAnswer).ToList();
            var wrongAnswers = otherCandidates
                .OrderBy(x => random.Next())
                .Take(3)
                .Select(w=> new QuizQuestionOption
                {
                    Text = askForMeaning ? w.Meaning : w.Text
                })
                .ToList();

            // Kết hợp đúng + sai, xáo trộn
            question.Options = wrongAnswers;
            question.Options.Add(new QuizQuestionOption
            {
                Text = correctAnswer,
                IsCorrect = true
            });
            question.Options = question.Options.OrderBy(x => random.Next()).ToList();

            questions.Add(question);
        }

        return questions;
    }
}
