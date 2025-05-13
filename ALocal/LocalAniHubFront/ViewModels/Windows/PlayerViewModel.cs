using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using System;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class PlayerViewModel : ObservableObject, IDisposable
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;
        private Media? _currentMedia;

        public MediaPlayer MediaPlayer => _mediaPlayer;

        [ObservableProperty]
        private string mediaPath = @"D:\Download\[ANi] mono女孩 - 05 [1080P][Baha][WEB-DL][AAC AVC][CHT].mp4";

        public PlayerViewModel()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

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
            NotifyAllCommands();
        }
        private bool CanPause() => _mediaPlayer.CanPause;

        [RelayCommand]
        private void Stop()
        {
            _mediaPlayer.Stop();
            NotifyAllCommands();
        }

        private void NotifyAllCommands()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                LoadCommand.NotifyCanExecuteChanged();
                PlayCommand.NotifyCanExecuteChanged();
                PauseCommand.NotifyCanExecuteChanged();
                StopCommand.NotifyCanExecuteChanged();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadCommand.NotifyCanExecuteChanged();
                    PlayCommand.NotifyCanExecuteChanged();
                    PauseCommand.NotifyCanExecuteChanged();
                    StopCommand.NotifyCanExecuteChanged();
                });
            }
        }


        public void Dispose()
        {
            _currentMedia?.Dispose();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }
    }
}