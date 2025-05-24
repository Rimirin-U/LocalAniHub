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
        private readonly int _entryId;
        private readonly MaterialService _materialService;
        private readonly EntryManager _entryManager;

        public string MaterialFolderPath { get; }
        public EntryWindow_MaterialManageViewModel(int entryId)
        {
            _entryId = entryId;
            _materialService = new MaterialService();//这个报错不用管
            _entryManager = new EntryManager();
            var entry = _entryManager.FindById(entryId);
            // 获取素材路径
            //MaterialFolderPath = _materialService.GetEntryMaterialPath(entry, "");
            // 获取全局父文件夹
            string globalBaseFolder = GlobalSettingsService.Instance.GetValue("globalBaseFolder");
            //拼接完整路径
            MaterialFolderPath = Path.Combine(globalBaseFolder, "Material", entry.MaterialFolder);
        }
        /* public int EntryId { get; }

         public EntryWindow_MaterialManageViewModel(int entryId)
         {
             EntryId = entryId;
         }

         public string MaterialFolderPath
         {
             get
             {
                 // 假设全局设置服务已实现-这里还需要再修改一下
                 string parentFolder = GlobalSettingsService.Instance.GetValue("defaultMaterialParentFolderPath");
                 return Path.Combine(parentFolder, EntryId.ToString());
             }
         }*/
    }
}
