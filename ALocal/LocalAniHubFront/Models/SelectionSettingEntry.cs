using BasicClassLibrary;

namespace LocalAniHubFront.Models
{
    public class SelectionSettingEntry
    {
        // 可以视情况更改
        public string EntryName { get; init; }
        public string Key { get; init; }//选择设置条目的键
        //此处的类型与.xaml中规定的类型不一致
        //public List<Dictionary<string, string>> Selections { get; init; }
        public List<string> Selections { get; init; }
        //获取默认选项值
        public string Selected
        {
            get => GlobalSettingsService.Instance.GetValue(Key);
            set => GlobalSettingsService.Instance.SetValue(Key, value);
        }
    }
}
