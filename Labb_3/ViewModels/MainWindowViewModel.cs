using Labb_3.Command;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Labb_3.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    private object? _currentView;
    private QuestionPackViewModel? _activePack;
    private readonly QuestionPackStorageService _storageService = new();
    
    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }
    public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new();
    public RelayCommand ShowConfigurationViewCommand { get; }
    public RelayCommand StartQuizCommand { get; }
    public RelayCommand ExitApplicationCommand { get; }
    public RelayCommand ExitWindowCommand { get; }

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

        LoadQuestionPacks();

        ShowConfigurationViewCommand = new RelayCommand(ShowConfigurationView);
        StartQuizCommand = new RelayCommand(StartQuiz, CanStartQuiz);
        ExitApplicationCommand = new RelayCommand(ExitApplication);
        ExitWindowCommand = new RelayCommand(ExitWindow);
    }

    public void RaiseCurrentViewChanged() => RaisePropertyChanged(nameof(CurrentView));

    public void ChangeCurrentView(object? viewModel)
    {
        CurrentView = viewModel;
    }

    public void ShowPlayerView()
    {
        ChangeCurrentView(PlayerViewModel);
    }
    public void ShowConfigurationView()
    {
        ChangeCurrentView(ConfigurationViewModel);
    }

    private void ShowConfigurationView(object? obj)
    {
        ShowConfigurationView();
    }
    private bool CanStartQuiz(object? arg)
    {
        return ActivePack != null && ActivePack.Questions.Count > 0;
    }

    private void StartQuiz(object? obj)
    {
        PlayerViewModel?.StartQuiz();
        ShowPlayerView();
        RaiseCurrentViewChanged();
    }

    public void SaveQuestionPacks()
    {
        _storageService.SaveQuestionPacks(Packs.Select(pack => pack.UpdatePack()));
    }

    private void ExitWindow(object? obj)
    {
        if (obj is Window window)
        {
            window.Close();
        }
    }

    private void ExitApplication(object? obj)
    {
        Application.Current.MainWindow?.Close();
    }

    public void LoadQuestionPacks()
    {
        foreach (QuestionPack pack in _storageService.LoadQuestionPacks())
        {
            Packs.Add(new QuestionPackViewModel(pack));
        }
        if (Packs.Count > 0)
        {
            ActivePack = Packs[0];
        }
    }
}


