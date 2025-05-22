using BasicClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.Models
{
    public partial class PageSettingEntry: ObservableObject
    {
        // 设置项的显示名称
        public string EntryName { get; init; }

        // 设置项的键，用于在 GlobalSettingsService 中存储和检索值
        public string Key { get; init; }

        // 当前设置的描述（例如预览内容）
        [ObservableProperty]
        private string _description;

        // 构造函数，初始化 PageSettingEntry
        public PageSettingEntry(string entryName, string key)
        {
            EntryName = entryName;
            Key = key;
            // 初始化 Description 值
            _description = BasicClassLibrary.GlobalSettingsService.Instance.GetValue(Key);
        }

        // 获取当前值（从 GlobalSettingsService 中加载）
       /* public string GetCurrentValue()
        {
            return BasicClassLibrary.GlobalSettingsService.Instance.GetValue(Key);
        }*/


        // 设置当前值（保存到 GlobalSettingsService）
       /* public void SetCurrentValue(string value)
        {
            BasicClassLibrary.GlobalSettingsService.Instance.SetValue(Key, value);
        }*/

        // 打开设置页面的方法
        public void OpenSettingPage()
        {
            //创建一个新的Windows类型的窗口
            var window = new System.Windows.Window();
            //显示窗口
            //window.Show();
            window.ShowDialog();
        }
    }
}
