using BasicClassLibrary;
using LocalAniHubFront.ViewModels.Windows;
using LocalAniHubFront.Views.Components;
using Microsoft.Win32;
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


        public AddEntryWindow(EntryInfoSet entryInfoSet)
        {
            ViewModel = new AddEntryWindowViewModel(entryInfoSet);
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
                ViewModel.AddPair(pair);
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
        private void EditableTextBlock_DropKey(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                var pair = (KeyValuePair<string, string>)e.Data.GetData(typeof(KeyValuePair<string, string>));
                var tb = sender as EditableTextBlock;
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
        private void EditableTextBlock_DropValue(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                var pair = (KeyValuePair<string, string>)e.Data.GetData(typeof(KeyValuePair<string, string>));
                var tb = sender as EditableTextBlock;
                tb.Text = pair.Value;
            }
        }

        private void TextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }
        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KeyValuePair<string, string>)))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void KeyVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|所有文件|*.*"
            };

            // 显示文件选择对话框
            bool? result = openFileDialog.ShowDialog();

            // 选择文件后
            if (result == true)
            {
                string filePath = openFileDialog.FileName;

                // 这里你可以处理文件路径，比如更新ViewModel的属性
                ViewModel.ChangeKeyVisual(filePath);
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 检查数据合法性
            if (!ViewModel.CheckDataValidity())
            {
                MessageBox.Show("部分数据不合法，请检查后再试。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 数据合法，调用保存函数
            ViewModel.Save();

            // 关闭窗口
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
