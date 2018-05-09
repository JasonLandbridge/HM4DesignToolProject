﻿using System;
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
using System.Windows.Shapes;
using DataNameSpace;

namespace UiWindows
{
    /// <summary>
    /// Interaction logic for CustomTreatmentWeightsWindow.xaml
    /// </summary>
    public partial class CustomTreatmentWeightsWindow : INotifyPropertyChanged
    {
    
        public CustomTreatmentWeightsWindow()
        {
            InitializeComponent();
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members





    }
}
