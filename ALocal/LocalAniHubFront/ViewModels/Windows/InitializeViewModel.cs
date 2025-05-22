using BasicClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class InitializeViewModel : ObservableObject
    {
        public InitializeViewModel() 
        {
            // 初始化时尝试从全局设置加载已有值
            var settings = GlobalSettingsService.Instance;
            GlobalBaseFolder = settings.GetValue("globalBaseFolder");
            DownloadPath = settings.GetValue("downloadPath");
        }

        [ObservableProperty]
        public string globalBaseFolder;
        [ObservableProperty]
        public string downloadPath;

        public bool Check()
        {
            // ...
            // 路径不能为空
            // debug
            // 检查路径是否为空
            if (string.IsNullOrWhiteSpace(GlobalBaseFolder))
            {
                MessageBox.Show("全局父文件夹路径不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(DownloadPath))
            {
                MessageBox.Show("下载暂存路径不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // 检查路径是否存在或是否可以创建
            try
            {
                if (!Directory.Exists(GlobalBaseFolder))
                {
                    Directory.CreateDirectory(GlobalBaseFolder);
                }

                if (!Directory.Exists(DownloadPath))
                {
                    Directory.CreateDirectory(DownloadPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"路径无效或无法创建: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
           
        }

        public void Save()
        {
            // ...
            // 修改设置，并将isInitialized设为true
            var settings = GlobalSettingsService.Instance;

            // 保存路径设置
            settings.SetValue("globalBaseFolder", GlobalBaseFolder);
            settings.SetValue("downloadPath", DownloadPath);

            // 将程序标记为已初始化
            settings.SetValue("isInitialized", "true");

            //// 可以在这里创建必要的子目录结构
            //try
            //{
            //    // 示例：在全局父文件夹下创建标准子目录
            //    var resourceDir = Path.Combine(GlobalBaseFolder, "Resources");
            //    var notesDir = Path.Combine(GlobalBaseFolder, "Notes");

            //    Directory.CreateDirectory(resourceDir);
            //    Directory.CreateDirectory(notesDir);
            //}
            //catch (Exception ex)
            //{
            //    // 记录错误但不阻止初始化完成
            //    Console.WriteLine($"创建子目录时出错: {ex.Message}");
            //}
        }
    }
}
