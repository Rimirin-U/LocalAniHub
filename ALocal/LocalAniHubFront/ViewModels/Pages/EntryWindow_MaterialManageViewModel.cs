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

        // 添加路径为filePath的文件
        public void AddMaterial(string filePath)
        {
            // 检查源文件是否存在
            if (!File.Exists(filePath))
            {
                MessageBox.Show("指定的文件不存在。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 获取关联的 Entry 对象
                var entry = _entryManager.FindById(_entryId);

                if (entry == null)
                {
                    MessageBox.Show("无法找到对应的条目。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 2. 调用 MaterialService 添加素材（内部会移动文件并保存）
                _materialService.AddMaterial(entry, filePath);

                MessageBox.Show("文件添加成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // 3. 使用 MessageBox.Show 显示异常信息给用户
                MessageBox.Show($"无法将文件 {filePath} 添加为素材：{ex.Message}", "添加素材失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
