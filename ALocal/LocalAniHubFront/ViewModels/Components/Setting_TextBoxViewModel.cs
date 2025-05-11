using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicClassLibrary;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LocalAniHubFront.Models;

namespace LocalAniHubFront.ViewModels.Components
{
    public class Setting_TextBoxViewModel : INotifyPropertyChanged
    {
        private string _settingText;

        // 设置项的名称，用于绑定到 TextBlock 的 Text 属性
        public string EntryName { get; }

        // 设置项的值，用于绑定到 TextBox 的 Text 属性
        public string SettingText
        {
            get => _settingText;
            set
            {
                if (_settingText != value)
                {
                    _settingText = value;
                    OnPropertyChanged();//通知视图属性值已更改。
                }
            }
        }

        // 失焦时保存设置的命令
        public ICommand LostFocusCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        // 构造函数，初始化 ViewModel
        public Setting_TextBoxViewModel(string entryName, string key)
        {
            EntryName = entryName;
            SettingText = GlobalSettingsService.Instance.GetValue(key);

            // 初始化 LostFocusCommand，调用 SaveSetting 方法
            LostFocusCommand = new RelayCommand(_ => SaveSetting(key));
        }

        // 保存设置到 GlobalSettingsService
        private void SaveSetting(string key)
        {
            if (!string.IsNullOrEmpty(SettingText))
            {
                GlobalSettingsService.Instance.SetValue(key, SettingText);
            }
        }

        // 通知属性更改
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /*
    // 简单的 RelayCommand 实现
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }*/
}
