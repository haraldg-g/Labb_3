using Labb_3.Models;
namespace Labb_3.ViewModels
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        private Question? _selectedQuestion;

        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

        public Question? SelectedQuestion { get => _selectedQuestion; set => SetProperty(ref _selectedQuestion, value); }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}
