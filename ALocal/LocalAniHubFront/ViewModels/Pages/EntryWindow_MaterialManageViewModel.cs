using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using LocalAniHubFront.Helpers;
using BasicClassLibrary;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class EntryWindow_MaterialManageViewModel
    {
        public int EntryId { get; }

        public EntryWindow_MaterialManageViewModel(int entryId)
        {
            EntryId = entryId;
        }

        public string MaterialFolderPath
        {
            get
            {
                // 假设全局设置服务已实现
                string parentFolder = GlobalSettingsService.Instance.GetValue("defaultMaterialParentFolderPath");
                return Path.Combine(parentFolder, EntryId.ToString());
            }
        }
    }
}
