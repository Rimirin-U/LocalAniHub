using LocalAniHubFront.Models;
using LocalAniHubFront.ViewModels.Components;
using System.Windows.Controls;

namespace LocalAniHubFront.Views.Components
{
    public partial class Setting_Textbox : UserControl
    {
        // 可以先写Setting_Selection
        public Setting_Textbox(TextboxSettingEntry settingEntry)// 这个类要定义在Models文件夹中
        {
            DataContext = new Setting_TextBoxViewModel(settingEntry);// 自行创建对应文件:)
            InitializeComponent();
        }
    }
}
