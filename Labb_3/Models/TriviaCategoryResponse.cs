using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Labb_3.Models
{
    internal class TriviaCategoryResponse
    {
        [JsonPropertyName("trivia_categories")]
        public List<TriviaCategory> TriviaCategoriesList { get; set; }


    }
}
