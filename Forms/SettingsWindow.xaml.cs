using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataNameSpace;

namespace UiWindows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// Based on How to Data Bind in WPF (C#/XAML) tutorial: https://www.youtube.com/watch?v=545NoF7Sab4
    /// </summary>
    /// 

    public class TestObject
    {
        public String TreatmentName { get; set; }

        public TestObject(String TreatmentName)
        {
            this.TreatmentName = TreatmentName;
        }
    }

    public partial class SettingsWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _projectDirectoryPathValue = Globals.GetSettings.projectPathData;
        public String ProjectDirectoryPathValue
        {
            get
            {
                return _projectDirectoryPathValue;
            }
            set
            {
                if (_projectDirectoryPathValue != value)
                {
                    _projectDirectoryPathValue = value;
                    OnPropertyChanged();
                    projectPathScriptText.Text = ProjectPathScriptValue;
                    projectPathLevelText.Text = ProjectPathLevelValue;
                    projectPathImagesText.Text = ProjectPathImagesValue;
                }
            }
        }
        public String ProjectPathScriptValue
        {
            get
            {
                return ProjectDirectoryPathValue + "\\script\\";
            }
        }
        public String ProjectPathLevelValue
        {
            get
            {
                return ProjectDirectoryPathValue + "\\script\\levels\\";
            }
        }
        public String ProjectPathImagesValue
        {
            get
            {
                return ProjectDirectoryPathValue + "\\images\\";
            }
        }

        //Patient Type Tab
        private int lastLoadedPatientTypeCategoriesIndex = 0;
        private Dictionary<String, List<String>> patientTypeCategoriesDict = Globals.GetSettings.GetPatientTypes();  // Room[N] -> List with only checked patientTypes

        //Treatment Type Tab
        private int lastLoadedTreatmentCategoriesIndex = 0;
        private Dictionary<String, List<Treatment>> treatmentCategoriesDict = Globals.GetSettings.GetTreatmentDictionary();
        private ObservableCollection<Treatment> LoadedTreatmentList = new ObservableCollection<Treatment> { };
        public ObservableCollection<Treatment> TreatmentList
        {
            get
            {
                if (lastLoadedTreatmentCategoriesIndex > -1 && treatmentRoomList.Items.Count > 0)
                {
                    String categoryName = treatmentRoomList.Items[lastLoadedTreatmentCategoriesIndex].ToString();

                    foreach (Treatment treatment in treatmentCategoriesDict[categoryName])
                    {
                        LoadedTreatmentList.Add(treatment);
                    }
                }

                return LoadedTreatmentList;
            }
            set
            {
                if (lastLoadedTreatmentCategoriesIndex > -1 && treatmentRoomList.Items.Count > 0)
                {
                    String categoryName = treatmentRoomList.Items[lastLoadedTreatmentCategoriesIndex].ToString();
                    List < Treatment > treatmentList = new List<Treatment>{ };
                    foreach (Treatment treatment in value)
                    {
                        treatmentList.Add(treatment);
                    }


                }

            }

        }


        //Balancing Tab
        private int lastLoadedBalancingCategoriesIndex = 0;
        private String GetPreviousBalancingCategoryKey
        {
            get
            {
                if (lastLoadedBalancingCategoriesIndex < balancingRoomList.Items.Count)
                {
                    return balancingRoomList.Items[lastLoadedBalancingCategoriesIndex].ToString();
                }
                else
                {
                    return null;
                }

            }
        }
        private String GetCurrentBalancingCategoryKey
        {
            get
            {
                if (balancingRoomList.SelectedIndex > -1 && balancingRoomList.SelectedIndex < balancingRoomList.Items.Count)
                {
                   return balancingRoomList.Items[balancingRoomList.SelectedIndex].ToString();
                }
                else
                {
                    return balancingRoomList.Items[lastLoadedBalancingCategoriesIndex].ToString();
                }

            }
        }
        private Dictionary<String, List<String>> balancingCategoriesDict = Globals.GetSettings.GetBalancingCategories();  // Room[N] -> List with double difficulty Modifiers
        private ObservableCollection<String> LoadedDifficultyModifierList = new ObservableCollection<String> { };
        public ObservableCollection<String> DifficultyModifierList {
            get
            {
                return LoadedDifficultyModifierList;

            }
            set
            {
                LoadedDifficultyModifierList = value;
            }
        }
        public GameValues GlobalValues
        {
            get{
                return Globals.GetGameValues;
            }
        }

        public Double AverageEntryTimePerPatient
        {
            get
            {
                Double difficultyModifier = 0;
                return GlobalValues.AverageEntryTimePerPatient(difficultyModifier);
            }
        }
        public Double TimeBetweenPatients
        {
            get
            {
                Double difficultyModifier = 0;
                return GlobalValues.TimeBetweenPatients(difficultyModifier);
            }
        }
        public Double NumberOfPatients
        {
            get
            {
                Double difficultyModifier = 0;
                return GlobalValues.NumberOfPatients(difficultyModifier);
            }
        }
        public Double TreatmentPerPatient
        {
            get
            {
                Double difficultyModifier = 0;
                return GlobalValues.TreatmentPerPatient(difficultyModifier);
            }
        }
        public Double TimePerTreatment
        {
            get
            {
                Double difficultyModifier = 0;
                return GlobalValues.TimePerTreatment(difficultyModifier);
            }
        }
        public Double MilliSecondsPerLevel
        {
            get
            {
                Double difficultyModifier = 0;
                return GlobalValues.MilliSecondsPerLevel(difficultyModifier);
            }
        }
        public Double MinutesPerLevel
        {
            get
            {
                Double difficultyModifier = 0;
                return GlobalValues.MinutesPerLevel(difficultyModifier);
            }
        }



        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = this;

            SetupSettingsWindow();
            //treatmentDataGridView.ItemsSource = null;
            //treatmentDataGridView.ItemsSource = TreatmentList; //Preferably do this somewhere else, not in the add method.
            //treatmentDataGridView.Items.Refresh();

            //add items
            TreatmentList.Add(new Treatment("test1", 5, 10, 20, false, true));
            TreatmentList.Add(new Treatment("test2", 5, 15, 10, true, false));
            TreatmentList.Add(new Treatment("test3", 5, 10, 30, false, true));

            //set the itemssource
            treatmentDataGridView.ItemsSource = this.TreatmentList;
        }

        private void SetupSettingsWindow()
        {
            //Set categories for the treatmentRoomList
            #region SetupTreatmentTab
            treatmentRoomList.Items.Clear();
            foreach (String roomName in Globals.roomCategories)
            {
                treatmentRoomList.Items.Add(roomName);

                if (!treatmentCategoriesDict.ContainsKey(roomName))
                {
                    treatmentCategoriesDict.Add(roomName, new List<Treatment> { });
                }

            }
            treatmentRoomList.SelectedIndex = 0;
            #endregion

            #region SetupBalancingTab
            balancingRoomList.Items.Clear();
            foreach (String roomName in Globals.roomCategories)
            {
                balancingRoomList.Items.Add(roomName);

                if (!balancingCategoriesDict.ContainsKey(roomName))
                {
                    balancingCategoriesDict.Add(roomName, new List<String> { });
                }

            }
            balancingRoomList.SelectedIndex = 0;
            #endregion

        }

        private void SendSaveData()
        {   //Connect all the stored settings in this window with the SettingsObject
            Globals.GetSettings.projectPathData = ProjectDirectoryPathValue;

            Globals.GetSettings.SetBalancingCategories(balancingCategoriesDict);

            Globals.GetSettings.SaveSettings();
        }

        #region StoreData
        /*
        private void StorePatientTypeCategory()
        {
            String categoryKey = patientTypeRoomList.Items[lastLoadedPatientTypeCategoriesIndex].ToString();
            List<String> checkedPatientTypes = new List<String> { };


            foreach (CheckedListBox checkList in new List<CheckedListBox> { patientTypeMaleCheckList, patientTypeFemaleCheckList, patientTypeOtherCheckList })
            {
                foreach (object itemChecked in checkList.CheckedItems)
                {
                    checkedPatientTypes.Add(itemChecked.ToString());
                }

            }

            if (patientTypeCategoriesDict.ContainsKey(categoryKey))
            {
                patientTypeCategoriesDict[categoryKey] = checkedPatientTypes;
            }
            else
            {
                Console.WriteLine("ERROR: windowSettings.StorePatientTypeCategory, patientTypeCategoriesDict does not contain key: " + categoryKey);
            }
        }

        private void StoreTreatmentCategory()
        {
            String categoryKey = treatmentRoomList.Items[lastLoadedTreatmentCategoriesIndex].ToString();
            List<Treatment> treatmentDataRows = new List<Treatment> { };

            foreach (DataGridViewRow Datarow in treatmentDataGridView.Rows)
            {
                //Check if TreatmentName has been filled in otherwise discard
                if (Datarow.Cells[1].Value != null && Datarow.Cells[1].Value.ToString() != "")
                {
                    treatmentDataRows.Add(new Treatment(Datarow));
                }
            }

            treatmentCategoriesDict[categoryKey] = treatmentDataRows;
        }
         */
        private void StoreBalancingCategory()
        {
            if (GetPreviousBalancingCategoryKey != null)
            {
                List<String> difficultyModifiers = new List<String> { };
                if (balancingCategoriesDict.ContainsKey(GetPreviousBalancingCategoryKey))
                {

                    foreach (String difficultyModifier in DifficultyModifierList)
                    {
                        difficultyModifiers.Add(difficultyModifier);
                    }

                    if (balancingCategoriesDict.ContainsKey(GetPreviousBalancingCategoryKey))
                    {
                        balancingCategoriesDict[GetPreviousBalancingCategoryKey] = difficultyModifiers;
                    }
                    
                }
                else
                {
                    Console.WriteLine("ERROR: settingsWindow.StoreBalancingCategory, balancingCategoriesDict does not contain key: " + GetPreviousBalancingCategoryKey);
                }
            }

        }
      
        #endregion

        #region LoadData
        /*
        private void LoadPatientTypeCategory()
        {
            if (patientTypeRoomList.SelectedIndex > -1)
            {
                String categoryKey = patientTypeRoomList.Items[patientTypeRoomList.SelectedIndex].ToString();
                List<String> patientTypeList = patientTypeCategoriesDict[categoryKey];
                foreach (CheckedListBox checkList in new List<CheckedListBox> { patientTypeMaleCheckList, patientTypeFemaleCheckList, patientTypeOtherCheckList })
                {
                    for (int i = 0; i < checkList.Items.Count; i++)
                    {
                        //Check if the patientType occurs
                        checkList.SetItemChecked(i, patientTypeList.Contains(checkList.Items[i].ToString()));
                    }
                }
                lastLoadedPatientTypeCategoriesIndex = patientTypeRoomList.SelectedIndex;
            }
        }
        */
        private void LoadTreatmentCategory(String categoryKey = null)
        {
            if (treatmentRoomList.SelectedIndex > -1)
            {
                if (categoryKey != null)
                {
                    categoryKey = treatmentRoomList.Items[treatmentRoomList.SelectedIndex].ToString();
                }
                List<Treatment> treatmentDataRows = treatmentCategoriesDict[categoryKey];

                LoadedTreatmentList.Clear();
                foreach (Treatment treatment in treatmentDataRows)
                {
                    LoadedTreatmentList.Add(treatment);
                }

                lastLoadedTreatmentCategoriesIndex = treatmentRoomList.SelectedIndex;
            }
        }
       
        private void LoadBalancingCategory()
        {
            if (balancingRoomList.SelectedIndex > -1 && balancingRoomList.SelectedIndex != lastLoadedBalancingCategoriesIndex)
            {
                //Clear and Load the list in the Difficulty Modifier
                DifficultyModifierList.Clear();
                foreach (String difficultyModifier in balancingCategoriesDict[GetCurrentBalancingCategoryKey])
                {
                    DifficultyModifierList.Add(difficultyModifier);
                }
                lastLoadedBalancingCategoriesIndex = balancingRoomList.SelectedIndex;
            }

        } 
        /*
        private void LoadDifficultyModifierData(String difficultyModifier)
        {

            Dictionary<String, Double> balancingData = Globals.GameValue.GetBalancingData(difficultyModifier);

            averageEntryTimePerPatientValue.Text = balancingData["averageEntryTimePerPatient"].ToString();
            timeBetweenPatientsValue.Text = balancingData["timeBetweenPatients"].ToString();
            numberOfPatientsValue.Text = balancingData["numberOfPatients"].ToString();
            treatmentPerPatientValue.Text = balancingData["treatmentPerPatient"].ToString();
            timePerTreatmentValue.Text = balancingData["timePerTreatment"].ToString();
            milliSecondsPerLevelValue.Text = balancingData["milliSecondsPerLevel"].ToString();
            minutesPerLevelValue.Text = String.Format("{0:0.0000000}", balancingData["minutesPerLevel"]);

        }
                */

        #endregion

        #region Signals
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

        private void treatmentRowButtonAdd_Click(object sender, RoutedEventArgs e)
        {

            List<Treatment> tmpList = new List<Treatment> { };
            tmpList.Add(new Treatment("test1", 5, 10, 20, false, true));
            tmpList.Add(new Treatment("test2", 5, 15, 10, true, false));
            tmpList.Add(new Treatment("test3", 5, 10, 30, false, true));

            treatmentCategoriesDict["Room 1"] = tmpList;

            treatmentDataGridView.Items.Refresh();
        }

        private void treatmentRoomList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(treatmentRoomList, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                LoadTreatmentCategory(item.ToString());
            }
        }

        private void balancingRoomList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(balancingRoomList, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                StoreBalancingCategory();
                LoadBalancingCategory();

            }
        }

        private void diffModifierRowButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Double newValue = (Double)diffModifierValue.Value;
            if (!DifficultyModifierList.Contains(newValue.ToString("0.##")))
            {
                DifficultyModifierList.Add(newValue.ToString("0.##"));
            }
        }


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

        #region Events
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
