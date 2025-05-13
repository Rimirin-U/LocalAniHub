using System.Windows;
using LocalAniHubFront.ViewModels.Windows;

namespace LocalAniHubFront.Views.Windows
{
    public partial class PlayerWindow
    {
        public PlayerWindow()
        {
            // 全屏
            ViewModel.ToggleFullScreenCommand.Executed += (_, _) => ToggleWindowFullScreen();
            ViewModel.ExitFullScreenCommand.Executed += (_, _) => ExitFullScreen();

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
        private void ExitFullScreen() => ToggleWindowFullScreen();

        private bool _isDragging;

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _isDragging = false;
            // 拖动结束后同步播放器位置
            ViewModel.SeekCommand.Execute((long)PositionSlider.Value);
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