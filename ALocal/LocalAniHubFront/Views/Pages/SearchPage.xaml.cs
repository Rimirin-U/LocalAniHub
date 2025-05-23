﻿using BasicClassLibrary;
using LocalAniHubFront.ViewModels.Pages;
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
using Wpf.Ui.Abstractions.Controls;

namespace LocalAniHubFront.Views.Pages
{
    public partial class SearchPage : INavigableView<SearchViewModel>
    {
        public SearchPage(SearchViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public SearchViewModel ViewModel { get; }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            Window addEntryWindow = new Views.Windows.AddEntryWindow();
            addEntryWindow.Show();
        }
    }
}
