using System.Windows.Controls;
using System.Windows.Input;
using LocalAniHub.ViewModels.Components;

namespace LocalAniHub.Views.Windows
{
    public partial class ResourceSearchWindow : Window
    {
        public ResourceSearchWindow()
        {
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