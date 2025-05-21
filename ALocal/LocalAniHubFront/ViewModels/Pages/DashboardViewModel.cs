﻿namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Window window = new Views.Windows.AddEntryWindow();
            window.Show();
            Counter++;
        }
    }
}
