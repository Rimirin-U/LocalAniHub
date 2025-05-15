using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class MarkdownViewModel : ObservableObject
    {
        private readonly string _filePath;

        [ObservableProperty]
        private string markdownText;

        public MarkdownViewModel(int noteId)
        {
            // 需要额外实现：每30s自动保存；窗口关闭时自动保存
            // _filePath = ...;
            if (File.Exists(_filePath))
                MarkdownText = File.ReadAllText(_filePath);
            else
                MarkdownText = string.Empty;
        }

        [RelayCommand]
        private void SaveFile()
        {
            File.WriteAllText(_filePath, MarkdownText ?? "");
        }
    }
}
