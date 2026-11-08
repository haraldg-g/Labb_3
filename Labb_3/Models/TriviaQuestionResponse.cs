using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Labb_3.Models
{
    internal class TriviaQuestionResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<Question> Results { get; set; }

    }
}
