using LocalAniHubFront.ViewModels.Windows;

namespace LocalAniHubFront.Views.Windows
{
    public partial class EntryWindow// : Window
    {
        public EntryWindow()
        {
            DataContext = new EntryWindowViewModel();
            InitializeComponent();
        }
    }
}
