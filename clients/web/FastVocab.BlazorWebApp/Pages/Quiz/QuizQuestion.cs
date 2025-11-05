namespace FastVocab.BlazorWebApp.Pages.Quiz;

public class QuizQuestion
{
    public string Text { get; set; } = string.Empty;
    public List<QuizQuestionOption> Options { get; set; } = [];
    public bool IsAskingForMeaning { get; set; } // true: hỏi nghĩa từ từ, false: hỏi từ từ nghĩa
}
public class QuizQuestionOption
{
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; } = false;
    public bool IsCorrect { get; set; }
    public string Color { get; set; } = "option-unselect";

    public void Select()
    {
        Color = IsCorrect ? "option-correct" : "option-wrong";
    }
}