// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsWindow.xaml.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using HM4DesignTool.Data;
    using HM4DesignTool.Level;
    using HM4DesignTool.Utilities;

    using Ookii.Dialogs.Wpf;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// Based on How to Data Bind in WPF (C#/XAML) tutorial: https://www.youtube.com/watch?v=545NoF7Sab4
    /// </summary>
    public partial class SettingsWindow : INotifyPropertyChanged
    {
        #region Fields

        #region General

        /// <summary>
        /// Only execute commands in this level when this is true.
        /// </summary>
        private readonly bool canExecuteCommands;

        #endregion

        #region GeneralTabFields

        private string projectDirectoryPathValue = Globals.GetSettings.ProjectPathData;

        #endregion

        #region PatientTypeTabFields

        private int lastLoadedPatientTypeCategoriesIndex = -1;

        private List<string> patientTypeList = Globals.GetSettings.GetPatientTypesFromDisk();

        private Dictionary<string, List<string>> patientTypeCategoriesDict = Globals.GetSettings.GetPatientTypes();

        #endregion

        #region StationTabFields

        private int lastLoadedStationCategoriesIndex = -1;

        private readonly Dictionary<string, List<Station>> stationCategoriesDict = Globals.GetSettings.GetStationDictionary();

        private ObservableCollection<Station> LoadedStationList = new ObservableCollection<Station>();

        private ObservableCollection<string> stationDifficultyModifierList = new ObservableCollection<string>();

        #endregion

        #region TreatmentTabFields

        private int lastLoadedTreatmentCategoriesIndex = -1;

        private Dictionary<string, List<Treatment>> treatmentCategoriesDict =
            Globals.GetSettings.GetTreatmentDictionary();


        private Color treatmentSelectColor = Colors.White;

        private ObservableCollection<Treatment> LoadedTreatmentList = new ObservableCollection<Treatment>();

        private ObservableCollection<string> treatmentDifficultyModifierList = new ObservableCollection<string>();

        private ObservableCollection<string> stationTreatmentList = new ObservableCollection<string>();

        #endregion

        #region BalancingTabFields

        private int lastLoadedBalancingCategoriesIndex = -1;

        private Dictionary<string, List<string>> balancingCategoriesDict = Globals.GetSettings.GetBalancingCategories();

        private ObservableCollection<string> LoadedDifficultyModifierList = new ObservableCollection<string>();

        private double difficultyModifier = 0;

        #endregion

        #region Commands

        /// <summary>
        /// The add Treatment row command field.
        /// </summary>
        private ICommand addTreatmentRowCommand;

        /// <summary>
        /// The remove Treatment row command field.
        /// </summary>
        private ICommand removeTreatmentRowCommand;

        /// <summary>
        /// The add Station row command field.
        /// </summary>
        private ICommand addStationRowCommand;

        /// <summary>
        /// The remove Station row command field.
        /// </summary>
        private ICommand removeStationRowCommand;

        #endregion

        #endregion

        #region Constructors

        public SettingsWindow()
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.SetupSettingsWindow();
            this.LoadSaveData();
            this.canExecuteCommands = true;
        }

        #endregion

        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion Events

        #region Properties

        #region Public
        #region PatientTypeTabProperties
        public string PatientListPreview
        {
            get
            {
                string text = string.Empty;
                foreach (string patientType in this.patientTypeList)
                {
                    text += patientType + Environment.NewLine;
                }

                return text;
            }
        }

        #endregion

        #region StationTabProperties

        public ObservableCollection<Station> StationList
        {
            get => this.LoadedStationList;
            set
            {
                this.LoadedStationList = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<string> StationDifficultyModifierList
        {
            get => this.stationDifficultyModifierList;
            set
            {
                this.stationDifficultyModifierList = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region TreatmentTabProperties
        public ObservableCollection<Treatment> TreatmentList
        {
            get => this.LoadedTreatmentList;
            set
            {
                this.LoadedTreatmentList = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<string> TreatmentDifficultyModifierList
        {
            get => this.treatmentDifficultyModifierList;
            set
            {
                this.treatmentDifficultyModifierList = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<string> StationTreatmentList
        {
            get => this.stationTreatmentList;
            set
            {
                this.stationTreatmentList = value;
               // this.StationTreatmentColumn.ItemsSource = value;
                this.OnPropertyChanged();
            }
        }
        public Color TreatmentSelectColor
        {
            get => this.treatmentSelectColor;
            set
            {
                this.treatmentSelectColor = value;
                this.OnPropertyChanged();
                this.UpdateTreatmentColor();
            }
        }

        #endregion

        #region BalancingTabProperties

        // Room[N] -> List with double difficulty Modifiers
        public ObservableCollection<string> DifficultyModifierList
        {
            get => this.LoadedDifficultyModifierList;
            set
            {
                this.LoadedDifficultyModifierList = value;
                this.OnPropertyChanged("TreatmentDifficultyModifierList");
            }
        }
        public double AverageEntryTimePerPatient =>
            GameValues.AverageEntryTimePerPatient(this.LoadedDifficultyModifier);

        public double TimeBetweenPatients => GameValues.TimeBetweenPatients(this.LoadedDifficultyModifier);

        public double NumberOfPatients => GameValues.NumberOfPatients(this.LoadedDifficultyModifier);

        public double TreatmentPerPatient => GameValues.TreatmentPerPatient(this.LoadedDifficultyModifier);

        public double TimePerTreatment => GameValues.TimePerTreatment(this.LoadedDifficultyModifier);

        public double MilliSecondsPerLevel => GameValues.MilliSecondsPerLevel(this.LoadedDifficultyModifier);

        public double MinutesPerLevel => Math.Round(GameValues.MinutesPerLevel(this.LoadedDifficultyModifier), 6);

        #endregion


        #region Commands

        /// <summary>
        /// Reload the level command.
        /// </summary>
        public ICommand AddTreatmentRowCommand => this.addTreatmentRowCommand ?? (this.addTreatmentRowCommand = new CommandHandler(this.AddTreatmentRow, this.canExecuteCommands));

        /// <summary>
        /// Generate the level command.
        /// </summary>
        public ICommand RemoveTreatmentRowCommand => this.removeTreatmentRowCommand ?? (this.removeTreatmentRowCommand = new CommandHandler(this.RemoveTreatmentRow, this.canExecuteCommands));

        /// <summary>
        /// Reload the level command.
        /// </summary>
        public ICommand AddStationRowCommand => this.addStationRowCommand ?? (this.addStationRowCommand = new CommandHandler(this.AddStationRow, this.canExecuteCommands));

        /// <summary>
        /// Generate the level command.
        /// </summary>
        public ICommand RemoveStationRowCommand => this.removeStationRowCommand ?? (this.removeStationRowCommand = new CommandHandler(this.RemoveStationRow, this.canExecuteCommands));


        #endregion


        #endregion

        #region Private
        #region GeneralTabProperties

        public string ProjectDirectoryPathValue
        {
            get => this.projectDirectoryPathValue;
            set
            {
                if (this.projectDirectoryPathValue != value)
                {
                    this.projectDirectoryPathValue = value;
                    this.OnPropertyChanged();
                    this.projectPathScriptText.Text = this.ProjectPathScriptValue;
                    this.projectPathLevelText.Text = this.ProjectPathLevelValue;
                    this.projectPathImagesText.Text = this.ProjectPathImagesValue;
                }
            }
        }

        public string ProjectPathScriptValue => this.ProjectDirectoryPathValue + "\\script\\";

        public string ProjectPathLevelValue => this.ProjectDirectoryPathValue + "\\script\\levels\\";

        public string ProjectPathImagesValue => this.ProjectDirectoryPathValue + "\\images\\";

        #endregion


        #region PatientTypeTabProperties
        private Dictionary<string, CheckBox> PatientTypeCheckboxDict
        {
            get
            {
                Dictionary<string, CheckBox> patientTypeCheckboxDict = new Dictionary<string, CheckBox>();

                foreach (ItemCollection checkList in new List<ItemCollection>
                                                         {
                                                             this.patientTypeMaleCheckList.Items,
                                                             this.patientTypeFemaleCheckList.Items,
                                                             this.patientTypeOtherCheckList.Items
                                                         })
                {
                    foreach (CheckBox itemChecked in checkList)
                    {
                        patientTypeCheckboxDict.Add(itemChecked.Content.ToString(), itemChecked);
                    }
                }

                return patientTypeCheckboxDict;
            }
        }


        #endregion

        #region StationTabProperties
        private string SelectedStationRoomCategoryKey
        {
            get
            {
                if (this.stationRoomList.Items.Count > 0)
                {
                    return this.stationRoomList.SelectedItem.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }




        #endregion

        #region TreatmentTabProperties
        private string SelectedTreatmentRoomCategoryKey
        {
            get
            {
                if (this.treatmentRoomList.Items.Count > 0)
                {
                    return this.treatmentRoomList.SelectedItem.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }




        #endregion

        #region BalancingTabProperties
        private double LoadedDifficultyModifier
        {
            get => this.difficultyModifier;
            set
            {
                this.difficultyModifier = value;
                this.OnPropertyChanged("AverageEntryTimePerPatient");
                this.OnPropertyChanged("TimeBetweenPatients");
                this.OnPropertyChanged("NumberOfPatients");
                this.OnPropertyChanged("TreatmentPerPatient");
                this.OnPropertyChanged("TimePerTreatment");
                this.OnPropertyChanged("MilliSecondsPerLevel");
                this.OnPropertyChanged("MinutesPerLevel");
            }
        }



        #endregion
        #endregion

        #endregion

        #region Methods

        #region Public
        private void SetupSettingsWindow()
        {
            // Set categories for the patientTypeRoomList
            foreach (string roomName in Globals.RoomCategories)
            {
                this.patientTypeRoomList.Items.Add(roomName);

                if (!this.patientTypeCategoriesDict.ContainsKey(roomName))
                {
                    this.patientTypeCategoriesDict.Add(roomName, new List<string>());
                }
            }

            // Sort and create all checkboxes for the different groups
            foreach (string patientType in this.patientTypeList)
            {
                CheckBox checkBox = new CheckBox { Content = patientType, IsChecked = false };
                if (patientType.Contains("_male"))
                {
                    this.patientTypeMaleCheckList.Items.Add(checkBox);
                }
                else if (patientType.Contains("_female"))
                {
                    this.patientTypeFemaleCheckList.Items.Add(checkBox);
                }
                else
                {
                    this.patientTypeOtherCheckList.Items.Add(checkBox);
                }
            }

            // Set categories for the treatmentRoomList
            #region SetupStationTab

            // set the itemssource
            foreach (string roomName in Globals.RoomCategories)
            {
                this.stationRoomList.Items.Add(roomName);

                if (!this.stationCategoriesDict.ContainsKey(roomName))
                {
                    this.stationCategoriesDict.Add(roomName, new List<Station>());
                }
            }

            #endregion

            // Set categories for the treatmentRoomList
            #region SetupTreatmentTab

            // set the itemssource
            foreach (string roomName in Globals.RoomCategories)
            {
                this.treatmentRoomList.Items.Add(roomName);

                if (!this.treatmentCategoriesDict.ContainsKey(roomName))
                {
                    this.treatmentCategoriesDict.Add(roomName, new List<Treatment>());
                }
            }

            #endregion

            // Set categories for the balancingRoomList
            #region SetupBalancingTab

            foreach (string roomName in Globals.RoomCategories)
            {
                this.balancingRoomList.Items.Add(roomName);

                if (!this.balancingCategoriesDict.ContainsKey(roomName))
                {
                    this.balancingCategoriesDict.Add(roomName, new List<string>());
                }
            }

            #endregion
        }

        private void LoadSaveData()
        {
            // Changing the index will automatically load the save data in the UI
            this.stationRoomList.SelectedIndex = 0;
            this.patientTypeRoomList.SelectedIndex = 0;
            this.treatmentRoomList.SelectedIndex = 0;
            this.balancingRoomList.SelectedIndex = 0;
        }

        private void SendSaveData()
        {
            // Connect all the stored settings in this window with the SettingsObject
            Globals.GetSettings.ProjectPathData = this.ProjectDirectoryPathValue;

            this.StorePatientTypeCategory();
            Globals.GetSettings.SetPatientTypes(this.patientTypeCategoriesDict);

            this.StoreStationCategory();
            Globals.GetSettings.SetStationCategories(this.stationCategoriesDict);

            this.StoreTreatmentCategory();
            Globals.GetSettings.SetTreatmentCategories(this.treatmentCategoriesDict);

            this.StoreBalancingCategory();
            Globals.GetSettings.SetBalancingCategories(this.balancingCategoriesDict);

            Globals.GetSettings.SaveSettings();
        }


        #endregion

        #region Private

        #region StationTab

        private void AddStationRow()
        {
            this.StationList.Add(new Station());
        }

        private void RemoveStationRow()
        {
            if (this.StationList.Count > 0)
            {
                bool anythingSelected = false;
                for (int i = this.StationList.Count - 1; i > 0; i--)
                {
                    if (this.StationList[i].IsSelected)
                    {
                        this.StationList.RemoveAt(i);
                        anythingSelected = true;
                    }
                }

                if (!anythingSelected)
                {
                    this.StationList.Remove(this.StationList.Last());
                }
            }
        }

        #endregion

        #region TreatmentTab

        private void AddTreatmentRow()
        {
            this.TreatmentList.Add(new Treatment());
        }

        private void RemoveTreatmentRow()
        {
            if (this.TreatmentList.Count > 0)
            {
                bool anythingSelected = false;
                for (int i = this.TreatmentList.Count - 1; i > 0; i--)
                {
                    if (this.TreatmentList[i].IsSelected)
                    {
                        this.TreatmentList.RemoveAt(i);
                        anythingSelected = true;
                    }
                }

                if (!anythingSelected)
                {
                    this.TreatmentList.Remove(this.TreatmentList.Last());
                }
            }
        }

        #endregion

        #region Update

        private void UpdateStationDifficultyModifierList()
        {
            ObservableCollection<string> stationDifficultyModifierList = new ObservableCollection<string>();
            if (this.balancingCategoriesDict.ContainsKey(this.SelectedStationRoomCategoryKey))
            {
                foreach (string stationDifficultyModifier in this.balancingCategoriesDict[
                    this.SelectedStationRoomCategoryKey])
                {
                    stationDifficultyModifierList.Add(stationDifficultyModifier);
                }

                this.StationDifficultyModifierList = stationDifficultyModifierList;
                this.StationDifficultyUnlockedColumn.ItemsSource = this.StationDifficultyModifierList;
            }
        }

        private void UpdateTreatmentDifficultyModifierList()
        {
            ObservableCollection<string> treatmentDifficultyModifierList = new ObservableCollection<string>();
            if (this.balancingCategoriesDict.ContainsKey(this.SelectedTreatmentRoomCategoryKey))
            {
                foreach (string treatmentDifficultyModifier in this.balancingCategoriesDict[this.SelectedTreatmentRoomCategoryKey])
                {
                    treatmentDifficultyModifierList.Add(treatmentDifficultyModifier);
                }

                this.TreatmentDifficultyModifierList = treatmentDifficultyModifierList;
                this.difficultyUnlockedColumn.ItemsSource = this.TreatmentDifficultyModifierList;
            }
        }

        private void UpdateStationTreatmentList()
        {
            ObservableCollection<string> stationTreatmentList = new ObservableCollection<string>();
            if (this.stationCategoriesDict.ContainsKey(this.SelectedTreatmentRoomCategoryKey))
            {
                foreach (Station station in this.stationCategoriesDict[this.SelectedTreatmentRoomCategoryKey])
                {
                    stationTreatmentList.Add(station.StationName);
                }

                this.StationTreatmentList = stationTreatmentList;
            }
        }

        private void UpdateTreatmentColor()
        {
            foreach (Treatment treatment in this.TreatmentList)
            {
                if (treatment.IsSelected)
                {
                    treatment.TreatmentColor = this.TreatmentSelectColor;
                }
            }
        }

        #endregion

        #region StoreData

        private void StorePatientTypeCategory()
        {
            if (this.lastLoadedPatientTypeCategoriesIndex > -1)
            {
                string categoryKey = this.patientTypeRoomList.Items[this.lastLoadedPatientTypeCategoriesIndex]
                    .ToString();

                if (this.patientTypeCategoriesDict.ContainsKey(categoryKey))
                {
                    List<string> checkedPatientTypes = new List<string>();

                    foreach (KeyValuePair<string, CheckBox> patientType in this.PatientTypeCheckboxDict)
                    {
                        if ((bool)patientType.Value.IsChecked)
                        {
                            checkedPatientTypes.Add(patientType.Key);
                        }
                    }

                    this.patientTypeCategoriesDict[categoryKey] = checkedPatientTypes;
                }
                else
                {
                    Console.WriteLine(
                        "ERROR: windowSettings.StorePatientTypeCategory, patientTypeCategoriesDict does not contain key: "
                        + categoryKey);
                }
            }
        }

        private void StoreStationCategory()
        {
            if (this.lastLoadedStationCategoriesIndex > -1)
            {
                string categoryName = this.stationRoomList.Items.GetItemAt(this.lastLoadedStationCategoriesIndex).ToString();
                if (this.stationCategoriesDict.ContainsKey(categoryName))
                {
                    this.stationCategoriesDict[categoryName] = this.StationList.ToList();
                }
            }
            else
            {
                Console.WriteLine(
                    "ERROR: settingsWindow.StoreStationCategory, lastLoadedStationCategoriesIndex is -1!");
            }
        }


        private void StoreTreatmentCategory()
        {
            if (this.lastLoadedTreatmentCategoriesIndex > -1)
            {
                string categoryName = this.treatmentRoomList.Items.GetItemAt(this.lastLoadedTreatmentCategoriesIndex).ToString();
                if (this.treatmentCategoriesDict.ContainsKey(categoryName))
                {
                    this.treatmentCategoriesDict[categoryName] = this.TreatmentList.ToList();
                }
            }
            else
            {
                Console.WriteLine(
                    "ERROR: settingsWindow.StoreTreatmentCategory, lastLoadedTreatmentCategoriesIndex is -1!");
            }
        }

        private void StoreBalancingCategory()
        {
            if (this.lastLoadedBalancingCategoriesIndex > -1)
            {
                string categoryName = this.balancingRoomList.Items.GetItemAt(this.lastLoadedBalancingCategoriesIndex).ToString();
                if (this.balancingCategoriesDict.ContainsKey(categoryName))
                {
                    this.balancingCategoriesDict[categoryName] = this.DifficultyModifierList.ToList();
                }
            }
            else
            {
                Console.WriteLine(
                    "ERROR: settingsWindow.StoreBalancingCategory, lastLoadedBalancingCategoriesIndex is -1!");
            }
        }

        #endregion


        #region LoadData

        private void LoadPatientTypeCategory(string categoryKey = null)
        {
            if (this.patientTypeRoomList.SelectedIndex > -1)
            {
                if (this.patientTypeCategoriesDict.ContainsKey(categoryKey))
                {
                    List<string> loadedPatientTypeList = this.patientTypeCategoriesDict[categoryKey];
                    foreach (KeyValuePair<string, CheckBox> patientType in this.PatientTypeCheckboxDict)
                    {
                        if (loadedPatientTypeList.Contains(patientType.Key))
                        {
                            patientType.Value.IsChecked = true;
                        }
                        else
                        {
                            patientType.Value.IsChecked = false;
                        }
                    }

                    this.lastLoadedPatientTypeCategoriesIndex = this.patientTypeRoomList.SelectedIndex;
                }
            }
        }

        private void LoadStationCategory(string categoryKey = null)
        {
            if (this.stationRoomList.SelectedIndex > -1)
            {
                if (this.stationCategoriesDict.ContainsKey(categoryKey))
                {
                    List<Station> treatmentDataRows = this.stationCategoriesDict[categoryKey];

                    this.LoadedStationList.Clear();
                    foreach (Station station in treatmentDataRows)
                    {
                        this.LoadedStationList.Add(station);
                    }

                    this.lastLoadedStationCategoriesIndex = this.stationRoomList.SelectedIndex;
                    this.UpdateStationDifficultyModifierList();
                }
            }
        }


        private void LoadTreatmentCategory(string categoryKey = null)
        {
            if (this.treatmentRoomList.SelectedIndex > -1)
            {
                if (this.treatmentCategoriesDict.ContainsKey(categoryKey))
                {
                    List<Treatment> treatmentDataRows = this.treatmentCategoriesDict[categoryKey];

                    this.LoadedTreatmentList.Clear();
                    foreach (Treatment treatment in treatmentDataRows)
                    {
                        this.LoadedTreatmentList.Add(treatment);
                    }

                    this.lastLoadedTreatmentCategoriesIndex = this.treatmentRoomList.SelectedIndex;
                    this.UpdateTreatmentDifficultyModifierList();
                    this.UpdateStationTreatmentList();
                }
            }
        }

        private void LoadBalancingCategory(string CategoryKey = null)
        {
            if (this.balancingRoomList.SelectedIndex > -1 && CategoryKey != null
                                                          && this.balancingCategoriesDict.ContainsKey(CategoryKey))
            {
                // Clear and Load the list in the Difficulty Modifier
                this.DifficultyModifierList.Clear();
                for (int i = 0; i < this.balancingCategoriesDict[CategoryKey].Count; i++)
                {
                    string difficultyModifier = this.balancingCategoriesDict[CategoryKey][i];

                    // Add n.0 behind whole numbers
                    if (!difficultyModifier.Contains("."))
                    {
                        difficultyModifier = difficultyModifier + ".0";
                    }

                    this.DifficultyModifierList.Add(difficultyModifier);
                }

                this.lastLoadedBalancingCategoriesIndex = this.balancingRoomList.SelectedIndex;
                if (this.difficultyModifierList.Items.Count > 0)
                {
                    this.difficultyModifierList.SelectedIndex = 0;
                }
            }
        }

        private void LoadDifficultyModifierData(string diffCategoryValue)
        {
            this.LoadedDifficultyModifier = Globals.StringToDouble(diffCategoryValue);
        }
        #endregion

        #region Signals

        #region GeneralTabSignals

        private void projectDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // http://www.ookii.org/software/dialogs/
            VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "Please select the [HM4 SVN]/data/ folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!Ookii.Dialogs.Wpf.VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(
                    this,
                    "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.",
                    "Sample folder browser dialog");
            if ((bool)dialog.ShowDialog(this))
            {
                this.ProjectDirectoryPathValue = dialog.SelectedPath;
            }
        }

        #endregion

        #region PatientTypeTabSignals

        private void patientTypeRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.StorePatientTypeCategory();
            string categoryKey = this.patientTypeRoomList.SelectedItem.ToString();
            this.LoadPatientTypeCategory(categoryKey);
        }

        #endregion

        #region StationTabSignals

        private void stationRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.StoreStationCategory();
            string categoryKey = this.stationRoomList.SelectedItem.ToString();
            this.LoadStationCategory(categoryKey);

        }

        #endregion

        #region TreatmentTabSignals
        private void treatmentRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.StoreTreatmentCategory();
            string categoryKey = this.treatmentRoomList.SelectedItem.ToString();
            this.LoadTreatmentCategory(categoryKey);
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        #endregion

        #region BalancingTabSignals

        private void balancingRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.StoreBalancingCategory();
            string categoryKey = this.balancingRoomList.SelectedItem.ToString();
            this.LoadBalancingCategory(categoryKey);
        }

        private void difficultyModifierList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.difficultyModifierList.SelectedItem != null)
            {
                this.ChangeGameValueEnabledState(true);
                string diffCategoryKey = this.difficultyModifierList.SelectedItem.ToString();
                this.LoadDifficultyModifierData(diffCategoryKey);
            }
            else
            {
                this.ChangeGameValueEnabledState(false);
                Console.WriteLine(
                    "ERROR: SettingsWindow.difficultyModifierList_SelectionChanged, difficultyModifierList.SelectedItem was null!");
            }
        }

        private void diffModifierRowButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            double newValue = (Double)this.diffModifierValue.Value;
            string newValueStr = string.Format("{0:N1}", newValue);
            if (!this.DifficultyModifierList.Contains(newValueStr))
            {
                this.DifficultyModifierList.Add(newValueStr);
            }
        }

        private void diffModifierRowButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (this.difficultyModifierList.SelectedItem != null)
            {
                string ValueStr = this.difficultyModifierList.SelectedItem.ToString();
                int index = this.difficultyModifierList.SelectedIndex;
                if (this.DifficultyModifierList.Contains(ValueStr))
                {
                    this.DifficultyModifierList.Remove(ValueStr);
                }

                if (this.difficultyModifierList.Items.Count > 0)
                {
                    if (index < this.difficultyModifierList.Items.Count)
                    {
                        this.difficultyModifierList.SelectedIndex = index;
                    }
                    else
                    {
                        this.difficultyModifierList.SelectedIndex = this.difficultyModifierList.Items.Count - 1;
                    }
                }
            }
        }

        private void ChangeGameValueEnabledState(bool State)
        {
            this.averageEntryTimePerPatientValue.IsEnabled = State;
            this.timeBetweenPatientsValue.IsEnabled = State;
            this.numberOfPatientsValue.IsEnabled = State;
            this.treatmentPerPatientValue.IsEnabled = State;
            this.timePerTreatmentValue.IsEnabled = State;
            this.milliSecondsPerLevelValue.IsEnabled = State;
            this.minutesPerLevelValue.IsEnabled = State;

            if (!State)
            {
                this.averageEntryTimePerPatientValue.Clear();
                this.timeBetweenPatientsValue.Clear();
                this.numberOfPatientsValue.Clear();
                this.treatmentPerPatientValue.Clear();
                this.timePerTreatmentValue.Clear();
                this.milliSecondsPerLevelValue.Clear();
                this.minutesPerLevelValue.Clear();
            }
        }

        #endregion

        #region GeneralControlsSignals

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            this.SendSaveData();
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

        #endregion

        #endregion
        #region Events

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


    }
}
