// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsWindow.xaml.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UiWindows
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using HM4DesignTool.Data;
    using HM4DesignTool.Level;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// Based on How to Data Bind in WPF (C#/XAML) tutorial: https://www.youtube.com/watch?v=545NoF7Sab4
    /// </summary>
    /// 


    public partial class SettingsWindow : INotifyPropertyChanged
    {

        #region Fields
        private string projectDirectoryPathValue = Globals.GetSettings.projectPathData;



        #region PatientTypeTabFields

        private int lastLoadedPatientTypeCategoriesIndex = -1;

        private List<string> patientTypeList = Globals.GetSettings.GetPatientTypesFromDisk();
        private Dictionary<string, List<string>> patientTypeCategoriesDict = Globals.GetSettings.GetPatientTypes();
        private Dictionary<string, CheckBox> PatientTypeCheckboxDict
        {
            get
            {
                Dictionary<string, CheckBox> patientTypeCheckboxDict = new Dictionary<string, CheckBox> { };

                foreach (ItemCollection checkList in new List<ItemCollection> { patientTypeMaleCheckList.Items, patientTypeFemaleCheckList.Items, patientTypeOtherCheckList.Items })
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
        #endregion



        #region Constructors
        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = this;

            SetupSettingsWindow();
            LoadSaveData();

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
        #region GeneralTabProperties
        public string ProjectDirectoryPathValue
        {
            get => this.projectDirectoryPathValue;
            set
            {
                if (this.projectDirectoryPathValue != value)
                {
                    this.projectDirectoryPathValue = value;
                    OnPropertyChanged();
                    projectPathScriptText.Text = ProjectPathScriptValue;
                    projectPathLevelText.Text = ProjectPathLevelValue;
                    projectPathImagesText.Text = ProjectPathImagesValue;
                }
            }
        }
        public string ProjectPathScriptValue => ProjectDirectoryPathValue + "\\script\\";

        public string ProjectPathLevelValue => ProjectDirectoryPathValue + "\\script\\levels\\";

        public string ProjectPathImagesValue => ProjectDirectoryPathValue + "\\images\\";

        #endregion


        #region Commands

        #endregion

        #endregion

        #region Private



        #endregion

        #endregion



        #region Methods

        #region Public



        #endregion

        #region Private

        #region Signals

        #endregion

        #endregion

        #endregion




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

        #region TreatmentTabProperties
        private int lastLoadedTreatmentCategoriesIndex = -1;
        private Dictionary<string, List<Treatment>> treatmentCategoriesDict = Globals.GetSettings.GetTreatmentDictionary();

        private Color _treatmentSelectColor = Colors.White;
        private ObservableCollection<Treatment> LoadedTreatmentList = new ObservableCollection<Treatment> { };

        private ObservableCollection<string> treatmentDifficultyModifierList = new ObservableCollection<string> { };

        private string SelectedTreatmentRoomCategoryKey
        {
            get
            {
                if (treatmentRoomList.Items.Count > 0)
                {
                    return treatmentRoomList.SelectedItem.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public ObservableCollection<Treatment> TreatmentList
        {
            get => LoadedTreatmentList;
            set
            {
                LoadedTreatmentList = value;
                OnPropertyChanged();
            }

        }
        public ObservableCollection<string> TreatmentDifficultyModifierList
        {
            get => treatmentDifficultyModifierList;
            set
            {
                treatmentDifficultyModifierList = value;
                OnPropertyChanged("TreatmentDifficultyModifierList");

            }
        }
        public Color TreatmentSelectColor
        {
            get => _treatmentSelectColor;
            set
            {
                _treatmentSelectColor = value;
                OnPropertyChanged("TreatmentSelectColor");
                UpdateTreatmentColor();
            }
        }
        #endregion

        #region BalancingTabProperties
        private int lastLoadedBalancingCategoriesIndex = -1;
        private Dictionary<string, List<string>> balancingCategoriesDict = Globals.GetSettings.GetBalancingCategories();  // Room[N] -> List with double difficulty Modifiers

        private ObservableCollection<string> LoadedDifficultyModifierList = new ObservableCollection<string> { };
        private Double difficultyModifier = 0;

        public ObservableCollection<string> DifficultyModifierList
        {
            get => LoadedDifficultyModifierList;
            set
            {
                LoadedDifficultyModifierList = value;
                OnPropertyChanged("TreatmentDifficultyModifierList");
            }
        }
        private Double LoadedDifficultyModifier
        {
            get => difficultyModifier;
            set
            {
                difficultyModifier = value;
                OnPropertyChanged("AverageEntryTimePerPatient");
                OnPropertyChanged("TimeBetweenPatients");
                OnPropertyChanged("NumberOfPatients");
                OnPropertyChanged("TreatmentPerPatient");
                OnPropertyChanged("TimePerTreatment");
                OnPropertyChanged("MilliSecondsPerLevel");
                OnPropertyChanged("MinutesPerLevel");
            }
        }
        public Double AverageEntryTimePerPatient => GameValues.AverageEntryTimePerPatient(LoadedDifficultyModifier);

        public Double TimeBetweenPatients => GameValues.TimeBetweenPatients(LoadedDifficultyModifier);

        public Double NumberOfPatients => GameValues.NumberOfPatients(LoadedDifficultyModifier);

        public Double TreatmentPerPatient => GameValues.TreatmentPerPatient(LoadedDifficultyModifier);

        public Double TimePerTreatment => GameValues.TimePerTreatment(LoadedDifficultyModifier);

        public Double MilliSecondsPerLevel => GameValues.MilliSecondsPerLevel(LoadedDifficultyModifier);

        public Double MinutesPerLevel => Math.Round(GameValues.MinutesPerLevel(LoadedDifficultyModifier), 6);

        #endregion
        private void SetupSettingsWindow()
        {

            //Set categories for the patientTypeRoomList
            #region SetupPatientTypeTab
            foreach (string roomName in Globals.RoomCategories)
            {
                patientTypeRoomList.Items.Add(roomName);

                if (!patientTypeCategoriesDict.ContainsKey(roomName))
                {
                    patientTypeCategoriesDict.Add(roomName, new List<string> { });
                }

            }
            // Sort and create all checkboxes for the different groups
            foreach (string patientType in this.patientTypeList)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = patientType;
                checkBox.IsChecked = false;
                if (patientType.Contains("_male"))
                {
                    patientTypeMaleCheckList.Items.Add(checkBox);
                }
                else if (patientType.Contains("_female"))
                {
                    patientTypeFemaleCheckList.Items.Add(checkBox);
                }
                else
                {
                    patientTypeOtherCheckList.Items.Add(checkBox);
                }

            }

            #endregion

            //Set categories for the treatmentRoomList
            #region SetupTreatmentTab
            //set the itemssource
            foreach (string roomName in Globals.RoomCategories)
            {
                treatmentRoomList.Items.Add(roomName);

                if (!treatmentCategoriesDict.ContainsKey(roomName))
                {
                    treatmentCategoriesDict.Add(roomName, new List<Treatment> { });
                }

            }
            #endregion

            //Set categories for the balancingRoomList
            #region SetupBalancingTab
            foreach (string roomName in Globals.RoomCategories)
            {
                balancingRoomList.Items.Add(roomName);

                if (!balancingCategoriesDict.ContainsKey(roomName))
                {
                    balancingCategoriesDict.Add(roomName, new List<string> { });
                }

            }
            #endregion
        }

        private void LoadSaveData()
        {
            //Changing the index will automatically load the save data in the UI
            patientTypeRoomList.SelectedIndex = 0;
            treatmentRoomList.SelectedIndex = 0;
            balancingRoomList.SelectedIndex = 0;
        }

        private void SendSaveData()
        {   //Connect all the stored settings in this window with the SettingsObject
            Globals.GetSettings.projectPathData = ProjectDirectoryPathValue;

            StorePatientTypeCategory();
            Globals.GetSettings.SetPatientTypes(patientTypeCategoriesDict);

            StoreTreatmentCategory();
            Globals.GetSettings.SetTreatmentCategories(treatmentCategoriesDict);

            StoreBalancingCategory();
            Globals.GetSettings.SetBalancingCategories(balancingCategoriesDict);

            Globals.GetSettings.SaveSettings();
        }

        #region StoreData

        private void StorePatientTypeCategory()
        {
            if (lastLoadedPatientTypeCategoriesIndex > -1)
            {
                string categoryKey = patientTypeRoomList.Items[lastLoadedPatientTypeCategoriesIndex].ToString();

                if (patientTypeCategoriesDict.ContainsKey(categoryKey))
                {
                    List<string> checkedPatientTypes = new List<string> { };

                    foreach (KeyValuePair<string, CheckBox> patientType in PatientTypeCheckboxDict)
                    {
                        if ((bool)patientType.Value.IsChecked)
                        {
                            checkedPatientTypes.Add(patientType.Key);
                        }
                    }

                    patientTypeCategoriesDict[categoryKey] = checkedPatientTypes;

                }
                else
                {
                    Console.WriteLine("ERROR: windowSettings.StorePatientTypeCategory, patientTypeCategoriesDict does not contain key: " + categoryKey);
                }

            }
        }

        private void StoreTreatmentCategory()
        {
            if (lastLoadedTreatmentCategoriesIndex > -1)
            {
                string categoryName = treatmentRoomList.Items.GetItemAt(lastLoadedTreatmentCategoriesIndex).ToString();
                if (treatmentCategoriesDict.ContainsKey(categoryName))
                {
                    treatmentCategoriesDict[categoryName] = TreatmentList.ToList();
                }

            }
            else
            {
                Console.WriteLine("ERROR: settingsWindowStoreTreatmentCategory, lastLoadedTreatmentCategoriesIndex is -1!");
            }

        }

        private void StoreBalancingCategory()
        {
            if (lastLoadedBalancingCategoriesIndex > -1)
            {
                string categoryName = balancingRoomList.Items.GetItemAt(lastLoadedBalancingCategoriesIndex).ToString();
                if (balancingCategoriesDict.ContainsKey(categoryName))
                {
                    balancingCategoriesDict[categoryName] = DifficultyModifierList.ToList();
                }

            }
            else
            {
                Console.WriteLine("ERROR: settingsWindow.StoreBalancingCategory, lastLoadedBalancingCategoriesIndex is -1!");
            }


        }

        #endregion

        #region LoadData
        private void LoadPatientTypeCategory(string categoryKey = null)
        {
            if (patientTypeRoomList.SelectedIndex > -1)
            {

                if (patientTypeCategoriesDict.ContainsKey(categoryKey))
                {
                    List<string> loadedPatientTypeList = patientTypeCategoriesDict[categoryKey];
                    foreach (KeyValuePair<string, CheckBox> patientType in PatientTypeCheckboxDict)
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

                    lastLoadedPatientTypeCategoriesIndex = patientTypeRoomList.SelectedIndex;
                }
            }
        }

        private void LoadTreatmentCategory(string categoryKey = null)
        {
            if (treatmentRoomList.SelectedIndex > -1)
            {
                if (treatmentCategoriesDict.ContainsKey(categoryKey))
                {
                    List<Treatment> treatmentDataRows = treatmentCategoriesDict[categoryKey];

                    LoadedTreatmentList.Clear();
                    foreach (Treatment treatment in treatmentDataRows)
                    {
                        LoadedTreatmentList.Add(treatment);
                    }

                    lastLoadedTreatmentCategoriesIndex = treatmentRoomList.SelectedIndex;
                    UpdateTreatmentDifficultyModifierList();
                }
            }
        }

        private void LoadBalancingCategory(string CategoryKey = null)
        {
            if (balancingRoomList.SelectedIndex > -1 && CategoryKey != null && balancingCategoriesDict.ContainsKey(CategoryKey))
            {
                //Clear and Load the list in the Difficulty Modifier
                DifficultyModifierList.Clear();
                for (int i = 0; i < balancingCategoriesDict[CategoryKey].Count; i++)
                {
                    string difficultyModifier = balancingCategoriesDict[CategoryKey][i];
                    //Add n.0 behind whole numbers
                    if (!difficultyModifier.Contains("."))
                    {
                        difficultyModifier = difficultyModifier + ".0";
                    }
                    DifficultyModifierList.Add(difficultyModifier);
                }
                lastLoadedBalancingCategoriesIndex = balancingRoomList.SelectedIndex;
                if (difficultyModifierList.Items.Count > 0)
                {
                    difficultyModifierList.SelectedIndex = 0;
                }
            }

        }

        private void LoadDifficultyModifierData(string diffCategoryValue)
        {
            LoadedDifficultyModifier = Globals.StringToDouble(diffCategoryValue);
        }
        #endregion


        private void UpdateTreatmentDifficultyModifierList()
        {
            ObservableCollection<string> treatmentDifficultyModifierList = new ObservableCollection<string> { };
            if (balancingCategoriesDict.ContainsKey(SelectedTreatmentRoomCategoryKey))
            {
                foreach (string treatmentDifficultyModifier in balancingCategoriesDict[SelectedTreatmentRoomCategoryKey])
                {
                    treatmentDifficultyModifierList.Add(treatmentDifficultyModifier);
                }
                TreatmentDifficultyModifierList = treatmentDifficultyModifierList;
                difficultyUnlockedColumn.ItemsSource = TreatmentDifficultyModifierList;

            }
        }

        private void UpdateTreatmentColor()
        {
            foreach (Treatment treatment in TreatmentList)
            {
                if (treatment.IsSelected)
                {
                    treatment.TreatmentColor = TreatmentSelectColor;
                }
            }


        }
        #region Signals

        #region GeneralTabSignals
        private void projectDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            //http://www.ookii.org/software/dialogs/
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "Please select the [HM4 SVN]/data/ folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!Ookii.Dialogs.Wpf.VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            if ((bool)dialog.ShowDialog(this))
            {
                ProjectDirectoryPathValue = dialog.SelectedPath;
            }

        }
        #endregion

        #region PatientTypeTabSignals
        private void patientTypeRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StorePatientTypeCategory();
            string categoryKey = patientTypeRoomList.SelectedItem.ToString();
            LoadPatientTypeCategory(categoryKey);

        }
        #endregion

        #region TreatmentTabSignals
        private void treatmentRowButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            TreatmentList.Add(new Treatment());
        }

        private void treatmentRowButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (TreatmentList.Count > 0)
            {
                bool anythingSelected = false;
                for (int i = TreatmentList.Count - 1; 0 < i; i--)
                {
                    if (TreatmentList[i].IsSelected)
                    {
                        TreatmentList.RemoveAt(i);
                        anythingSelected = true;
                    }
                }
                if (!anythingSelected)
                {
                    TreatmentList.Remove(TreatmentList.Last());
                }

            }
        }

        private void treatmentRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StoreTreatmentCategory();
            string categoryKey = treatmentRoomList.SelectedItem.ToString();
            LoadTreatmentCategory(categoryKey);
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
            StoreBalancingCategory();
            string categoryKey = balancingRoomList.SelectedItem.ToString();
            LoadBalancingCategory(categoryKey);
        }

        private void difficultyModifierList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (difficultyModifierList.SelectedItem != null)
            {
                ChangeGameValueEnabledState(true);
                string diffCategoryKey = difficultyModifierList.SelectedItem.ToString();
                LoadDifficultyModifierData(diffCategoryKey);
            }
            else
            {
                ChangeGameValueEnabledState(false);
                Console.WriteLine("ERROR: SettingsWindow.difficultyModifierList_SelectionChanged, difficultyModifierList.SelectedItem was null!");
            }
        }

        private void diffModifierRowButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Double newValue = (Double)diffModifierValue.Value;
            string newValueStr = string.Format("{0:N1}", newValue);
            if (!DifficultyModifierList.Contains(newValueStr))
            {
                DifficultyModifierList.Add(newValueStr);
            }
        }

        private void diffModifierRowButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (difficultyModifierList.SelectedItem != null)
            {
                string ValueStr = difficultyModifierList.SelectedItem.ToString();
                int index = difficultyModifierList.SelectedIndex;
                if (DifficultyModifierList.Contains(ValueStr))
                {
                    DifficultyModifierList.Remove(ValueStr);
                }
                if (difficultyModifierList.Items.Count > 0)
                {
                    if (index < difficultyModifierList.Items.Count)
                    {
                        difficultyModifierList.SelectedIndex = index;
                    }
                    else
                    {
                        difficultyModifierList.SelectedIndex = difficultyModifierList.Items.Count - 1;
                    }
                }
            }
        }

        private void ChangeGameValueEnabledState(bool State)
        {
            averageEntryTimePerPatientValue.IsEnabled = State;
            timeBetweenPatientsValue.IsEnabled = State;
            numberOfPatientsValue.IsEnabled = State;
            treatmentPerPatientValue.IsEnabled = State;
            timePerTreatmentValue.IsEnabled = State;
            milliSecondsPerLevelValue.IsEnabled = State;
            minutesPerLevelValue.IsEnabled = State;

            if (!State)
            {
                averageEntryTimePerPatientValue.Clear();
                timeBetweenPatientsValue.Clear();
                numberOfPatientsValue.Clear();
                treatmentPerPatientValue.Clear();
                timePerTreatmentValue.Clear();
                milliSecondsPerLevelValue.Clear();
                minutesPerLevelValue.Clear();
            }
        }

        #endregion

        #region GeneralControlsSignals
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SendSaveData();
            this.Close();
        }
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #endregion

        #region Events
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





        #endregion

    }
}
