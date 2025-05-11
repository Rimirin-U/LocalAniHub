using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace LocalAniHubFront.Views.Pages
{
    public partial class EntryWindow_MainInfo : Page, INotifyPropertyChanged
    {
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

        public EntryWindow_MainInfo()
        {
            //
            
            InitializeComponent();
        }

        public void InitializeViewModel()
        {
            // DataContext = new EntryWindow_MainInfoViewModel(EntryId);
        }

        // 属性更新通知
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
