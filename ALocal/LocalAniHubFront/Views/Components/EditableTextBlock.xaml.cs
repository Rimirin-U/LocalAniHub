using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LocalAniHubFront.Views.Components
{
    public partial class EditableTextBlock : UserControl
    {
        public EditableTextBlock()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableTextBlock), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly) return;

            IsEditing = true;
            Editor.Focus();
            Editor.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => CommitEdit();

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CommitEdit();
            else if (e.Key == Key.Escape)
                IsEditing = false;
        }

        private void CommitEdit()
        {
            Editor.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            IsEditing = false;
            TextChanged?.Invoke(this, new RoutedEventArgs());
        }

        public event RoutedEventHandler TextChanged;
    }
}
