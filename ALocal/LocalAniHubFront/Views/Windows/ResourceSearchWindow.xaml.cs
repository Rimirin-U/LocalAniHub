using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LocalAniHub.ViewModels.Components;
using LocalAniHub.ViewModels.Windows;

namespace LocalAniHub.Views.Windows
{
    public partial class ResourceSearchWindow
    {
        public ResourceSearchViewModel ViewModel { get; }

        public ResourceSearchWindow()
        {
            ViewModel = new();
            DataContext = this;
            InitializeComponent();

            ViewModel.RequestFocus = FocusTextBoxByViewModel;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is TextBoxItemViewModel vm)
            {
                vm.OnKeyDown(e);
            }
        }

        private void FocusTextBoxByViewModel(TextBoxItemViewModel? vm)
        {
            if (vm == null) return;

            // 遍历所有 TextBox 找到对应 ViewModel 的控件
            foreach (var child in FindVisualChildren<TextBox>(this))
            {
                if (child.DataContext == vm)
                {
                    child.Focus();
                    child.SelectAll(); // 可选：选中文本
                    break;
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T variable)
                    yield return variable;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }
}