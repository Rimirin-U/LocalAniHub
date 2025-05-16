using System.Windows;
using System.Windows.Input;

namespace LocalAniHubFront.Helpers
{
    public static class DragHelper
    {
        public static bool GetIsDragEnabled(UIElement element) => (bool)element.GetValue(IsDragEnabledProperty);
        public static void SetIsDragEnabled(UIElement element, bool value) => element.SetValue(IsDragEnabledProperty, value);

        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.RegisterAttached("IsDragEnabled", typeof(bool), typeof(DragHelper),
                new UIPropertyMetadata(false, OnIsDragEnabledChanged));

        private static void OnIsDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                    element.PreviewMouseLeftButtonDown += Element_PreviewMouseLeftButtonDown;
                else
                    element.PreviewMouseLeftButtonDown -= Element_PreviewMouseLeftButtonDown;
            }
        }

        private static void Element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(sender as DependencyObject);
            if (window != null && e.LeftButton == MouseButtonState.Pressed)
                window.DragMove();
        }
    }

}
