using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.IO;
using DataNameSpace;
using System.Windows.Media;

namespace UiWindows
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class NewLevelWindow : INotifyPropertyChanged
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
        public ObservableCollection<ListBoxItem> FileNameListBoxItems
        {
            get
            {
                ObservableCollection<ListBoxItem> fileItemList = new ObservableCollection<ListBoxItem> { };

                foreach (String fileName in FileNameList)
                {

                    ListBoxItem item = new ListBoxItem();
                    item.Content = fileName;

                    if (LevelExist(fileName))
                    {
                        item.Background = Brushes.Red;
                    }
                    else
                    {
                        item.Background = Brushes.Green;

                    }
                    fileItemList.Add(item);

                }
                return fileItemList;
            }
        }
        private bool FinishedLoading = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public NewLevelWindow()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            FinishedLoading = true;
            SetRangeState();
            //levelListDisplay.ItemsSource = FileNameList;
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

        private void SetRangeState()
        {
            levelInstanceRangeLabel.IsEnabled = UseRange;
            levelInstanceRangeValue.IsEnabled = UseRange;
        }

        private void CreateLevelFiles()
        {
            foreach (String FileName in FileNameList)
            {
                String path = Globals.GetSettings.projectPathLevel + FileName;
                if (!LevelExist(FileName))
                {
                    File.Create(path).Dispose();
                }
            }
        }
        private bool LevelExist(String Filename)
        {
            if (Filename.Length > 0)
            {
                return File.Exists(Globals.GetSettings.projectPathLevel + Filename);
            }
            else
            {
                return false;
            }
        }

        private void UpdateLevelList()
        {
            OnPropertyChanged("FileNameList");
            OnPropertyChanged("FileNameListBoxItems");
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
                SetRangeState();
                UpdateLevelList();
            }

        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            CreateLevelFiles();
            UpdateLevelList();
        }
    }
}
