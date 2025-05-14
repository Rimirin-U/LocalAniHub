using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LocalAniHubFront.ViewModels.Windows;

namespace LocalAniHubFront.Views.Windows
{
    public partial class PlayerWindow
    {
        public PlayerWindow(int resourceId)
        {
            DataContext = new PlayerViewModel(resourceId);
            InitializeComponent();
        }

        private PlayerViewModel ViewModel => DataContext as PlayerViewModel;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            ViewModel.LoadCommand.Execute(null);
            ViewModel.MediaPlayer.Play();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.OnWindowClosing();
            ViewModel.Dispose();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
                ToggleWindowFullScreen();
        }
        private void ToggleWindowFullScreen()
        {
            if (WindowStyle != WindowStyle.None) // 全屏
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                TitleBar.Visibility = Visibility.Collapsed;
                ControlBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                TitleBar.Visibility = Visibility.Visible;
                ControlBar.Visibility = Visibility.Visible;
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