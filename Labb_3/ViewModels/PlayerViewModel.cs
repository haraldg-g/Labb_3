using Labb_3.Command;
using Labb_3.Models;
using Labb_3.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Threading;

namespace Labb_3.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private int _currentQuestionIndex;
        private string? _selectedAnswer;
        private List<string> _shuffledAnswers = new();
        private int _selectAnswerIndex;
        private int _correctAnswerIndex;
        private bool _hasAnswered = false;
        private bool _isAnswerCorrect = false;
        private int _correctAnswerCounter;
        private bool _showResults;
        private double _secondsRemaining = 30;
        private Stopwatch _stopwatch = new();
        private DispatcherTimer _timer = new();
        private List<Question> _shuffledQuestions = new();

        public RelayCommand NextQuestionCommand { get; }
        public RelayCommand AnswerQuestionClickCommand { get; }
        public RelayCommand RestartQuizCommand { get; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        public Question? ActiveQuestion { get => _shuffledQuestions[CurrentQuestionIndex]; }  // Felmedelande Finns inget QuestionPack
        public List<string> ShuffledAnswers { get => _shuffledAnswers; }
        

        public int CorrectAnswerIndex { get => _correctAnswerIndex; set => SetProperty(ref _correctAnswerIndex, value); }
        public int SelectedAnswerIndex { get => _selectAnswerIndex; set => SetProperty(ref _selectAnswerIndex, value); }
        public bool HasAnswered { get => _hasAnswered; set => SetProperty(ref _hasAnswered, value); }
        public bool IsAnswerCorrect { get => _isAnswerCorrect; set => SetProperty(ref _isAnswerCorrect, value); }
        public string? SelectedAnswer { get => _selectedAnswer; set => SetProperty(ref _selectedAnswer, value); }
        public int CorrectAnswerCounter { get => _correctAnswerCounter; set => SetProperty(ref _correctAnswerCounter, value); }
        public bool ShowResults { get => _showResults; set => SetProperty(ref _showResults, value); }


        public double SecondsRemaining { get => _secondsRemaining; set => SetProperty(ref _secondsRemaining, value); }

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

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            NextQuestionCommand = new RelayCommand(NextQuestion);
            AnswerQuestionClickCommand = new RelayCommand(AnswerQuestionClick);
            RestartQuizCommand = new RelayCommand(RestartQuiz);
        }

        public void RaiseActiveQuestionChanged() => RaisePropertyChanged(nameof(ActiveQuestion));

        public void StartQuiz()
        {
            if (ActivePack == null)
                return; 

            _timer.Stop();
            
            CurrentQuestionIndex = 0;
            HasAnswered = false;
            ShowResults = false;

            _shuffledQuestions = ActivePack.Questions.ToList();
            ShuffleOrder(_shuffledQuestions);
            GetActiveQuestionAnswers();
            RaiseActiveQuestionChanged();
            StartTimer();

        }

        private void RestartQuiz(object? obj)
        {
            StartQuiz();
        }

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

            if (IsAnswerCorrect)
            {
                CorrectAnswerCounter++;
            }

            HasAnswered = true;
            _stopwatch.Stop();

            await Task.Delay(3000);

            NextQuestion(ActivePack);
            HasAnswered = false;
            StartTimer();
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
                ShowResults = true;
                _stopwatch.Stop();
            }
        }

        public List<string> GetActiveQuestionAnswers()
        {

            Question? activeQuestion = ActiveQuestion;

            if (activeQuestion == null)
                return new List<string>();
            
            List<string> list = new List<string> { activeQuestion.CorrectAnswer, activeQuestion.IncorrectAnswers[0], activeQuestion.IncorrectAnswers[1], activeQuestion.IncorrectAnswers[2] };
            _shuffledAnswers = list;
            ShuffleOrder(_shuffledAnswers);
            RaisePropertyChanged(nameof(ShuffledAnswers));
            return _shuffledAnswers;
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan remaining = TimeSpan.FromSeconds(30) - _stopwatch.Elapsed;
            SecondsRemaining = Math.Max(0, (int)Math.Ceiling(remaining.TotalSeconds));

            if (ActiveQuestion == null)
                return;

            if (SecondsRemaining <= 0)
            {
                _timer.Stop();
                HasAnswered = true;
                IsAnswerCorrect = true;
                CorrectAnswerIndex = ShuffledAnswers.IndexOf(ActiveQuestion.CorrectAnswer);
                SelectedAnswerIndex = CorrectAnswerIndex;
                AnswerQuestionClick(SelectedAnswer);

                await Task.Delay(3000);
                NextQuestion(ActivePack);
                HasAnswered = false;
                IsAnswerCorrect = false;
                StartTimer();
            }
        }

        private void StartTimer()
        {
            _stopwatch.Restart();
            SecondsRemaining = 30;
            _timer.Start();
        }
        public void ShuffleOrder<T>(IList<T> list)
        {
            Random random = new();
            int lastIndex = list.Count() - 1;
            
            while (lastIndex > 0)
            {
                T tempValue = list[lastIndex];
                int randomIndex = random.Next(0, lastIndex);
                list[lastIndex] = list[randomIndex];
                list[randomIndex] = tempValue;
                lastIndex--;
            }
        }
        
    }
}
