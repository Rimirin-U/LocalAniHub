using LocalAniHub.ViewModels.Components;
using System.Collections.ObjectModel;

namespace LocalAniHub.ViewModels.Windows
{
    public partial class ResourceSearchViewModel : ObservableObject
    {
        public ObservableCollection<TextBoxItemViewModel> TextBoxes { get; } = new();

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
