﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Level.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HM4DesignTool.Level
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    using HM4DesignTool.Data;
    using HM4DesignTool.Utilities;

    using Weighted_Randomizer;

    /// <inheritdoc />
    /// <summary>
    /// Level Object which stores all the data related to the Level.
    /// </summary>
    public class Level : INotifyPropertyChanged
    {
        #region Fields

        #region PatientSimulator

        /// <summary>
        /// The patient simulate data table which is used to parse the treatments in the right indentation.
        /// </summary>
        private readonly DataTable patientSimulateDataTable = new DataTable();

        #endregion PatientSimulator

        #region General

        /// <summary>
        /// The gameplay character that can optionally be added.
        /// </summary>
        private string gameplayCharacter = string.Empty;


        /// <summary>
        /// Only execute commands in this level when this is true.
        /// </summary>
        private readonly bool canExecuteCommands;

        private bool levelFinishedLoading;

        private bool isEdited;
        #endregion General

        #region DesignToolData

        /// <summary>
        /// The DesignToolData object where all data is stored in.
        /// </summary>
        private readonly DesignToolData designToolData = new DesignToolData();

        #endregion DesignToolData

        #region Treatment

        /// <summary>
        /// Storage for the available treatments based on the Difficulty Modifier
        /// </summary>
        private List<Treatment> availableTreatmentList = new List<Treatment>();

        /// <summary>
        /// Storage for the available treatments in String Format based on the Difficulty Modifier
        /// </summary>
        private string treatmentsAvailableString = string.Empty;

        #endregion Treatment

        #region Patient

        /// <summary>
        /// The Patient Object list.
        /// </summary>
        private ObservableCollection<Patient> patientCollection = new ObservableCollection<Patient>();

        #endregion Patient

        #region PatientChance

        /// <summary>
        /// The PatientChance Object list.
        /// </summary>
        private List<PatientChance> patientChanceList = new List<PatientChance>();

        #endregion PatientChance

        #region LevelScript

        /// <summary>
        /// LevelScript buffer which in bound to the Front-End.
        /// </summary>
        private string levelScriptBuffer = string.Empty;

        #endregion LevelScript

        #region Commands

        private ICommand addPatientRowToLevelCommand;

        private ICommand removePatientRowFromLevelCommand;

        /// <summary>
        /// The randomizeAllWeightCommand property.
        /// </summary>
        private ICommand randomizeAllWeightCommand;

        /// <summary>
        /// The openLevelFromFileCommand property.
        /// </summary>
        private ICommand openLevelFromFileCommand;

        /// <summary>
        /// The saveLevelToFile property.
        /// </summary>
        private ICommand saveLevelToFile;

        #endregion Commands

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        /// <param name="levelName">
        /// The level name.
        /// </param>
        public Level(string levelName)
        {
            this.LevelName = levelName;
            this.PatientChanceList = Globals.GetSettings.GetPatientChanceList(this.CategoryKey);
            //this.PatientList = new List<Patient>();

            // Translate the levelScript into workable variables.
            this.ParseRawText();

            this.UpdateAvailableTreatmentList();
            this.UpdateLevelOutput();
            this.canExecuteCommands = true;
            this.LevelFinishedLoading = true;
        }

        #endregion Constructors

        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion 

        #region Properties

        #region Public Properties

        #region General

        /// <summary>
        /// Gets or sets the level name, based on the Filename without the extension.
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// The file name with the extension.
        /// </summary>
        public string FileName => $"{this.LevelName}.lua";

        /// <summary>
        /// Gets the level name with the status of the level.
        /// </summary>
        public string LevelNameWithStatus
        {
            get
            {
                string levelName = $"{this.LevelName}";

                if (this.IsEdited)
                {
                    levelName += $" (*)";
                }

                if (this.IsEmpty)
                {
                    levelName += $" (e)";
                }

                if (this.GetLevelType != LevelTypeEnum.Unknown)
                {
                    levelName += $" - {this.GetLevelType.ToString()}";
                }

                return levelName;
            }
        }

        /// <summary>
        /// Based on the roomIndex translated to the Category, 0 => Room 1, 1 => Room 2, 2 => Room 3, etc
        /// </summary>
        public string CategoryKey => Globals.GetCategoryKey(this.GetRoomIndex);

        /// <summary>
        /// Check if this Level has an enabled weight for Patient Occurence in TimeTrial type levels.
        /// </summary>
        public bool WeightEnabled => this.designToolData.LevelType == LevelTypeEnum.TimeTrial;

        public string GameplayCharacter
        {
            get => this.gameplayCharacter;
            set
            {
                this.gameplayCharacter = value;
                this.OnPropertyChanged();
                this.UpdateLevelOutput();
            }
        }

        public bool LevelFinishedLoading
        {
            get => this.levelFinishedLoading;
            set
            {
                this.levelFinishedLoading = value;
                this.OnPropertyChanged();

            }
        }

        public bool IsEdited
        {
            get => this.isEdited;
            set
            {
                this.isEdited = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (this.GetLevelType == LevelTypeEnum.Bonus || this.GetLevelType == LevelTypeEnum.Story || this.GetLevelType == LevelTypeEnum.TimeTrial)
                {
                    return this.PatientCollection.Count == 0;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion General

        #region DesignToolData

        /// <summary>
        /// Get the String output of Design Tool Data
        /// </summary>
        public string DesignToolDataString => this.designToolData.ToString();

        /// <summary>
        /// Gets or sets the get difficulty modifier of the Level.
        /// </summary>
        public double GetDifficultyModifier
        {
            get => this.designToolData.DifficultyLevel;
            set
            {
                this.designToolData.DifficultyLevel = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("GetDifficultyModifierString");
                this.OnPropertyChanged("GetTreatmentOptions");
                this.OnPropertyChanged("GetTreatmentsAvailableString");
                this.UpdateAvailableTreatmentList();
            }
        }

        /// <summary>
        /// Gets or sets the difficulty modifier string.
        /// </summary>
        public string GetDifficultyModifierString
        {
            get => this.GetDifficultyModifier.ToString("0.0");
            set
            {
                if (value != null)
                {
                    this.GetDifficultyModifier = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    this.GetDifficultyModifier = 1;
                }

                this.UpdateLevelOutput();
            }
        }


        /// <summary>
        /// Gets or sets the Level type Enum.
        /// </summary>
        public LevelTypeEnum GetLevelType
        {
            get => this.designToolData.LevelType;
            set
            {
                this.designToolData.LevelType = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("DesignToolDataString");
                this.OnPropertyChanged("WeightEnabled");
                this.UpdateLevelOutput();
            }
        }

        /// <summary>
        /// Gets or sets the Level type string.
        /// </summary>
        public string GetLevelTypeString
        {
            get => this.designToolData.LevelTypeString;
            set
            {
                this.designToolData.LevelTypeString = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("DesignToolDataString");
                this.OnPropertyChanged("WeightEnabled");
                this.UpdateLevelOutput();
            }
        }

        /// <summary>
        /// Gets or sets the RoomIndex.
        /// </summary>
        public int GetRoomIndex
        {
            get
            {
                if (this.designToolData.Roomindex != 0)
                {
                    return this.designToolData.Roomindex;
                }
                else
                {
                    int roomIndex = 0;

                    // Generate the RoomIndex based on the naming convention of the levelName
                    if (this.LevelName != string.Empty)
                    {
                        // Assuming the room index is in r2_b04... naming convention]
                        if (this.LevelName.StartsWith("level"))
                        {
                            string filterdString = Globals.FilterToNumerical(this.LevelName.Replace("level", string.Empty));
                            int unused;
                            bool successfullyParsed = int.TryParse(filterdString, out unused);
                            if (successfullyParsed)
                            {
                                int levelIndex = Convert.ToInt16(filterdString);
                                while (roomIndex < 100)
                                {
                                    int minIndex = ((roomIndex - 1) * 10) + 1;
                                    int maxIndex = (roomIndex * 10) + 1;

                                    if (Enumerable.Range(minIndex, maxIndex).Contains(levelIndex))
                                    {
                                        break;
                                    }

                                    roomIndex++;
                                }
                            }
                        }
                        else if (this.LevelName.StartsWith("r"))
                        {


                            string filterdString = Globals.FilterToNumerical(this.LevelName.Substring(1, 1));
                            int unused;
                            bool successfullyParsed = int.TryParse(filterdString, out unused);
                            if (successfullyParsed)
                            {
                                roomIndex = Convert.ToInt16(filterdString);
                            }
                        }
                        else
                        {
                            roomIndex = -1;
                        }
                    }

                    // Store the newly generated RoomIndex
                    this.designToolData.Roomindex = roomIndex;
                    return roomIndex;
                }
            }

            set
            {
                this.designToolData.Roomindex = value;
                this.OnPropertyChanged("DesignToolDataString");
                this.OnPropertyChanged("GetDifficultyModifier");
                this.UpdateLevelOutput();
            }
        }

        #endregion DesignToolData

        #region Treatment

        /// <summary>
        /// Gets or sets the available treatment list from which the level chooses the correct Treatments.
        /// </summary>
        public List<Treatment> AvailableTreatmentList
        {
            get => this.availableTreatmentList;
            set
            {
                this.availableTreatmentList = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("AvailableTreatmentStringList");
            }
        }

        /// <summary>
        /// Gets the available treatment list from which the level chooses the correct Treatments in string format.
        /// </summary>
        public List<string> AvailableTreatmentStringList
        {
            get
            {
                List<string> treatmentStringList = new List<string>();
                foreach (Treatment treatment in this.AvailableTreatmentList)
                {
                    treatmentStringList.Add(treatment.TreatmentName);
                }

                return treatmentStringList;
            }
        }

        /// <summary>
        /// Gets or sets the available treatments in string Format.
        /// </summary>
        public string TreatmentsAvailableString
        {
            get => this.treatmentsAvailableString;
            set
            {
                this.treatmentsAvailableString = value;
                this.OnPropertyChanged();
            }
        }

        #endregion Treatment

        #region Patient

        /// <summary>
        /// Gets or sets the list with all the Patient objects in this level converted to an ObservableCollection.
        /// </summary>
        public ObservableCollection<Patient> PatientCollection
        {
            get => this.patientCollection;

            set
            {
                foreach (Patient patient in value)
                {
                    patient.ParentLevel = this;
                }

                this.patientCollection = value;
                this.OnPropertyChanged();
            }
        }

        #endregion Patient

        #region PatientChance

        /// <summary>
        /// Gets or sets the list with all the PatientChance objects in this level converted to an ObservableCollection.
        /// </summary>
        public ObservableCollection<PatientChance> PatientChanceCollection
        {
            get => new ObservableCollection<PatientChance>(this.PatientChanceList);
            set
            {
                this.PatientChanceList = value.ToList();
                this.OnPropertyChanged();
                this.UpdateLevelOutput();
            }
        }

        #endregion PatientChance

        #region PatientSimulator

        /// <summary>
        /// The patient simulate data table which is used to parse the treatments in the right indentation.
        /// </summary>
        public DataTable PatientSimulateDataTable => this.patientSimulateDataTable;

        #endregion PatientSimulator

        #region LevelScript

        /// <summary>
        /// Gets the start comments at the top of the LevelScript.
        /// </summary>
        public string StartComments { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the currently saved Level Script from file.
        /// </summary>
        public string GetCurrentLevelScript => LevelOverview.ReadLevelTextFromFile(this.LevelName);

        /// <summary>
        /// Gets the compiled new script from this level
        /// </summary>
        public string GetNewLevelScript
        {
            get
            {
                string output = string.Empty;

                if (Globals.GetLevelOverview.InsertDesignData)
                {
                    output += this.designToolData.ToString();
                    output += Environment.NewLine;

                }

                // Output an optional GamePlay character.
                if (this.GameplayCharacter != string.Empty)
                {
                    output += $"levelDesc.gameplayCharacterId = \"{this.GameplayCharacter}\"{Environment.NewLine}levelDesc.decoId = \"{this.GameplayCharacter}\"{Environment.NewLine}{Environment.NewLine}";
                }

                // Output the patient chances
                if (this.GetLevelType != LevelTypeEnum.OliverOne &&
                    this.GetLevelType != LevelTypeEnum.OliverAll &&
                    this.GetLevelType != LevelTypeEnum.MiniGame &&
                    this.PatientChanceCollection.Count > 0)
                {
                    output += "levelDesc.patientChances = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    int i = 0;
                    foreach (PatientChance patientChance in this.PatientChanceCollection)
                    {
                        if (patientChance.Weight > 0)
                        {
                            // Depending on the Length of the PatientName add another tab. 
                            if (patientChance.PatientName.Length <= 8)
                            {
                                output += $"\t{patientChance.PatientName}\t\t\t = \t{patientChance.Weight}";
                            }
                            else if (patientChance.PatientName.Length > 8 && patientChance.PatientName.Length <= 16)
                            {
                                output += $"\t{patientChance.PatientName}\t\t = \t{patientChance.Weight}";
                            }
                            else if (patientChance.PatientName.Length > 16)
                            {
                                output += $"\t{patientChance.PatientName}\t = \t{patientChance.Weight}";

                            }

                            if (i < this.PatientChanceCollection.Count - 1)
                            {
                                output += ",";
                            }

                            i++;
                            output += Environment.NewLine;
                        }
                    }

                    output += "}" + Environment.NewLine;
                }

                output += Environment.NewLine;

                // Output the patient treatments
                if (this.GetLevelType != LevelTypeEnum.OliverOne &&
                    this.GetLevelType != LevelTypeEnum.OliverAll &&
                    this.GetLevelType != LevelTypeEnum.MiniGame &&
                    this.PatientChanceCollection.Count > 0)
                {
                    output += "levelDesc.triggers = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    foreach (Patient patient in this.PatientCollection)
                    {
                        output += patient.ToOutput();
                    }

                    output += "}" + Environment.NewLine;
                }

                return output;
            }
        }

        /// <summary>
        /// Gets or sets the LevelScript buffer.
        /// </summary>
        public string LevelScriptBuffer
        {
            get => this.levelScriptBuffer;
            set
            {
                this.levelScriptBuffer = value;
                this.OnPropertyChanged();
            }
        }
        #endregion LevelScript

        #region Commands

        /// <summary>
        /// Add patient row to level command.
        /// </summary>
        public ICommand AddPatientRowToLevelCommand => this.addPatientRowToLevelCommand ?? (this.addPatientRowToLevelCommand = new CommandHandler(this.AddPatient, this.canExecuteCommands));

        /// <summary>
        /// Remove patient row from level command.
        /// </summary>
        public ICommand RemovePatientRowFromLevelCommand => this.removePatientRowFromLevelCommand ?? (this.removePatientRowFromLevelCommand = new CommandHandler(this.RemovePatient, this.canExecuteCommands));

        /// <summary>
        /// The RandomizeAllWeight command => RandomizePatientChancesWeight.
        /// </summary>
        public ICommand RandomizeAllWeightCommand => this.randomizeAllWeightCommand ?? (this.randomizeAllWeightCommand = new CommandHandler(this.RandomizePatientChancesWeight, this.canExecuteCommands));

        /// <summary>
        /// The OpenLevelFromFile command => OpenLevelFromFile.
        /// </summary>
        public ICommand OpenLevelFromFileCommand => this.openLevelFromFileCommand ?? (this.openLevelFromFileCommand = new CommandHandler(this.OpenLevelFromFile, this.canExecuteCommands));

        /// <summary>
        /// The SaveLevelToFile command => SaveLevelToFile.
        /// </summary>
        public ICommand SaveLevelToFileCommand => this.saveLevelToFile ?? (this.saveLevelToFile = new CommandHandler(this.SaveLevelToFile, this.canExecuteCommands));

        #endregion Commands

        #endregion Public Properties

        #region Private Properties

        #region Patient

        #endregion Patient

        #region PatientChance

        /// <summary>
        /// Gets or sets the list with all the PatientChance objects in this level.
        /// </summary>
        private List<PatientChance> PatientChanceList
        {
            get => this.patientChanceList;
            set
            {
                foreach (PatientChance patientChance in value)
                {
                    patientChance.ParentLevel = this;
                }

                this.patientChanceList = value;
            }
        }

        #endregion PatientChance

        #endregion Private Properties

        #endregion Properties

        #region Methods

        #region Public

        #region Treatment

        /// <summary>
        /// Updates the AvailbleTreatmentList taking into account customizedWeights for this specific level.
        /// </summary>
        public void UpdateAvailableTreatmentList()
        {
            List<Treatment> treatmentList = Globals.GetSettings.GetTreatmentList(this.CategoryKey, this.GetDifficultyModifier);
            Dictionary<string, int> customTreatmentWeights = Globals.GetSettings.GetCustomizedTreatmentWeights(this.LevelName);

            foreach (Treatment treatment in treatmentList)
            {
                treatment.SetLevelParent(this);

                if (customTreatmentWeights.ContainsKey(treatment.TreatmentName))
                {
                    treatment.CustomizedWeight = customTreatmentWeights[treatment.TreatmentName];
                }
            }

            this.AvailableTreatmentList = treatmentList;
            this.UpdateTreatmentsAvailableString();
        }

        /// <summary>
        /// Updates the available treatments string
        /// </summary>
        public void UpdateTreatmentsAvailableString()
        {
            string output = string.Empty;

            // TODO Make this function dependant whetaher ShowAvailableTreatmentsCheck is true, currently it's not working where it is assumed false.
            if (true)
            {
                // Globals.GetLevelOverview.ShowAvailableTreatmentsCheck
                List<Treatment> treatmentList = this.AvailableTreatmentList;
                if (treatmentList.Count > 0)
                {
                    // Calculate totalWeight
                    int totalWeight = 0;
                    foreach (Treatment treatment in treatmentList)
                    {
                        totalWeight += treatment.Weight;
                    }

                    foreach (Treatment treatment in treatmentList)
                    {
                        output += treatment.TreatmentName + " (" + Math.Round(
                                      (treatment.Weight / (double)totalWeight) * 100,
                                      1).ToString("N1") + "%), ";
                        if (treatment.Weight != treatment.CustomizedWeight)
                        {
                            output += " => CustomWeight: " + treatment.CustomizedWeight.ToString();
                        }

                        output += Environment.NewLine;
                    }
                }
            }

            this.TreatmentsAvailableString = output;
        }

        /// <summary>
        /// This is used in the CustomTreatmentWeightsWindow to show the percentage, updates all treatment percentages.
        /// </summary>
        public void UpdateTreatmentWeightPercentage()
        {
            foreach (Treatment treatment in this.AvailableTreatmentList)
            {
                treatment.UpdatePercentage();
            }
        }

        /// <summary>
        /// Returns the percentage based on the customizedWeight of the Treatment in the CustomTreatmentWeightsWindow to show the percentage
        /// </summary>
        /// <param name="customizedWeight">
        /// The customized weight.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double GetTreatmentWeightPercentage(int customizedWeight)
        {
            int totalWeight = 0;
            foreach (Treatment treatment in this.AvailableTreatmentList)
            {
                totalWeight += treatment.CustomizedWeight;
            }

            double percentage = customizedWeight / (double)totalWeight * 100;
            percentage = Math.Round(percentage, 1, MidpointRounding.AwayFromZero);
            return percentage;
        }

        /// <summary>
        /// Update the maximum amount of treatments visible in each patient
        /// </summary>
        /// <param name="value">
        /// The new amount value.
        /// </param>
        public void UpdateMaxTreatments(int value)
        {
            foreach (Patient patient in this.PatientCollection)
            {
                patient.SetMaxTreatments(value);
            }
        }

        #endregion Treatment

        #region Patient


        public void AddPatient()
        {
            int index = this.PatientCollection.Count;
            string patientName = $"Patient_{index}";
            Patient patientObject = new Patient(this, patientName);
            patientObject.SetMaxTreatments(Globals.GetLevelOverview.MaxTreatmentsVisible);
            this.PatientCollection.Add(patientObject);
            LevelLoaded();

        }

        /// <summary>
        /// Add a PatientObject representing a PatientRow from the LevelScript.
        /// </summary>
        /// <param name="patientData">
        /// The patient data from the LevelScript.
        /// </param>
        /// <param name="index">
        /// The optional patient index.
        /// </param>
        public void AddPatient(string patientData = null, int index = -1)
        {
            if (index == -1)
            {
                index = this.PatientCollection.Count();
            }

            string patientName = $"Patient_{index}";
            Patient patientObject = new Patient(this, patientName);

            if (patientData != null)
            {
                patientObject.SetPatientData(patientData);
            }

            this.PatientCollection.Insert(index, patientObject);
        }


        public void RemovePatient()
        {
            if (this.PatientCollection.Count > 0)
            {
                bool patientRemoved = false;

                for (int i = this.PatientCollection.Count - 1; i >= 0; i--)
                {
                    if (this.PatientCollection[i] != null && this.PatientCollection[i].IsSelected)
                    {
                        this.PatientCollection.RemoveAt(i);
                        patientRemoved = true;
                    }
                }

                // If nothing was removed than by default remove the last row
                if (!patientRemoved)
                {
                    this.PatientCollection.RemoveAt(this.PatientCollection.Count - 1);
                }

                this.LevelLoaded();
            }

        }


        /// <summary>
        /// Set the amount of Patient Objects in this Level.
        /// </summary>
        /// <param name="newAmount">
        /// The new amount.
        /// </param>
        public void SetPatientAmount(int newAmount)
        {
            int patientCount = this.PatientCollection.Count;

            if (newAmount < patientCount)
            {
                int amountToSubtract = patientCount - newAmount;

                for (int i = 1; i <= amountToSubtract; i++)
                {
                    int index = patientCount - i;

                    this.PatientCollection.RemoveAt(index);
                }
                this.UpdateLevelEdited();

            }
            else if (newAmount > patientCount)
            {
                int amountToAdd = newAmount - patientCount;

                for (int i = 0; i < amountToAdd; i++)
                {
                    this.AddPatient();
                }
                this.UpdateLevelEdited();
            }
            this.UpdateLevelOutput();
        }

        /// <summary>
        /// Randomize all treatments in all Patients in this level
        /// </summary>
        /// <param name="treatmentMinValue">
        /// The minimum treatment amount value.
        /// </param>
        /// <param name="treatmentMaxValue">
        /// The maximum treatment amount value.
        /// </param>
        public void RandomizeTreatments(int treatmentMinValue, int treatmentMaxValue)
        {
            // Filter List by treatments with a CustomizedWeight higher than 0
            List<Treatment> treatmentOptions = this.AvailableTreatmentList.FindAll(t => t.CustomizedWeight > 0);

            // https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp/wiki/Getting-Started
            IWeightedRandomizer<string> randomizer = new DynamicWeightedRandomizer<string>();

            foreach (Patient patient in this.PatientCollection)
            {
                // Get the number of treatments to generate.
                int treatmentNumber = Globals.GetRandom.Next(
                    Math.Min(treatmentMinValue, treatmentMaxValue),
                    treatmentMaxValue + 1);

                // Ensure that the treatment number is not highter than the available treatments.
                treatmentNumber = Math.Min(treatmentOptions.Count, treatmentNumber);
                List<string> randomTreatmentNameList = new List<string>();

                // Create the randomizer
                randomizer.Clear();
                foreach (Treatment treatment in treatmentOptions)
                {
                    randomizer.Add(treatment.TreatmentName, treatment.CustomizedWeight);
                }

                for (int i = 0; i < treatmentNumber; i++)
                {
                    if (randomizer.Count == 0 || randomizer.TotalWeight == 0)
                    {
                        break;
                    }

                    string randomTreatmentName = randomizer.NextWithRemoval();

                    randomTreatmentNameList.Add(randomTreatmentName);
                }

                // Convert from String back to Treatments
                List<Treatment> randomTreatmentList = new List<Treatment>();
                foreach (string x in randomTreatmentNameList)
                {
                    foreach (Treatment t in treatmentOptions)
                    {
                        if (x == t.TreatmentName)
                        {
                            randomTreatmentList.Add(t);
                            break;
                        }
                    }
                }

                // Make sure all AtLast treatment are at the end of the list
                int treatmentCount = randomTreatmentList.Count;
                for (int i = 0; i < treatmentCount; i++)
                {
                    Treatment treatment = randomTreatmentList[i];

                    if (randomTreatmentList[i].AlwaysLast)
                    {
                        randomTreatmentList.RemoveAt(i);
                        randomTreatmentList.Add(treatment);
                    }
                }

                // Store new TreatmentList in the patient
                patient.SetTreatments(randomTreatmentList);

            }

            this.UpdateLevelEdited();
        }

        /// <summary>
        /// Randomize all the Delay values in the patientList
        /// </summary>
        public void RandomizeDelay()
        {
            if (this.GetDifficultyModifier > 0)
            {
                double timePerTreatment = GameValues.TimePerTreatment(this.GetDifficultyModifier);
                double timeBetweenPatients = GameValues.TimeBetweenPatients(this.GetDifficultyModifier);
                int checkoutPerPatient = GameValues.CheckoutPerPatient;
                int patientDelayMin = Globals.GetLevelOverview.GeneratePatientDelayMin;
                int patientDelayMax = Globals.GetLevelOverview.GeneratePatientDelayMax;

                for (int patientIndex = 0; patientIndex < this.PatientCollection.Count; patientIndex++)
                {
                    if (timePerTreatment > 0)
                    {
                        Patient patient = this.PatientCollection[patientIndex];
                        int delayModifier = Globals.GetRandom.Next(patientDelayMin, patientDelayMax);
                        int patientTreatmentCount;

                        if (patientIndex == 0)
                        {
                            if (this.GetLevelType != LevelTypeEnum.TimeTrial)
                            {
                                patient.Delay = 1000;
                            }
                            else
                            {
                                patient.Delay = Globals.GetRandom.Next(1000, 9999);
                            }

                            continue;
                        }

                        if (patientIndex > 0 && patientIndex < 2)
                        {
                            patientTreatmentCount = 0;
                        }
                        else
                        {
                            // Take the patientTreatmentCount from two patients ago. 
                            patientTreatmentCount = this.PatientCollection[patientIndex - 2].ValidTreatmentCount;
                        }

                        if (patientTreatmentCount > -1)
                        {
                            // Formula: Delay = (patientTreatmentCount * timePerTreatment) + timeBetweenPatients + checkoutPerPatient + delayModifier;
                            double tmpDelay = patientTreatmentCount * timePerTreatment;
                            tmpDelay += timeBetweenPatients;
                            tmpDelay += checkoutPerPatient;

                            // Add Delay Modifier
                            tmpDelay += delayModifier;

                            // MROUND In Excel, round down to the nearest multiple of 100
                            tmpDelay = tmpDelay - (tmpDelay % 100);
                            patient.Delay = Convert.ToInt32(Math.Round(tmpDelay, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            patient.Delay = -1;
                        }
                    }
                }

                this.UpdateLevelEdited();

                this.UpdateLevelOutput();
            }
        }

        /// <summary>
        /// The randomize the Patient weights.
        /// </summary>
        /// <param name="weightMinValue">
        /// The weight min value.
        /// </param>
        /// <param name="weightMaxValue">
        /// The weight max value.
        /// </param>
        public void RandomizePatientWeight(int weightMinValue, int weightMaxValue)
        {
            foreach (Patient patient in this.PatientCollection)
            {
                patient.Weight = Globals.GetRandom.Next(Math.Min(weightMinValue, weightMaxValue), weightMaxValue);
            }
            this.UpdateLevelEdited();
        }

        #endregion

        #region PatientChance

        /// <summary>
        /// This is used in the PatientChances Tab to show the percentage, updates all PatientChance percentages.
        /// </summary>
        public void UpdatePatientChancePercentage()
        {
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                patientChance.UpdatePercentage();
            }

            this.UpdateLevelOutput();
        }

        /// <summary>
        /// Return the percentage based on the weight / totalWeight.
        /// </summary>
        /// <param name="weight">
        /// The weight.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double GetPatientChancePercentage(int weight)
        {
            int totalWeight = 0;
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                totalWeight += patientChance.Weight;
            }

            double percentage = weight / (double)totalWeight * 100;
            percentage = Math.Round(percentage, 1, MidpointRounding.AwayFromZero);
            return percentage;
        }

        /// <summary>
        /// (De)Select all the PatientChance rows in the overview
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        public void SelectAllPatientChances(bool state)
        {
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                patientChance.IsSelected = state;
            }
        }

        /// <summary>
        /// The randomize patient chances weight.
        /// </summary>
        public void RandomizePatientChancesWeight()
        {
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                if (patientChance.IsSelected)
                {
                    patientChance.RandomizeWeight(Globals.GetRandom.Next(1, 100));
                }
            }
            this.UpdateLevelEdited();
        }

        #endregion PatientChance

        #region LevelScript

        /// <summary>
        /// Update the LevelScript output to the Front-End.
        /// </summary>
        public void UpdateLevelOutput()
        {
            if (Globals.GetLevelOverview.AllowLevelScriptUpdate)
            {
                this.LevelScriptBuffer = this.GetNewLevelScript;
                this.OnPropertyChanged("LevelScriptBuffer");
            }

        }

        /// <summary>
        /// Call this function when the Level is loaded. 
        /// </summary>
        public void LevelLoaded()
        {
            this.OnPropertyChanged("PatientList");
            this.OnPropertyChanged("PatientCollection");

            foreach (Patient patient in this.PatientCollection)
            {
                if (patient != null)
                {
                    patient.TreatmentsUpdated();
                }
            }

            this.UpdateLevelOutput();
        }

        /// <summary>
        /// Call this method when the level has been edited. 
        /// </summary>
        public void UpdateLevelEdited()
        {
            if (this.LevelFinishedLoading && !this.IsEdited)
            {
                this.IsEdited = true;
                Globals.GetLevelOverview.UpdateLevelList();
            }
        }

        #endregion LevelScript

        #endregion Public

        #region Private

        #region ParseData

        /// <summary>
        /// Read from the LevelScript and parse everything into workable variables.
        /// </summary>
        private void ParseRawText()
        {
            string rawLevelText = this.GetCurrentLevelScript;

            // Clean up the text by removing tabs, enters and spaces and special characters.
            rawLevelText = rawLevelText.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("{\n", "{")
                .Replace(" ", string.Empty).Replace("\n\n", string.Empty);

            // Parse possible GamePlayCharacters
            rawLevelText = this.ParseGameplayCharacter(rawLevelText);

            // Parse DesignToolData embeded in file.
            string startDesignToolData = "--[[HM4DesignToolData:";
            string endDesignToolData = "--]]";
            if (rawLevelText.Contains(startDesignToolData) && rawLevelText.Contains(endDesignToolData))
            {
                int startDesignToolDataIndex = rawLevelText.IndexOf(startDesignToolData, StringComparison.Ordinal);
                int endDesignToolDataIndex = rawLevelText.IndexOf(endDesignToolData, StringComparison.Ordinal) + endDesignToolData.Length;

                if (startDesignToolDataIndex > -1 && endDesignToolDataIndex - endDesignToolData.Length > -1
                                                  && startDesignToolDataIndex < endDesignToolDataIndex)
                {
                    string designToolDataText = rawLevelText.Substring(
                        startDesignToolDataIndex,
                        endDesignToolDataIndex - startDesignToolDataIndex);
                    this.designToolData.ParseDesignData(designToolDataText);
                    rawLevelText = rawLevelText.Replace(designToolDataText, string.Empty);
                }
            }

            rawLevelText = this.ParsePatientChances(rawLevelText);

            string unused = this.ParsePatientTriggers(rawLevelText);

        }

        /// <summary>
        /// The parse gameplay character from the LevelScript.
        /// </summary>
        /// <param name="rawLevelText">
        /// The raw level text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ParseGameplayCharacter(string rawLevelText)
        {
            const string GameplayCharacterStartText = "levelDesc.gameplayCharacterId=\"";
            const string GameplayCharacterEndText = "\"";
            if (rawLevelText.Contains(GameplayCharacterStartText))
            {
                int startGameplayCharacterIndex = rawLevelText.IndexOf(GameplayCharacterStartText, StringComparison.Ordinal)
                                                  + GameplayCharacterStartText.Length;
                int endGameplayCharacterIndex = rawLevelText.IndexOf(GameplayCharacterEndText, startGameplayCharacterIndex, StringComparison.Ordinal);

                this.GameplayCharacter = rawLevelText.Substring(startGameplayCharacterIndex, endGameplayCharacterIndex - startGameplayCharacterIndex);

                rawLevelText = rawLevelText.Replace($"{GameplayCharacterStartText}{this.GameplayCharacter}{GameplayCharacterEndText}", string.Empty);
            }

            // Assuming the levelDesc.decoId is the same GameplayCharacter then just remove it. 
            const string DecoStartText = "levelDesc.decoId=\"";
            const string DecoEndText = "\"";
            if (rawLevelText.Contains(DecoStartText))
            {
                int startDecoIndex = rawLevelText.IndexOf(DecoStartText, StringComparison.Ordinal);
                int endDecoIndex = rawLevelText.IndexOf(DecoEndText, startDecoIndex + DecoStartText.Length, StringComparison.Ordinal) + DecoEndText.Length;
                rawLevelText = rawLevelText.Remove(startDecoIndex, endDecoIndex - startDecoIndex);
            }

            return rawLevelText;
        }

        /// <summary>
        /// Parse the patient chances into workable PatientChance objects
        /// </summary>
        /// <param name="rawLevelText">
        /// The raw level text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> left over rawLevelText that can be parsed by other functions.
        /// </returns>
        private string ParsePatientChances(string rawLevelText)
        {
            // Parse the PatientChances section.
            const string StartPatientChancesText = "levelDesc.patientChances=";
            const string EndPatientChancesText = "}";
            if (rawLevelText.Contains(StartPatientChancesText) && rawLevelText.Contains(EndPatientChancesText))
            {
                int startPatientChancesIndex = rawLevelText.IndexOf(StartPatientChancesText, StringComparison.Ordinal);
                int endPatientChancesIndex = rawLevelText.IndexOf(EndPatientChancesText, StringComparison.Ordinal) + EndPatientChancesText.Length;

                if (startPatientChancesIndex > -1 && endPatientChancesIndex - EndPatientChancesText.Length > -1 && startPatientChancesIndex < endPatientChancesIndex)
                {
                    // Do some extra formatting and cleaning up.
                    string patientsChancesRawText = rawLevelText.Substring(startPatientChancesIndex, endPatientChancesIndex - startPatientChancesIndex);
                    rawLevelText = rawLevelText.Remove(startPatientChancesIndex, endPatientChancesIndex);

                    // Filter all new lines, remove the unnecessary text
                    patientsChancesRawText = patientsChancesRawText.Replace("/n", string.Empty);
                    patientsChancesRawText = patientsChancesRawText.Replace(StartPatientChancesText, string.Empty).Replace(",}", "}");
                    patientsChancesRawText = patientsChancesRawText.Trim(' ').Replace("=", ":");

                    // Translate to workable Dictionary, use double because some weights have been set als float. Always save back as an integer. 
                    Dictionary<string, double> patientChancesDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, double>>(patientsChancesRawText);

                    foreach (KeyValuePair<string, double> patientChance in patientChancesDict)
                    {
                        bool patientChanceExist = false;

                        foreach (PatientChance patientChanceListItem in this.PatientChanceList)
                        {
                            if (patientChanceListItem.PatientName == patientChance.Key)
                            {
                                patientChanceExist = true;
                                patientChanceListItem.Weight = Convert.ToInt32(patientChance.Value);
                                break;
                            }
                        }

                        // Create new entry if patientChance does not exist.
                        if (!patientChanceExist)
                        {
                            PatientChance newPatientChance = new PatientChance(patientChance.Key, Convert.ToInt32(patientChance.Value));
                            newPatientChance.ParentLevel = this;
                            this.PatientChanceList.Add(newPatientChance);
                        }
                    }
                }
            }

            // Return to continue parsing in other functions
            return rawLevelText;
        }

        /// <summary>
        /// Parse the patient triggers into workable Patient objects
        /// </summary>
        /// <param name="rawLevelText">
        /// The raw level text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> left over rawLevelText that can be parsed by other functions.
        /// </returns>
        private string ParsePatientTriggers(string rawLevelText)
        {
            // Parse the PatientList with treatments
            const string StartPatientTriggerText = "levelDesc.triggers=";
            const string EndPatientTriggerText = "},\n}";
            string[] delimiter = { "},\n" };

            if (rawLevelText.Contains(StartPatientTriggerText) && rawLevelText.Contains(EndPatientTriggerText))
            {
                // Determin the exact range of the text we need to parse.
                int startPatientTriggerIndex = rawLevelText.IndexOf(StartPatientTriggerText, StringComparison.Ordinal);
                int endPatientTriggerIndex = rawLevelText.IndexOf(EndPatientTriggerText, StringComparison.Ordinal) + EndPatientTriggerText.Length;

                if (startPatientTriggerIndex > -1 && endPatientTriggerIndex - EndPatientTriggerText.Length > -1 && startPatientTriggerIndex < endPatientTriggerIndex)
                {
                    string patientsTriggersRawText = rawLevelText.Substring(startPatientTriggerIndex, endPatientTriggerIndex - startPatientTriggerIndex);

                    // Filter all new lines, remove the unnecessary text
                    patientsTriggersRawText = patientsTriggersRawText.Replace(StartPatientTriggerText, string.Empty).TrimEnd('}');

                    // Remove the first new line
                    if (patientsTriggersRawText.StartsWith("\n"))
                    {
                        patientsTriggersRawText = patientsTriggersRawText.Remove(0, 2);
                    }

                    if (patientsTriggersRawText.StartsWith("{{"))
                    {
                        patientsTriggersRawText = patientsTriggersRawText.Remove(0, 1);
                    }

                    // Split into seperate items divided by PatientTrigger
                    List<string> patientTriggers = patientsTriggersRawText.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();

                    for (int i = 0; i < patientTriggers.Count; i++)
                    {
                        // Remove all newlines ("\n") and tabs ("\t")
                        string correctedEntry = patientTriggers[i].Replace("\n", string.Empty).Replace("\t", string.Empty);

                        // Add the delimiter back again at the end. 
                        patientTriggers[i] = $"{correctedEntry}}}";
                    }

                    // Create all the patients
                    foreach (string patientTrigger in patientTriggers)
                    {
                        if (patientTrigger.Length > 5)
                        {
                            this.AddPatient(patientTrigger);
                        }
                    }
                }
            }

            return rawLevelText;
        }

        #endregion ParseData

        #region ReadWrite

        /// <summary>
        /// Save level to File
        /// </summary>
        private void SaveLevelToFile()
        {
            string levelScript = Globals.GetLevelOverview.AllowLevelScriptUpdate ? this.GetNewLevelScript : this.LevelScriptBuffer;

            string path = Globals.GetSettings.ProjectPathLevel + this.FileName;
            if (File.Exists(path))
            {
                File.WriteAllText(path, levelScript);
                this.OnPropertyChanged("GetCurrentLevelScript");
            }
        }

        /// <summary>
        /// Open Level in external editor
        /// </summary>
        private void OpenLevelFromFile()
        {
            string fileName = Globals.GetSettings.ProjectPathLevel + this.FileName;
            if (File.Exists(fileName))
            {
                Process.Start(fileName);
            }
        }

        #endregion ReadWrite

        #endregion Private

        #endregion Methods

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

        #endregion
    }
}