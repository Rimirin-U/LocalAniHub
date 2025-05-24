using BasicClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using LocalAniHubFront.Helpers;



namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class EntryWindowViewModel : ObservableObject
    {
        private readonly Entry _entry;
        private bool _isInitialized = false;

        // 绑定到 XAML 的属性
        [ObservableProperty]
        private string _mainTitle = string.Empty;

        [ObservableProperty]
        private string _timeSubTitle = string.Empty;

        [ObservableProperty]
        private ImageSource _keyVisual;

        [ObservableProperty]
        private Color _averageColor;

        [ObservableProperty]
        private Color _transparentAverageColor;

        EntryManager entryManager=new EntryManager();
        public EntryWindowViewModel(int entryId)
        {
            _entry = entryManager.FindById(entryId)
        ?? throw new ArgumentException($"未找到ID为{entryId}的条目");
            InitializeViewModel(); // 直接在构造函数中初始化
        }

        // 初始化 ViewModel
        public Task InitializeAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();
            return Task.CompletedTask;
        }

        private void InitializeViewModel()
        {
            // 1. 设置主标题（根据全局设置选择原名或译名）
            var useOriginalName = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle") == "0";
            MainTitle = useOriginalName ? _entry.OriginalName : _entry.TranslatedName;

            // 2. 设置副标题（格式：2023 · 动画 · 在看）
            TimeSubTitle = $"{_entry.ReleaseDate:yyyy.M}";

            // 3. 加载背景图
            LoadKeyVisual();

            // 4. 计算背景图主色（用于渐变遮罩和纯色背景）
            CalculateAverageColor();

            _isInitialized = true;
        }

        // 加载背景图
        //BitmapImage - WPF中用于显示图像的核心类
        //UriKind.Absolute - 指定使用绝对路径（如果是相对路径应使用UriKind.Relative）
        private void LoadKeyVisual()
        {
            try
            {
                // 获取全局父文件夹
                string globalBaseFolder = GlobalSettingsService.Instance.GetValue("globalBaseFolder");
                // 拼接完整路径
                string imagePath = Path.Combine(
                    globalBaseFolder,
                    "Material",
                    _entry.MaterialFolder,
                    _entry.KeyVisualId
                );
                if (File.Exists(imagePath))
                {
                    // 如果存在，创建BitmapImage对象并赋给KeyVisual属性
                    // 使用绝对路径URI初始化图片
                    Console.WriteLine($"图片完整路径: {imagePath}");
                    KeyVisual = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                // 日志记录或使用默认图片
                Console.WriteLine($"加载主视觉图失败: {ex.Message}");
            }
        }

        // 计算图片主色
        private void CalculateAverageColor()
        {
            if (KeyVisual != null)
            {
                AverageColor = ImageHelper.CalculateAverageColor(KeyVisual);
                TransparentAverageColor = Color.FromArgb(0, AverageColor.R, AverageColor.G, AverageColor.B);
            }
            else
            {
                // 默认颜色（深灰色 + 透明）
                AverageColor = Colors.DarkSlateGray;
                TransparentAverageColor = Colors.Transparent;
            }
        }

        // 将观看状态枚举转为中文显示
        private string GetStateString(State state)
        {
            return state switch
            {
                State.NotWatched => "未看",
                State.Watching => "在看",
                State.Watched => "已看",
                State.GivenUp => "抛弃",
                _ => "未知"
            };
        }
    }
}