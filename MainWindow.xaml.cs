using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DataNameSpace;
using UiWindows;

namespace HM4DesignTool
{
    using HM4DesignTool.Forms;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private DataTemplate _patientRowDataTemplate = null;
        public DataTemplate PatientRowDataTemplate
        {
            get
            {
                if (_patientRowDataTemplate == null)
                {
                    _patientRowDataTemplate = FindResource("PatientRowControlTemplate") as DataTemplate;
                }
                return _patientRowDataTemplate;
            }
        }

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
            //Set Global reference to this Window
            Globals.GetMainWindow = this;

            this.DataContext = this;
            this.levelDataLayout.DataContext = Globals.GetLevelOverview;
            this.mainLayout.DataContext = Globals.GetLevelOverview;

            levelTypeDropDown.ItemsSource = GetLevelTypes;
            OnPropertyChanged("PatientRowDataTemplate");

            SetupWindow();


        }

        private void SetupWindow()
        {


            // BeforeLoadWindowSettings();


            // AfterLoadWindowSettings();
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members

        private void Window_Closed(object sender, EventArgs e)
        {
            StoreWindowSettings();

        }

        private void selectAllPatientChancesCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Globals.GetLevelOverview.GetLevelLoaded.SelectAllPatientChances((bool)selectAllPatientChancesCheckbox.IsChecked);
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
