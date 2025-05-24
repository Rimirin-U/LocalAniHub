using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "LocalAniHubFront";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            // 导航栏定义处
            new NavigationViewItem()
            {
                Content = "主页",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "最近",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Clock24 },
                TargetPageType = typeof(Views.Pages.RecentPage)
            },
            new NavigationViewItem()
            {
                Content = "发现",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Search24 },
                TargetPageType = typeof(Views.Pages.SearchPage)
            },
            new NavigationViewItem()
            {
                Content = "收藏",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Star24 },
                TargetPageType = typeof(Views.Pages.CollectionPage)
            },
            new NavigationViewItem()
            {
                Content = "下载",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ArrowDownload24 },
                TargetPageType = typeof(Views.Pages.DownloadPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "设置",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
