using Labb_3.Models;
using Labb_3.Command;
using System.Windows;
using Labb_3.Dialogs;
namespace Labb_3.ViewModels
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private Question? _selectedQuestion;


        public RelayCommand ChangedPackNameCommand { get; }
        public RelayCommand AddQuestionCommand { get; }
        public RelayCommand RemoveQuestionCommand { get; }
        public RelayCommand OpenPackOptionsCommand { get; }
        public RelayCommand OpenCreateNewQuestionPackCommand { get; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        public Array Difficulties => Enum.GetValues(typeof(Difficulty));

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

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            ChangedPackNameCommand = new RelayCommand(ChangePackName, CanChangePackName);
            AddQuestionCommand = new RelayCommand(AddQuestion);
            RemoveQuestionCommand = new RelayCommand(RemoveQuestion, CanRemoveQuestion);
            OpenPackOptionsCommand = new RelayCommand(OpenPackOptions);
            OpenCreateNewQuestionPackCommand = new RelayCommand(OpenCreateNewQuestionPack);
        }

        

        public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));

        private void OpenCreateNewQuestionPack(object? obj)
        {
            var window = new NewQuestionPackDialog();
            window.Owner = Application.Current.MainWindow;
            window.DataContext = this;
            window.ShowDialog();
        }

        private void OpenPackOptions(object? obj)
        {
            
            var window = new PackOptionsDialog();
            window.Owner = Application.Current.MainWindow;
            window.DataContext = this;
            window.ShowDialog();
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
