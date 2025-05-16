using BasicClassLibrary;
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

        private readonly Resource _resource; //保存Resource对象
        private readonly ResourceManager _resourceManager = new ResourceManager();


        public MediaPlayer MediaPlayer => _mediaPlayer;
        public bool IsPlaying => _mediaPlayer.IsPlaying;

        [ObservableProperty]
        private string _mediaPath = "";

        private bool _hasReachedCompletion; // 新增：是否已达到看完状态的标志位
        public PlayerViewModel(int resourceId)
        {
            // 已有代码基本不需要改变 
            // 需要设定mediaPath
            // 需要读取Episode中的观看进度
            // 需要实现一个公有函数OnWindowClosing()，在窗口关闭时被调用：
            //     如果已经看完了就修改观看进度为已看，否则改为在看，并记录观看进度（记录到Episode中）（以毫秒形式计入）

          
            _resource = _resourceManager.FindById(resourceId)
                ?? throw new ArgumentException($"未找到ID为{resourceId}的资源");
            if (_resource == null)
            {
                throw new ArgumentException($"未找到ID为{resourceId}的资源");
            }

            // 设置媒体路径
            MediaPath = _resource.ResourcePath ?? "";

            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            // 如果Episode不为null且有关联的观看进度，则加载
            if (_resource.Episode != null && _resource.Episode.Progress > 0)
            {
                _mediaPlayer.Time = _resource.Episode.Progress;
            }

            _mediaPlayer.LengthChanged += (_, e) =>
                Application.Current.Dispatcher.Invoke(() => MediaLength = e.Length);

            _mediaPlayer.TimeChanged += (_, e) =>
                Application.Current.Dispatcher.Invoke(() => {
                    if (!IsSeeking)
                    {
                        // Position  拖动时不更新
                        Position = e.Time;

                        // 实时检测是否达到看完条件
                        if (!_hasReachedCompletion &&
                            _mediaPlayer.Length > 0 &&
                            (_mediaPlayer.Length - e.Time) <= 90000) // 剩余≤90秒
                        {
                            _hasReachedCompletion = true;
                        }
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

        /// <summary>
        /// 窗口关闭时调用，保存观看状态和进度
        /// </summary>
        public void OnWindowClosing()
        {
            if (_resource.Episode == null)
            {
                return; // 如果没有关联的Episode，不做任何操作
            }

            // 如果初始状态就是“已看”，则不再改变观看状态
            if (_resource.Episode.State == State.Watched)
            {
                return;
            }

            if (_hasReachedCompletion)
            {
                _resource.Episode.State = State.Watched;
                _resource.Episode.Progress = _mediaPlayer.Length;
            }
            else
            {
                _resource.Episode.State = State.Watching;
                _resource.Episode.Progress = _mediaPlayer.Time;
            }

          
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