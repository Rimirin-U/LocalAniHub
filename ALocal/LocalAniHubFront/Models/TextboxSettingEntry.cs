using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicClassLibrary;

namespace LocalAniHubFront.Models
{
    public class TextboxSettingEntry
    {
        // 设置项显示名称
        public string EntryName { get; init; }
        // 对应全局设置中的键名
        public string SettingKey { get; init; }
        // 获取当前值（从全局设置中获取）
        public string CurrentValue => BasicClassLibrary.GlobalSettingsService.Instance.GetValue(SettingKey);

        public TextboxSettingEntry(string entryName, string settingKey)
        {
            EntryName = entryName;
            SettingKey = settingKey;
        }
    }
}
