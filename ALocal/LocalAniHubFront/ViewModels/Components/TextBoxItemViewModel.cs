using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace LocalAniHub.ViewModels.Components
{
    public partial class TextBoxItemViewModel : ObservableObject
    {
        public Guid Id { get; } = Guid.NewGuid();

        [ObservableProperty]
        private string? text;

        public Action<TextBoxItemViewModel>? RequestDelete { get; set; }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Back && string.IsNullOrEmpty(this.Text))
            {
                RequestDelete?.Invoke(this);
                e.Handled = true;
            }
        }
    }
}