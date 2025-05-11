using LocalAniHubFront.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BasicClassLibrary;
using System.Windows.Shapes;
using System.Diagnostics;

namespace LocalAniHubFront.ViewModels.Pages
{
    public class PlayerViewModel : ObservableObject
    {
        private PlayerModel model;

        public PlayerModel Model
        {
            get { return model; }
            set { SetProperty(ref model, value); }
        }
        private MediaElement media;//WPF 的 MediaElement 控件引用，用于播放媒体（就是播放的屏幕）

        private Task mediaTask;//用于更新播放进度的后台任务
        private bool IsRunning = false;//标记媒体是否正在播放

        public PlayerViewModel()
        {
            Model = new PlayerModel()
            {
                Resources = new ObservableCollection<Resource>()
            };

        }

        #region Loaded

        //窗口加载完成时执行
        private RelayCommand<object> loadedCommand;

        public RelayCommand<object> LoadedCommand
        {
            get
            {
                if (loadedCommand == null)
                {
                    loadedCommand = new RelayCommand<object>(Loaded);
                }
                return loadedCommand;
            }
        }

        //获取窗口中的 MediaElement 控件(播放的屏幕）引用
        private void Loaded(object obj)
        {
            // 将传入的对象转换为RoutedEventArgs类型
            var args = obj as RoutedEventArgs;
            if (args != null)
            {
             // 从事件参数中获取事件源对象，并将其转换为主窗口类型(MainWindow)
                var win = args.OriginalSource as MainWindow;
                this.media = win.mediaElement;
            }
        }

        #endregion

        #region 浏览视频

        private RelayCommand browserCommand;

        public RelayCommand BrowserCommand
        {
            get
            {
                if (browserCommand == null)
                {
                    browserCommand = new RelayCommand(Browser);
                }
                return browserCommand;
            }
        }

        private void Browser()
        {
            // 创建一个打开文件对话框对象
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择要播放的影片";
            // 设置文件筛选器，只显示扩展名为.mp4的文件
            dialog.Filter = "MP4|*.mp4";
            // 设置是否允许多选文件，这里设置为允许
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                // 获取用户选择的所有文件的路径集合
                var files = dialog.FileNames;
                // 遍历用户选择的每个文件路径
                foreach (var file in files)
                {
                    // 如果影片集合不为空，获取影片集合中最大的Id值，否则设置为0
                    var maxId = this.Model.Resources.Count > 0 ? this.Model.Resources.Max(x => x.Id) : 0;
                    if (this.Model.Resources.FirstOrDefault(item => item.Url == file) == null)
                    {
                        // 创建一个新的影片对象，并添加到影片集合中，新影片的Id为最大Id加1，
                        // 名称为文件名（不包含扩展名），路径为当前文件路径
                        this.Model.Resources.Add(new Resource(episodeId: null,
                            episode: null,
                            importData: DateTime.Now,
                            path: file)
                        {
                           Id=maxId+1,
                           ResourcePath=file
                        });
                    }
                }
            }
        }

        #endregion

        #region 双击列表打开视频


        private RelayCommand<object> mouseDoubleCommand;

        public RelayCommand<object> MouseDoubleCommand
        {
            get
            {
                if (mouseDoubleCommand == null)
                {
                    mouseDoubleCommand = new RelayCommand<object>(MouseDoubleClick);
                }
                return mouseDoubleCommand;
            }
        }

        private void MouseDoubleClick(object obj)
        {
            if (obj == null)
            {
                return;
            }
            this.Model.CurMovie = obj as Resource;
            this.Play();
        }

        #endregion

        #region 视频操作

        private RelayCommand playCommand;

        public RelayCommand PlayCommand
        {
            get
            {
                if (playCommand == null)
                {
                    playCommand = new RelayCommand(Play);
                }
                return playCommand;
            }
        }

        //视频播放功能，开始播放后，单独打开一个线程，用来刷新播放的进度
        private void Play()
        {
            //影片播放，如果没有，则打开选择文件夹
            if (this.Model.CurMovie == null)
            {
                this.Browser();
                // 如果选择后资源集合仍为空，则退出方法
                if (this.Model.Resources.Count < 1)
                {
                    return;
                }
                // 选择集合中的最后一个影片作为当前影片
                this.Model.CurMovie = this.Model.Resources.Last();
            }
            // 设置当前媒体源为选中影片的URL
            if (Uri.TryCreate(this.Model.CurMovie.Url, UriKind.RelativeOrAbsolute, out var uri))
            {
                this.Model.CurSource = uri;
            }
            else
            {
                // 处理无效URI的情况
                Debug.WriteLine("无效的媒体URL");
                // 可以设置默认源或通知用户
            }
            // 如果媒体已加载完成且已播放到末尾，则将播放位置重置为开始处
            if (this.media.NaturalDuration != Duration.Automatic && this.media.Position.TotalSeconds == this.media.NaturalDuration.TimeSpan.TotalSeconds)
            {
                this.media.Position = new TimeSpan(0, 0, 0);
            }
            // 开始播放媒体
            this.media.Play();
            this.IsRunning = true;
            // 隐藏播放按钮（通常与暂停按钮切换显示）
            this.Model.PlayButtonVisibility = Visibility.Collapsed;
            // 启动一个后台任务来更新播放进度
            this.mediaTask = Task.Run(() =>
            {
                while (this.IsRunning)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.Model.Position = this.media.Position.TotalSeconds;
                    });
                    Thread.Sleep(100);
                }
            });
        }


        private RelayCommand pauseCommand;

        public RelayCommand PauseCommand
        {
            get
            {
                if (pauseCommand == null)
                {
                    pauseCommand = new RelayCommand(Pause);
                }
                return pauseCommand;
            }
        }

        //Pause功能是将播放位置保持在当前位置；Stop功能是将视频的播放位置重置到初始位置。
        private void Pause()
        {
            if (this.Model.CurMovie == null)
            {
                return;
            }
            this.media.Pause();
            this.IsRunning = false;
        }

        private RelayCommand stopCommand;

        public RelayCommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                {
                    stopCommand = new RelayCommand(Stop);
                }
                return stopCommand;
            }
        }

        private void Stop()
        {
            if (this.Model.CurMovie == null)
            {
                return;
            }
            this.media.Stop();
            this.IsRunning = false;
        }


        private RelayCommand mediaOpenedCommand;

        public RelayCommand MediaOpenedCommand
        {
            get
            {
                if (mediaOpenedCommand == null)
                {
                    mediaOpenedCommand = new RelayCommand(MediaOpened);
                }
                return mediaOpenedCommand;
            }
        }

        //视频打开事件，即当视频开始时触发的路由事件
        //视频开始时，初始化播放时长
        private void MediaOpened()
        {
            if (this.media.NaturalDuration.TimeSpan.TotalMinutes < 60)
            {
                this.Model.TimeLen = this.media.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
            else
            {
                this.Model.TimeLen = this.media.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
            }
            this.Model.MediaMaximum = this.media.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private RelayCommand mediaEndedCommand;

        public RelayCommand MediaEndedCommand
        {
            get
            {
                if (mediaEndedCommand == null)
                {
                    mediaEndedCommand = new RelayCommand(MediaEnded);
                }
                return mediaEndedCommand;
            }
        }

        //视频结束事件，即当视频播放完毕时触发的路由事件
        //将播放状态置为flase
        private void MediaEnded()
        {
            this.Model.PlayButtonVisibility = Visibility.Visible;
            this.IsRunning = false;
        }
        #endregion
    }
}
