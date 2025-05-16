using LocalAniHubFront.Helpers;
using LocalAniHubFront.ViewModels.Windows;
using LocalAniHubFront.Views.Pages;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace LocalAniHubFront.Views.Windows
{
    public partial class EntryWindow : INotifyPropertyChanged
    {
        // 事件
        public event Action MaskFullyRaised;
        public event Action MaskLowerFromFullyRaised;

        // 比例
        private double _imageGridheightRatio = 1;
        public double ImageGridHeightRatio
        {
            get => _imageGridheightRatio;
            set
            {
                if (_imageGridheightRatio != value)
                {
                    _imageGridheightRatio = value;
                    OnPropertyChanged();
                    RecalculateHeight();
                }
            }
        }
        // Full(Window)->Full(Window)->200
        private double _topGridheightRatio = 1;
        public double TopGridHeightRatio
        {
            get => _topGridheightRatio;
            set
            {
                if (_topGridheightRatio != value)
                {
                    _topGridheightRatio = value;
                    OnPropertyChanged();
                    RecalculateHeight();
                }
            }
        }
        // Full(Window)->Full(Window)->100
        private double _maskRectangleheightRatio = 100.0 / 450.0;
        public double MaskRectangleHeightRatio
        {
            get => _maskRectangleheightRatio;
            set
            {
                if (Math.Abs(_maskRectangleheightRatio - value) > 0.0001)
                {
                    _maskRectangleheightRatio = value;
                    OnPropertyChanged();
                    RecalculateHeight();
                }
            }
        }
        // 100->Full(Gird:Top)->Full(Gird:Top)

        // 控件高度绑定
        private double _maskRectangleHeight;
        public double MaskRectangleHeight
        {
            get => _maskRectangleHeight;
            set
            {
                if (Math.Abs(_maskRectangleHeight - value) > 0.5) // 避免频繁更新
                {
                    _maskRectangleHeight = value;
                    OnPropertyChanged();
                    //RecalculateHeight();
                }
            }
        }

        private double _topGridHeight;
        public double TopGridHeight
        {
            get => _topGridHeight;
            set
            {
                if (Math.Abs(_topGridHeight - value) > 0.5) // 避免频繁更新
                {
                    _topGridHeight = value;
                    OnPropertyChanged();
                    //RecalculateHeight();
                }
            }
        }

        private double _imageGridHeight;
        public double ImageGridHeight
        {
            get => _imageGridHeight;
            set
            {
                if (Math.Abs(_imageGridHeight - value) > 0.5) // 避免频繁更新
                {
                    _imageGridHeight = value;
                    OnPropertyChanged();
                    //RecalculateHeight();
                }
            }
        }

        // MaskRectangleHeightRatio控制
        private readonly SmoothValueController _maskHeightController;
        private readonly double _initialMaskHeightRatio;
        private readonly double _stepIncrement = 0.10;
        private bool _hasReachedMax = false;

        // TopGridHeightRatio控制
        private readonly SmoothValueController _topGridHeightController;
        private readonly double _finalTopGridHeightRatio;

        // TopGridHeightRatio控制
        private readonly SmoothValueController _imageGridHeightController;
        private readonly double _finalImageGridHeightRatio;


        // 构造函数
        public EntryWindow(int entryId)
        {
            // 添加ViewModel
            DataContext = new EntryWindowViewModel(entryId);
            InitializeComponent();

            var Page_MainInfo = new EntryWindow_MainInfo();
            Page_MainInfo.EntryId = entryId;
            Page_MainInfo.InitializeViewModel();
            EntryWindowFrame.Navigate(Page_MainInfo);

            // MaskRectangleHeightRatio平滑控制器、回弹逻辑、到顶逻辑
            _initialMaskHeightRatio = _maskRectangleheightRatio;
            _maskHeightController = new SmoothValueController(MaskRectangleHeightRatio);
            _maskHeightController.ValueChanged += value =>
            {
                MaskRectangleHeightRatio = value;
                // 到顶逻辑
                if (!_hasReachedMax && Math.Abs(value - 1.0) < 0.01)
                {
                    _hasReachedMax = true;
                    MaskFullyRaised?.Invoke();
                }
                // 回弹逻辑
                if (!_hasReachedMax &&
                    Math.Abs(_maskHeightController.Target - value) < 0.01 &&
                    Math.Abs(value - _initialMaskHeightRatio) > 0.01)
                {
                    _maskHeightController.SetTarget(_initialMaskHeightRatio);
                }
            };

            // TopGridHeightRatio平滑控制器
            _finalTopGridHeightRatio = 100.0 / 450.0;
            _topGridHeightController = new SmoothValueController(TopGridHeightRatio);
            _topGridHeightController.ValueChanged += value =>
            {
                TopGridHeightRatio = value;
            };

            // ImageGridHeightRatio平滑控制器
            _finalImageGridHeightRatio = 200.0 / 450.0;
            _imageGridHeightController = new SmoothValueController(ImageGridHeightRatio);
            _imageGridHeightController.ValueChanged += value =>
            {
                ImageGridHeightRatio = value;
            };


            // 只要Size(Grid)发生改变 自动触发RecalculateMaskHeight
            SizeChanged += (s, e) => RecalculateHeight();
            TopGrid.SizeChanged += (s, e) => RecalculateHeight();
            MaskRectangle.SizeChanged += (s, e) => RecalculateHeight();
            Loaded += (s, e) => RecalculateHeight();
            MaskFullyRaised += () => _topGridHeightController.SetTarget(_finalTopGridHeightRatio);
            MaskLowerFromFullyRaised += () => _topGridHeightController.SetTarget(1.0);
            MaskFullyRaised += () => _imageGridHeightController.SetTarget(_finalImageGridHeightRatio);
            MaskLowerFromFullyRaised += () => _imageGridHeightController.SetTarget(1.0);

            // Debug控制台输出
            // RectDebug();
        }

        // 实时重新计算
        private void RecalculateHeight()
        {
            if (MainGrid != null)
                MaskRectangleHeight = MainGrid.RowDefinitions[0].ActualHeight * MaskRectangleHeightRatio;

            if (EntryWindowInstance != null)
            {
                TopGridHeight = EntryWindowInstance.ActualHeight * TopGridHeightRatio;
                ImageGridHeight = EntryWindowInstance.ActualHeight * ImageGridHeightRatio;
            }
        }


        // 属性更新通知
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // 滚轮滚动事件
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0) // 向下滚动
            {
                if (_hasReachedMax) return;// 已到顶后不再接受向下滚动

                double newTarget = Math.Min(1.0, _maskHeightController.Target + _stepIncrement);
                _maskHeightController.SetTarget(newTarget);
            }
            else if (e.Delta > 0) // 向上滚动
            {
                if (_hasReachedMax) MaskLowerFromFullyRaised?.Invoke();// 触发下降事件
                _hasReachedMax = false;
                _maskHeightController.SetTarget(_initialMaskHeightRatio);
            }
        }

        // Debug控制台输出
        private async Task RectDebug()
        {
            await Task.Delay(2000);
            await Task.Run(async () =>
            {
                while (true)
                {
                    Debug.WriteLine(
                        $"MaskRectangleHeightRatio {MaskRectangleHeightRatio}\n" +
                        $"MaskRectangleHeight {MaskRectangleHeight}\n" +
                        $"MaskRectangle.ActualHeight {MaskRectangle.ActualHeight}\n" +
                        $"MaskRectangleHeight = TopGrid.ActualHeight * MaskRectangleHeightRatio\n" +
                        $"EntryWindow.ActualHeight {EntryWindowInstance.ActualHeight}\n" +
                        $"TopGridHeight {TopGridHeight}\n" +
                        $"TopGridHeightRatio {TopGridHeightRatio}\n" +
                        $"TopGrid.ActualHeight {TopGrid.ActualHeight}\n" +
                        $"TopGridHeight = EntryWindow.ActualHeight * TopGridHeightRatio;\n" +
                        $"{MainGrid.RowDefinitions[0].ActualHeight}\n");
                    await Task.Delay(500);
                }
            });
        }
    }


}
