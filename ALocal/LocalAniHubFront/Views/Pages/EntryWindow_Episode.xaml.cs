using BasicClassLibrary;
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
    public partial class EntryWindow_Episode : Page, INotifyPropertyChanged
    {
        private int _episodeId;
        public int EpisodeId
        {
            get => _episodeId;
            set
            {
                if (_episodeId != value)
                {
                    _episodeId = value;
                    OnPropertyChanged();
                }
            }
        }

        public EntryWindow_Episode()
        {
            DataContext = new EntryWindow_EpisodeViewModel(EpisodeId);
            InitializeComponent();
        }

        public void InitializeViewModel()
        {
            DataContext = new EntryWindow_EpisodeViewModel(EpisodeId);
        }

        // 属性更新通知
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            var page = new EntryWindow_EpisodeHistory();
            page.EpisodeId = EpisodeId;
            page.InitializeViewModel();
            NavigationService.Navigate(page);
            */
        }

        private void AddResourceButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
            openFileDialog.Multiselect = false;

            // 显示对话框并判断用户是否点击了“确定”
            if (openFileDialog.ShowDialog() == true)
            {
                // 获取选中的文件路径
                string filePath = openFileDialog.FileName;
                var vm = DataContext as EntryWindow_EpisodeViewModel;
                if (vm is EntryWindow_EpisodeViewModel) vm.AddResource(filePath);
            }
        }
    }
}
