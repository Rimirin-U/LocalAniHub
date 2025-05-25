using BasicClassLibrary;
using LocalAniHub.Views.Windows;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

       [RelayCommand]
       private void OnCounterIncrement()
       {
            int episodeId = 1; // 示例ID，实际使用时应根据需要获取
            var window = new ResourceSearchWindow(episodeId);
            window.Show();
            Counter++;
        
       }
    }
}
