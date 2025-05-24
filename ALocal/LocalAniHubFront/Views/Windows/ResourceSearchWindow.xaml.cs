using System.Windows.Controls;
using System.Windows.Input;
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
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is TextBoxItemViewModel vm)
            {
                vm.OnKeyDown(e);
            }
        }
    }
}