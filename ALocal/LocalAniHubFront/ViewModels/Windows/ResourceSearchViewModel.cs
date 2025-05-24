using LocalAniHub.ViewModels.Components;
using System.Collections.ObjectModel;

namespace LocalAniHub.ViewModels.Windows
{
    public partial class ResourceSearchViewModel : ObservableObject
    {
        public ObservableCollection<TextBoxItemViewModel> TextBoxes { get; } = new();

        [ObservableProperty]
        private string windowTitle = "资源搜索: BanG Dream! Ave Mujica 第2集";// 示例

        [RelayCommand]
        private void AddTextBox()
        {
            var item = new TextBoxItemViewModel();
            item.RequestDelete = RemoveTextBox;
            TextBoxes.Add(item);
        }

        private void RemoveTextBox(TextBoxItemViewModel item)
        {
            TextBoxes.Remove(item);
        }
    }
}
