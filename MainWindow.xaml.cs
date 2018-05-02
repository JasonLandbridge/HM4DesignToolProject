﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DataNameSpace;
using UiWindows;
using LevelData;
using System.Text;
using System.Windows.Markup;

namespace HM4DesignTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DataTemplate PatientRowDataTemplate;

        public SettingsWindow SettingsWindow;
        public String StatusbarText
        {
            get
            {
                OnPropertyChanged();
                return Globals.GetData.StatusbarText;
            }
        }
        public List<String> GetLevelTypes
        {
            get
            {
                return Globals.GetLevelTypes;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.levelDataLayout.DataContext = Globals.GetLevelOverview;
            PatientRowDataTemplate = FindResource("PatientRowControlTemplate") as DataTemplate;
            patientOverviewLayout.ItemTemplate = PatientRowDataTemplate;
            levelTypeDropDown.ItemsSource = GetLevelTypes;


            SetupWindow();

        }

        private void SetupWindow()
        {
            //Populate LevelListFilter Dropdown
            levelListFilter.Items.Add("All");
            levelListFilter.SelectedIndex = 0;
            foreach (String category in Globals.roomCategories)
            {
                levelListFilter.Items.Add(category);
            }

            BeforeLoadWindowSettings();

            //Populate LevelList
            LoadLevelList();

            AfterLoadWindowSettings();
        }

        private void LoadLevelList()
        {
            int roomIndex = levelListFilter.SelectedIndex;
            bool firstCategoryOpen = true; 
            bool storyFilter = (bool)levelListStoryCheck.IsChecked;
            bool bonusFilter = (bool)levelListBonusCheck.IsChecked;
            bool unknownFilter = (bool)levelListUnknownCheck.IsChecked;


            Dictionary<String, List<String>> levelDictionary = Globals.GetLevelOverview.GetCategorizedFilteredLevels(roomIndex, storyFilter, bonusFilter, unknownFilter);
            levelListDisplay.Items.Clear();
            foreach (KeyValuePair<String, List<String>> category in levelDictionary)
            {
                TreeViewItem categoryItem = new TreeViewItem();
                categoryItem.Header = category.Key;
                if (category.Value.Count > 0)
                {
                    if (firstCategoryOpen)
                    {
                        categoryItem.ExpandSubtree();
                        firstCategoryOpen = false;
                    }
                    foreach (String levelName in category.Value)
                    {
                        TreeViewItem levelItem = new TreeViewItem();
                        levelItem.Header = levelName;
                        levelItem.Selected += levelListItem_Selected;
                        categoryItem.Items.Add(levelItem);
                    }

                    levelListDisplay.Items.Add(categoryItem);
                }

            }
        }

        private void BeforeLoadWindowSettings()
        {
            //Set saved Room Filter settings
            levelListFilter.SelectedIndex = Globals.GetSettings.RoomFilterDropdownIndex;
            levelListStoryCheck.IsChecked = Globals.GetSettings.RoomFilterStoryCheck;
            levelListBonusCheck.IsChecked = Globals.GetSettings.RoomFilterBonusCheck;
            levelListUnknownCheck.IsChecked = Globals.GetSettings.RoomFilterUnknownCheck;
        }


        private void AfterLoadWindowSettings()
        {
            if (levelListDisplay.Items.Count > 0)
            {
                TreeViewItem categoryItem = levelListDisplay.Items[0] as TreeViewItem;
                categoryItem.IsExpanded = Globals.GetSettings.LevelListFirstCategoryOpen;
            }

        }



        private void StoreWindowSettings()
        {
            Globals.GetSettings.RoomFilterDropdownIndex = levelListFilter.SelectedIndex;
            Globals.GetSettings.RoomFilterStoryCheck = (bool)levelListStoryCheck.IsChecked;
            Globals.GetSettings.RoomFilterBonusCheck = (bool)levelListBonusCheck.IsChecked;
            Globals.GetSettings.RoomFilterUnknownCheck = (bool)levelListUnknownCheck.IsChecked;

            if (levelListDisplay.Items.Count > 0)
            {
                TreeViewItem categoryItem = levelListDisplay.Items[0] as TreeViewItem;
                Globals.GetSettings.LevelListFirstCategoryOpen = categoryItem.IsExpanded;
            }
            else
            {
                Globals.GetSettings.LevelListFirstCategoryOpen = true;
            }
            
            Globals.GetSettings.SaveSettings();
        }

        #region Signals
        #region Menu

        private void menuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow dialog = new SettingsWindow();
            Nullable<bool> dialogResult = dialog.ShowDialog();
        }

        private void menuItemNewLevel_Click(object sender, RoutedEventArgs e)
        {
            NewLevelWindow dialog = new NewLevelWindow();
            Nullable<bool> dialogResult = dialog.ShowDialog();

        }

        #endregion

        #region LevelList

        private void levelListItem_Selected(object sender, RoutedEventArgs e)
        {
            //https://stackoverflow.com/questions/24880824/how-to-add-wpf-treeview-node-click-event-to-get-the-node-value
            TreeViewItem item = sender as TreeViewItem;
            Globals.GetLevelOverview.LoadLevel(item.Header.ToString());

        }
        private void levelListFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadLevelList();
        }

        private void levelListStoryCheck_Click(object sender, RoutedEventArgs e)
        {
            LoadLevelList();

        }

        private void levelListBonusCheck_Click(object sender, RoutedEventArgs e)
        {
            LoadLevelList();
        }

        private void levelListUnknownCheck_Click(object sender, RoutedEventArgs e)
        {
            LoadLevelList();
        }

        #endregion


        #region PatientOverview

        private void buttonPatientOverviewAddRow_Click(object sender, RoutedEventArgs e)
        {
            Globals.GetLevelOverview.AddPatientToLoadedLevel();
            patientOverviewLayout.ItemsSource = Globals.GetLevelOverview.GetLevelLoaded.PatientCollection;
            //ContentControl patientRow = new ContentControl();
            //patientRow.ContentTemplate = PatientRowDataTemplate;

        }

        #endregion



        #endregion

        #region Events
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            StoreWindowSettings();

        }


    }
}