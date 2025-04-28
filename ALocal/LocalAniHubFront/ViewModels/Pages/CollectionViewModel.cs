using LocalAniHubFront.Views.Components;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class CollectionViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private ObservableCollection<string> _displayModes = new ObservableCollection<string>
        {
            "SimpleEntryTimeList",
            "Mode2",
            "Mode3"
        };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentView))]
        private string _selectedMode = "SimpleEntryTimeList"; // 设置默认值为第一个模式

        [ObservableProperty]
        private UserControl _currentView = new Collection_SimpleEntryTimeList(); // 默认视图

        private bool _isInitialized = false;

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            // 初始化视图模型
            _isInitialized = true;

            // 根据默认选中的模式加载初始视图
            OnSelectedModeChanged();
        }

        [RelayCommand]
        private void OnSelectedModeChanged()
        {
            // 根据SelectedMode 切换用户控件
            CurrentView = SelectedMode switch
            {
                "SimpleEntryTimeList" => new Collection_SimpleEntryTimeList(),
                "Mode2" => throw new NotSupportedException("Mode2 is not supported yet."),
                "Mode3" => throw new NotSupportedException("Mode3 is not supported yet."),
                _ => throw new NotSupportedException($"SelectedMode '{SelectedMode}' is not supported.")
            };
        }
    }
}