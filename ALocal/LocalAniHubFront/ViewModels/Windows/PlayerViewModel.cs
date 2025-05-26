using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class PlayerViewModel : ObservableObject, IDisposable
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;
        private Media? _currentMedia;

        private readonly Resource _resource;
        private readonly ResourceManager _resourceManager = new ResourceManager();

        private readonly EpisodeManager _episodeManager = new EpisodeManager(); // 新增 EpisodeManager

        public MediaPlayer MediaPlayer => _mediaPlayer;
        public bool IsPlaying => _mediaPlayer.IsPlaying;

        [ObservableProperty]
        private string _mediaPath = "";

        private bool _hasReachedCompletion;

        public PlayerViewModel(int resourceId)
        {
            _resource = _resourceManager.FindById(resourceId)
                ?? throw new ArgumentException($"未找到ID为{resourceId}的资源");

            //var episode = _resource.Episode;
            var episode = _episodeManager.FindById(_resource.EpisodeId.Value);

            MediaPath = _resource.ResourcePath ?? string.Empty;

            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            // 恢复观看进度
            if (episode != null && episode.Progress > 0)
            {
                _mediaPlayer.Time = episode.Progress;
            }

            // 使用 BeginInvoke 避免阻塞或死锁
            _mediaPlayer.LengthChanged += (_, e) =>
                Application.Current.Dispatcher.BeginInvoke(new Action(() => MediaLength = e.Length));

            _mediaPlayer.TimeChanged += (_, e) =>
                Application.Current.Dispatcher.BeginInvoke(new Action(() => OnTimeChanged(e.Time)));

            _mediaPlayer.EndReached += (_, e) =>
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    StopCommand.Execute(null);
                    PlayCommand.Execute(null);
                    ShowMessage("已重播");
                }));

            _mediaPlayer.Playing += (_, _) => Application.Current.Dispatcher.BeginInvoke(new Action(NotifyAllCommands));
            _mediaPlayer.Paused += (_, _) => Application.Current.Dispatcher.BeginInvoke(new Action(NotifyAllCommands));
            _mediaPlayer.Stopped += (_, _) => Application.Current.Dispatcher.BeginInvoke(new Action(NotifyAllCommands));
        }


        private void SaveEpisodeState()
        {
            var episode = _episodeManager.FindById(_resource.EpisodeId.Value);
            if (episode== null) return;
            try
            {
                _episodeManager.Modify(episode);
            }
            catch (Exception ex)
            {
                ShowMessage($"保存进度失败: {ex.Message}");
            }
        }

        private void OnTimeChanged(long time)
        {
            var episode = _episodeManager.FindById(_resource.EpisodeId.Value);
            if (IsSeeking) return;

            Position = time;
            // 检测是否看完
            if (!_hasReachedCompletion && MediaLength > 0 && (MediaLength - time) <= 90_000)
            {
                _hasReachedCompletion = true;

                // 立即更新状态为“已看”
                if (episode != null)
                {
                   episode.State = State.Watched;
                episode.Progress = _mediaPlayer.Length; // 记录完整进度
                    // 保存到数据库
                    SaveEpisodeState();
                    ShowMessage("已看完本集");
                }
            }

            NotifyAllCommands();
        }

        public void OnWindowClosing()
        {
            var episode = _episodeManager.FindById(_resource.EpisodeId.Value);
            if (episode == null) return;
            if (episode.State == State.Watched) return;

            episode.State = State.Watching;
            episode.Progress = _mediaPlayer.Time;
            // 保存到数据库
            SaveEpisodeState();

        }

        [RelayCommand(CanExecute = nameof(CanLoad))]
        private void Load()
        {
            _currentMedia = new Media(_libVLC, MediaPath, FromType.FromPath);
            _mediaPlayer.Media = _currentMedia;
            NotifyAllCommands();
        }
        private bool CanLoad() => !string.IsNullOrWhiteSpace(MediaPath);

        [RelayCommand(CanExecute = nameof(CanPlay))]
        private async Task Play()
        {
            await Task.Run(() => _mediaPlayer.Play());
            Application.Current.Dispatcher.BeginInvoke(new Action(NotifyAllCommands));
        }
        private bool CanPlay() => !_mediaPlayer.IsPlaying;

        [RelayCommand(CanExecute = nameof(CanPause))]
        private async Task Pause()
        {
            await Task.Run(() => _mediaPlayer.Pause());
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ShowMessage(FormatMilliseconds(_mediaPlayer.Time));
                NotifyAllCommands();
            }));
        }
        private bool CanPause() => _mediaPlayer.CanPause;

        [RelayCommand]
        private async Task Stop()
        {
            await Task.Run(() => _mediaPlayer.Stop());
            Application.Current.Dispatcher.BeginInvoke(new Action(NotifyAllCommands));
        }

        [ObservableProperty]
        private bool isSeeking;

        [ObservableProperty]
        private long mediaLength;

        [ObservableProperty]
        private long position;

        [RelayCommand]
        private void Seek(long position)
        {
            MediaPlayer.Time = position;
            NotifyAllCommands();
        }

        [RelayCommand]
        private void Skip(string offsetMs1)
        {
            long offsetMs = long.Parse(offsetMs1);
            var newTime = Math.Clamp(MediaPlayer.Time + offsetMs, 0, MediaPlayer.Length);
            MediaPlayer.Time = newTime;
            ShowMessage(FormatMilliseconds(_mediaPlayer.Time));
            NotifyAllCommands();
        }

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
            ShowMessage($"播放速度 {CurrentRate:F1}x");
        }

        [RelayCommand]
        private void ResetSpeed()
        {
            CurrentRate = 1f;
            MediaPlayer.SetRate(CurrentRate);
            ShowMessage($"播放速度 {CurrentRate:F1}x");
        }

        public event EventHandler<string>? SnapshotCompleted;

        [RelayCommand]
        private void Snapshot()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PotPlayerMini64", "Capture", "Snapshots");
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, $"snap_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            MediaPlayer.TakeSnapshot(0, file, 0, 0);
            SnapshotCompleted?.Invoke(this, file);
        }

        private void NotifyAllCommands()
        {
            // 非阻塞地更新所有命令状态
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadCommand.NotifyCanExecuteChanged();
                PlayCommand.NotifyCanExecuteChanged();
                PauseCommand.NotifyCanExecuteChanged();
                StopCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(IsPlaying));
            }));
        }

        [ObservableProperty]
        private string messageText = "";
        private int _messageToken;
        private async void ShowMessage(string message)
        {
            int token = ++_messageToken;
            MessageText = message;
            await Task.Delay(1500);
            if (token == _messageToken)
            {
                MessageText = string.Empty;
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
