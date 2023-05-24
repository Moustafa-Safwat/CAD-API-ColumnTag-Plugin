﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFCADAPI.ViewModel;

namespace WPFCADAPI.View
{
    /// <summary>
    /// Interaction logic for Mainview.xaml
    /// </summary>
    public partial class Mainview : Window
    {
        public Mainview mainViewObject { get; set; }
        public Mainview()
        {
            InitializeComponent();
            mainViewObject = this;
            this.DataContext = new Viewmodel(mainViewObject);
            
        }

  
    }
}
