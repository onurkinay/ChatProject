﻿using System;
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

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for DownloadScreen.xaml
    /// </summary>
    public partial class DownloadScreen : Window
    {
        public DownloadScreen(string max)
        {
            InitializeComponent();
            pbProgress.Maximum = Convert.ToInt32(max);
            pbProgress.Minimum = 0;
        }
    }
}
