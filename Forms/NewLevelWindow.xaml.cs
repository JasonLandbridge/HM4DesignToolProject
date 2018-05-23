namespace UiWindows
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using HM4DesignTool.Data;

    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class NewLevelWindow : INotifyPropertyChanged
    {

        private int RoomIndex
        {
            get
            {
                if (this.levelRoomValue != null)
                {
                    return (int)this.levelRoomValue.Value;
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
                if (this.levelBranchValue != null)
                {
                    return (int)this.levelBranchValue.Value;
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
                if (this.levelInstanceValue != null)
                {
                    return (int)this.levelInstanceValue.Value;
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
                if (this.levelInstanceRangeValue != null)
                {
                    return (int)this.levelInstanceRangeValue.Value;
                }
                else
                {
                    return 0;
                }

            }

        }
        private string LevelType
        {
            get
            {
                if (this.levelTypeValue != null)
                {
                    return this.levelTypeValue.Text;
                }
                else
                {
                    return string.Empty;
                }

            }

        }
        private bool UseRange
        {
            get
            {
                if (this.useRangeCheckbox != null)
                {
                    return (bool)this.useRangeCheckbox.IsChecked;
                }
                else
                {
                    return false;
                }
            }
        }

        public ObservableCollection<string> FileNameList
        {
            get
            {
                ObservableCollection<string> fileList = new ObservableCollection<string> { };
                if (this.UseRange)
                {
                    for (int i = this.LevelIndex; i <= this.LevelIndexMax; i++)
                    {
                        fileList.Add(this.GetFileName(i));
                    }
                }
                else
                {
                    fileList.Add(this.GetFileName(this.LevelIndex));
                }

                return fileList;
            }
        }
        public ObservableCollection<ListBoxItem> FileNameListBoxItems
        {
            get
            {
                ObservableCollection<ListBoxItem> fileItemList = new ObservableCollection<ListBoxItem> { };

                foreach (string fileName in this.FileNameList)
                {

                    ListBoxItem item = new ListBoxItem();
                    item.Content = fileName;

                    if (this.LevelExist(fileName))
                    {
                        item.Background = Brushes.IndianRed;
                    }
                    else
                    {
                        item.Background = Brushes.LightGreen;

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
            this.InitializeComponent();
            this.mainGrid.DataContext = this;
            this.FinishedLoading = true;
            this.SetRangeState();

            // levelListDisplay.ItemsSource = FileNameList;
        }

        private string GetFileName(int index)
        {
            // Get room index
            string filename = "r";
            filename += this.RoomIndex.ToString();
            filename += "_";

            // Get branch index
            if (this.BranchIndex < 10)
            {
                filename += "b0" + this.BranchIndex.ToString();
            }
            else
            {
                filename += "b" + this.BranchIndex.ToString();
            }

            // Get room type
            filename += "_";
            filename += this.LevelType;

            // Get instance N
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
            this.levelInstanceRangeLabel.IsEnabled = this.UseRange;
            this.levelInstanceRangeValue.IsEnabled = this.UseRange;
        }

        private void CreateLevelFiles()
        {
            foreach (string FileName in this.FileNameList)
            {
                string path = Globals.GetSettings.projectPathLevel + FileName;
                if (!this.LevelExist(FileName))
                {
                    File.Create(path).Dispose();
                }
            }
        }

        private bool LevelExist(string Filename)
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
            this.OnPropertyChanged("FileNameList");
            this.OnPropertyChanged("FileNameListBoxItems");
        }

        private void levelRoomValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.FinishedLoading)
            {
                this.UpdateLevelList();
            }
        }

        private void levelBranchValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.FinishedLoading)
            {
                this.UpdateLevelList();
            }
        }

        private void levelTypeValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FinishedLoading)
            {
                this.UpdateLevelList();
            }
        }

        private void levelInstanceValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.FinishedLoading)
            {
                this.UpdateLevelList();
            }
        }

        private void levelInstanceRangeValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.FinishedLoading)
            {
                this.UpdateLevelList();
            }
        }

        #region Events
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void useRangeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (this.FinishedLoading)
            {
                this.SetRangeState();
                this.UpdateLevelList();
            }

        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            this.CreateLevelFiles();
            this.UpdateLevelList();
            Globals.GetLevelOverview.UpdateLevelList();
        }
    }
}
