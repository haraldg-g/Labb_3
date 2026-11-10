using Labb_3.Command;
using Labb_3.Enums;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;

namespace Labb_3.ViewModels
{
    internal class ImportQuestionsViewModel : ViewModelBase
    {
        private int _selectedCategoryId;
        private Difficulties _selectedDifficulty;
        private int _amountOfQuestions;
        private int _importedQuestionsValue;
        private int _filteredQuestionsCounter;
        private readonly ImportTriviaService _triviaService = new();
        private readonly MainWindowViewModel _mainWindowViewModel;
        private QuestionPack? _questionPack;
        
        public ObservableCollection<TriviaCategory> Categories { get => _mainWindowViewModel.Categories; }
        public QuestionPack? QuestionPack { get => _questionPack; set => SetProperty(ref _questionPack, value); }
        public Array DifficultiesList => Enum.GetValues(typeof(Difficulties));
        public int SelectedCategoryId { get => _selectedCategoryId; set => SetProperty(ref _selectedCategoryId, value); }
        public Difficulties SelectedDifficulty { get => _selectedDifficulty; set => SetProperty(ref _selectedDifficulty, value); }
        public int AmountOfQuestions { get => _amountOfQuestions; set => SetProperty(ref _amountOfQuestions, value); }
        public RelayCommand ImportQuestionsCommand { get; }

        public ImportQuestionsViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            ImportQuestionsCommand = new RelayCommand(ImportQuestions);

        }

        private async void ImportQuestions(object? obj)
        {
            try
            {
                await ImportQuestionsAsync();

                if (QuestionPack == null)
                    return;

                var newPackViewModel = new QuestionPackViewModel(QuestionPack);
                _mainWindowViewModel?.Packs.Add(newPackViewModel);

                _mainWindowViewModel?.ActivePack = newPackViewModel;
                _mainWindowViewModel?.ConfigurationViewModel?.SelectedQuestion = QuestionPack.Questions[0];

                if (obj is Window window)
                    window.Close();

                if (_filteredQuestionsCounter > 0)
                {
                    MessageBox.Show($"Successfully imported {_importedQuestionsValue} questions!\n\n{_filteredQuestionsCounter} questions were skipped because they did not match the required question format.", "Import Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (_importedQuestionsValue == 0)
                {
                    MessageBox.Show("No questions could be imported. All returned questions were filtered out because they did not match the required format.","Import Failed",MessageBoxButton.OK,MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Successfully imported {_importedQuestionsValue} questions!", "Import Success!", MessageBoxButton.OK);
                }       
            }
            catch (JsonException ex)
            {
                string errorMsg = $"Failed to process the response received from Open Trivia DB.\n\nDetails: {ex.Message}";
                MessageBox.Show(errorMsg, "Json Reading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (HttpRequestException ex)
            {
                string errorMsg = $"Failed to communicate with Open Trivia DB. Please check your internet connection and try again.\n\nDetails: {ex.Message}";
                MessageBox.Show(errorMsg, "HttpRequest - API/Network Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (TaskCanceledException ex)
            {
                string errorMsg = $"The request to Open Trivia DB timed out or was cancelled. Please try again.\n\nDetails: {ex.Message}";
                MessageBox.Show(errorMsg, "HttpRequest - Timeout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            if (Categories.Any())
            {
                SelectedCategoryId = Categories[0].Id;
            }
        }

        public async Task ImportQuestionsAsync()
        {
            TriviaQuestionResponse result = new();
            List<Question> questionListImport = new();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = await _triviaService.ImportQuestionsAsync(SelectedCategoryId,SelectedDifficulty,AmountOfQuestions);
            result = JsonSerializer.Deserialize<TriviaQuestionResponse>(json, options) ?? new TriviaQuestionResponse();

            var selectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedCategoryId);

            if (selectedCategory == null || result.Results == null || result.Results.Count == 0)
                return;

            _importedQuestionsValue = 0;
            _filteredQuestionsCounter = 0;
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
                    _importedQuestionsValue++;
                }
                else
                {
                    _filteredQuestionsCounter++;
                }
            }
            QuestionPack.Questions = questionListImport;  
        }
    }    
}
