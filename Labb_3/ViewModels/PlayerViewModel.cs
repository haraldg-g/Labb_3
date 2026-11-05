using Labb_3.Models;
using Labb_3.Command;
using System.Windows;
using System.Diagnostics.Eventing.Reader;

namespace Labb_3.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private int _currentQuestionIndex;
        private string _selectedAnswer;
        private List<string>? _shuffledAnswers;
        private int _selectAnswerIndex;
        private int _correctAnswerIndex;
        private bool _hasAnswered = false;
        private bool _isAnswerCorrect = false;

        public RelayCommand NextQuestionCommand { get; }
        public RelayCommand AnswerQuestionClickCommand { get; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        public Question? ActiveQuestion { get => ActivePack?.Questions[CurrentQuestionIndex]; }
        public List<string> ShuffledAnswers { get => _shuffledAnswers; }

        public int CorrectAnswerIndex { get => _correctAnswerIndex; set => SetProperty(ref _correctAnswerIndex, value); }
        public int SelectedAnswerIndex { get => _selectAnswerIndex; set => SetProperty(ref _selectAnswerIndex, value); }
        public bool HasAnswered { get => _hasAnswered; set => SetProperty(ref _hasAnswered, value); }
        public bool IsAnswerCorrect { get => _isAnswerCorrect; set => SetProperty(ref _isAnswerCorrect, value); }
        public string SelectedAnswer { get => _selectedAnswer; set => SetProperty(ref _selectedAnswer, value); }

        public int CurrentQuestionNumber => CurrentQuestionIndex + 1;

        public int CurrentQuestionIndex 
        { 
            get => _currentQuestionIndex;
            set
            {
                if (ActivePack == null || ActivePack.Questions.Count == 0)
                    return;

                if (value > ActivePack.Questions.Count - 1 || value < 0)
                    return;

                if (SetProperty(ref _currentQuestionIndex, value))
                {
                    RaiseActiveQuestionChanged();
                    RaisePropertyChanged(nameof(CurrentQuestionNumber));
                }
            }
        }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            NextQuestionCommand = new RelayCommand(NextQuestion);
            AnswerQuestionClickCommand = new RelayCommand(AnswerQuestionClick);
        }

        public void RaiseActiveQuestionChanged() => RaisePropertyChanged(nameof(ActiveQuestion));

        private async void AnswerQuestionClick(object? obj)
        {
            if (HasAnswered)
                return;

            if (obj is not string answer || ActiveQuestion == null)
                return;

            SelectedAnswer = answer;

            SelectedAnswerIndex = ShuffledAnswers.IndexOf(SelectedAnswer);
            CorrectAnswerIndex = ShuffledAnswers.IndexOf(ActiveQuestion.CorrectAnswer);

            IsAnswerCorrect = SelectedAnswerIndex == CorrectAnswerIndex;

            HasAnswered = true;


            await Task.Delay(3000);

            HasAnswered = false;

            NextQuestion(ActivePack);
        }

        private void NextQuestion(object? obj)
        {
            if (ActivePack == null)
                return;

            if (CurrentQuestionIndex < ActivePack.Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                RaiseActiveQuestionChanged();
                GetActiveQuestionAnswers();
            }
            else if (CurrentQuestionIndex == ActivePack.Questions.Count - 1)
            {
                // Change to final screen command
            }
        }

        public List<string> GetActiveQuestionAnswers()
        {
            List<string> list = new List<string> { ActiveQuestion.CorrectAnswer, ActiveQuestion.IncorrectAnswers[0], ActiveQuestion.IncorrectAnswers[1], ActiveQuestion.IncorrectAnswers[2] };
            _shuffledAnswers = list;
            ShuffleOrder(_shuffledAnswers);
            RaisePropertyChanged(nameof(ShuffledAnswers));
            return _shuffledAnswers;

        }

        public List<string> ShuffleOrder(List<string> list)
        {
            Random random = new();
            int lastIndex = list.Count() - 1;
            while (lastIndex > 0)
            {
                string tempValue = list[lastIndex];
                int randomIndex = random.Next(0, lastIndex);
                list[lastIndex] = list[randomIndex];
                list[randomIndex] = tempValue;
                lastIndex--;
            }
            return list;
        }
    }
}
