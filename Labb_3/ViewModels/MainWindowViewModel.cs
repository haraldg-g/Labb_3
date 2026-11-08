using Labb_3.Command;
using Labb_3.Models;
using Labb_3.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;

namespace Labb_3.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
    private object? _currentView;
    private QuestionPackViewModel? _activePack;
    private readonly QuestionPackStorageService _storageService = new();
    private bool _canSave = true;
    private string? GetNewFilePath()
    {
        SaveFileDialog dialog = new SaveFileDialog
        {
            Title = "Save Question Packs",
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = ".json",
            FileName = "QuestionPacks.json"
        };

        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }

        return null;
    }

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
                ConfigurationViewModel?.RaiseHasActivePackChanged();
            }
        }
    }

    public MainWindowViewModel()
    {
        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);

        CurrentView = ConfigurationViewModel;

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

    public async Task<bool> SaveQuestionPacks()
    {
        try
        {
            if (!_canSave)
            {
                MessageBoxResult result = MessageBox.Show("Your previous question packs could not be loaded. \n\nDo you want to save your current work to a new file?", "Create new file", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string? filePath = GetNewFilePath();

                    if (filePath == null)
                        return false;

                    await _storageService.SaveQuestionPacksToNewFile(Packs.Select(pack => pack.UpdatePack()), filePath);
                    return true;
                }
                return true;
            }

            await _storageService.SaveQuestionPacks(Packs.Select(pack => pack.UpdatePack()));
            return true;
        }
        catch (IOException ex)
        {
            string errorMsg = $"An error occurred while saving the file.\n\nDetails: {ex.Message}";
            MessageBox.Show(errorMsg, "Input/Output Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            string errorMsg = $"Access Denied. You do not have permission to access or modify this file/folder.\n\nPlease check if the file is read-only or try running the application as an Administrator.\n\nDetails: {ex.Message}";
            MessageBox.Show(errorMsg, "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
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

    public async Task LoadQuestionPacks()
    {

        try
        {
            foreach (QuestionPack pack in await _storageService.LoadQuestionPacksAsync())
            {
                QuestionPack packCheck = new("");

                foreach (Question question in pack.Questions)
                {
                    if (question.CorrectAnswer != null && question.IncorrectAnswers != null && question.IncorrectAnswers.Length == 3)
                    {
                        packCheck.Questions.Add(question);
                    }
                }
                packCheck.Name = pack.Name;
                Packs.Add(new QuestionPackViewModel(packCheck));
            }
            if (Packs.Count > 0)
            {
                ActivePack = Packs[0];
            }

            ConfigurationViewModel?.RaiseHasQuestionChanged();
            ConfigurationViewModel?.AddQuestionCommand.RaiseCanExecuteChanged();
            ConfigurationViewModel?.OpenPackOptionsCommand.RaiseCanExecuteChanged();
            _canSave = true;

        }
        catch (JsonException ex)
        {
            string errorMsg = $"The saved question pack file contains invalid or corrupted data and could not be loaded. \n\nYour previously saved question packs will not be available \n\nDetails: {ex.Message}";
            MessageBoxResult result = MessageBox.Show(errorMsg, "Question Pack Load Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);

            if (result == MessageBoxResult.Cancel)
            {
                Application.Current.MainWindow?.Close();
            }
            _canSave = false;
        }
        catch (IOException ex)
        {
            string errorMsg = $"An error occurred while accessing the file.\n\nDetails: {ex.Message}";
            MessageBox.Show(errorMsg, "Input/Output Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _canSave = false;
        }
        catch (UnauthorizedAccessException ex)
        {
            string errorMsg = $"Access Denied. You do not have permission to access or modify this file/folder.\n\nPlease check if the file is read-only or try running the application as an Administrator.\n\nDetails: {ex.Message}";
            MessageBox.Show(errorMsg, "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
            _canSave = false;
        }
    }

    public async Task InitializeAsync()
    {
        await LoadQuestionPacks();
    }
}


