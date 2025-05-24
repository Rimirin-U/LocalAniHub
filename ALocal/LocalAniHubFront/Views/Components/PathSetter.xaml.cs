using System.Windows;
using System.Windows.Controls;
using LocalAniHubFront.ViewModels.Components;

namespace LocalAniHubFront.Views.Components
{
    public partial class PathSetter : UserControl
    {
        private PathSetterViewModel viewModel;

        public static readonly DependencyProperty SettingKeyProperty =
            DependencyProperty.Register(nameof(SettingKey), typeof(string), typeof(PathSetter),
                new PropertyMetadata(null, OnSettingKeyChanged));

        public static readonly DependencyProperty SettingTitleProperty =
            DependencyProperty.Register(nameof(SettingTitle), typeof(string), typeof(PathSetter),
                new PropertyMetadata(string.Empty, OnSettingTitleChanged));

        public string SettingKey
        {
            get => (string)GetValue(SettingKeyProperty);
            set => SetValue(SettingKeyProperty, value);
        }

        public string SettingTitle
        {
            get => (string)GetValue(SettingTitleProperty);
            set => SetValue(SettingTitleProperty, value);
        }

        public PathSetter()
        {
            InitializeComponent();
        }

        private static void OnSettingKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PathSetter control)
            {
                control.SetupViewModel();
            }
        }

        private static void OnSettingTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PathSetter control && control.viewModel != null)
            {
                control.viewModel.SettingTitle = (string)e.NewValue;
            }
        }

        private void SetupViewModel()
        {
            if (!string.IsNullOrEmpty(SettingKey))
            {
                viewModel = new PathSetterViewModel(SettingKey, SettingTitle);
                this.DataContext = viewModel;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            viewModel?.SaveIfValid();
        }
    }
}
