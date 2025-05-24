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
            var window = new ResourceSearchWindow();
            window.Show();
            Counter++;
        }
    }
}
