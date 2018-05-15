namespace HM4DesignTool.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using DataNameSpace;

    /// <summary>
    /// Window used to create a list of level names. 
    /// </summary>
    public partial class LevelListExport : Window
    {
        public LevelListExport()
        {
            this.InitializeComponent();

            //Populate LevelListFilter Dropdown
            this.levelListFilter.Items.Add("All");
            this.levelListFilter.SelectedIndex = 0;
            foreach (String category in Globals.roomCategories)
            {
                this.levelListFilter.Items.Add(category);
            }


            //Populate LevelList
            this.LoadLevelList();
        }

        private void LoadLevelList()
        {
            int roomIndex = this.levelListFilter.SelectedIndex;
            bool storyFilter = (bool)this.levelListStoryCheck.IsChecked;
            bool bonusFilter = (bool)this.levelListBonusCheck.IsChecked;
            bool unknownFilter = (bool)this.levelListUnknownCheck.IsChecked;


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

                        if ((bool)this.filterLevelEditorFormat.IsChecked)
                        {
                            Output = $"{Output}\"{levelName}\",";
                        }
                        else
                        {
                            Output = $"{Output}{levelName}";

                            if ((bool)levelListAddExtension.IsChecked)
                            {
                                Output = $"{Output}.lua";
                            }

                        }

                        Output = $"{Output}{Environment.NewLine}";

                    }

                }

            }

            this.levelListDisplay.Text = Output;
        }

        private void UpdateLevelList(object sender, SelectionChangedEventArgs e)
        {
            this.LoadLevelList();
        }

        private void UpdateLevelList(object sender, RoutedEventArgs e)
        {
            this.LoadLevelList();
        }

        private void copyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.levelListDisplay.Text);
            this.levelListDisplay.SelectAll();

        }
    }
}
