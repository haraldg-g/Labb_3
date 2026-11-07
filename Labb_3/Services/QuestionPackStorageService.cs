using Labb_3.Models;
using System.Text.Json;
using System.IO;
using System.Windows;


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
        public async void SaveQuestionPacks(IEnumerable<QuestionPack> questionPacks)
        {
            string json = JsonSerializer.Serialize(questionPacks);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<QuestionPack>> LoadQuestionPacksAsync()
        {

            List<QuestionPack>? questions = new();

            if (File.Exists(_filePath))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(_filePath);
                    questions = JsonSerializer.Deserialize<List<QuestionPack>>(json) ?? new List<QuestionPack>();  
                }
                catch (JsonException)
                {
                    MessageBoxResult result = MessageBox.Show("The saved question pack file contains invalid or corrupted data and could not be loaded. \n\nYour previously saved question packs will not be available", "Question Pack Load Error", MessageBoxButton.OKCancel);
                    
                    if (result == MessageBoxResult.Cancel)
                    {
                        Application.Current.MainWindow?.Close();
                    }
                }
            }
            return questions;
        }
    }
}
