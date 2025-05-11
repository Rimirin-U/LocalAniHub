using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.Models
{
    public class PageSettingEntry
    {
        // 设置项的显示名称
        public string EntryName { get; init; }

        // 设置项的键，用于在 GlobalSettingsService 中存储和检索值
        public string Key { get; init; }

        // 当前设置的描述（例如预览内容）
        public string Description
        {
            get => GetCurrentValue();
        }

        // 构造函数，初始化 PageSettingEntry
        public PageSettingEntry(string entryName, string key)
        {
            EntryName = entryName;
            Key = key;
        }

        // 获取当前值（从 GlobalSettingsService 中加载）
        public string GetCurrentValue()
        {
            return BasicClassLibrary.GlobalSettingsService.Instance.GetValue(Key);
        }

        // 设置当前值（保存到 GlobalSettingsService）
        public void SetCurrentValue(string value)
        {
            BasicClassLibrary.GlobalSettingsService.Instance.SetValue(Key, value);
        }

        // 打开设置页面的方法
        public void OpenSettingPage()
        {
            //这里还没有实现
            // 这里可以实现打开页面的逻辑，例如通过 NavigationService 或其他页面管理工具
            //System.Diagnostics.Debug.WriteLine($"Opening settings page for key: {Key}");
        }
    }
}
