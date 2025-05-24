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
            "简单列表（收藏时间降序）",
        };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentView))]
        private string _selectedMode = "简单列表（收藏时间降序）"; // 设置默认值为第一个模式

        [ObservableProperty]
        private UserControl _currentView;

        [RelayCommand]
        private void Refresh()
        {
            OnSelectedModeChanged(SelectedMode);
        }

        private bool _isInitialized = false;

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                CurrentView = new Collection_SimpleEntryTimeList();
                InitializeViewModel();
            }
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            // 初始化视图模型
            _isInitialized = true;
        }
        partial void OnSelectedModeChanged(string value)
        {
            // 根据SelectedMode 切换用户控件
            CurrentView = value switch
            {
                "简单列表（收藏时间降序）" => new Collection_SimpleEntryTimeList(),
                _ => throw new NotSupportedException($"SelectedMode '{value}' is not supported.")
            };
        }
    }
}