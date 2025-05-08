using LocalAniHubFront.ViewModels.Components;
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
    public partial class Recent_SeasonTable : UserControl
    {
        public Recent_SeasonTable()
        {
            DataContext = new Recent_SeasonTableViewModel();
            InitializeComponent(); Loaded += Recent_SeasonTable_Loaded; // 在控件加载时绑定事件
        }


        private void Recent_SeasonTable_Loaded(object sender, RoutedEventArgs e)
        {
            // 获取绑定的视图模型
            var viewModel = DataContext as Recent_SeasonTableViewModel;
            if (viewModel == null) return;

            // 清空现有列
            UnifiedEntriesDataGrid.Columns.Clear();

            // 根据用户设置动态生成列
            foreach (var property in viewModel.VisibleProperties)
            {
                var column = new DataGridTextColumn
                {
                    Header = property.DisplayName, // 列标题
                    Binding = new Binding($"[{property.Name}]") // 绑定到动态属性
                };
                UnifiedEntriesDataGrid.Columns.Add(column); // 添加列到 DataGrid
            }
        }
    }
}
