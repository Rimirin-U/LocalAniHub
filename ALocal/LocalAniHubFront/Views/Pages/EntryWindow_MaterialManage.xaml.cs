using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LocalAniHubFront.Helpers;
using LocalAniHubFront.ViewModels.Pages;
using Microsoft.Win32;

namespace LocalAniHubFront.Views.Pages
{
    public partial class EntryWindow_MaterialManage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<FileItem> _files = new();
        public ObservableCollection<FileItem> Files
        {
            get => _files;
            set
            {
                if (_files != value)
                {
                    _files = value;
                    OnPropertyChanged(nameof(Files));
                }
            }
        }


        private FileItem _selectedFile;
        public FileItem SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        private int _entryId;
        public int EntryId
        {
            get => _entryId;
            set
            {
                if (_entryId != value)
                {
                    _entryId = value;
                    OnPropertyChanged();
                }
            }
        }

        public EntryWindow_MaterialManage()
        {
            InitializeComponent();
        }

        public void InitializeViewModel()
        {
            DataContext = new EntryWindow_MaterialManageViewModel(EntryId);
            InitializeFolder();
        }

        private void InitializeFolder()
        {
            var viewModel = DataContext as EntryWindow_MaterialManageViewModel;
            string folderPath = viewModel.MaterialFolderPath;// <---------------- !!
            Files = new();
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath);
                foreach (var file in files)
                {
                    var item = new FileItem
                    {
                        Name = Path.GetFileName(file),
                        FullPath = file,
                        Thumbnail = ThumbnailHelper.GetThumbnail(file, 128),
                        Size = new FileInfo(file).Length,
                        IsImage = IsImageFile(file)
                    };
                    Files.Add(item);
                }
            }
        }

        private bool IsImageFile(string file)
        {
            var extensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
            return extensions.Contains(Path.GetExtension(file).ToLower());
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFile != null && File.Exists(SelectedFile.FullPath))
            {
                Process.Start(new ProcessStartInfo(SelectedFile.FullPath) { UseShellExecute = true });
            }
        }

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFile == null || !File.Exists(SelectedFile.FullPath))
                return;

            var result = MessageBox.Show($"确定要删除文件：\n{SelectedFile.Name}？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    File.Delete(SelectedFile.FullPath);
                    Files.Remove(SelectedFile);
                    SelectedFile = null;
                }
                catch
                {
                    MessageBox.Show("删除失败。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EntryWindow_MaterialManageViewModel;
            if (viewModel == null) return;

            var openFileDialog = new OpenFileDialog
            {
                Title = "选择要添加的文件",
                Filter = "所有文件|*.*",
                InitialDirectory = viewModel.MaterialFolderPath
            };

            // 显示文件选择对话框
            bool? result = openFileDialog.ShowDialog();

            // 选择文件后
            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                viewModel.AddMaterial(filePath);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeFolder();
        }
    }
}
