using Labb_3.Enums;
using System.Net.Http;

namespace Labb_3.Services
{
    internal class ImportTriviaService
    {
        private readonly HttpClient _httpClient;

        public ImportTriviaService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetCategoriesAsync()
        {
         
            var response = await _httpClient.GetAsync("https://opentdb.com/api_category.php");
            var result = await response.Content.ReadAsStringAsync();
           
            return result;

        }

        public async Task<string> ImportQuestionsAsync(int categoryId, Difficulties difficulty, int amount)
        {
            string diff = difficulty.ToString().ToLower();
            
            var response = await _httpClient.GetAsync($"https://opentdb.com/api.php?amount={amount}&category={categoryId}&difficulty={diff}&type=multiple");
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

    }
}
