using BasicClassLibrary;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            // debug
            Window window = new Views.Windows.AddEntryWindow();
            window.Show();
            Counter++;
        }
    }
}
