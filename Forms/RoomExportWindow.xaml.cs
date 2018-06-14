// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomExportWindow.xaml.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  Global references </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    using HM4DesignTool.Data;

    /// <summary>
    /// Window used to create a list of level names. 
    /// </summary>
    public partial class LevelListExport : INotifyPropertyChanged
    {



        #region Fields

        private bool enableAddLuaExtension;

        private bool enableLevelEditorFormat;

        private bool enablePrefix;

        private string prefixText = string.Empty;


        private bool enableSuffix;

        private string suffixText = string.Empty;


        private bool enableEndTab;
        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelListExport"/> class.
        /// </summary>
        public LevelListExport()
        {
            this.InitializeComponent();
            this.DataContext = this;

            // Populate LevelListFilter Dropdown
            this.levelListFilter.Items.Add("All");
            this.levelListFilter.SelectedIndex = 0;
            foreach (string category in Globals.RoomCategories)
            {
                this.levelListFilter.Items.Add(category);
            }

            // Populate LevelList
            this.LoadLevelList();
        }

        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        #region Properties

        #region Public



        public bool EnableLevelEditorFormat
        {
            get => this.enableLevelEditorFormat;
            set
            {
                this.enableLevelEditorFormat = value;
                this.OnPropertyChanged();
                this.LoadLevelList();

            }
        }


        public bool EnableAddLuaExtension
        {
            get => this.enableAddLuaExtension;
            set
            {
                this.enableAddLuaExtension = value;
                this.OnPropertyChanged();
                this.LoadLevelList();

            }
        }

        public string PrefixText
        {
            get => this.prefixText;
            set
            {
                this.prefixText = value;
                this.OnPropertyChanged();
                this.LoadLevelList();
            }
        }

        public bool EnablePrefix
        {
            get => this.enablePrefix;
            set
            {
                this.enablePrefix = value;
                this.OnPropertyChanged();
                this.LoadLevelList();

            }
        }

        public string SuffixText
        {
            get => this.suffixText;
            set
            {
                this.suffixText = value;
                this.OnPropertyChanged();
                this.LoadLevelList();
            }
        }

        public bool EnableSuffix
        {
            get => this.enableSuffix;
            set
            {
                this.enableSuffix = value;
                this.OnPropertyChanged();
                this.LoadLevelList();
            }
        }

        public bool EnableEndTab
        {
            get => this.enableEndTab;
            set
            {
                this.enableEndTab = value;
                this.OnPropertyChanged();
                this.LoadLevelList();
            }
        }
        #endregion

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// The load level list.
        /// </summary>
        private void LoadLevelList()
        {
            int roomIndex = this.levelListFilter.SelectedIndex;
            bool storyFilter = (bool)this.levelListStoryCheck.IsChecked;
            bool bonusFilter = (bool)this.levelListBonusCheck.IsChecked;
            bool unknownFilter = (bool)this.levelListUnknownCheck.IsChecked;

            Dictionary<string, List<string>> levelDictionary = Globals.GetLevelOverview.GetCategorizedFilteredLevels(roomIndex, storyFilter, bonusFilter, unknownFilter);

            string output = string.Empty;

            foreach (KeyValuePair<string, List<string>> category in levelDictionary)
            {
                if (category.Value.Count > 0)
                {

                    foreach (string levelName in category.Value)
                    {
                        string line = levelName;

                        if (this.enablePrefix)
                        {
                            line = $"{this.PrefixText}{line}";
                        }

                        if (this.enableSuffix)
                        {
                            line = $"{line}{this.SuffixText}";
                        }

                        if (this.EnableAddLuaExtension)
                        {
                            line = $"{line}.lua";
                        }

                        if (this.EnableLevelEditorFormat)
                        {
                            line = $"\"{line}\",";
                        }

                        if (this.EnableEndTab)
                        {
                            line = $"{line}\t";
                        }


                        output = $"{output}{line}\n";
                    }
                }
            }

            this.levelListDisplay.Text = output;
        }

        #endregion

        #region Private

        #region Signals

        /// <summary>
        /// Copy to clipboard button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CopyToClipboardButtonClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.levelListDisplay.Text);
            this.levelListDisplay.SelectAll();
        }

        /// <summary>
        /// The update level list.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UpdateLevelList(object sender, RoutedEventArgs e)
        {
            this.LoadLevelList();
        }

        #endregion

        #endregion

        #endregion

        private void levelListFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.LoadLevelList();
        }

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
