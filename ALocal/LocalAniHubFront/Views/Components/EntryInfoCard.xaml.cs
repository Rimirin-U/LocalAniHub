using BasicClassLibrary;
using LocalAniHubFront.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalAniHubFront.Views.Components
{
    public partial class EntryInfoCard : UserControl
    {
        public EntryInfoCard()
        {
            DataContext = new EntryInfoCardViewModel(EntryInfoSet);
            InitializeComponent();
        }

        
        public EntryInfoSet EntryInfoSet { get; set; }

        public static readonly DependencyProperty EntryInfoSetProperty =
            DependencyProperty.Register(
                nameof(EntryInfoSet),
                typeof(EntryInfoSet),
                typeof(EntryInfoCard));
    }
}
