namespace Labb_3.ViewModels
{
    internal class PlayerViewModel
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}
