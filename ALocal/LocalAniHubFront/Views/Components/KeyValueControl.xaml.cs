using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalAniHubFront.Views.Components
{
    public partial class KeyValueControl : UserControl
    {
        public KeyValueControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        // 属性：Pair（键值对）
        public static readonly DependencyProperty PairProperty =
            DependencyProperty.Register(
                nameof(Pair),
                typeof(KeyValuePair<string, string>),
                typeof(KeyValueControl),
                new PropertyMetadata(default(KeyValuePair<string, string>))
            );
        public KeyValuePair<string, string> Pair
        {
            get => (KeyValuePair<string, string>)GetValue(PairProperty);
            set => SetValue(PairProperty, value);
        }

        // 属性：IsDragEnabled
        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.Register(
                nameof(IsDragEnabled),
                typeof(bool),
                typeof(KeyValueControl),
                new PropertyMetadata(true) // 默认启用拖动
            );
        public bool IsDragEnabled
        {
            get => (bool)GetValue(IsDragEnabledProperty);
            set => SetValue(IsDragEnabledProperty, value);
        }

        // 拖动
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragEnabled)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, new DataObject(typeof(KeyValuePair<string, string>), this.Pair), DragDropEffects.Copy);
            }
        }
    }
}
