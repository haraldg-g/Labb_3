namespace Labb_3.Models
{
    class Question
    {
        public Question(string query, string correctAnswer, string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            IncorrectAnswers = new string[] { incorrectAnswer1, incorrectAnswer2, incorrectAnswer3 };
            // IncorrectAnswers = [incorrectAnswer1, incorrectAnswer2, incorrectAnswer3]; // Alternativ syntax.
        }

        public string Query { get; set; }
        public string CorrectAnswer { get; set; }
        public string[] IncorrectAnswers { get; set; }
    }
}
