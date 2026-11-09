using Labb_3.Command;
using Labb_3.Enums;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace Labb_3.ViewModels
{
    internal class ImportQuestionsViewModel : ViewModelBase
    {
        private int _selectedCategoryId;
        private Difficulties _selectedDifficulty;
        private int _amountOfQuestions;
        private readonly ImportTriviaService _triviaService = new();
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private List<Question> questionListImport = new();
        private QuestionPack _questionPack;
        
        
        public ObservableCollection<TriviaCategory> Categories { get => _mainWindowViewModel.Categories; }
        public QuestionPack QuestionPack { get => _questionPack; set => SetProperty(ref _questionPack, value); }
        public Array DifficultiesList => Enum.GetValues(typeof(Difficulties));
        public int SelectedCategoryId { get => _selectedCategoryId; set => SetProperty(ref _selectedCategoryId, value); }
        public Difficulties SelectedDifficulty { get => _selectedDifficulty; set => SetProperty(ref _selectedDifficulty, value); }
        public int AmountOfQuestions { get => _amountOfQuestions; set => SetProperty(ref _amountOfQuestions, value); }
        public RelayCommand ImportQuestionsCommand { get; }


        public ImportQuestionsViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            if (Categories.Any())
            {
                SelectedCategoryId = Categories[0].Id;
            }

            ImportQuestionsCommand = new RelayCommand(ImportQuestions);

        }

        private async void ImportQuestions(object? obj)
        {
            await ImportQuestionsAsync();

            var newPackViewModel = new QuestionPackViewModel(QuestionPack);
            _mainWindowViewModel?.Packs.Add(newPackViewModel);

            _mainWindowViewModel?.ActivePack = newPackViewModel;
            _mainWindowViewModel?.ConfigurationViewModel?.SelectedQuestion = QuestionPack.Questions[0];

            if (obj is Window window)
                window.Close();
        }

        public async Task ImportCategoriesAsync()
        {
            TriviaCategoryResponse result = new();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = await _triviaService.GetCategoriesAsync();
            result = JsonSerializer.Deserialize<TriviaCategoryResponse>(json, options) ?? new TriviaCategoryResponse();

            foreach (TriviaCategory cat in result.TriviaCategoriesList)
            {
                Categories.Add(cat);
                
            }
        }

        public async Task ImportQuestionsAsync()
        {
            TriviaQuestionResponse result = new();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = await _triviaService.ImportQuestionsAsync(SelectedCategoryId,SelectedDifficulty,AmountOfQuestions);
            result = JsonSerializer.Deserialize<TriviaQuestionResponse>(json, options) ?? new TriviaQuestionResponse();

            var selectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedCategoryId);

            if (selectedCategory == null)
                return;

            QuestionPack = new QuestionPack(selectedCategory.Name, SelectedDifficulty);
            

            foreach (Question question in result.Results)
            {
                if (question.CorrectAnswer != null && question.IncorrectAnswers != null && question.IncorrectAnswers.Length == 3)
                {
                    question.Query = WebUtility.HtmlDecode(question.Query);
                    question.CorrectAnswer = WebUtility.HtmlDecode(question.CorrectAnswer);
                    question.IncorrectAnswers[0] = WebUtility.HtmlDecode(question.IncorrectAnswers[0]);
                    question.IncorrectAnswers[1] = WebUtility.HtmlDecode(question.IncorrectAnswers[1]);
                    question.IncorrectAnswers[2] = WebUtility.HtmlDecode(question.IncorrectAnswers[2]);

                    questionListImport.Add(question);
                }
            }

            QuestionPack.Questions = questionListImport;
        }


    }    
}
