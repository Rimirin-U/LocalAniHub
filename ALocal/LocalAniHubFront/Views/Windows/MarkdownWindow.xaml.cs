using ICSharpCode.AvalonEdit.Highlighting;
using LocalAniHubFront.ViewModels.Windows;
using Rhizine.AvalonEdit.MarkdownEditor;

namespace LocalAniHubFront.Views.Windows
{
    public partial class MarkdownWindow
    {
        public MarkdownWindow(int noteId)
        {
            InitializeComponent();
            var vm = new MarkdownViewModel(noteId);
            DataContext = vm;

            // 初始化时设置编辑器内容
            Editor.Text = vm.MarkdownText ?? "";

            // 编辑器内容变化时更新 ViewModel
            Editor.TextChanged += (s, e) =>
            {
                if (Editor.Text != vm.MarkdownText)
                    vm.MarkdownText = Editor.Text;
            };

            // ViewModel 内容变化时更新编辑器（可选，防止外部修改）
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.MarkdownText) && Editor.Text != vm.MarkdownText)
                {
                    Editor.Text = vm.MarkdownText ?? "";
                }
            };
        }
    }

    public enum MarkdownWindow_OpenOp
    {
        Edit = 0,
        View = 1
    }
}
