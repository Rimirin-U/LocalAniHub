using LocalAniHubFront.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BasicClassLibrary;

namespace LocalAniHubFront.ViewModels.Components
{
    public class Setting_SelectionViewModel : INotifyPropertyChanged
    //实现了 INotifyPropertyChanged 接口，用于在属性值发生变化时通知视图。
    {
        private string _selected;//用于存储当前选中的值。

        public string EntryName { get; }//表示选择设置条目的名称，只读属性。
        public List<string> Selections { get; }//表示选择设置条目的可选值列表，只读属性。
        public string Selected //表示当前选中的值
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged();//当选中的值发生变化时，触发属性变化通知。
                    SaveSelectedValue();//保存选中的值。
                }
            }
        }

        public ICommand SelectionChangedCommand { get; }//表示选择变化时的命令，用于处理用户选择的变化。

        public event PropertyChangedEventHandler PropertyChanged;

        public Setting_SelectionViewModel(SelectionSettingEntry entry)
        {
            EntryName = entry.EntryName;
            Selections = entry.Selections;
            Selected = entry.GetDefaultValue();
           
            SelectionChangedCommand = new RelayCommand(_ => SaveSelectedValue());
            //初始化 SelectionChangedCommand 属性，使用 RelayCommand 类创建一个命令，当命令执行时调用 SaveSelectedValue 方法。
        }

        private void SaveSelectedValue()
        {
            if (!string.IsNullOrEmpty(Selected))
            {
                GlobalSettingsService.Instance.SetValue(EntryName, Selected);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 简单的 RelayCommand 实现
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;//字段存储了命令执行时要调用的方法。
        private readonly Predicate<object> _canExecute;//字段存储了一个用于判断命令是否可以执行的谓词。

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
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
    }
}