using BasicClassLibrary;
using LocalAniHub.ViewModels.Components;
using System.Collections.ObjectModel;

namespace LocalAniHub.ViewModels.Windows
{
    public partial class ResourceSearchViewModel : ObservableObject
    {
        public ObservableCollection<TextBoxItemViewModel> TextBoxes { get; } = new();

        [ObservableProperty]
        private ObservableCollection<ResourceItem> resourceItems;

        [ObservableProperty]
        private string windowTitle = "资源搜索: BanG Dream! Ave Mujica 第2集";// 示例

        [RelayCommand]
        private void AddTextBox()
        {
            var item = new TextBoxItemViewModel();
            item.RequestDelete = (self, _) => RemoveTextBox(self);
            TextBoxes.Add(item);
        }


        [RelayCommand]
        private void Search()
        {
            // ...
            // ... 以TextBoxes中的所有Text为List<string>，进行资源搜索，以填充resourceItems
        }

        private void RemoveTextBox(TextBoxItemViewModel item)
        {
            var index = TextBoxes.IndexOf(item);
            TextBoxItemViewModel? previous = index > 0 ? TextBoxes[index - 1] : null;
            TextBoxes.Remove(item);
            RequestFocus?.Invoke(previous);
        }

        public Action<TextBoxItemViewModel?>? RequestFocus { get; set; }
    }
}
