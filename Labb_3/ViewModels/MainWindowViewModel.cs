using Labb_3.Command;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using static Labb_3.ViewModels.MainWindowViewModel;

namespace Labb_3.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    private object? _currentView;
    private QuestionPackViewModel? _activePack;
    private readonly QuestionPackStorageService _storageService = new();

    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }
    public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new();
    public RelayCommand? ChangeCurrentViewCommand { get; }
    public RelayCommand? StartQuizCommand { get; }
    public RelayCommand ExitApplicationCommand { get; }

    public bool CanPlay => CurrentView != PlayerViewModel;
    public bool CanEdit => CurrentView != ConfigurationViewModel;

    public object? CurrentView
    {
        get => _currentView;
        set
        {
            if (SetProperty(ref _currentView, value))
            {
                RaisePropertyChanged(nameof(CanPlay));
                RaisePropertyChanged(nameof(CanEdit));
            }
        } 
    }
    
    public QuestionPackViewModel? ActivePack
    {
        get => _activePack;
        set
        { 
            if(SetProperty(ref _activePack, value))
            {
                ConfigurationViewModel?.RaiseActivePackChanged();
            }
        }
    }

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
        ExitApplicationCommand = new RelayCommand(ExitApplication);
    }

    public void RaiseCurrentViewChanged() => RaisePropertyChanged(nameof(CurrentView));

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
        PlayerViewModel?.StartQuiz();
        ChangeCurrentView(CurrentView);
        RaiseCurrentViewChanged();
    }

    public void SaveQuestionPacks()
    {
        _storageService.SaveQuestionPacks(Packs.Select(pack => pack.UpdatePack()));
    }

    private void ExitApplication(object? obj)
    {
        Application.Current.MainWindow?.Close();
    }
}


