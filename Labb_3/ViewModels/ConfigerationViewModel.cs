using Labb_3.Models;
using Labb_3.Command;
namespace Labb_3.ViewModels
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private Question? _selectedQuestion;


        public RelayCommand ChangedPackNameCommand { get; }
        public RelayCommand AddQuestionCommand { get; private set; }
        public RelayCommand RemoveQuestionCommand { get; private set; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                if (SetProperty(ref _selectedQuestion, value))
                {
                    RemoveQuestionCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            ChangedPackNameCommand = new RelayCommand(ChangePackName, CanChangePackName);
            AddQuestionCommand = new RelayCommand(AddQuestion);
            RemoveQuestionCommand = new RelayCommand(RemoveQuestion, CanRemoveQuestion);
        }

        private bool CanRemoveQuestion(object? arg)
        {
            return SelectedQuestion != null;
        }
        private void RemoveQuestion(object? obj)
        {
            ActivePack?.Questions.Remove(SelectedQuestion);
            SelectedQuestion = null;

        }

        private void AddQuestion(object? obj)
        {
            Question question = new Question("New Question", "", "", "", "");
            ActivePack?.Questions.Add(question);
            SelectedQuestion = question;
        }

        private bool CanChangePackName(object? arg)
        {
            return ActivePack?.Name != string.Empty;
        }

        private void ChangePackName(object? obj)
        {
            throw new NotImplementedException();
        }
    }
}
