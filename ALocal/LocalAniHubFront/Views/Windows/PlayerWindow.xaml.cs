using System.Windows;
using LocalAniHubFront.ViewModels.Windows;

namespace LocalAniHubFront.Views.Windows
{
    public partial class PlayerWindow 
    {
        public PlayerWindow()
        {
            InitializeComponent();
        }

        public PlayerWindow(string mediaUrl) : this()
        {
            ViewModel.MediaPath = mediaUrl;
        }

        // private PlayerViewModel ViewModel => DataContext as PlayerViewModel;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadCommand.Execute(null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.Dispose();
        }
    }
}