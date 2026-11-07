using Labb_3.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace Labb_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            Loaded += async (_, _) => await viewModel.InitializeAsync();
        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;

            if (DataContext is MainWindowViewModel viewModel)
            {
                await viewModel.SaveQuestionPacks();
            }

            Closing -= MainWindow_Closing;
            Close();
        }
    }
}