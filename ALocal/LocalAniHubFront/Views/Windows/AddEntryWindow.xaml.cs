using LocalAniHubFront.ViewModels.Windows;
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
using System.Windows.Shapes;

namespace LocalAniHubFront.Views.Windows
{
    public partial class AddEntryWindow
    {
        public AddEntryWindowViewModel ViewModel { get; set; }

        public AddEntryWindow(/*EntryInfoSet entryInfoSet*/)
        {
            ViewModel = new AddEntryWindowViewModel(/*entryInfoSet*/);
            DataContext = this;
            InitializeComponent();
        }

        private void Button_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                var pair = (KeyValuePair<string, string>)e.Data.GetData(typeof(KeyValuePair<string, string>));
                if (DataContext is AddEntryWindowViewModel vm)
                {
                    vm.AddPair(pair);
                }
            }
        }

        private void TextBox_DropKey(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                var pair = (KeyValuePair<string, string>)e.Data.GetData(typeof(KeyValuePair<string, string>));
                var tb = sender as TextBox;
                tb.Text = pair.Key;
            }
        }

        private void TextBox_DropValue(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                var pair = (KeyValuePair<string, string>)e.Data.GetData(typeof(KeyValuePair<string, string>));
                var tb = sender as TextBox;
                tb.Text = pair.Value;
            }
        }

    }
}
