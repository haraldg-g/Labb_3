using Labb_3.Enums;

namespace Labb_3.Models
{

    internal class QuestionPack
    {
        public QuestionPack(string name, Difficulties difficulty = Difficulties.Medium, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = new List<Question>();
        }

        public string Name { get; set; }
        public Difficulties Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public List<Question> Questions { get; set; }
    }
}
