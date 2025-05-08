using LocalAniHubFront.ViewModels.Components;
using System.Windows.Controls;

namespace LocalAniHubFront.Views.Components
{
    public partial class Collection_SimpleEntryTimeList : UserControl
    {
        public Collection_SimpleEntryTimeList()
        {
            DataContext = new Collection_SimpleEntryTimeListViewModel();
            InitializeComponent();
        }
    }
}
