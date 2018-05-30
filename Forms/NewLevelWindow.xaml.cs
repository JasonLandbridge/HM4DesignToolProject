// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewLevelWindow.xaml.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  Global references </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Forms
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
    /// The new level window.
    /// </summary>
    public partial class NewLevelWindow : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The finished loading.
        /// </summary>
        private readonly bool finishedLoading;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewLevelWindow"/> class.
        /// </summary>
        public NewLevelWindow()
        {
            this.InitializeComponent();
            this.mainGrid.DataContext = this;
            this.finishedLoading = true;
            this.SetRangeState();

            // levelListDisplay.ItemsSource = FileNameList;
        }

        #endregion

        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        #region Properties

        #region Public

        /// <summary>
        /// Gets the file name list.
        /// </summary>
        public ObservableCollection<string> FileNameList
        {
            get
            {
                ObservableCollection<string> fileList = new ObservableCollection<string>();
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

        /// <summary>
        /// Gets the file name list box items used to display all the file names to be made.
        /// </summary>
        public ObservableCollection<ListBoxItem> FileNameListBoxItems
        {
            get
            {
                ObservableCollection<ListBoxItem> fileItemList = new ObservableCollection<ListBoxItem>();

                foreach (string fileName in this.FileNameList)
                {
                    fileItemList.Add(new ListBoxItem
                                           {
                                               Content = fileName,
                                               Background = LevelExist(fileName) ? Brushes.IndianRed : Brushes.LightGreen
                                           });
                }
                return fileItemList;
            }
        }

        #region Input

        /// <summary>
        /// Gets the branch index.
        /// </summary>
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

        /// <summary>
        /// Gets the level index.
        /// </summary>
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

        /// <summary>
        /// Gets the level index max.
        /// </summary>
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

        /// <summary>
        /// Gets the level type.
        /// </summary>
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

        /// <summary>
        /// Gets the room index.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether to use range.
        /// </summary>
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

        #endregion

        #endregion

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Checks if the level exist.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool LevelExist(string filename)
        {
            return filename.Length > 0 && File.Exists(Globals.GetSettings.projectPathLevel + filename);
        }

        /// <summary>
        /// The get file name.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFileName(int index)
        {
            // Get room index
            string filename = "r";
            filename += this.RoomIndex.ToString();
            filename += "_";

            // Get branch index
            if (this.BranchIndex < 10)
            {
                filename += "b0" + this.BranchIndex;
            }
            else
            {
                filename += "b" + this.BranchIndex;
            }

            // Get room type
            filename += "_";
            filename += this.LevelType;

            // Get instance N
            filename += "_";

            if (index < 10)
            {
                filename += "0" + index;
            }
            else
            {
                filename += index.ToString();
            }

            filename += ".lua";
            return filename;
        }

        /// <summary>
        /// Create the level files for every filename.
        /// </summary>
        private void CreateLevelFiles()
        {
            foreach (string fileName in this.FileNameList)
            {
                string path = Globals.GetSettings.projectPathLevel + fileName;
                if (!LevelExist(fileName))
                {
                    File.Create(path).Dispose();
                }
            }
        }

        /// <summary>
        /// Update the level list display.
        /// </summary>
        private void UpdateLevelList()
        {
            this.OnPropertyChanged("FileNameList");
            this.OnPropertyChanged("FileNameListBoxItems");
        }

        /// <summary>
        /// Enable or disable the range state.
        /// </summary>
        private void SetRangeState()
        {
            this.levelInstanceRangeLabel.IsEnabled = this.UseRange;
            this.levelInstanceRangeValue.IsEnabled = this.UseRange;
        }

        #region Signals

        /// <summary>
        /// Update whenever a value is changed in a integer up down box.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UpdateOnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.finishedLoading)
            {
                this.UpdateLevelList();
            }
        }

        /// <summary>
        /// Update whenever a selection is changed in a combo box.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UpdateOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.finishedLoading)
            {
                this.UpdateLevelList();
            }
        }

        /// <summary>
        /// Buttons signal to save all displayed filenames. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            this.CreateLevelFiles();
            this.UpdateLevelList();
            Globals.GetLevelOverview.UpdateLevelList();
        }

        /// <summary>
        /// Checkbox signal to enable/disable the range. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UseRangeCheckboxClick(object sender, RoutedEventArgs e)
        {
            if (this.finishedLoading)
            {
                this.SetRangeState();
                this.UpdateLevelList();
            }
        }

        #endregion

        #endregion

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        /// <param name="propertyName">
        /// The property Name.
        /// </param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}
