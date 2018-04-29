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
        private Dictionary<String, List<String>> balancingCategoriesDict = Globals.GetSettings.GetBalancingCategories();  // Room[N] -> List with double difficulty Modifiers


        

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
        }

        private void SendSaveData()
        {   //Connect all the stored settings in this window with the SettingsObject
            Globals.GetSettings.projectPathData = ProjectDirectoryPathValue;

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

        private void StoreBalancingCategory()
        {
            if (lastLoadedBalancingCategoriesIndex > -1)
            {
                String categoryKey = balancingRoomList.Items[lastLoadedBalancingCategoriesIndex].ToString();
                List<String> difficultyModifiers = new List<String> { };
                if (balancingCategoriesDict.ContainsKey(categoryKey))
                {

                    foreach (object difficultyModifier in difficultyModifierList.Items)
                    {
                        difficultyModifiers.Add(difficultyModifier.ToString());
                    }

                    balancingCategoriesDict[categoryKey] = difficultyModifiers;
                }
                else
                {
                    Console.WriteLine("ERROR: windowSettings.StoreBalancingCategory, balancingCategoriesDict does not contain key: " + categoryKey);
                }
            }

        }
        */
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
        /*
        private void LoadBalancingCategory()
        {
            if (balancingRoomList.SelectedIndex > -1)
            {
                String categoryKey = balancingRoomList.Items[balancingRoomList.SelectedIndex].ToString();
                List<String> difficultyModifiers = balancingCategoriesDict[categoryKey];

                difficultyModifierList.Items.Clear();
                foreach (String difficultyModifier in difficultyModifiers)
                {
                    difficultyModifierList.Items.Add(difficultyModifier.ToString());
                }
                if (difficultyModifierList.Items.Count > 0)
                {
                    difficultyModifierList.SelectedIndex = 0;
                }
                else
                {
                    ClearDifficultyModifierData();
                }
                lastLoadedBalancingCategoriesIndex = balancingRoomList.SelectedIndex;
            }


        }
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
    }
}
