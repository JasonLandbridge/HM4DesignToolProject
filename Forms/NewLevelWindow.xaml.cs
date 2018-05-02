using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace UiWindows
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class NewLevelWindow :  INotifyPropertyChanged
    {

        private int RoomIndex
        {
            get
            {
                if (levelRoomValue != null)
                {
                    return (int)levelRoomValue.Value;
                }
                else
                {
                    return 0;
                }

            }
        }
        private int BranchIndex
        {
            get
            {
                if (levelBranchValue != null)
                {
                    return (int)levelBranchValue.Value;
                }
                else
                {
                    return 0;
                }

            }
        }
        private int LevelIndex
        {
            get
            {
                if (levelInstanceValue != null)
                {
                    return (int)levelInstanceValue.Value;
                }
                else
                {
                    return 0;
                }

            }
        }
        private int LevelIndexMax
        {
            get
            {
                if (levelInstanceRangeValue != null)
                {
                    return (int)levelInstanceRangeValue.Value;
                }
                else
                {
                    return 0;
                }

            }

        }
        private String LevelType
        {
            get
            {
                if (levelTypeValue != null)
                {
                    return levelTypeValue.Text;
                }
                else
                {
                    return String.Empty;
                }

            }

        }
        private bool UseRange
        {
            get
            {
                if (useRangeCheckbox != null)
                {
                    return (bool)useRangeCheckbox.IsChecked;
                }
                else
                {
                    return false;
                }
            }
        }

        public ObservableCollection<String> FileNameList
        {
            get
            {
                ObservableCollection<String> fileList = new ObservableCollection<String> { };
                if (UseRange)
                {
                    for (int i = LevelIndex; i <= LevelIndexMax; i++)
                    {
                        fileList.Add(GetFileName(i));
                    }
                }
                else
                {
                    fileList.Add(GetFileName(LevelIndex));
                }
                return fileList;
            }
        }

        private bool FinishedLoading = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public NewLevelWindow()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            FinishedLoading = true;
           // levelListDisplay.ItemsSource = FileNameList;
        }

        private void GenerateLevelList()
        {

            String levelList = String.Empty;

        }

        private String GetFileName(int index)
        {
            //Get room index
            String filename = "r";
            filename += RoomIndex.ToString();
            filename += "_";
            //Get branch index
            if (BranchIndex < 10)
            {
                filename += "b0" + BranchIndex.ToString();
            }
            else
            {
                filename += "b" + BranchIndex.ToString();

            }
            //Get room type
            filename += "_";
            filename += LevelType;

            //Get instance N
            filename += "_";

            if (index < 10)
            {
                filename += "0" + index.ToString();
            }
            else
            {
                filename += index.ToString();

            }

            filename += ".lua";
            return filename;

        }

        private void UpdateLevelList()
        {
            OnPropertyChanged("FileNameList");
        }

        private void levelRoomValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FinishedLoading)
            {
                UpdateLevelList();
            }
        }

        private void levelBranchValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FinishedLoading)
            {
                UpdateLevelList();
            }
        }

        private void levelTypeValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FinishedLoading)
            {
                UpdateLevelList();
            }
        }

        private void levelInstanceValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FinishedLoading)
            {
                UpdateLevelList();
            }
        }

        private void levelInstanceRangeValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FinishedLoading)
            {
                UpdateLevelList();
            }
        }

        #region Events
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void useRangeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (FinishedLoading)
            {
                UpdateLevelList();
            }

        }
    }
}
