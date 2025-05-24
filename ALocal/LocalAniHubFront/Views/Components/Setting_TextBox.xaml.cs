using LocalAniHubFront.Models;
using LocalAniHubFront.ViewModels.Components;
using System.Windows.Controls;

namespace LocalAniHubFront.Views.Components
{
    public partial class Setting_Textbox : UserControl
    {
        public Setting_Textbox()
        {
            // DataContext = new Setting_TextBoxViewModel(settingEntry);
            InitializeComponent();
        }

        public TextboxSettingEntry TextboxSettingEntry { get; set; }

        public static readonly DependencyProperty TextboxSettingEntryProperty =
            DependencyProperty.Register(
                nameof(TextboxSettingEntry),
                typeof(TextboxSettingEntry),
                typeof(Setting_Textbox),
                new PropertyMetadata(null, OnTextboxSettingEntryChanged));

        private static void OnTextboxSettingEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Setting_Textbox control && e.NewValue is TextboxSettingEntry newSettingEntry)
            {
                control.DataContext = new Setting_TextBoxViewModel(newSettingEntry);
                control.TextboxSettingEntry = newSettingEntry;
            }
        }

    }
}
