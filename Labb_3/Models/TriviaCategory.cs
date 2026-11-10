
namespace Labb_3.Models
{
    internal class TriviaCategory
    {

        public TriviaCategory(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public TriviaCategory()
        {
            Name = string.Empty;
        }

        public int Id { get; set; }
        public string Name { get; set; }

    }
}
