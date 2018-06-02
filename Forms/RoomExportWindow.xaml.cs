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
    using System.Windows;

    using HM4DesignTool.Data;

    /// <summary>
    /// Window used to create a list of level names. 
    /// </summary>
    public partial class LevelListExport
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelListExport"/> class.
        /// </summary>
        public LevelListExport()
        {
            this.InitializeComponent();

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
                        if ((bool)this.filterLevelEditorFormat.IsChecked)
                        {
                            output = $"{output}\"{levelName}\",";
                        }
                        else
                        {
                            output = $"{output}{levelName}";

                            if ((bool)this.levelListAddExtension.IsChecked)
                            {
                                output = $"{output}.lua";
                            }
                        }

                        output = $"{output}{Environment.NewLine}";
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

        private void levelListFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.LoadLevelList();
        }
    }
}
