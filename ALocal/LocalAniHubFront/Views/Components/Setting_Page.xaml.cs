﻿using LocalAniHubFront.Models;
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
    public partial class Setting_Page : UserControl
    {
        public Setting_Page(PageSettingEntry settingEntry)
        {
            DataContext = new Setting_PageViewModel(settingEntry);// 该ViewModel构造函数传参
            InitializeComponent();
        }
    }
}
