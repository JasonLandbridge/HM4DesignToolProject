// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  The main window for the Design Tool </summary>
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

    using UiWindows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        public SettingsWindow SettingsWindow;
        public String StatusbarText
        {
            get
            {
                //TODO Create Statusbar functionality
                this.OnPropertyChanged();
                return string.Empty;
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
            this.InitializeComponent();
            //Set Global reference to this Window
            Globals.GetMainWindow = this;

            this.DataContext = this;
            this.levelDataLayout.DataContext = Globals.GetLevelOverview;
            this.mainLayout.DataContext = Globals.GetLevelOverview;
            this.levelControls.DataContext = Globals.GetLevelOverview;
            this.levelTypeDropDown.ItemsSource = this.GetLevelTypes;
            this.OnPropertyChanged("PatientRowDataTemplate");

            this.SetupWindow();


        }

        private void SetupWindow()
        {


            // BeforeLoadWindowSettings();


            // AfterLoadWindowSettings();
        }





        private void BeforeLoadWindowSettings()
        {
            //Set saved Room Filter settings
            this.levelListFilter.SelectedIndex = Globals.GetSettings.RoomFilterDropdownIndex;
            this.levelListStoryCheck.IsChecked = Globals.GetSettings.RoomFilterStoryCheck;
            this.levelListBonusCheck.IsChecked = Globals.GetSettings.RoomFilterBonusCheck;
            this.levelListUnknownCheck.IsChecked = Globals.GetSettings.RoomFilterUnknownCheck;
        }


        private void AfterLoadWindowSettings()
        {
            if (this.levelListDisplay.Items.Count > 0)
            {
                TreeViewItem categoryItem = this.levelListDisplay.Items[0] as TreeViewItem;
                categoryItem.IsExpanded = Globals.GetSettings.LevelListFirstCategoryOpen;
            }

        }



        private void StoreWindowSettings()
        {
            Globals.GetSettings.RoomFilterDropdownIndex = this.levelListFilter.SelectedIndex;
            Globals.GetSettings.RoomFilterStoryCheck = (bool)this.levelListStoryCheck.IsChecked;
            Globals.GetSettings.RoomFilterBonusCheck = (bool)this.levelListBonusCheck.IsChecked;
            Globals.GetSettings.RoomFilterUnknownCheck = (bool)this.levelListUnknownCheck.IsChecked;

            if (this.levelListDisplay.Items.Count > 0)
            {
                TreeViewItem categoryItem = this.levelListDisplay.Items[0] as TreeViewItem;
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




        #region PatientOverview

        private void buttonPatientOverviewAddRow_Click(object sender, RoutedEventArgs e)
        {
            Globals.GetLevelOverview.AddPatientToLoadedLevel();
            //patientOverviewLayout.ItemsSource = Globals.GetLevelOverview.GetLevelLoaded.PatientCollection;
            //ContentControl patientRow = new ContentControl();
            //patientRow.ContentTemplate = PatientRowDataTemplate;

        }

        #endregion



        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members

        private void Window_Closed(object sender, EventArgs e)
        {
            this.StoreWindowSettings();

        }

        private void selectAllPatientChancesCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Globals.GetLevelOverview.GetLevelLoaded.SelectAllPatientChances((bool)this.selectAllPatientChancesCheckbox.IsChecked);
        }

        private void customTreatmentWeightsButton_Click(object sender, RoutedEventArgs e)
        {
            CustomTreatmentWeightsWindow dialog = new CustomTreatmentWeightsWindow();
            Nullable<bool> dialogResult = dialog.ShowDialog();

        }

        private void generateLevelButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.GetLevelOverview.RandomizeLevel();
        }

        private void menuItemExportLevelList_Click(object sender, RoutedEventArgs e)
        {
            LevelListExport dialog = new LevelListExport();
            Nullable<bool> dialogResult = dialog.ShowDialog();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow dialog = new HelpWindow();
            Nullable<bool> dialogResult = dialog.ShowDialog();

        }


    }
}
