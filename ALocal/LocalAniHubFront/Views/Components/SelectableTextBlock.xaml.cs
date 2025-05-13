using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LocalAniHubFront.Views.Components
{
    public partial class SelectableTextBlock : UserControl
    {
        public SelectableTextBlock()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(SelectableTextBlock));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SelectableTextBlock));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(SelectableTextBlock), new PropertyMetadata(false));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                ToggleButton.IsChecked = false;
                return;
            }
            Popup.IsOpen = ToggleButton.IsChecked == true;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleButton.IsChecked = false;
            Popup.IsOpen = false;
        }

        public static readonly DependencyProperty TextStyleProperty =
            DependencyProperty.Register(nameof(TextStyle), typeof(Style), typeof(SelectableTextBlock));


        public Style TextStyle { get=>(Style)GetValue(TextStyleProperty); set=>SetValue(TextStyleProperty,value); }
    }
}
