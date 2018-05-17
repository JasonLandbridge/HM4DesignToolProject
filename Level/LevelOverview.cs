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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    using DataNameSpace;

    using HM4DesignTool.Utilities;

    using NaturalSort.Extension;

    /// <inheritdoc />
    /// <summary>
    /// The Level Overview is used provide all the level data to the Front-End and manage all Level objects.
    /// </summary>
    public class LevelOverview : INotifyPropertyChanged
    {
        #region Fields

        #region General
        /// <summary>
        /// true if the LevelOverview has finished loading and setting up.
        /// </summary>
        private bool levelOverviewFinishedLoading;

        /// <summary>
        /// The allow level script to update field.
        /// </summary>
        private bool allowLevelScriptUpdate = true;

        /// <summary>
        /// The current level loaded field.
        /// </summary>
        private Level currentLevelLoaded;

        /// <summary>
        /// The max treatments visible in the Patient Overview.
        /// </summary>
        private int maxTreatmentsVisible = 5;

        /// <summary>
        /// The show available treatments check in the Patient Overview.
        /// </summary>
        private bool showAvailableTreatmentsCheck;

        private int roomIndexValue;

        private double difficultyModifierValue;

        #endregion
        /// <summary>
        /// List of the level names field.
        /// </summary>
        private List<string> levelList = new List<string>();

        /// <summary>
        /// The level object data.
        /// </summary>
        private Dictionary<string, Level> levelObjectData = new Dictionary<string, Level>();

        #region LevelList

        /// <summary>
        /// The level list display collection used to display the list of levels in treeview in the Front-End.
        /// </summary>
        private ObservableCollection<TreeViewItem> levelListDisplayCollection = new ObservableCollection<TreeViewItem>();

        /// <summary>
        /// The LevelListDisplay Room filter selected index field.
        /// </summary>
        private int levelListFilterSelectedIndex;

        /// <summary>
        /// The LevelListDisplay story check field.
        /// </summary>
        private bool levelListStoryCheck = Globals.GetSettings.RoomFilterStoryCheck;

        /// <summary>
        /// The LevelListDisplay bonus check field.
        /// </summary>
        private bool levelListBonusCheck = Globals.GetSettings.RoomFilterBonusCheck;

        /// <summary>
        /// The LevelListDisplay unknown check field.
        /// </summary>
        private bool levelListUnknownCheck = Globals.GetSettings.RoomFilterUnknownCheck;
        #endregion

        #region LevelGeneratingFields

        private bool randomGroupEnabled = true;

        /// <summary>
        /// The use random recommendations field.
        /// </summary>
        private bool useRandomRecommendations = true;

        /// <summary>
        /// Generate the PatientChances field.
        /// </summary>
        private bool generatePatientTypeCheck = true;

        /// <summary>
        /// Generate the PatientChances min value field.
        /// </summary>
        private int generatePatientTypeMin;

        /// <summary>
        /// Generate the PatientChances max value field.
        /// </summary>
        private int generatePatientTypeMax;
        /// <summary>
        /// Generate the amount of patients check field.
        /// </summary>
        private bool generatePatientsCheck = true;

        /// <summary>
        /// Generate the amount of patients min value field.
        /// </summary>
        private int generatePatientsMin = 1;

        /// <summary>
        /// Generate the amount of patients max value field.
        /// </summary>
        private int generatePatientsMax = 1;
        /// <summary>
        /// Generate the delay of each patient field.
        /// </summary>
        private bool generatePatientDelayCheck = true;

        /// <summary>
        /// Generate patient delay max value field.
        /// </summary>
        private int generatePatientDelayMin = -1000;

        /// <summary>
        /// Generate patient delay min value field.
        /// </summary>
        private int generatePatientDelayMax = 1000;
        /// <summary>
        /// Generate the treatments for each patients check field.
        /// </summary>
        private bool generateTreatmentsCheck = true;

        /// <summary>
        /// Generate the treatments for each patients check min value field.
        /// </summary>
        private int generateTreatmentsMin = 1;

        /// <summary>
        /// Generate the treatments for each patients check max value field.
        /// </summary>
        private int generateTreatmentsMax = 1;

        /// <summary>
        /// Generate the weight for each patients check field.
        /// </summary>
        private bool generateWeightCheck = true;

        /// <summary>
        /// Generate the weight for each patients check min value field.
        /// </summary>
        private int generateWeightMin = 10;

        /// <summary>
        /// Generate the weight for each patients check max value field.
        /// </summary>
        private int generateWeightMax = 90;
        #endregion

        #region Commands

        private ICommand reloadLevelCommand;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelOverview"/> class.
        /// </summary>
        public LevelOverview()
        {
        }
        #endregion

        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        #region Properties

        #region Public

        #region General
        public int RoomIndex
        {
            get => this.roomIndexValue;
            set
            {
                this.roomIndexValue = value;
                this.OnPropertyChanged();
                this.RoomIndexUpdated();

            }
        }

        /// <summary>
        /// Gets or sets the get difficulty modifier of the Level.
        /// </summary>
        public double DifficultyModifier
        {
            get
            {
                if (this.GetLevelLoaded != null)
                {
                    return this.GetLevelLoaded.GetDifficultyModifier;
                }
                else
                {
                    return this.difficultyModifierValue;
                }
            }
            set
            {
                if (this.GetLevelLoaded != null)
                {
                    this.GetLevelLoaded.GetDifficultyModifier = value;
                }
                this.difficultyModifierValue = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged("DifficultyModifierString");
                this.DifficultyModifierUpdated();

            }
        }

        /// <summary>
        /// Gets or sets the difficulty modifier string.
        /// </summary>
        public string DifficultyModifierString
        {
            get => this.DifficultyModifier.ToString("0.0");
            set
            {
                if (value != null)
                {
                    this.DifficultyModifier = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether to allow  the level script to update.
        /// </summary>
        public bool AllowLevelScriptUpdate
        {
            get => this.allowLevelScriptUpdate;
            set
            {
                this.allowLevelScriptUpdate = value;
                this.GetLevelLoaded?.UpdateLevelOutput();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the level overview is active.
        /// </summary>
        public bool LevelOverviewActive => this.GetLevelLoaded != null;

        /// <summary>
        /// Gets or sets the current loaded level.
        /// </summary>
        public Level GetLevelLoaded
        {
            get => this.currentLevelLoaded;
            set
            {
                if (value != this.currentLevelLoaded)
                {
                    this.currentLevelLoaded = value;
                    this.OnPropertyChanged();
                    this.CurrentLevelUpdated();
                }
            }
        }

        /// <summary>
        /// Gets or sets the max treatments visible in the PatientOverview
        /// </summary>
        public int MaxTreatmentsVisible
        {
            get => this.maxTreatmentsVisible;

            set
            {
                this.maxTreatmentsVisible = value;
                this.OnPropertyChanged();
                this.GetLevelLoaded.UpdateMaxTreatments(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show available treatments in the General tab
        /// </summary>
        public bool ShowAvailableTreatmentsCheck
        {
            get => this.showAvailableTreatmentsCheck;

            set
            {
                this.showAvailableTreatmentsCheck = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("GetTreatmentsAvailableString");
            }
        }

        #region ItemCollections

        /// <summary>
        /// Gets or sets the level list display collection used to display the list of levels in treeview in the Front-End.
        /// </summary>
        public ObservableCollection<TreeViewItem> LevelListDisplayCollection
        {
            get => this.levelListDisplayCollection;
            set
            {
                this.levelListDisplayCollection = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the difficulty modifier list for the currently loaded level.
        /// </summary>
        public ObservableCollection<string> DifficultyModifierList
        {
            get
            {
                if (this.GetLevelLoaded != null)
                {
                    string categoryKey = this.GetLevelLoaded.CategoryKey;
                    if (categoryKey != string.Empty)
                    {
                        return new ObservableCollection<string>(Globals.GetSettings.GetDifficultyModifierList(categoryKey));
                    }
                }

                return new ObservableCollection<string>();
            }
        }

        #endregion

        #region LevelList

        /// <summary>
        /// Gets or sets the LevelListDisplay Room filter selected index value.
        /// </summary>
        public int LevelListFilterSelectedIndex
        {
            get => this.levelListFilterSelectedIndex;
            set
            {
                this.levelListFilterSelectedIndex = value;
                this.OnPropertyChanged();
                if (this.levelOverviewFinishedLoading)
                {
                    this.UpdateLevelList();
                }
            }
        }

        /// <summary>
        /// Gets the level list filter option items.
        /// </summary>
        public ObservableCollection<string> LevelListFilterItems
        {
            get
            {
                // Populate LevelListFilter Dropdown items
                ObservableCollection<string> levelListFilterItems = new ObservableCollection<string>(Globals.roomCategories);
                levelListFilterItems.Insert(0, "All");
                return levelListFilterItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show Story levels in the LevelListDisplay.
        /// </summary>
        public bool LevelListStoryCheck
        {
            get => this.levelListStoryCheck;
            set
            {
                this.levelListStoryCheck = value;
                this.OnPropertyChanged();
                if (this.levelOverviewFinishedLoading)
                {
                    this.UpdateLevelList();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show Bonus levels in the LevelListDisplay.
        /// </summary>
        public bool LevelListBonusCheck
        {
            get => this.levelListBonusCheck;
            set
            {
                this.levelListBonusCheck = value;
                this.OnPropertyChanged();
                if (this.levelOverviewFinishedLoading)
                {
                    this.UpdateLevelList();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show Unknown levels in the LevelListDisplay.
        /// </summary>
        public bool LevelListUnknownCheck
        {
            get => this.levelListUnknownCheck;
            set
            {
                this.levelListUnknownCheck = value;
                this.OnPropertyChanged();
                if (this.levelOverviewFinishedLoading)
                {
                    this.UpdateLevelList();
                }
            }
        }

        #endregion

        #region LevelGeneratingFields

        public bool RandomGroupEnabled
        {
            get => this.randomGroupEnabled;
            set
            {
                this.randomGroupEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use random recommendations.
        /// </summary>
        public bool UseRandomRecommendations
        {
            get => this.useRandomRecommendations;

            set
            {
                this.useRandomRecommendations = value;
                this.OnPropertyChanged();
                this.UpdateRandomRecommendations();
            }
        }

        #region Patients

        /// <summary>
        /// Gets or sets a value indicating whether to generate patients check.
        /// </summary>
        public bool GeneratePatientsCheck
        {
            get => this.generatePatientsCheck;

            set
            {
                this.generatePatientsCheck = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the min value to generate patients.
        /// </summary>
        public int GeneratePatientsMin
        {
            get => this.generatePatientsMin;

            set
            {
                this.generatePatientsMin = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the max value to generate patients.
        /// </summary>
        public int GeneratePatientsMax
        {
            get => this.generatePatientsMax;

            set
            {
                this.generatePatientsMax = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region PatientType

        /// <summary>
        /// Gets or sets a value indicating whether to generate the patient types check.
        /// </summary>
        public bool GeneratePatientTypeCheck
        {
            get => this.generatePatientTypeCheck;

            set
            {
                this.generatePatientTypeCheck = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the min value to generate patients.
        /// </summary>
        public int GeneratePatientTypeMin
        {
            get => this.generatePatientTypeMin;

            set
            {
                this.generatePatientTypeMin = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the max value to generate patients types.
        /// </summary>
        public int GeneratePatientTypeMax
        {
            get => this.generatePatientTypeMax;

            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                this.generatePatientTypeMax = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region PatientDelay

        /// <summary>
        /// Gets or sets a value indicating whether to generate the patient delay check.
        /// </summary>
        public bool GeneratePatientDelayCheck
        {
            get => this.generatePatientDelayCheck;

            set
            {
                this.generatePatientDelayCheck = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate patient delay min modifier.
        /// </summary>
        public int GeneratePatientDelayMin
        {
            get => this.generatePatientDelayMin;

            set
            {
                this.generatePatientDelayMin = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate patient delay max modifier.
        /// </summary>
        public int GeneratePatientDelayMax
        {
            get => this.generatePatientDelayMax;

            set
            {
                this.generatePatientDelayMax = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Treatment

        /// <summary>
        /// Gets or sets a value indicating whether to generate treatments check.
        /// </summary>
        public bool GenerateTreatmentsCheck
        {
            get => this.generateTreatmentsCheck;

            set
            {
                this.generateTreatmentsCheck = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate treatments min value.
        /// </summary>
        public int GenerateTreatmentsMin
        {
            get => this.generateTreatmentsMin;

            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                this.generateTreatmentsMin = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate treatments max value.
        /// </summary>
        public int GenerateTreatmentsMax
        {
            get => this.generateTreatmentsMax;

            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                this.generateTreatmentsMax = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Weight

        /// <summary>
        /// Gets or sets a value indicating whether to generate Weight check.
        /// </summary>
        public bool GenerateWeightCheck
        {
            get => this.generateWeightCheck;

            set
            {
                this.generateWeightCheck = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate Weight min value.
        /// </summary>
        public int GenerateWeightMin
        {
            get => this.generateWeightMin;

            set
            {
                this.generateWeightMin = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate Weight max value.
        /// </summary>
        public int GenerateWeightMax
        {
            get => this.generateWeightMax;

            set
            {
                this.generateWeightMax = value;
                this.OnPropertyChanged();
            }
        }
        #endregion


        #endregion


        #region Commands
        public ICommand ReloadLevelCommand
        {
            get
            {
                return this.reloadLevelCommand ?? (this.reloadLevelCommand = new CommandHandler(this.ReloadLevel, this.levelOverviewFinishedLoading));
            }
        }



        #endregion

        #endregion

        #endregion

        #region Methods

        #region Public
        #region Static
        /// <summary>
        /// Read the levelscript from file.
        /// </summary>
        /// <param name="levelName">
        /// The level name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReadLevelTextFromFile(string levelName)
        {
            levelName = CleanLevelName(levelName);
            if (!levelName.EndsWith(".lua"))
            {
                levelName += ".lua";
            }

            string levelPath = Globals.GetSettings.projectPathLevel + levelName;

            if (File.Exists(levelPath))
            {
                using (StreamReader streamReader = new StreamReader(levelPath, Encoding.UTF8))
                {
                    string readContents = streamReader.ReadToEnd();
                    return readContents;
                }
            }
            else
            {
                Console.WriteLine("ERROR: Data.ReadLevelText, Could not find " + levelPath + "!");
                return null;
            }
        }

        /// <summary>
        /// Setup the LevelOverview.
        /// </summary>
        public void SetupLevelOverview()
        {
            this.LevelListFilterSelectedIndex = Globals.GetSettings.RoomFilterDropdownIndex;
            this.LevelListStoryCheck = Globals.GetSettings.RoomFilterStoryCheck;
            this.LevelListBonusCheck = Globals.GetSettings.RoomFilterBonusCheck;
            this.LevelListUnknownCheck = Globals.GetSettings.RoomFilterUnknownCheck;

            this.UpdateLevelList();

            this.levelOverviewFinishedLoading = true;
        }

        #endregion
        #region LevelManagement
        /// <summary>
        /// Add patient to the Loaded Level
        /// </summary>
        public void AddPatientToLoadedLevel()
        {
            this.GetLevelLoaded?.AddPatient();
        }

        /// <summary>
        /// Returns the Level Object by name
        /// </summary>
        /// <param name="levelName">
        /// The level name.
        /// </param>
        /// <returns>
        /// The <see cref="Level"/>.
        /// </returns>
        public Level GetLevel(string levelName)
        {
            levelName = CleanLevelName(levelName);

            if (!this.LevelExist(levelName))
            {
                this.AddLevelByName(levelName);
            }

            return this.levelObjectData[levelName];
        }

        /// <summary>
        /// Load the current level in the Front-End
        /// </summary>
        /// <param name="levelName">
        /// The level name to load.
        /// </param>
        public void LoadLevel(string levelName)
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

        /// <summary>
        /// Check if the Level already exist
        /// </summary>
        /// <param name="levelName">
        /// The level name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool LevelExist(string levelName)
        {
            levelName = CleanLevelName(levelName);

            return this.levelObjectData.ContainsKey(levelName);
        }
        #endregion

        #region LevelList

        /// <summary>
        /// Get a list with the filterd level list.
        /// </summary>
        /// <param name="roomIndex">
        /// The room index.
        /// </param>
        /// <param name="storyLevels">
        /// The story levels.
        /// </param>
        /// <param name="bonusLevels">
        /// The bonus levels.
        /// </param>
        /// <param name="unknownLevels">
        /// The unknown levels.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> GetFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            List<string> rawLevelList = this.GetLevelsFromDisk(false, true);
            List<string> outputLevelList = new List<string>();

            foreach (string level in rawLevelList)
            {
                if (storyLevels && level.StartsWith("level"))
                {
                    int levelIndex = Convert.ToInt16(level.Replace("level", string.Empty));
                    int minIndex = ((roomIndex - 1) * 10) + 1;

                    if (roomIndex == 0)
                    {
                        outputLevelList.Add(level);
                    }
                    else if (Enumerable.Range(minIndex, 10).Contains(levelIndex))
                    {
                        outputLevelList.Add(level);
                    }
                }
                else if (bonusLevels && level.StartsWith("r") && int.TryParse(level[1].ToString(), out int _) && level[2].ToString() == "_")
                {
                    // Ensure that the second character in the level name is a number as well to be in line with naming conventions.
                    if (roomIndex == 0)
                    {
                        outputLevelList.Add(level);
                    }
                    else if (level.StartsWith("r" + roomIndex.ToString() + "_"))
                    {
                        outputLevelList.Add(level);
                    }
                }
                else if (unknownLevels && !level.StartsWith("level") && !(level.StartsWith("r") && int.TryParse(level[1].ToString(), out int _) && level[2].ToString() == "_"))
                {
                    outputLevelList.Add(level);
                }
            }

            // Naturaly sort the levelList
            // TODO Find more efficient method of natural sorting without first converting to sequence and back
            IEnumerable<string> levelSequence = outputLevelList.OrderBy(
                x => x,
                StringComparer.OrdinalIgnoreCase.WithNaturalSort());
            return levelSequence.ToList();
        }

        /// <summary>
        /// Get categorized filtered level list.
        /// </summary>
        /// <param name="roomIndex">
        /// The room index.
        /// </param>
        /// <param name="storyLevels">
        /// Return story levels.
        /// </param>
        /// <param name="bonusLevels">
        /// Return bonus levels.
        /// </param>
        /// <param name="unknownLevels">
        /// Return unknown levels.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, List<string>> GetCategorizedFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            Dictionary<string, List<string>> categorizedFilterdLevels = new Dictionary<string, List<string>>();

            // List<string> filteredLevelList = this.GetFilteredLevels(roomIndex, storyLevels, bonusLevels, unknownLevels);
            if (roomIndex == 0)
            {
                if (storyLevels)
                {
                    categorizedFilterdLevels.Add("Story", this.GetFilteredLevels(0, true));
                }

                if (bonusLevels)
                {
                    for (int i = 1; i <= Globals.roomCategories.Count; i++)
                    {
                        categorizedFilterdLevels.Add(
                            Globals.roomCategories[i - 1],
                            this.GetFilteredLevels(i, false, true));
                    }
                }
            }
            else
            {
                if (storyLevels)
                {
                    categorizedFilterdLevels.Add("Story", this.GetFilteredLevels(roomIndex, true));
                }

                if (bonusLevels)
                {
                    categorizedFilterdLevels.Add(
                        Globals.roomCategories[roomIndex - 1],
                        this.GetFilteredLevels(roomIndex, false, true));
                }
            }

            // Add all the uncategorized rooms
            if (unknownLevels)
            {
                categorizedFilterdLevels.Add("Unknown", this.GetFilteredLevels(roomIndex, false, false, true));
            }

            return categorizedFilterdLevels;
        }

        #endregion

        #region ReadWrite

        /// <summary>
        /// Get the levels from disk.
        /// </summary>
        /// <param name="reload">
        /// Force reload
        /// </param>
        /// <param name="filterExtension">
        /// The filter extension from strings.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> GetLevelsFromDisk(bool reload = false, bool filterExtension = false)
        {
            if (this.levelList == null || this.levelList.Count() == 0 || reload)
            {
                string projectPath = Globals.GetSettings.projectPathLevel;
                if (Directory.Exists(projectPath))
                {
                    List<string> rawLevelList = new List<string>(Directory.GetFiles(projectPath));
                    List<string> tmpLevelList = new List<string>();

                    foreach (string level in rawLevelList)
                    {
                        string levelName = Path.GetFileName(level);
                        if (filterExtension)
                        {
                            if (levelName != null)
                            {
                                tmpLevelList.Add(levelName.Replace(".lua", string.Empty));
                            }
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

        #region GenerateLevel

        /// <summary>
        /// Update the random recommendations for Level Generating.
        /// </summary>
        public void UpdateRandomRecommendations()
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

        /// <summary>
        /// Generate Level
        /// </summary>
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

                if (this.GenerateWeightCheck && this.GetLevelLoaded.GetLevelType == LevelTypeEnum.TimeTrial)
                {
                    this.GetLevelLoaded.RandomizePatientWeight(this.GenerateWeightMin, this.GenerateWeightMax);
                }
                this.GetLevelLoaded.UpdateLevelOutput();
            }
        }

        #endregion

        #region UpdateEvents


        public void CurrentLevelUpdated()
        {
            this.OnPropertyChanged("LevelOverviewActive");

            this.RoomIndex = this.GetLevelLoaded.GetRoomIndex;

            this.DifficultyModifier = this.GetLevelLoaded.GetDifficultyModifier;

            this.GetLevelLoaded.LevelLoaded();

            this.UpdatePatientSimulator();

        }

        /// <summary>
        /// Event fired when the RoomIndex is updated. 
        /// </summary>
        public void RoomIndexUpdated()
        {
            this.GetLevelLoaded.GetRoomIndex = this.RoomIndex;
            this.OnPropertyChanged("DifficultyModifierList");
            this.DifficultyModifierUpdated();

        }


        public void DifficultyModifierUpdated()
        {
            this.OnPropertyChanged("DifficultyModifier");
            this.RandomGroupEnabled = this.DifficultyModifier == 0;
            this.UpdateRandomRecommendations();
        }


        #endregion

        #endregion

        #region Private

        #region Static
        /// <summary>
        /// Clean the LevelName before using it.
        /// </summary>
        /// <param name="levelName">
        /// The level name.
        /// </param>
        /// <returns>
        /// The cleaned level name <see cref="string"/>.
        /// </returns>
        private static string CleanLevelName(string levelName)
        {
            // Remove everything from the first ' '
            if (levelName.IndexOf(' ') > -1)
            {
                levelName = levelName.Substring(0, levelName.IndexOf(' '));
            }

            if (levelName.EndsWith(".lua"))
            {
                levelName = levelName.Replace(".lua", string.Empty);
            }
            return levelName;
        }

        #endregion
        #region LevelManagement
        /// <summary>
        /// Create the level
        /// </summary>
        /// <param name="levelName">
        /// The level name to create the level with.
        /// </param>
        /// <returns>
        /// The <see cref="Level"/>.
        /// </returns>
        private Level CreateLevel(string levelName)
        {
            levelName = CleanLevelName(levelName);

            Level newLevel = new Level(levelName);

            return newLevel;
        }

        /// <summary>
        /// The add the level by name, if it exist then remove it and recreate it.
        /// </summary>
        /// <param name="levelName">
        /// The level name.
        /// </param>
        /// <returns>
        /// The <see cref="Level"/>.
        /// </returns>
        private Level AddLevelByName(string levelName)
        {
            levelName = CleanLevelName(levelName);
            if (this.LevelExist(levelName))
            {
                this.levelObjectData.Remove(levelName);
            }

            this.levelObjectData.Add(levelName, this.CreateLevel(levelName));
            return this.levelObjectData[levelName];
        }


        private void RemoveLevelByName(string levelName)
        {
            if (this.LevelExist(levelName))
            {
                this.levelObjectData.Remove(levelName);
            }
        }

        /// <summary>
        /// Reloads the currently loaded level
        /// </summary>
        public void ReloadLevel()
        {
            if (this.GetLevelLoaded != null)
            {
                // Remove and re-create again.
                string levelName = this.GetLevelLoaded.LevelName;
                this.RemoveLevelByName(levelName);

                this.AddLevelByName(levelName);

                // Set the Level
                this.GetLevelLoaded = this.levelObjectData[levelName];

                this.UpdateLevelList();
            }
        }
        #endregion

        #region LevelList

        /// <summary>
        /// Update the Level List Display.
        /// </summary>
        public void UpdateLevelList()
        {
            bool firstCategoryOpen = true;

            Dictionary<string, List<string>> levelDictionary = this.GetCategorizedFilteredLevels(this.LevelListFilterSelectedIndex, this.LevelListStoryCheck, this.LevelListBonusCheck, this.LevelListUnknownCheck);

            // First clear the Level List
            this.LevelListDisplayCollection.Clear();

            foreach (KeyValuePair<string, List<string>> category in levelDictionary)
            {
                TreeViewItem categoryItem = new TreeViewItem { Header = category.Key };
                if (category.Value.Count > 0)
                {
                    if (firstCategoryOpen)
                    {
                        categoryItem.ExpandSubtree();
                        firstCategoryOpen = false;
                    }

                    foreach (string levelName in category.Value)
                    {
                        // Load all levels that are found and display there status in the header.
                        string levelHeader = levelName;
                        if (!this.LevelExist(levelName))
                        {
                            this.AddLevelByName(levelName);
                        }

                        levelHeader = this.GetLevel(levelName).LevelNameWithStatus;


                        TreeViewItem levelItem = new TreeViewItem { Header = levelHeader };
                        levelItem.Selected += this.LevelListItemSelected;
                        categoryItem.Items.Add(levelItem);
                    }

                    this.LevelListDisplayCollection.Add(categoryItem);
                }
            }
        }
        #endregion

        #region PatientSimulator

        /// <summary>
        /// Update the patient simulator in the Front-end
        /// </summary>
        private void UpdatePatientSimulator()
        {
            // A DataTable is first created where an indentation is inserted to mimic a delay
            DataTable treatmentDataTable = new DataTable();
            treatmentDataTable.Columns.Add("PatientName");

            DataGrid simulatorGrid = Globals.GetMainWindow.patientSimulatorGrid;

            // SimulatorGrid.ItemsSource = new Binding("treatmentDataTable");    //GetLevelLoaded.PatientCollection;

            // Create first column for the PatientName
            DataGridTextColumn patientNameColumn =
                new DataGridTextColumn
                {
                    Header = "Patient Name:",
                    Binding = new Binding(treatmentDataTable.Columns[0].ToString())
                };
            simulatorGrid.Columns.Add(patientNameColumn);

            // Create the columns
            for (int i = 0; i < 40; i++)
            {
                // Add treatment column to the treatmentDataTable
                treatmentDataTable.Columns.Add(i.ToString());

                string columnName = (i * 5).ToString() + " Seconds";
                DataGridComboBoxColumn treatmentColumn =
                    new DataGridComboBoxColumn
                    {
                        Header = columnName,
                        SelectedValueBinding = new Binding(treatmentDataTable.Columns[i + 1].ToString()),
                        ItemsSource = this.GetLevelLoaded.AvailableTreatmentStringList,
                        Width = 120
                    };
                simulatorGrid.Columns.Add(treatmentColumn);
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
            simulatorGrid.ItemsSource = treatmentDataTable.DefaultView;

            // Globals.GetMainWindow.patientSimulatorGrid = SimulatorGrid;
        }

        #endregion

        #endregion

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        /// <param name="propertyName">
        /// The property Name.
        /// </param>
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members

        #region Signals

        #region LevelList

        /// <summary>
        /// Fired when an Level has been selected in the LevelListDisplay.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LevelListItemSelected(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/questions/24880824/how-to-add-wpf-treeview-node-click-event-to-get-the-node-value
            if (sender is TreeViewItem item)
            {
                Globals.GetLevelOverview.LoadLevel(CleanLevelName(item.Header.ToString()));
            }
        }

        #endregion

        #endregion


    }
}