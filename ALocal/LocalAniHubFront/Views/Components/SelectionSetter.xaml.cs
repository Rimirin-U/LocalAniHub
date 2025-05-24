using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LocalAniHubFront.Models;
using LocalAniHubFront.Services;
using LocalAniHubFront.ViewModels.Components;

namespace LocalAniHubFront.Views.Components
{
    public partial class SelectionSetter : UserControl
    {
        public static readonly DependencyProperty SettingKeyProperty = DependencyProperty.Register(
            nameof(SettingKey), typeof(string), typeof(SelectionSetter), new PropertyMetadata(default(string), OnSettingKeyChanged));

        public static readonly DependencyProperty SettingTitleProperty = DependencyProperty.Register(
            nameof(SettingTitle), typeof(string), typeof(SelectionSetter), new PropertyMetadata(default(string), OnSettingTitleChanged));

        public static readonly DependencyProperty SettingItemsProperty = DependencyProperty.Register(
            nameof(SettingItems), typeof(ObservableCollection<SelectionItem>), typeof(SelectionSetter), new PropertyMetadata(null, OnSettingItemsChanged));

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

        public ObservableCollection<SelectionItem> SettingItems
        {
            get => (ObservableCollection<SelectionItem>)GetValue(SettingItemsProperty);
            set => SetValue(SettingItemsProperty, value);
        }

        private SelectionSetterViewModel viewModel;

        private bool _hasKey;
        private bool _hasTitle;
        private bool _hasItems;

        public SelectionSetter()
        {
            InitializeComponent();
        }

        private static void OnSettingKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectionSetter c)
            {
                c._hasKey = true;
                c.TryInitializeViewModel();
            }
        }

        private static void OnSettingTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectionSetter c)
            {
                c._hasTitle = true;
                if (c.viewModel != null)
                    c.viewModel.SettingTitle = (string)e.NewValue;
                c.TryInitializeViewModel();
            }
        }

        private static void OnSettingItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectionSetter c)
            {
                c._hasItems = true;
                if (c.viewModel != null && e.NewValue != null)
                    c.viewModel.SelectionItems = (ObservableCollection<SelectionItem>)e.NewValue;
                c.TryInitializeViewModel();
            }
        }

        private void TryInitializeViewModel()
        {
            if (_hasKey && _hasTitle && _hasItems && viewModel == null)
            {
                viewModel = new SelectionSetterViewModel(SettingKey, SettingTitle, SettingItems);
                this.DataContext = viewModel;
            }
        }
    }
}
