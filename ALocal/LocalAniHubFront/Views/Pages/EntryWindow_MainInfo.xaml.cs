using System;
using System.Collections.Generic;
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
    public partial class EntryWindow_MainInfo : Page, INotifyPropertyChanged
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

        public EntryWindow_MainInfo()
        {
            DataContext = new EntryWindow_MainInfoViewModel(EntryId);
            InitializeComponent();
        }

        public void InitializeViewModel()
        {
            DataContext = new EntryWindow_MainInfoViewModel(EntryId);
        }

        // 属性更新通知
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // 页面导航按钮
        private void ResourceButton_Click(object sender, RoutedEventArgs e)
        {
            var page = new EntryWindow_ResourceManage();
            page.EntryId = EntryId;
            page.InitializeViewModel();
            NavigationService.Navigate(page);
        }
        private void NoteButton_Click(object sender, RoutedEventArgs e)
        {
            var page = new EntryWindow_NoteManage();
            page.EntryId = EntryId;
            page.InitializeViewModel();
            NavigationService.Navigate(page);
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var page = new EntryWindow_History();
            page.EntryId = EntryId;
            page.InitializeViewModel();
            NavigationService.Navigate(page);
        }
        private void MaterialButton_Click(object sender, RoutedEventArgs e)
        {
            var page = new EntryWindow_MaterialManage();
            page.EntryId = EntryId;
            page.InitializeViewModel();
            NavigationService.Navigate(page);
        }
        private void EpisodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int id))
            {
                NavigateToEpisodePage(id);
            }
        }
        private void NavigateToEpisodePage(int id)
        {
            var page = new EntryWindow_Episode();
            page.EpisodeId = id;
            page.InitializeViewModel();
            NavigationService.Navigate(page);
        }

        // 评分
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb &&
                int.TryParse(tb.Tag?.ToString(), out int value) &&
                DataContext is EntryWindow_MainInfoViewModel vm)
            {
                vm.SetScore(value); // 调用 ViewModel 中的方法
            }
        }
    }
}
