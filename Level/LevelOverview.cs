//--------------------------------------------------------------------------------------------------------------------
// <copyright file="LevelOverview.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
//
//--------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Level
{
    using DataNameSpace;
    using LevelData;
    using NaturalSort.Extension;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class LevelOverview : INotifyPropertyChanged
    {
        #region Fields

        private bool _allowLevelScriptUpdate = true;
        private Level _currentLevelLoaded = null;
        private int _maxTreatmentsVisible = 5;
        private bool _showAvailableTreatmentsCheck = false;
        private List<String> levelList = new List<String> { };
        private Dictionary<string, Level> levelObjectData = new Dictionary<string, Level>();

        #region LevelGeneratingFields
        private bool _useRandomRecommendations = true;
        private bool _generatePatientTypeCheck = true;
        private int _generatePatientTypeMax = 0;
        private int _generatePatientTypeMin = 0;
        private bool _generatePatientsCheck = true;
        private int _generatePatientsMax = 0;
        private int _generatePatientsMin = 0;
        private bool _generatePatientDelayCheck = true;
        private int _generatePatientDelayMax = 1000;
        private int _generatePatientDelayMin = -1000;
        private bool _generateTreatmentsCheck = true;
        private int _generateTreatmentsMax = 0;
        private int _generateTreatmentsMin = 0;


        #endregion



        #endregion

        #region Constructors

        public LevelOverview()
        {
        }

        #endregion

        #region Properties

        #region Public

        public bool AllowLevelScriptUpdate
        {
            get => this._allowLevelScriptUpdate;
            set
            {
                this._allowLevelScriptUpdate = value;
                this.GetLevelLoaded.UpdateLevelOutput();
            }
        }

        public ObservableCollection<String> DifficultyModifierList
        {
            get
            {
                if (this.GetLevelLoaded != null)
                {
                    String categoryKey = Globals.GetCategoryKey(this.GetLevelLoaded.GetRoomIndex);
                    if (categoryKey != String.Empty)
                    {
                        return new ObservableCollection<String>(Globals.GetSettings.GetDifficultyModifierList(categoryKey));
                    }
                }
                return new ObservableCollection<String> { };
            }
        }

        public Level GetLevelLoaded
        {
            get
            {
                return this._currentLevelLoaded;
            }
            set
            {
                if (value != this._currentLevelLoaded)
                {
                    this._currentLevelLoaded = value;
                    this.OnPropertyChanged("GetLevelLoaded");
                    this.OnPropertyChanged("LevelOverviewActive");
                    this.OnPropertyChanged("DifficultyModifierList");
                    this.UpdateRandomRecommendations();
                    this.UpdatePatientSimulator();
                }
            }
        }

        public bool LevelOverviewActive
        {
            get
            {
                if (this.GetLevelLoaded != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int MaxTreatmentsVisible
        {
            get
            {
                return this._maxTreatmentsVisible;
            }
            set
            {
                this._maxTreatmentsVisible = value;
                this.OnPropertyChanged("MaxTreatmentsVisible");
                this.GetLevelLoaded.UpdateMaxTreatments(value);
            }
        }

        public bool ShowAvailableTreatmentsCheck
        {
            get
            {
                return this._showAvailableTreatmentsCheck;
            }
            set
            {
                this._showAvailableTreatmentsCheck = value;
                this.OnPropertyChanged("ShowAvailableTreatmentsCheck");
                this.OnPropertyChanged("GetTreatmentsAvailableString");
            }
        }

        #endregion

        #endregion

        #region LevelGeneratingFields
        public bool UseRandomRecommendations
        {
            get
            {
                return this._useRandomRecommendations;
            }
            set
            {
                this._useRandomRecommendations = value;
                this.OnPropertyChanged("UseRandomRecommendations");
                this.UpdateRandomRecommendations();
            }
        }

        #region PatientType
        public bool GeneratePatientTypeCheck
        {
            get
            {
                return this._generatePatientTypeCheck;
            }
            set
            {
                this._generatePatientTypeCheck = value;
                this.OnPropertyChanged("GeneratePatientTypeCheck");
            }
        }

        public int GeneratePatientTypeMax
        {
            get
            {
                return this._generatePatientTypeMax;
            }
            set
            {
                this._generatePatientTypeMax = value;
                this.OnPropertyChanged("GeneratePatientTypeMax");
            }
        }

        public int GeneratePatientTypeMin
        {
            get
            {
                return this._generatePatientTypeMin;
            }
            set
            {
                this._generatePatientTypeMin = value;
                this.OnPropertyChanged("GeneratePatientTypeMin");
            }
        }

        #endregion

        #region Patients
        public bool GeneratePatientsCheck
        {
            get
            {
                return this._generatePatientsCheck;
            }
            set
            {
                this._generatePatientsCheck = value;
                this.OnPropertyChanged("GeneratePatientsCheck");
            }
        }

        public int GeneratePatientsMax
        {
            get
            {
                return this._generatePatientsMax;
            }
            set
            {
                this._generatePatientsMax = value;
                this.OnPropertyChanged("GeneratePatientsMax");
            }
        }

        public int GeneratePatientsMin
        {
            get
            {
                return this._generatePatientsMin;
            }
            set
            {
                this._generatePatientsMin = value;
                this.OnPropertyChanged("GeneratePatientsMin");
            }
        }

        #endregion

        #region PatientDelay
        public bool GeneratePatientDelayCheck
        {
            get
            {
                return this._generatePatientDelayCheck;
            }
            set
            {
                this._generatePatientDelayCheck = value;
                this.OnPropertyChanged("GeneratePatientDelayCheck");
            }
        }

        public int GeneratePatientDelayMax
        {
            get
            {
                return this._generatePatientDelayMax;
            }
            set
            {
                this._generatePatientDelayMax = value;
                this.OnPropertyChanged("GeneratePatientDelayMax");
            }
        }

        public int GeneratePatientDelayMin
        {
            get
            {
                return this._generatePatientDelayMin;
            }
            set
            {
                this._generatePatientDelayMin = value;
                this.OnPropertyChanged("GeneratePatientDelayMin");
            }
        }

        #endregion

        #region Treatment
        public bool GenerateTreatmentsCheck
        {
            get
            {
                return this._generateTreatmentsCheck;
            }
            set
            {
                this._generateTreatmentsCheck = value;
                this.OnPropertyChanged("GenerateTreatmentsCheck");
            }
        }

        public int GenerateTreatmentsMax
        {
            get
            {
                return this._generateTreatmentsMax;
            }
            set
            {
                this._generateTreatmentsMax = value;
                this.OnPropertyChanged("GenerateTreatmentsMax");
            }
        }

        public int GenerateTreatmentsMin
        {
            get
            {
                return this._generateTreatmentsMin;
            }
            set
            {
                this._generateTreatmentsMin = value;
                this.OnPropertyChanged("GenerateTreatmentsMin");
            }
        }

        #endregion

        #endregion

        #region LevelManagement

        public void AddPatientToLoadedLevel()
        {
            if (this.GetLevelLoaded != null)
            {
                this.GetLevelLoaded.AddPatient();
            }
        }

        public Dictionary<String, List<String>> GetCategorizedFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            Dictionary<String, List<String>> CategorizedFilterdLevels = new Dictionary<String, List<String>> { };
            List<String> FilteredLevelList = this.GetFilteredLevels(roomIndex, storyLevels, bonusLevels, unknownLevels);

            if (roomIndex == 0)
            {
                if (storyLevels)
                {
                    CategorizedFilterdLevels.Add("Story", this.GetFilteredLevels(0, true, false, false));
                }

                if (bonusLevels)
                {
                    for (int i = 1; i <= Globals.roomCategories.Count; i++)
                    {
                        CategorizedFilterdLevels.Add(Globals.roomCategories[i - 1], this.GetFilteredLevels(i, false, true, false));
                    }
                }
            }
            else
            {
                if (storyLevels)
                {
                    CategorizedFilterdLevels.Add("Story", this.GetFilteredLevels(roomIndex, true, false, false));
                }

                if (bonusLevels)
                {
                    CategorizedFilterdLevels.Add(Globals.roomCategories[roomIndex - 1], this.GetFilteredLevels(roomIndex, false, true, false));
                }
            }
            //Add all the uncategorized rooms
            if (unknownLevels)
            {
                CategorizedFilterdLevels.Add("Unknown", this.GetFilteredLevels(roomIndex, false, false, true));
            }

            return CategorizedFilterdLevels;
        }

        public List<String> GetFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            List<String> rawLevelList = this.GetLevelsFromDisk(false, true);
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

        public Level GetLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            if (!this.LevelExist(levelName))
            {
                this.AddLevelByName(levelName);
            }
            return this.levelObjectData[levelName];
        }

        public void LoadLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            if (this.LevelExist(levelName))
            {
                this.GetLevelLoaded = this.levelObjectData[levelName];
            }
            else
            {
                this.GetLevelLoaded = this.AddLevelByName(levelName);
            }
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

        private Level AddLevelByName(String levelName)
        {
            levelName = CleanLevelName(levelName);
            if (this.LevelExist(levelName))
            {
                this.levelObjectData.Remove(levelName);
            }

            this.levelObjectData.Add(levelName, this.CreateLevel(levelName));
            return this.levelObjectData[levelName];
        }

        private Level CreateLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            Level newLevel = new Level(levelName);

            return newLevel;
        }

        private bool LevelExist(String levelName)
        {
            levelName = CleanLevelName(levelName);

            return this.levelObjectData.ContainsKey(levelName);
        }

        #endregion

        #region GenerateLevel

        public void RandomizeLevel()
        {
            if (this.GetLevelLoaded != null)
            {
                if (this.GeneratePatientTypeCheck)
                {
                    foreach (PatientChance patientChance in this.GetLevelLoaded.PatientChanceCollection)
                    {
                        patientChance.RandomizeWeight(Globals.GetRandom.Next(1, 100));
                    }
                }

                if (this.GeneratePatientsCheck)
                {
                    int patientAmount = Globals.GetRandom.Next(Math.Min(this.GeneratePatientsMin, this.GeneratePatientsMax), this.GeneratePatientsMax);
                    this.GetLevelLoaded.SetPatientAmount(patientAmount);
                }

                if (this.GenerateTreatmentsCheck)
                {
                    this.GetLevelLoaded.RandomizeTreatments(Math.Min(this.GenerateTreatmentsMin, this.GenerateTreatmentsMax), this.GenerateTreatmentsMax);
                }

                if (this.GeneratePatientDelayCheck)
                {
                    this.GetLevelLoaded.RandomizeDelay();
                }

                this.GetLevelLoaded.UpdateLevelOutput();
            }
        }

        internal void UpdateRandomRecommendations()
        {
            if (this.UseRandomRecommendations)
            {
                int numberOfPatients = Globals.GetGameValues.NumberOfPatientsToInt(this.GetLevelLoaded.GetDifficultyModifier);
                int treatmentPerPatients = Globals.GetGameValues.TreatmentPerPatientToInt(this.GetLevelLoaded.GetDifficultyModifier);
                int patientTypeCount = Globals.GetSettings.GetPatientChanceList(this.GetLevelLoaded.CategoryKey).Count;

                this.GeneratePatientTypeMin = patientTypeCount;
                this.GeneratePatientTypeMax = patientTypeCount;

                this.GeneratePatientsMin = numberOfPatients - 1;
                this.GeneratePatientsMax = numberOfPatients + 1;

                this.GenerateTreatmentsMin = treatmentPerPatients - 1;
                this.GenerateTreatmentsMax = treatmentPerPatients + 1;
            }
        }

        #endregion

        #region PatientSimulator

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
                treatmentColumn.ItemsSource = this.GetLevelLoaded.AvailableTreatmentStringList;
                treatmentColumn.Width = 120;
                SimulatorGrid.Columns.Add(treatmentColumn);
            }

            for (int patientIndex = 0; patientIndex < this.GetLevelLoaded.PatientCollection.Count; patientIndex++)
            {
                Patient patient = this.GetLevelLoaded.PatientCollection[patientIndex];
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

        #endregion

        #region ReadWrite

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

        public List<String> GetLevelsFromDisk(bool reload = false, bool filterExtension = false)
        {
            if (this.levelList == null || this.levelList.Count() == 0 || reload)
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

                    this.levelList = tmpLevelList;
                }
            }

            return this.levelList;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}