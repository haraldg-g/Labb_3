using Labb_3.Models;
using Labb_3.Command;
using System.Windows;

namespace Labb_3.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private int _currentQuestionIndex;
        private List<string> _shuffledAnswers;
        

        public List<string> GetActiveQuestionAnswers()
        {
            List<string> list = new List<string> { ActiveQuestion.CorrectAnswer, ActiveQuestion.IncorrectAnswers[0], ActiveQuestion.IncorrectAnswers[1], ActiveQuestion.IncorrectAnswers[2] };
            _shuffledAnswers = list;
            ShuffleOrder(_shuffledAnswers);
            RaisePropertyChanged(nameof(ShuffledAnswers));
            return _shuffledAnswers;
        }

        public List<string> ShuffledAnswers { get => _shuffledAnswers; }

        public RelayCommand NextQuestionCommand { get; }


        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        public void RaiseActiveQuestionChanged() => RaisePropertyChanged(nameof(ActiveQuestion));
        public Question? ActiveQuestion { get => ActivePack?.Questions[CurrentQuestionIndex]; }

        public int CurrentQuestionIndex 
        { 
            get => _currentQuestionIndex;
            set
            {
                if (SetProperty(ref _currentQuestionIndex, value))
                {
                    RaiseActiveQuestionChanged();
                    RaisePropertyChanged(nameof(CurrentQuestionNumber));
                    NextQuestionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            NextQuestionCommand = new RelayCommand(NextQuestion, CanNextQuestion);
        }

      

        private bool CanNextQuestion(object? arg)
        {
            return ActivePack != null && CurrentQuestionIndex + 1 < ActivePack?.Questions.Count;
        }

        private void NextQuestion(object? obj)
        {
            CurrentQuestionIndex++;
            RaiseActiveQuestionChanged();
            GetActiveQuestionAnswers();
        }

        public int CurrentQuestionNumber => CurrentQuestionIndex + 1;

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
