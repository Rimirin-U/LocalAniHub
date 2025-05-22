using LocalAniHubFront.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalAniHubFront.Views.Pages
{
    /// <summary>
    /// EntryWindow_ResourceManage.xaml 的交互逻辑
    /// </summary>
    public partial class EntryWindow_ResourceManage : Page,INotifyPropertyChanged
    {
        private int _entryId;
        public int EntryId
        {
            get => _entryId;
            set
            {
                if (_entryId != value)
                {
                    _entryId = value;
                    OnPropertyChanged();
                }
            }
        }

        public EntryWindow_ResourceManage()
        {
            DataContext = new EntryWindow_ResourceManageViewModel(EntryId);
            InitializeComponent();
        }

        public void InitializeViewModel()
        {
            DataContext = new EntryWindow_ResourceManageViewModel(EntryId);
        }


        // 属性更新通知
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /*
        public readonly record struct ResourceTempData(int EpisodeNumber, string ResourceName);

        public ObservableCollection<ResourceTempData> ResourcesData { get; } = new()
        {
            new ResourceTempData(1,@"[Nekomoe kissaten][Shumatsu Train Dokoe Iku]?[01][1080p][JPSC].mp4"),
            new ResourceTempData(2,@"[Nekomoe kissaten][Shumatsu Train Dokoe Iku]?[02][720p][JPSC].mp4")
        };
        */

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "导入资源",
                Filter = "视频文件 (*.mp4;*.avi;*.mov;*.mkv;*.wmv;*.flv)|*.mp4;*.avi;*.mov;*.mkv;*.wmv;*.flv",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedFiles = openFileDialog.FileNames;
                EntryWindow_ResourceManageViewModel vm = DataContext as EntryWindow_ResourceManageViewModel;
                vm.AddResources(selectedFiles);// 需要ViewModel实现
            }
        }
    }
}
