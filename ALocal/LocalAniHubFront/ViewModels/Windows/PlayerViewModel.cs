using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using System;
using System.IO;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class PlayerViewModel : ObservableObject, IDisposable
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;
        private Media? _currentMedia;

        public MediaPlayer MediaPlayer => _mediaPlayer;
        public bool IsPlaying => _mediaPlayer.IsPlaying;

        [ObservableProperty]
        private string mediaPath = @"D:\Download\[ANi] mono女孩 - 05 [1080P][Baha][WEB-DL][AAC AVC][CHT].mp4";

        public PlayerViewModel()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            _mediaPlayer.LengthChanged += (_, e) =>
                Application.Current.Dispatcher.Invoke(() => MediaLength = e.Length);

            _mediaPlayer.TimeChanged += (_, e) =>
                Application.Current.Dispatcher.Invoke(() => {
                    if (!IsSeeking)
                    {
                        // Position  拖动时不更新
                        Position = e.Time;
                    }
                });

            _mediaPlayer.EndReached += (_, e) =>
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Pause();
                    Stop();
                    Play();
                    ShowMessage("已重播");
                }));

            _mediaPlayer.Playing += (_, _) => NotifyAllCommands();
            _mediaPlayer.Paused += (_, _) => NotifyAllCommands();
            _mediaPlayer.Stopped += (_, _) => NotifyAllCommands();
        }



        [RelayCommand(CanExecute = nameof(CanLoad))]
        private void Load()
        {
            _currentMedia = new Media(_libVLC, MediaPath, FromType.FromPath);
            _mediaPlayer.Media = _currentMedia;
            // _mediaPlayer.Play(_currentMedia);
            NotifyAllCommands();
        }
        private bool CanLoad() => !string.IsNullOrWhiteSpace(MediaPath);

        [RelayCommand(CanExecute = nameof(CanPlay))]
        private void Play()
        {
            _mediaPlayer.Play();
            NotifyAllCommands();
        }
        private bool CanPlay() => !_mediaPlayer.IsPlaying;

        [RelayCommand(CanExecute = nameof(CanPause))]
        private void Pause()
        {
            _mediaPlayer.Pause();
            ShowMessage(FormatMilliseconds(_mediaPlayer.Time));
            NotifyAllCommands();
        }
        private bool CanPause() => _mediaPlayer.CanPause;

        [RelayCommand]
        private void Stop()
        {
            _mediaPlayer.Stop();
            NotifyAllCommands();
        }

        // 6.1 全屏切换

        // 6.2 拖动进度条 Seek
        [ObservableProperty]
        private bool isSeeking;

        [ObservableProperty]
        private long mediaLength;

        [ObservableProperty]
        private long position;

        [RelayCommand]
        private void Seek(long position)
        {
            //long position = long.Parse(position1);
            MediaPlayer.Time = position;
            NotifyAllCommands();
        }

        // 6.3 左/右 键快进/快退
        [RelayCommand]
        private void Skip(string offsetMs1)
        {
            long offsetMs = long.Parse(offsetMs1);
            var newTime = Math.Clamp(MediaPlayer.Time + offsetMs, 0, MediaPlayer.Length);
            MediaPlayer.Time = newTime;
            ShowMessage(FormatMilliseconds(_mediaPlayer.Time));
            NotifyAllCommands();
        }

        // 6.4 播放速度控制
        [ObservableProperty]
        private float currentRate = 1f;

        [RelayCommand]
        private void IncreaseSpeed()
        {
            CurrentRate = MathF.Min(CurrentRate + 0.1f, 4f);
            MediaPlayer.SetRate(CurrentRate);
            ShowMessage($"播放速度 {CurrentRate:F1}x");
        }

        [RelayCommand]
        private void DecreaseSpeed()
        {
            CurrentRate = MathF.Max(CurrentRate - 0.1f, 0.1f);
            MediaPlayer.SetRate(CurrentRate);
            ShowMessage($"播放速度 {CurrentRate}x");
        }

        [RelayCommand]
        private void ResetSpeed()
        {
            CurrentRate = 1f;
            MediaPlayer.SetRate(CurrentRate);
            ShowMessage($"播放速度 {CurrentRate}x");
        }

        // 6.5 截屏并保存
        public event EventHandler<string>? SnapshotCompleted;

        [RelayCommand]
        private void Snapshot()
        {
            var dir = Path.Combine(
                @"C:\Users\95842\AppData\Roaming\PotPlayerMini64\Capture",
                "Snapshots");
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, $"snap_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            MediaPlayer.TakeSnapshot(0, file, 0, 0);
            SnapshotCompleted?.Invoke(this, file);
        }

        private void NotifyAllCommands()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                LoadCommand.NotifyCanExecuteChanged();
                PlayCommand.NotifyCanExecuteChanged();
                PauseCommand.NotifyCanExecuteChanged();
                StopCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(IsPlaying)); // 新增
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadCommand.NotifyCanExecuteChanged();
                    PlayCommand.NotifyCanExecuteChanged();
                    PauseCommand.NotifyCanExecuteChanged();
                    StopCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged(nameof(IsPlaying)); // 新增
                });
            }
        }

        // 信息文字
        [ObservableProperty]
        private string messageText = "";
        private int _messageToken = 0;
        private async void ShowMessage(string message)
        {
            int token = ++_messageToken;
            MessageText = message;
            await Task.Delay(1500);
            if (token == _messageToken)
            {
                MessageText = "";
            }
        }

        public void Dispose()
        {
            _currentMedia?.Dispose();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }

        private string FormatMilliseconds(long milliseconds)
        {
            var ts = TimeSpan.FromMilliseconds(milliseconds);
            return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}";
        }

    }
}