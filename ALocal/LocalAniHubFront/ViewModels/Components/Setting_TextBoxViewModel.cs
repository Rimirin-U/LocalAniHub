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
using CommunityToolkit.Mvvm.Input;

namespace LocalAniHubFront.ViewModels.Components
{
    public partial class Setting_TextBoxViewModel : ObservableObject
    {
        //设置项名称
        public string EntryName { get; }
        //对应设置项在 GlobalSettingsService 中的键名
        private readonly string _settingKey;
        // 文本框绑定内容（双向绑定）
        [ObservableProperty]
        private string _settingText = string.Empty;
        // 默认构造函数（用于设计器支持）
        public Setting_TextBoxViewModel()
        {
            EntryName = string.Empty;
            _settingKey = string.Empty;
        }


        // 构造函数
        public Setting_TextBoxViewModel(TextboxSettingEntry entry)
           : this(entry.EntryName, entry.SettingKey)
        {
        }
        public Setting_TextBoxViewModel(string entryName, string settingKey)
        {
            EntryName = entryName;
            _settingKey = settingKey;

            // 初始化文本框内容
            SettingText = GlobalSettingsService.Instance.GetValue(_settingKey);
        }
        [RelayCommand]
        private void LostFocus()
        {
            if (SettingText is not null)
            {
                GlobalSettingsService.Instance.SetValue(_settingKey, SettingText);
            }
        }

    }






    /*public class Setting_TextBoxViewModel : INotifyPropertyChanged
    {
        private string _settingText;

        // 设置项的名称，用于绑定到 TextBlock 的 Text 属性
        public string EntryName { get; }
        // 设置项的键，用于在 GlobalSettingsService 中存储和获取值
        private readonly string _key;

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
            _key=key;
            SettingText = GlobalSettingsService.Instance.GetValue(key);

            // 初始化 LostFocusCommand，调用 SaveSetting 方法
            LostFocusCommand = new RelayCommand(SaveSetting);
        }

        // 保存设置到 GlobalSettingsService
        private void SaveSetting()
        {
            if (!string.IsNullOrEmpty(SettingText))
            {
                GlobalSettingsService.Instance.SetValue(_key, SettingText);
            }
        }

        // 通知属性更改
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }*/
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
