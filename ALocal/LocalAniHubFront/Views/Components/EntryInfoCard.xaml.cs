using BasicClassLibrary;
using LocalAniHubFront.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
            InitializeComponent();
        }


        public EntryInfoSet EntryInfoSet{ get; set; }

        private bool initialized = false;

        public static readonly DependencyProperty EntryInfoSetProperty =
            DependencyProperty.Register(
                nameof(EntryInfoSet),
                typeof(EntryInfoSet),
                typeof(EntryInfoCard),
                new PropertyMetadata(null, OnEntryInfoSetChanged));

        private static void OnEntryInfoSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EntryInfoCard control && e.NewValue is EntryInfoSet newSet)
            {
                control.DataContext = new EntryInfoCardViewModel(newSet);
                control.EntryInfoSet = newSet;
            }
        }

        private void EntryInfoSetCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window addEntryWindow = new Views.Windows.AddEntryWindow(EntryInfoSet);
            addEntryWindow.Show();
        }
    }
}
