using DataNameSpace;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UiWindows
{
    /// <summary>
    /// Window used to create a list of level names. 
    /// </summary>
    public partial class LevelListExport : Window
    {
        public LevelListExport()
        {
            InitializeComponent();

            //Populate LevelListFilter Dropdown
            levelListFilter.Items.Add("All");
            levelListFilter.SelectedIndex = 0;
            foreach (String category in Globals.roomCategories)
            {
                levelListFilter.Items.Add(category);
            }


            //Populate LevelList
            LoadLevelList();
        }

        private void LoadLevelList()
        {
            int roomIndex = levelListFilter.SelectedIndex;
            bool storyFilter = (bool)levelListStoryCheck.IsChecked;
            bool bonusFilter = (bool)levelListBonusCheck.IsChecked;
            bool unknownFilter = (bool)levelListUnknownCheck.IsChecked;


            Dictionary<String, List<String>> levelDictionary = Globals.GetLevelOverview.GetCategorizedFilteredLevels(roomIndex, storyFilter, bonusFilter, unknownFilter);

            String Output = String.Empty;

            foreach (KeyValuePair<String, List<String>> category in levelDictionary)
            {
                TreeViewItem categoryItem = new TreeViewItem();
                categoryItem.Header = category.Key;
                if (category.Value.Count > 0)
                {
                    foreach (String levelName in category.Value)
                    {
                        Output += levelName;

                        if ((bool)levelListAddExtension.IsChecked)
                        {
                            Output += ".lua";
                        }

                        Output += Environment.NewLine;

                    }

                }

            }

            levelListDisplay.Text = Output;
        }

        private void UpdateLevelList(object sender, SelectionChangedEventArgs e)
        {
            LoadLevelList();
        }

        private void UpdateLevelList(object sender, RoutedEventArgs e)
        {
            LoadLevelList();
        }

        private void copyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(levelListDisplay.Text);
            levelListDisplay.SelectAll();

        }
    }
}
