using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LocalAniHubFront.Helpers
{
    public static class GridHelper
    {
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(GridHelper),
                new PropertyMetadata(0, OnRowCountChanged));

        public static int GetRowCount(DependencyObject obj) => (int)obj.GetValue(RowCountProperty);
        public static void SetRowCount(DependencyObject obj, int value) => obj.SetValue(RowCountProperty, value);

        private static void OnRowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid)
            {
                int count = (int)e.NewValue;
                grid.RowDefinitions.Clear();
                for (int i = 0; i < count; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }
            }
        }
    }

}
