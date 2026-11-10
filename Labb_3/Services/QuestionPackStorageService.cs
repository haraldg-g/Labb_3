using Labb_3.Models;
using System.Text.Json;
using System.IO;

namespace Labb_3.Services
{
    class QuestionPackStorageService
    {
        private string _filePath;
   
        public QuestionPackStorageService()
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appPath = Path.Combine(localPath, "Labb_3");
            Directory.CreateDirectory(appPath);
            _filePath = Path.Combine(appPath, "QuestionPacks.json"); 

        }
        public async Task SaveQuestionPacks(IEnumerable<QuestionPack> questionPacks)
        {
            string json = JsonSerializer.Serialize(questionPacks);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task SaveQuestionPacksToNewFile(IEnumerable<QuestionPack> questionPacks, string filePath)
        {
            string json = JsonSerializer.Serialize(questionPacks);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<QuestionPack>> LoadQuestionPacksAsync()
        {

            List<QuestionPack>? questions = new();

            if (File.Exists(_filePath))
            {
                string json = await File.ReadAllTextAsync(_filePath);
                questions = JsonSerializer.Deserialize<List<QuestionPack>>(json) ?? new List<QuestionPack>();
            }
            return questions;
        }
    }
}
