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
        public void SaveQuestionPacks(IEnumerable<QuestionPack> questionPacks)
        {
            string json = JsonSerializer.Serialize(questionPacks);
            File.WriteAllText(_filePath, json);
        }
    }
}
