using LocalAniHubFront.ViewModels.Components;
using System.Collections.ObjectModel;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        [ObservableProperty]
        private ObservableCollection<SelectionItem> entryWindowMainTitleItems = [ 
            new(0,"主标题原名 & 副标题译名"),
            new(1,"主标题译名 & 副标题原名")
        ];
        
        [ObservableProperty]
        private ObservableCollection<SelectionItem> defaultEntryFetchSourceItems = [ 
            new(0,"来自Yuc's Anime List")
        ];
        
        [ObservableProperty]
        private ObservableCollection<SelectionItem> defaultResourceSearchSourceItems = [ 
            new(0,"来自Animes Garden")
        ];
        
        [ObservableProperty]
        private ObservableCollection<SelectionItem> defaultCollectionDisplayViewItems = [ 
            new(0,"简单列表（按收藏时间降序）")
        ];

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"LocalAniHubFront - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }
    }
}
