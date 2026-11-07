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
        private QuestionPackViewModel? _newPackViewModel;

        public RelayCommand AddQuestionCommand { get; }
        public RelayCommand RemoveQuestionCommand { get; }
        public RelayCommand OpenPackOptionsCommand { get; }
        public RelayCommand OpenCreateNewQuestionPackCommand { get; }
        public RelayCommand CreateQuestionPackCommand { get; }
        public RelayCommand DeleteQuestionPackCommand { get; }
        public RelayCommand SelectQuestionPackCommand { get; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        public QuestionPackViewModel? NewPackViewModel { get => _newPackViewModel; set => SetProperty(ref _newPackViewModel, value); }
        public static Array Difficulties => Enum.GetValues(typeof(Difficulty));
        public bool HasActivePack => ActivePack != null;
        public bool HasQuestion => ActivePack?.Questions.Count != 0;

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

            AddQuestionCommand = new RelayCommand(AddQuestion, CanAddQuestion);
            RemoveQuestionCommand = new RelayCommand(RemoveQuestion, CanRemoveQuestion);
            OpenPackOptionsCommand = new RelayCommand(OpenPackOptions, CanOpenPackOptions);
            OpenCreateNewQuestionPackCommand = new RelayCommand(OpenCreateNewQuestionPack);
            CreateQuestionPackCommand = new RelayCommand(CreateQuestionPack);
            DeleteQuestionPackCommand = new RelayCommand(DeleteQuestionPack);
            SelectQuestionPackCommand = new RelayCommand(SelectQuestionPack);
        }

        public void RaiseActivePackChanged() => RaisePropertyChanged(nameof(ActivePack));
        public void RaiseHasActivePackChanged() => RaisePropertyChanged(nameof(HasActivePack));
        public void RaiseHasQuestionChanged() => RaisePropertyChanged(nameof(HasQuestion));

        private void OpenCreateNewQuestionPack(object? obj)
        {
            var newPack = new QuestionPack("<PackName>");
            var newPackViewModel = new QuestionPackViewModel(newPack);
            NewPackViewModel = newPackViewModel;
            var window = new NewQuestionPackDialog(NewPackViewModel);
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.ShowDialog();
        }

        private bool CanOpenPackOptions(object? arg)
        {
            return _mainWindowViewModel?.CurrentView is not PlayerViewModel && _mainWindowViewModel?.ActivePack != null;
        }
        private void OpenPackOptions(object? obj)
        {
            var window = new PackOptionsDialog();
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.DataContext = this;
            window.ShowDialog();
        }
        private bool CanRemoveQuestion(object? arg)
        {
            return SelectedQuestion != null && _mainWindowViewModel?.CurrentView is not PlayerViewModel;
        }
        private void RemoveQuestion(object? obj)
        {
            if (SelectedQuestion == null)
                return;

            ActivePack?.Questions.Remove(SelectedQuestion);
            RaiseHasQuestionChanged();
            SelectedQuestion = null;
            _mainWindowViewModel?.StartQuizCommand.RaiseCanExecuteChanged();
        }
        private bool CanAddQuestion(object? arg)  
        {
            return _mainWindowViewModel?.CurrentView is not PlayerViewModel && _mainWindowViewModel?.ActivePack != null;
        }
        private void AddQuestion(object? obj)
        {
            Question question = new Question("New Question", "", "", "", "");
            ActivePack?.Questions.Add(question);
            RaiseHasQuestionChanged();
            SelectedQuestion = question;
            _mainWindowViewModel?.StartQuizCommand.RaiseCanExecuteChanged();
        }

        private void CreateQuestionPack(object? obj)
        {
            if (_mainWindowViewModel == null || NewPackViewModel == null)
                return;

            _mainWindowViewModel.Packs.Add(NewPackViewModel);
            _mainWindowViewModel.ActivePack = NewPackViewModel;
            AddQuestionCommand.RaiseCanExecuteChanged();
            OpenPackOptionsCommand.RaiseCanExecuteChanged();
            _mainWindowViewModel.StartQuizCommand.RaiseCanExecuteChanged();

            if (obj is Window window)
                window.Close();

            _mainWindowViewModel.ShowConfigurationView();

            NewPackViewModel = null;
        }
        private void DeleteQuestionPack(object? obj)
        {
            var packToDelete = ActivePack;

            if (_mainWindowViewModel == null || packToDelete == null)
                return;

            if (!ConfirmDeletion($"Are you sure you want to delete {packToDelete.Name}"))
                return;

            _mainWindowViewModel.Packs.Remove(packToDelete);

            if (_mainWindowViewModel.Packs.Count > 0)
            {
                _mainWindowViewModel.ActivePack = _mainWindowViewModel.Packs[0];
            }
            else
            {
                _mainWindowViewModel.ActivePack = null;
                AddQuestionCommand.RaiseCanExecuteChanged();
                OpenPackOptionsCommand.RaiseCanExecuteChanged();
                _mainWindowViewModel.StartQuizCommand.RaiseCanExecuteChanged();
            }

            RaiseActivePackChanged();
            RaiseHasActivePackChanged();
        }

        private void SelectQuestionPack(object? obj)
        {
            if (obj is QuestionPackViewModel pack && _mainWindowViewModel != null)
            {
                _mainWindowViewModel.ActivePack = pack;
                RaiseActivePackChanged();
                RaiseHasActivePackChanged();
                _mainWindowViewModel.ShowConfigurationView();
            } 
        }
        private bool ConfirmDeletion(string message)
        {
            return MessageBox.Show(message, "Delete question pack?", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}
