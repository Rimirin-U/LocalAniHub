using BasicClassLibrary;
using LocalAniHubFront.Services;
using LocalAniHubFront.ViewModels.Pages;
using LocalAniHubFront.ViewModels.Windows;
using LocalAniHubFront.Views.Pages;
using LocalAniHubFront.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace LocalAniHubFront
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)); })
            .ConfigureServices((context, services) =>
            {
                services.AddNavigationViewPageProvider();

                services.AddHostedService<ApplicationHostService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                // 导航栏声明处
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                //services.AddSingleton<DataPage>();
                //services.AddSingleton<DataViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();

                // LocalAniHub
                services.AddSingleton<CollectionPage>();
                services.AddSingleton<CollectionViewModel>();
                services.AddSingleton<RecentPage>();
                services.AddSingleton<RecentPageViewModel>();
                services.AddSingleton<SearchPage>();
                services.AddSingleton<SearchViewModel>();
                services.AddSingleton<DownloadPage>();
                services.AddSingleton<DownloadViewModel>();


            }).Build();

        /// <summary>
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            // 初始化程序段
            // 数据库初始化
            using (var context = new AppDbContext())
            {
                // 自动创建数据库和表结构（如果不存在）
                context.Database.EnsureCreated();
            }

            // load trackers
            TrackerManager trackerManager = new();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "preloadTrackers.txt"); ;
            var lst = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToList();
            foreach (var str in lst)
            {
                trackerManager.Add(new(str));
            }

            // debug: 插入初始数据
            // var settingService = GlobalSettingsService.Instance;
            // settingService.SetValue("defaultPlayerPath", @"C:\Program Files\DAUM\PotPlayer\PotPlayerMini64.exe");
            // var entryManager = new EntryManager();
            // entryManager.Add(new("迷途之子!!!!!","BanG Dream! It's MyGO!!!!!",new(2023,6,29),new(2023,10,29),"原创动画",13,State.Watching)
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
