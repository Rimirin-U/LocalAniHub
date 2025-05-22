using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class InitializeViewModel : ObservableObject
    {
        public InitializeViewModel() { }

        [ObservableProperty]
        public string globalBaseFolder;
        [ObservableProperty]
        public string downloadPath;

        public bool Check()
        {
            // ...
            // 路径不能为空
            // debug
            return true;
        }

        public void Save()
        {
            // ...
            // 修改设置，并将isInitialized设为true
        }
    }
}
