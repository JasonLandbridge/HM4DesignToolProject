using DataNameSpace;
using NaturalSort.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace LevelData
{
    public class LevelOverview : INotifyPropertyChanged
    {
        private Dictionary<string, Level> levelObjectData = new Dictionary<string, Level>();
        private Level _currentLevelLoaded = null;

        public Level GetLevelLoaded
        {
            get
            {
                return _currentLevelLoaded;
            }
            set
            {
                if (value != _currentLevelLoaded)
                {
                    _currentLevelLoaded = value;
                    OnPropertyChanged("GetLevelLoaded");
                    OnPropertyChanged("LevelOverviewActive");
                    OnPropertyChanged("DifficultyModifierList");
                    UpdateRandomRecommendations();
                    UpdatePatientSimulator();
                }
            }
        }

        public bool LevelOverviewActive
        {
            get
            {
                if (GetLevelLoaded != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private int _maxTreatmentsVisible = 5;
        public int MaxTreatmentsVisible
        {
            get
            {
                return _maxTreatmentsVisible;
            }
            set
            {
                _maxTreatmentsVisible = value;
                OnPropertyChanged("MaxTreatmentsVisible");
                GetLevelLoaded.UpdateMaxTreatments(value);
            }
        }

        private List<String> levelList = new List<String> { };

        public ObservableCollection<String> DifficultyModifierList
        {
            get
            {
                if (GetLevelLoaded != null)
                {
                    String categoryKey = Globals.GetCategoryKey(GetLevelLoaded.GetRoomIndex);
                    if (categoryKey != String.Empty)
                    {
                        return new ObservableCollection<String>(Globals.GetSettings.GetDifficultyModifierList(categoryKey));
                    }
                }
                return new ObservableCollection<String> { };
            }
        }

        private bool _showAvailableTreatmentsCheck = false;
        public bool ShowAvailableTreatmentsCheck
        {
            get
            {
                return _showAvailableTreatmentsCheck;
            }
            set
            {
                _showAvailableTreatmentsCheck = value;
                OnPropertyChanged("ShowAvailableTreatmentsCheck");
                OnPropertyChanged("GetTreatmentsAvailableString");
            }
        }

        #region RandomProperties
        private bool _useRandomRecommendations = true;
        public bool UseRandomRecommendations
        {
            get
            {
                return _useRandomRecommendations;
            }
            set
            {
                _useRandomRecommendations = value;
                OnPropertyChanged("UseRandomRecommendations");
                UpdateRandomRecommendations();
            }
        }

        #region PatientType
        private bool _generatePatientTypeCheck = true;
        public bool GeneratePatientTypeCheck
        {
            get
            {
                return _generatePatientTypeCheck;
            }
            set
            {
                _generatePatientTypeCheck = value;
                OnPropertyChanged("GeneratePatientTypeCheck");
            }
        }

        private int _generatePatientTypeMin = 0;
        public int GeneratePatientTypeMin
        {
            get
            {
                return _generatePatientTypeMin;
            }
            set
            {
                _generatePatientTypeMin = value;
                OnPropertyChanged("GeneratePatientTypeMin");
            }
        }

        private int _generatePatientTypeMax = 0;
        public int GeneratePatientTypeMax
        {
            get
            {
                return _generatePatientTypeMax;
            }
            set
            {
                _generatePatientTypeMax = value;
                OnPropertyChanged("GeneratePatientTypeMax");
            }
        }
        #endregion

        #region Patients
        private bool _generatePatientsCheck = true;
        public bool GeneratePatientsCheck
        {
            get
            {
                return _generatePatientsCheck;
            }
            set
            {
                _generatePatientsCheck = value;
                OnPropertyChanged("GeneratePatientsCheck");
            }
        }

        private int _generatePatientsMin = 0;
        public int GeneratePatientsMin
        {
            get
            {
                return _generatePatientsMin;
            }
            set
            {
                _generatePatientsMin = value;
                OnPropertyChanged("GeneratePatientsMin");
            }
        }

        private int _generatePatientsMax = 0;
        public int GeneratePatientsMax
        {
            get
            {
                return _generatePatientsMax;
            }
            set
            {
                _generatePatientsMax = value;
                OnPropertyChanged("GeneratePatientsMax");
            }
        }
        #endregion

        #region PatientDelay
        private bool _generatePatientDelayCheck = true;
        public bool GeneratePatientDelayCheck
        {
            get
            {
                return _generatePatientDelayCheck;
            }
            set
            {
                _generatePatientDelayCheck = value;
                OnPropertyChanged("GeneratePatientDelayCheck");
            }
        }

        private int _generatePatientDelayMin = -1000;
        public int GeneratePatientDelayMin
        {
            get
            {
                return _generatePatientDelayMin;
            }
            set
            {
                _generatePatientDelayMin = value;
                OnPropertyChanged("GeneratePatientDelayMin");
            }
        }

        private int _generatePatientDelayMax = 1000;
        public int GeneratePatientDelayMax
        {
            get
            {
                return _generatePatientDelayMax;
            }
            set
            {
                _generatePatientDelayMax = value;
                OnPropertyChanged("GeneratePatientDelayMax");
            }
        }
        #endregion

        #region Treatment
        private bool _generateTreatmentsCheck = true;
        public bool GenerateTreatmentsCheck
        {
            get
            {
                return _generateTreatmentsCheck;
            }
            set
            {
                _generateTreatmentsCheck = value;
                OnPropertyChanged("GenerateTreatmentsCheck");
            }
        }

        private int _generateTreatmentsMin = 0;
        public int GenerateTreatmentsMin
        {
            get
            {
                return _generateTreatmentsMin;
            }
            set
            {
                _generateTreatmentsMin = value;
                OnPropertyChanged("GenerateTreatmentsMin");
            }
        }

        private int _generateTreatmentsMax = 0;
        public int GenerateTreatmentsMax
        {
            get
            {
                return _generateTreatmentsMax;
            }
            set
            {
                _generateTreatmentsMax = value;
                OnPropertyChanged("GenerateTreatmentsMax");
            }
        }

        #endregion
        #endregion


        public LevelOverview()
        {
        }

        public static String ReadLevelTextFromFile(String levelName)
        {
            levelName = CleanLevelName(levelName);
            if (!levelName.EndsWith(".lua"))
            {
                levelName += ".lua";
            }

            String levelPath = Globals.GetSettings.projectPathLevel + levelName;

            if (File.Exists(levelPath))
            {
                string readContents;
                using (StreamReader streamReader = new StreamReader(levelPath, Encoding.UTF8))
                {
                    readContents = streamReader.ReadToEnd();
                    return readContents;
                }
            }
            else
            {
                Console.WriteLine("ERROR: Data.ReadLevelText, Could not find " + levelPath + "!");
                return null;
            }
        }

        public List<String> GetFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            List<String> rawLevelList = GetLevelsFromDisk(false, true);
            List<String> outputLevelList = new List<String> { };

            foreach (String level in rawLevelList)
            {
                if (storyLevels && level.StartsWith("level"))
                {
                    int levelIndex = Convert.ToInt16(level.Replace("level", ""));
                    int minIndex = (roomIndex - 1) * 10 + 1;
                    int maxIndex = (roomIndex * 10) + 1;

                    if (roomIndex == 0)
                    {
                        outputLevelList.Add(level);
                    }
                    else if (Enumerable.Range(minIndex, 10).Contains(levelIndex))
                    {
                        outputLevelList.Add(level);
                    }
                }
                // Ensure that the second character in the level name is a number as well to be in line with naming conventions.
                else if (bonusLevels && level.StartsWith("r") && int.TryParse(level[1].ToString(), out int n) && level[2].ToString() == "_")
                {
                    if (roomIndex == 0)
                    {
                        outputLevelList.Add(level);
                    }
                    else if (level.StartsWith("r" + roomIndex.ToString() + "_"))
                    {
                        outputLevelList.Add(level);
                    }
                }
                else if (unknownLevels && !level.StartsWith("level") && !(level.StartsWith("r") && int.TryParse(level[1].ToString(), out int j) && level[2].ToString() == "_"))
                {
                    outputLevelList.Add(level);
                }
            }
            //Naturaly sort the levelList
            //TODO Find more efficient method of natural sorting without first converting to sequence and back
            IEnumerable<String> LevelSequence = outputLevelList.OrderBy(x => x, StringComparer.OrdinalIgnoreCase.WithNaturalSort());
            return LevelSequence.ToList();
        }

        public Dictionary<String, List<String>> GetCategorizedFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            Dictionary<String, List<String>> CategorizedFilterdLevels = new Dictionary<String, List<String>> { };
            List<String> FilteredLevelList = GetFilteredLevels(roomIndex, storyLevels, bonusLevels, unknownLevels);

            if (roomIndex == 0)
            {
                if (storyLevels)
                {
                    CategorizedFilterdLevels.Add("Story", GetFilteredLevels(0, true, false, false));
                }

                if (bonusLevels)
                {
                    for (int i = 1; i <= Globals.roomCategories.Count; i++)
                    {
                        CategorizedFilterdLevels.Add(Globals.roomCategories[i - 1], GetFilteredLevels(i, false, true, false));
                    }
                }
            }
            else
            {
                if (storyLevels)
                {
                    CategorizedFilterdLevels.Add("Story", GetFilteredLevels(roomIndex, true, false, false));
                }

                if (bonusLevels)
                {
                    CategorizedFilterdLevels.Add(Globals.roomCategories[roomIndex - 1], GetFilteredLevels(roomIndex, false, true, false));
                }
            }
            //Add all the uncategorized rooms
            if (unknownLevels)
            {
                CategorizedFilterdLevels.Add("Unknown", GetFilteredLevels(roomIndex, false, false, true));
            }

            return CategorizedFilterdLevels;
        }

        public void LoadLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            if (LevelExist(levelName))
            {
                GetLevelLoaded = levelObjectData[levelName];
            }
            else
            {
                GetLevelLoaded = AddLevelByName(levelName);
            }
        }

        public Level GetLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            if (!LevelExist(levelName))
            {
                AddLevelByName(levelName);
            }
            return levelObjectData[levelName];
        }

        private Level CreateLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            Level newLevel = new Level(levelName);

            return newLevel;
        }

        private Level AddLevelByName(String levelName)
        {
            levelName = CleanLevelName(levelName);
            if (LevelExist(levelName))
            {
                levelObjectData.Remove(levelName);
            }

            levelObjectData.Add(levelName, CreateLevel(levelName));
            return levelObjectData[levelName];
        }

        public void AddPatientToLoadedLevel()
        {
            if (GetLevelLoaded != null)
            {
                GetLevelLoaded.AddPatient();
            }
        }

        private bool LevelExist(String levelName)
        {
            levelName = CleanLevelName(levelName);

            return levelObjectData.ContainsKey(levelName);
        }

        private static String CleanLevelName(String levelName)
        {
            levelName = levelName.Replace(" ", "");
            if (levelName.EndsWith(".lua"))
            {
                levelName = levelName.Replace(".lua", "");
            }
            if (levelName.Contains("(e)"))
            {
                levelName = levelName.Replace("(e)", "");
            }
            if (levelName.Contains("*"))
            {
                levelName = levelName.Replace("*", "");
            }

            return levelName;
        }


        internal void UpdateRandomRecommendations()
        {

            if (UseRandomRecommendations)
            {
                int numberOfPatients = Globals.GetGameValues.NumberOfPatientsToInt(GetLevelLoaded.GetDifficultyModifier);
                int treatmentPerPatients = Globals.GetGameValues.TreatmentPerPatientToInt(GetLevelLoaded.GetDifficultyModifier);
                int patientTypeCount = Globals.GetSettings.GetPatientChanceList(GetLevelLoaded.CategoryKey).Count;

                GeneratePatientTypeMin = patientTypeCount;
                GeneratePatientTypeMax = patientTypeCount;

                GeneratePatientsMin = numberOfPatients - 1;
                GeneratePatientsMax = numberOfPatients + 1;

                GenerateTreatmentsMin = treatmentPerPatients - 1;
                GenerateTreatmentsMax = treatmentPerPatients + 1;




            }


        }


        public void RandomizeLevel()
        {
            if (GetLevelLoaded != null)
            {
                Random rnd = new Random();

                if (GeneratePatientTypeCheck)
                {

                    foreach (PatientChance patientChance in GetLevelLoaded.PatientChanceCollection)
                    {
                        patientChance.RandomizeWeight(rnd.Next(1, 100));
                    }

                }


                if (GeneratePatientsCheck)
                {
                    int patientAmount = rnd.Next(GeneratePatientsMin, GeneratePatientsMax);
                    GetLevelLoaded.SetPatientAmount(patientAmount);
                }

                if (GenerateTreatmentsCheck)
                {
                    GetLevelLoaded.RandomizeTreatments(GenerateTreatmentsMin, GenerateTreatmentsMax);
                }

                GetLevelLoaded.UpdateLevelOutput();

            }
        }

        private void UpdatePatientSimulator()
        {

            //A DataTable is first created where an indentation is inserted to mimic a delay

            DataTable treatmentDataTable = new DataTable();
            treatmentDataTable.Columns.Add("PatientName");





            DataGrid SimulatorGrid = Globals.GetMainWindow.patientSimulatorGrid;
            //            SimulatorGrid.ItemsSource = new Binding("treatmentDataTable");    //GetLevelLoaded.PatientCollection;

            //Create first column for the PatientName


            DataGridTextColumn patientNameColumn =
                new DataGridTextColumn
                {
                    Header = "Patient Name:",
                    Binding = new Binding(treatmentDataTable.Columns[0].ToString())
                };
            SimulatorGrid.Columns.Add(patientNameColumn);

            // Create the columns
            for (int i = 0; i < 40; i++)
            {
                // Add treatment column to the treatmentDataTable
                treatmentDataTable.Columns.Add(i.ToString());

                String ColumnName = (i * 5).ToString() + " Seconds";
                DataGridComboBoxColumn treatmentColumn = new DataGridComboBoxColumn();
                treatmentColumn.Header = ColumnName;
                treatmentColumn.SelectedValueBinding = new Binding(treatmentDataTable.Columns[i + 1].ToString());
                treatmentColumn.ItemsSource = GetLevelLoaded.GetTreatmentOptions;
                treatmentColumn.Width = 120;
                SimulatorGrid.Columns.Add(treatmentColumn);


            }

            for (int patientIndex = 0; patientIndex < GetLevelLoaded.PatientCollection.Count; patientIndex++)
            {
                Patient patient = GetLevelLoaded.PatientCollection[patientIndex];
                DataRow patientDataRow = treatmentDataTable.NewRow();

                patientDataRow["PatientName"] = patient.PatientName;

                for (int i = 0; i < 15; i++)
                {
                    if (i < patient.TreatmentCollection.Count)
                    {
                        patientDataRow[patientIndex + i + 1] = patient.TreatmentCollection[i].TreatmentName;
                    }
                }
                treatmentDataTable.Rows.Add(patientDataRow);
            }


            // Set a DataGrid control's DataContext to the DataView.
            SimulatorGrid.ItemsSource = treatmentDataTable.DefaultView;

            // Globals.GetMainWindow.patientSimulatorGrid = SimulatorGrid;

        }
        #region Getters

        public List<String> GetLevelsFromDisk(bool reload = false, bool filterExtension = false)
        {
            if (levelList == null || levelList.Count() == 0 || reload)
            {
                String projectPath = Globals.GetSettings.projectPathLevel;
                if (Directory.Exists(projectPath))
                {
                    List<String> rawLevelList = new List<String>(System.IO.Directory.GetFiles(projectPath));
                    List<String> tmpLevelList = new List<String> { };

                    foreach (String level in rawLevelList)
                    {
                        String levelName = Path.GetFileName(level);
                        if (filterExtension)
                        {
                            tmpLevelList.Add(levelName.Replace(".lua", ""));
                        }
                        else
                        {
                            tmpLevelList.Add(levelName);
                        }
                    }

                    levelList = tmpLevelList;
                }
            }

            return levelList;
        }

        #endregion Getters

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }

}
