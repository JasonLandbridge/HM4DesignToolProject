// --------------------------------------------------------------------------------------------------------------------
// <copyright file="settings.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  This is where all settings are retrieved and stored in JSON files. The Settings class functions as a gateway between the personal and global settings and the rest of the program. </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HM4DesignTool.Level
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media;

    using HM4DesignTool.Data;

    using nucs.JsonSettings;

    /// <summary>
    /// The settings class based on JSON storage.
    /// Storing settings in JSON: https://github.com/Nucs/JsonSettings
    /// </summary>
    public class Settings
    {
        #region Fields

        /// <summary>
        /// The personal settings reference.
        /// </summary>
        private readonly PersonalSettings personalSettings;

        /// <summary>
        /// The global settings reference.
        /// </summary>
        private readonly GlobalSettings globalSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.personalSettings = new PersonalSettings();
            this.globalSettings = new GlobalSettings();
            this.LoadSettings();
        }

        #endregion

        #region Properties

        #region Public
        #region General

        /// <summary>
        /// Gets or sets the project path.
        /// </summary>
        public string ProjectPathData
        {
            get => this.personalSettings.ProjectDirectoryPath;

            set => this.personalSettings.ProjectDirectoryPath = value;
        }

        /// <summary>
        /// The project path script directory.
        /// </summary>
        public string ProjectPathScript => this.ProjectPathData + "\\script\\";

        /// <summary>
        /// The project path level directory.
        /// </summary>
        public string ProjectPathLevel => this.ProjectPathData + "\\script\\levels\\";

        /// <summary>
        /// The project path images directory.
        /// </summary>
        public string ProjectPathImages => this.ProjectPathData + "\\images\\";

        #endregion

        #region PersonalSettings

        /// <summary>
        /// Gets or sets the room filter dropdown index.
        /// </summary>
        public int RoomFilterDropdownIndex
        {
            get => this.personalSettings.RoomFilterDropdownIndex;

            set => this.personalSettings.RoomFilterDropdownIndex = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether level list first category is open.
        /// </summary>
        public bool LevelListFirstCategoryOpen
        {
            get => this.personalSettings.LevelListFirstCategoryOpen;

            set => this.personalSettings.LevelListFirstCategoryOpen = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the room filter story is checked.
        /// </summary>
        public bool RoomFilterStoryCheck
        {
            get => this.personalSettings.RoomFilterStoryCheck;

            set => this.personalSettings.RoomFilterStoryCheck = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the room filter bonus is checked.
        /// </summary>
        public bool RoomFilterBonusCheck
        {
            get => this.personalSettings.RoomFilterBonusCheck;

            set => this.personalSettings.RoomFilterBonusCheck = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the room filter unknown is checked.
        /// </summary>
        public bool RoomFilterUnknownCheck
        {
            get => this.personalSettings.RoomFilterUnknownCheck;

            set => this.personalSettings.RoomFilterUnknownCheck = value;
        }

        #endregion

        #endregion

        #region Private

        /// <summary>
        /// Gets or sets the customized treatment weights dictionary.
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> CustomizedTreatmentWeights
        {
            get => this.globalSettings.CustomizedTreatmentWeights;

            set => this.globalSettings.CustomizedTreatmentWeights = value;
        }

        /// <summary>
        /// Gets or sets the patient type categories dict.
        /// </summary>
        private Dictionary<string, List<string>> PatientTypeCategoriesDict
        {
            get => this.globalSettings.PatientTypeCategories ?? new Dictionary<string, List<string>>();

            set => this.globalSettings.PatientTypeCategories = value;
        }

        /// <summary>
        /// Gets or sets the treatment categories dict.
        /// </summary>
        private Dictionary<string, List<Treatment>> TreatmentCategoriesDict
        {
            get
            {
                if (this.globalSettings.TreatmentCategories != null)
                {
                    // Convert Dictionary<String, Dictionary<String, List<String>>> -> Dictionary<String, List<Treatment>>
                    Dictionary<string, List<Treatment>> convertedDict = new Dictionary<string, List<Treatment>>();

                    // Loop over each room/category
                    foreach (KeyValuePair<string, Dictionary<string, string>> roomCategory in this.globalSettings
                        .TreatmentCategories)
                    {
                        List<Treatment> treatmentList = new List<Treatment>();

                        // Loop over each treatment
                        foreach (KeyValuePair<string, string> treatmentRow in roomCategory.Value)
                        {
                            Treatment treatment = new Treatment(treatmentRow.Key, treatmentRow.Value);

                            treatmentList.Add(treatment);
                        }

                        convertedDict.Add(roomCategory.Key, treatmentList);
                    }

                    return convertedDict;
                }
                else
                {
                    return new Dictionary<string, List<Treatment>>();
                }
            }

            set
            {
                // Convert Dictionary<String, List<Treatment>> -> Dictionary<String, Dictionary<String, List<String>>>
                Dictionary<string, Dictionary<string, string>> convertedDict =
                    new Dictionary<string, Dictionary<string, string>>();

                // Loop over each room/category
                foreach (KeyValuePair<string, List<Treatment>> roomCategory in value)
                {
                    Dictionary<string, string> treatmentList = new Dictionary<string, string>();

                    // Loop over each treatment
                    foreach (Treatment treatmentRow in roomCategory.Value)
                    {
                        if (treatmentRow != null && treatmentRow.TreatmentName != null)
                        {
                            if (treatmentList.ContainsKey(treatmentRow.TreatmentName))
                            {
                                treatmentList[treatmentRow.TreatmentName] = treatmentRow.ToString();
                            }
                            else
                            {
                                treatmentList.Add(treatmentRow.TreatmentName, treatmentRow.ToString());
                            }
                        }
                    }

                    convertedDict.Add(roomCategory.Key, treatmentList);
                }

                this.globalSettings.TreatmentCategories = convertedDict;
            }
        }

        /// <summary>
        /// Gets or sets the balancing categories dict.
        /// </summary>
        private Dictionary<string, List<string>> BalancingCategoriesDict
        {
            get
            {
                if (this.globalSettings.BalancingCategories != null)
                {
                    return this.globalSettings.BalancingCategories;
                }
                else
                {
                    return new Dictionary<string, List<string>>();
                }
            }

            set => this.globalSettings.BalancingCategories = value;
        }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Loads all settings from the personal and global settings.
        /// </summary>
        public void LoadSettings()
        {
            this.personalSettings.Load();
            this.globalSettings.Load();
        }

        /// <summary>
        /// Saves all settings from the personal and global settings.
        /// </summary>
        public void SaveSettings()
        {
            this.personalSettings.Save();
            this.globalSettings.Save();
        }
        #endregion

        #endregion
        #region Settings
        #region Getters

        #region PatientTypes

        /// <summary>
        /// Gets the patient types in a dictionary categorized by room.
        /// </summary>
        /// <param name="categoryKey">
        /// The category key.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, List<string>> GetPatientTypes(string categoryKey = null)
        {
            if (categoryKey != null)
            {
                Dictionary<string, List<string>> filterdPatientTypeDict = new Dictionary<string, List<string>>();  // Room[N] -> List with only checked patientTypes

                if (this.PatientTypeCategoriesDict.ContainsKey(categoryKey))
                {
                    filterdPatientTypeDict.Add(categoryKey, this.PatientTypeCategoriesDict[categoryKey]);
                }
                else
                {
                    Console.WriteLine("ERROR: Settings.GetPatientTypes, patientTypeCategoriesDict does not contain key: " + categoryKey);
                }

                return filterdPatientTypeDict;
            }
            else
            {
                return this.PatientTypeCategoriesDict;
            }
        }

        /// <summary>
        /// Reads the patient types from disk. 
        /// </summary>
        /// <param name="reload">
        /// The reload.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> GetPatientTypesFromDisk(bool reload = false)
        {
            string projectPath = Globals.GetSettings.ProjectPathImages + "patients\\";

            if (Directory.Exists(projectPath))
            {
                List<string> rawPatientTypeList = new List<string>(Directory.GetDirectories(projectPath));
                List<string> rawPatientList = new List<string>();

                foreach (string patientType in rawPatientTypeList)
                {
                    rawPatientList.Add(patientType.Replace(projectPath, string.Empty));
                }

                return rawPatientList;
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// The get patient type list.
        /// </summary>
        /// <param name="categoryKey">
        /// The category key.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> GetPatientTypeList(string categoryKey)
        {
            Dictionary<string, List<string>> patientTypeDict = this.GetPatientTypes(categoryKey);  // Room[N] -> List with only checked patientTypes

            if (patientTypeDict.ContainsKey(categoryKey))
            {
                return patientTypeDict[categoryKey];
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Get the patient chance list
        /// </summary>
        /// <param name="categoryKey">
        /// The category key.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<PatientChance> GetPatientChanceList(string categoryKey)
        {
            List<string> patientTypeList = this.GetPatientTypeList(categoryKey);  // Room[N] -> List with only checked patientTypes

            List<PatientChance> patientChanceList = new List<PatientChance>();

            if (patientTypeList.Count > 0)
            {
                foreach (string patientType in patientTypeList)
                {
                    patientChanceList.Add(new PatientChance(patientType));
                }
            }

            return patientChanceList;
        }

        #endregion

        #region Stations



        #endregion


        #region Treatments

        /// <summary>
        /// Get the treatment list
        /// </summary>
        /// <param name="categoryKey">
        /// The category key.
        /// </param>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Treatment> GetTreatmentList(string categoryKey, double difficultyModifier = 0)
        {
            if (!string.IsNullOrEmpty(categoryKey))
            {
                Dictionary<string, List<Treatment>> treatmentDictionary = this.GetTreatmentDictionary(categoryKey);
                if (treatmentDictionary.ContainsKey(categoryKey))
                {
                    switch (difficultyModifier)
                    {
                        case 0:
                            return treatmentDictionary[categoryKey];
                        default:
                            {
                                // Filter List by the difficulty modifier
                                return treatmentDictionary[categoryKey].FindAll(t => t.DifficultyUnlocked <= difficultyModifier);
                            }
                    }
                }
            }

            return new List<Treatment>();
        }

        /// <summary>
        /// Get the customized treatment weights.
        /// </summary>
        /// <param name="levelName">
        /// The level name.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, int> GetCustomizedTreatmentWeights(string levelName)
        {
            Dictionary<string, int> output = new Dictionary<string, int>();
            if (this.CustomizedTreatmentWeights.ContainsKey(levelName))
            {
                output = this.CustomizedTreatmentWeights[levelName];
            }

            return output;
        }

        /// <summary>
        /// The get treatment.
        /// </summary>
        /// <param name="treatmentName">
        /// The treatment name.
        /// </param>
        /// <param name="roomIndex">
        /// The room index.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Treatment</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Treatment GetTreatment(string treatmentName, int roomIndex = -1)
        {
            if (roomIndex > -1 && Globals.GetCategoryKey(roomIndex) != string.Empty)
            {
                List<Treatment> treatmentList = this.GetTreatmentList(Globals.GetCategoryKey(roomIndex));
                if (treatmentList != null && treatmentList.Count > 0)
                {
                    foreach (Treatment t in treatmentList)
                    {
                        if (t.TreatmentName == treatmentName)
                        {
                            return t;
                        }
                    }
                }
            }
            else
            {
                // Longer method of retrieving the treatment without roomIndex
                foreach (KeyValuePair<string, List<Treatment>> category in this.GetTreatmentDictionary())
                {
                    foreach (Treatment treatment in category.Value)
                    {
                        if (treatment.TreatmentName == treatmentName)
                        {
                            return treatment;
                        }
                    }
                }
            }

            return new Treatment("Unknown");
        }

        /// <summary>
        /// Get the treatment color
        /// </summary>
        /// <param name="treatmentName">
        /// The treatment name.
        /// </param>
        /// <param name="roomIndex">
        /// The room index.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetTreatmentColor(string treatmentName, int roomIndex = -1)
        {
            return this.GetTreatment(treatmentName, roomIndex).TreatmentColor;
        }

        /// <summary>
        /// Gets the treatment dictionary.
        /// </summary>
        /// <param name="categoryKey">
        /// The category key.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, List<Treatment>> GetTreatmentDictionary(string categoryKey = null)
        {
            if (categoryKey != null)
            {
                Dictionary<string, List<Treatment>> filterdTreatmentDict = new Dictionary<string, List<Treatment>>();  // Room[N] -> List with Treatment Class

                if (this.TreatmentCategoriesDict.ContainsKey(categoryKey))
                {
                    filterdTreatmentDict.Add(categoryKey, this.TreatmentCategoriesDict[categoryKey]);
                }
                else
                {
                    Console.WriteLine("ERROR: Settings.GetTreatmentDictionary, treatmentCategoriesDict does not contain key: " + categoryKey);
                }

                return filterdTreatmentDict;
            }
            else
            {
                return this.TreatmentCategoriesDict;
            }
        }

        #endregion

        #region Balancing

        /// <summary>
        /// Gets the balancing categories.
        /// </summary>
        /// <param name="categoryKey">
        /// The category key.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, List<string>> GetBalancingCategories(string categoryKey = null)
        {
            if (categoryKey != null)
            {
                Dictionary<string, List<string>> filterdBalancingCategory = new Dictionary<string, List<string>>();  // Room[N] -> List with Difficulty modifiers in String

                if (this.BalancingCategoriesDict.ContainsKey(categoryKey))
                {
                    filterdBalancingCategory.Add(categoryKey, this.BalancingCategoriesDict[categoryKey]);
                }
                else
                {
                    Console.WriteLine("ERROR: Settings.GetBalancingCategories, balancingCategoriesDict does not contain key: " + categoryKey);
                }

                return filterdBalancingCategory;
            }
            else
            {
                return this.BalancingCategoriesDict;
            }
        }

        /// <summary>
        /// Get the difficulty modifier list.
        /// </summary>
        /// <param name="categoryKey">
        /// The category key.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> GetDifficultyModifierList(string categoryKey)
        {
            Dictionary<string, List<string>> balancingCategoriesDict = this.GetBalancingCategories(categoryKey);
            if (balancingCategoriesDict.ContainsKey(categoryKey))
            {
                return balancingCategoriesDict[categoryKey];
            }
            else
            {
                return new List<string>();
            }
        }

        #endregion
        #endregion

        #region Setters

        /// <summary>
        /// Set the patient Types to be stored in the Global settings.
        /// </summary>
        /// <param name="patientTypeCategoriesDict">
        /// The patient type categories dict.
        /// </param>
        public void SetPatientTypes(Dictionary<string, List<string>> patientTypeCategoriesDict)
        {
            this.PatientTypeCategoriesDict = patientTypeCategoriesDict;
        }

        /// <summary>
        /// Set the treatment categories to be stored in the Global settings.
        /// </summary>
        /// <param name="treatmentDataDict">
        /// The treatment data dict.
        /// </param>
        public void SetTreatmentCategories(Dictionary<string, List<Treatment>> treatmentDataDict)
        {
            this.TreatmentCategoriesDict = treatmentDataDict;
        }

        /// <summary>
        /// Set the balancing categories to be stored in the Global settings.
        /// </summary>
        /// <param name="balancingCategoriesDict">
        /// The balancing categories dict.
        /// </param>
        public void SetBalancingCategories(Dictionary<string, List<string>> balancingCategoriesDict)
        {
            this.BalancingCategoriesDict = balancingCategoriesDict;
        }

        /// <summary>
        /// Set the customized treatment weights.
        /// </summary>
        /// <param name="customizedTreatmentWeightsDict">
        /// The customized treatment weights dictionary.
        /// </param>
        public void SetCustomizedTreatmentWeightsDict(Dictionary<string, Dictionary<string, int>> customizedTreatmentWeightsDict)
        {
            foreach (KeyValuePair<string, Dictionary<string, int>> treatmentWeightsDict in customizedTreatmentWeightsDict)
            {
                if (this.CustomizedTreatmentWeights.ContainsKey(treatmentWeightsDict.Key))
                {
                    this.CustomizedTreatmentWeights[treatmentWeightsDict.Key] = treatmentWeightsDict.Value;
                }
                else
                {
                    this.CustomizedTreatmentWeights.Add(treatmentWeightsDict.Key, treatmentWeightsDict.Value);
                }
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// The personal settings which is unique to every user.
    /// </summary>
    public class PersonalSettings : JsonSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalSettings"/> class.
        /// </summary>
        public PersonalSettings()
        {
            // Check if save file exist and otherwise create it
            string defaultPath = AppDomain.CurrentDomain.BaseDirectory + this.FileName;
            if (!File.Exists(defaultPath))
            {
                File.Create(defaultPath).Dispose();
            }
        }

        #endregion

        #region Properties

        #region Public

        /// <summary>
        /// Gets or sets the file name to which to save.
        /// </summary>
        public override sealed string FileName { get; set; } = "personal.json";

        /// <summary>
        /// Gets or sets the project directory path.
        /// </summary>
        public string ProjectDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the room filter dropdown index.
        /// </summary>
        public int RoomFilterDropdownIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether level list first category open.
        /// </summary>
        public bool LevelListFirstCategoryOpen { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the room story filter is checked.
        /// </summary>
        public bool RoomFilterStoryCheck { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the room bonus filter is checked.
        /// </summary>
        public bool RoomFilterBonusCheck { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the room unknown filter is checked.
        /// </summary>
        public bool RoomFilterUnknownCheck { get; set; } = true;

        #endregion

        #endregion
    }

    /// <summary>
    /// The global settings which are shared for every user.
    /// </summary>
    public class GlobalSettings : JsonSettings
    {
        // for loading and saving.
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSettings"/> class.
        /// </summary>
        public GlobalSettings()
        {
            // Check if save file exist and otherwise create it
            string defaultPath = AppDomain.CurrentDomain.BaseDirectory + this.FileName;
            if (!File.Exists(defaultPath))
            {
                File.Create(defaultPath).Dispose();
            }
        }

        #endregion
        #region Properties

        #region Public

        /// <summary>
        /// Gets or sets the file name to which to save.
        /// </summary>
        public override sealed string FileName { get; set; } = "global.json";

        /// <summary>
        /// Gets or sets the patient type categories.
        /// </summary>
        public Dictionary<string, List<string>> PatientTypeCategories { get; set; }

        /// <summary>
        /// Gets or sets the station categories.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> StationCategories { get; set; }

        /// <summary>
        /// Gets or sets the treatment categories.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> TreatmentCategories { get; set; }

        /// <summary>
        /// Gets or sets the balancing categories.
        /// </summary>
        public Dictionary<string, List<string>> BalancingCategories { get; set; }

        /// <summary>
        /// Gets or sets the customized treatment weights.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> CustomizedTreatmentWeights { get; set; } = new Dictionary<string, Dictionary<string, int>>();

        #endregion

        #endregion
    }
}