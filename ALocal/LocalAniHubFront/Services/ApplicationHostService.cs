using BasicClassLibrary;
using LocalAniHubFront.Views.Pages;
using LocalAniHubFront.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace LocalAniHubFront.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        private INavigationWindow _navigationWindow;

        public ApplicationHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                _navigationWindow = (
                    _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
                )!;

                // 初始化状态检测与窗口打开
                // 检查是否是首次运行
                bool isInitialized = GlobalSettingsService.Instance.GetValue("isInitialized") == "true";
                if (!isInitialized)
                {
                    // 弹出初始化设置窗口
                    var initializeWindow = new InitializeWindow();
                    initializeWindow.ShowDialog();  // ShowDialog:模态窗口，关闭后才能继续
                    if (initializeWindow.ShouldShutDown) return;
                }

                // 打开MainWindow
                _navigationWindow!.ShowWindow();
                // 默认导航定义处 //
                _navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));
            }

            await Task.CompletedTask;
        }
    }
}
