using Labb_3.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labb_3.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportQuestionsDialog.xaml
    /// </summary>
    public partial class ImportQuestionsDialog : Window
    {
        internal ImportQuestionsDialog(ImportQuestionsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
