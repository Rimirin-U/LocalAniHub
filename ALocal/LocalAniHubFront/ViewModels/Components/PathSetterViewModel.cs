using BasicClassLibrary;
using System.IO;
using System.Windows.Forms;

namespace LocalAniHubFront.ViewModels.Components
{
    public partial class PathSetterViewModel : ObservableObject
    {
        private readonly string _settingKey;

        public PathSetterViewModel(string settingKey, string settingTitle)
        {
            _settingKey = settingKey;
            SettingTitle = settingTitle;
            SettingValue = GlobalSettingsService.Instance.GetValue(_settingKey) ?? string.Empty;
            _originalValue = SettingValue;
        }

        [ObservableProperty]
        private string settingTitle;

        [ObservableProperty]
        private string settingValue;

        private string _originalValue;

        [RelayCommand]
        private void SelectPath()
        {
            using var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = SettingValue;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var selected = dialog.SelectedPath;
                if (Directory.Exists(selected))
                {
                    SettingValue = selected;
                    Save();
                }
                else
                {
                    ShowError();
                }
            }
        }

        public void SaveIfValid()
        {
            if (Directory.Exists(SettingValue))
            {
                Save();
            }
            else
            {
                ShowError();
                SettingValue = _originalValue;
            }
        }

        private void Save()
        {
            GlobalSettingsService.Instance.SetValue(_settingKey, SettingValue);
            _originalValue = SettingValue;
        }

        private void ShowError()
        {
            System.Windows.MessageBox.Show("路径不存在，无法保存设置。", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
