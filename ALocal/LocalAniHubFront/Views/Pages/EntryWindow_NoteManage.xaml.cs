using LocalAniHubFront.ViewModels.Pages;
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
    public partial class EntryWindow_NoteManage : Page,INotifyPropertyChanged
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

        public EntryWindow_NoteManage()
        {
            DataContext = new EntryWindow_NoteManageViewModel(EntryId);
            InitializeComponent();
        }

        public void InitializeViewModel()
        {
            DataContext = new EntryWindow_NoteManageViewModel(EntryId);
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
                Title = "导入评价",
                Filter = "Markdown (*.md)|*.md",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedFiles = openFileDialog.FileNames;
                EntryWindow_NoteManageViewModel vm = DataContext as EntryWindow_NoteManageViewModel;
                vm.AddResources(selectedFiles[0]);// 需要ViewModel实现（
            }
        }
    }
}
