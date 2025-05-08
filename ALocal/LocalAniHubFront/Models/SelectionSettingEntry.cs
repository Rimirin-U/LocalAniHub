namespace LocalAniHubFront.Models
{
    public class SelectionSettingEntry
    {
        // 可以视情况更改
        public string EntryName { get; init; }
        public string Key { get; init; }
        public List<Dictionary<string, string>> Selections { get; init; }
    }
}
