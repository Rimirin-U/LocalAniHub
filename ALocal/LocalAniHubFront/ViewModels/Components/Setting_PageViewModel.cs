using LocalAniHubFront.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.ViewModels.Components
{
    public partial class Setting_PageViewModel : ObservableObject
    {
        private readonly PageSettingEntry _entry;

        // 设置项的名称，用于绑定到 TextBlock 的 Text 属性
        public string EntryName => _entry.EntryName;

        // 设置项的描述，用于绑定到 TextBlock 的 Text 属性
        public string Description => _entry.Description;

        // 构造函数，初始化 ViewModel
        public Setting_PageViewModel(PageSettingEntry entry)
        {
            _entry = entry;
        }

        // 使用 [RelayCommand] 特性定义命令
        [RelayCommand]
        private void ShowSettingPageCommand()
        {
            // 调用 PageSettingEntry的 OpenSettingPage 方法
            _entry.OpenSettingPage();
        }
    }
}
