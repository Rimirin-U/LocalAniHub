﻿using LocalAniHubFront.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LocalAniHubFront.ViewModels.Components
{

   public partial class Setting_PageViewModel : ObservableObject
   {
        private readonly PageSettingEntry _entry;

        // 设置项的名称，用于绑定到 TextBlock 的 Text 属性
        public string EntryName => _entry.EntryName;

        // 设置项的描述，用于绑定到 TextBlock 的 Text 属性
        public string Description
        {
            get => _entry.Description;
            set
            {
                if (_entry.Description != value)
                {
                    _entry.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        // 打开设置页面的命令
        [RelayCommand]
        private void ShowSettingPage()
        {
            _entry.OpenSettingPage();
        }

        // 构造函数，接收一个 PageSettingEntry 实例
        public Setting_PageViewModel(PageSettingEntry entry)
        {
            _entry = entry;
        }

        /* // 打开设置页面的命令
         public IRelayCommand ShowSettingPageCommand { get; }

         public event PropertyChangedEventHandler? PropertyChanged;
         // 构造函数，初始化 ViewModel
         public Setting_PageViewModel(PageSettingEntry entry)
         {
             _entry = entry;
             // 初始化命令，调用 PageSettingEntry 的 OpenSettingPage 方法
             ShowSettingPageCommand = new RelayCommand(_entry.OpenSettingPage);
         }
         // 通知属性更改
         protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
         {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
         }*/
    }
}
