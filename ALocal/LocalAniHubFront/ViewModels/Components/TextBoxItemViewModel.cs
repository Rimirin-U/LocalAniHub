using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace LocalAniHub.ViewModels.Components
{
    public partial class TextBoxItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? text;

        public Action<TextBoxItemViewModel, TextBoxItemViewModel?>? RequestDelete { get; set; }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Back && string.IsNullOrEmpty(Text))
            {
                // 前一个由 ResourceSearchViewModel 查找后回调 Focus
                RequestDelete?.Invoke(this, null);
                e.Handled = true;
            }
        }

    }
}