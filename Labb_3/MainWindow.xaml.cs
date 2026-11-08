using Labb_3.Services;
using Labb_3.ViewModels;
using System.ComponentModel;
using System.IO;
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
            
            try
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                   bool canClose = await viewModel.SaveQuestionPacks();

                    if (canClose)
                    {
                        Closing -= MainWindow_Closing;
                        Close();
                    }
                }
            }
        }
    }
}