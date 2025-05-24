using BasicClassLibrary;
using System.Collections.ObjectModel;

namespace LocalAniHubFront.ViewModels.Components
{
    public partial class SelectionSetterViewModel : ObservableObject
    {
        private readonly string _settingKey;

        public SelectionSetterViewModel(string settingKey, string settingTitle, ObservableCollection<SelectionItem> items)
        {
            _settingKey = settingKey;
            SettingTitle = settingTitle;
            SelectionItems = items;

            if (int.TryParse(GlobalSettingsService.Instance.GetValue(settingKey), out int storedValue))
            {
                SelectedValue = storedValue;
            }
        }

        [ObservableProperty]
        private string settingTitle;

        [ObservableProperty]
        private ObservableCollection<SelectionItem> selectionItems;

        private int selectedValue;
        public int SelectedValue
        {
            get => selectedValue;
            set
            {
                if (SetProperty(ref selectedValue, value))
                {
                    GlobalSettingsService.Instance.SetValue(_settingKey, value.ToString());
                }
            }
        }
    }

    public class SelectionItem
    {
        public SelectionItem(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }
        public int Value { get; set; }
        public string DisplayName { get; set; }
        public override string ToString() => DisplayName;
    }
}
