using LocalAniHubFront.ViewModels.Windows;
using Microsoft.Win32;
using System.ComponentModel;

namespace LocalAniHubFront.Views.Windows
{
    public partial class InitializeWindow
    {
        public InitializeViewModel ViewModel { get; }

        public InitializeWindow()
        {
            ViewModel = new();
            DataContext = this;
            InitializeComponent();
        }

        private void SelectGlobalBaseFolder_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewModel.GlobalBaseFolder = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void SelectDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewModel.DownloadPath = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private bool _isSettingsSaved = false;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.Check())
            {
                return;
            }
            ViewModel.Save();
            _isSettingsSaved = true;
            Close();
        }

        public bool ShouldShutDown = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!_isSettingsSaved)
            {
                ShouldShutDown = true;
                Application.Current.Shutdown();
            }
        }
    }
}
