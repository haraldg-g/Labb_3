using System.Text.Json.Serialization;

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

        [JsonPropertyName("question")]
        public string Query { get; set; }
        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; }
        [JsonPropertyName("incorrect_answers")]
        public string[] IncorrectAnswers { get; set; }

        public Question()
        {
        }
    }
}
