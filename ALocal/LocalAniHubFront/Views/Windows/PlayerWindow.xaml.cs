using System.Windows;
using System.Windows.Input;
using LocalAniHubFront.ViewModels.Windows;

namespace LocalAniHubFront.Views.Windows
{
    public partial class PlayerWindow
    {
        public PlayerWindow()
        {
            // 全屏
            // ViewModel.ToggleFullScreenCommand.Executed += (_, _) => ToggleWindowFullScreen();
            // ViewModel.ExitFullScreenCommand.Executed += (_, _) => ExitFullScreen();

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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
                ToggleWindowFullScreen();
        }
        private void ToggleWindowFullScreen()
        {
            if (WindowStyle != WindowStyle.None)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
        }
        // private void ExitFullScreen() => ToggleWindowFullScreen();

        private bool _isDragging = false;

        private void PositionSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            ViewModel.IsSeeking = true;
        }

        private void PositionSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                ViewModel.IsSeeking = false;
                ViewModel.SeekCommand.Execute((long)PositionSlider.Value);
            }
        }

        private void PositionSlider_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Released)
            {
                _isDragging = false;
                ViewModel.SeekCommand.Execute((long)PositionSlider.Value);
            }
        }

        private void VideoView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 单击后调用 ViewModel 的 PlayPause 命令或直接切换
            if (ViewModel.MediaPlayer.IsPlaying)
                ViewModel.PauseCommand.Execute(null);
            else
                ViewModel.PlayCommand.Execute(null);
        }
    }
}