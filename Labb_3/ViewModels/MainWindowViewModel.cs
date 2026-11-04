using Labb_3.Command;
using Labb_3.Models;
using System.Collections.ObjectModel;
using static Labb_3.ViewModels.MainWindowViewModel;

namespace Labb_3.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<QuestionPackViewModel> Packs { get; }
    public RelayCommand ChangeCurrentViewCommand { get; }
    public RelayCommand StartQuizCommand { get; }

    private object? _currentView;

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

   

    private QuestionPackViewModel _activePack;
    public QuestionPackViewModel ActivePack
    {
        get => _activePack;
        set
        { 
            if(SetProperty(ref _activePack, value));
            {
                ConfigurationViewModel?.RaiseActivePackChanged();
            }
        }
    }

    
    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }
    public void RaiseCurrentViewChanged() => RaisePropertyChanged(nameof(CurrentView));


    public MainWindowViewModel()
    {
        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);

        CurrentView = ConfigurationViewModel;

        var questionPack = new QuestionPack("Mina frågor!", Difficulty.Medium);
        questionPack.Questions.Add(new Question("Vad heter sveriges huvudstad?", "Stockholm", "Oslo", "Göteborg", "London"));
        questionPack.Questions.Add(new Question("Vad är 2 + 3?", "5", "10", "8", "1"));

        ActivePack = new QuestionPackViewModel(questionPack);

        ChangeCurrentViewCommand = new RelayCommand(ChangeCurrentView);
        StartQuizCommand = new RelayCommand(StartQuiz);
    }

    public void ChangeCurrentView(object? obj)
    {
        if (CurrentView == ConfigurationViewModel)
        {
            CurrentView = PlayerViewModel;
        }
        else if (CurrentView == PlayerViewModel)
        {
            CurrentView = ConfigurationViewModel;
        }
    }
    private void StartQuiz(object? obj)
    {
        PlayerViewModel?.GetActiveQuestionAnswers();
        PlayerViewModel?.RaiseActiveQuestionChanged();
        ChangeCurrentView(CurrentView);
        RaiseCurrentViewChanged();
    }
}


