using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using DataNameSpace;

using Utilities;

using Weighted_Randomizer;

namespace LevelData
{
    using System.Diagnostics;
    using System.IO;

    public class DesignToolData : INotifyPropertyChanged
    {
        private const string DifficultyLevelText = "DifficultyLevel:";
        private const string endDesignToolDataText = "--]]";
        private const string LevelTypeText = "LevelType:";
        private const string RoomIndexText = "RoomIndex:";
        private const string startDesignToolDataText = "--[[HM4DesignToolData:";
        private double _difficultyLevel = 0;
        private LevelTypeEnum _levelType = LevelTypeEnum.Unknown;
        private int _roomIndex = 0;

        public DesignToolData()
        {
        }

        public double DifficultyLevel
        {
            get => this._difficultyLevel;
            set
            {
                this._difficultyLevel = value;
                this.OnPropertyChanged("DifficultyLevel");
            }
        }

        public LevelTypeEnum LevelType
        {
            get => this._levelType;
            set
            {
                this._levelType = value;
                this.OnPropertyChanged("LevelType");
            }
        }

        public string LevelTypeString
        {
            get => Enum.GetName(typeof(LevelTypeEnum), this.LevelType);
            set => this.LevelType = (LevelTypeEnum)Enum.Parse(typeof(LevelTypeEnum), value);
        }

        public int Roomindex
        {
            get => this._roomIndex;
            set
            {
                this._roomIndex = value;
                this.OnPropertyChanged("Roomindex");
            }
        }

        public void ParseDesignData(string designToolData)
        {
            designToolData = designToolData.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty);
            designToolData.Replace(startDesignToolDataText, string.Empty).Replace(endDesignToolDataText, string.Empty);
            string[] delimiter = { "\n" };
            List<string> designToolList = designToolData.Split(delimiter, StringSplitOptions.None).ToList<string>();

            foreach (string entry in designToolList)
            {
                string textItem = entry;

                if (textItem.Contains(RoomIndexText))
                {
                    textItem = textItem.Replace(RoomIndexText, string.Empty);
                    textItem = Globals.FilterToNumerical(textItem);
                    this.Roomindex = Convert.ToInt32(textItem);
                }

                if (textItem.Contains(DifficultyLevelText))
                {
                    textItem = textItem.Replace(DifficultyLevelText, string.Empty);
                    this.DifficultyLevel = (float)Convert.ToDouble(textItem);
                }

                if (textItem.Contains(LevelTypeText))
                {
                    textItem = textItem.Replace(LevelTypeText, string.Empty);
                    switch (textItem)
                    {
                        case "Bonus":
                            this.LevelType = LevelTypeEnum.Bonus;
                            break;

                        case "Story":
                            this.LevelType = LevelTypeEnum.Story;
                            break;

                        case "MiniGame":
                            this.LevelType = LevelTypeEnum.MiniGame;
                            break;

                        case "TimeTrial":
                            this.LevelType = LevelTypeEnum.TimeTrial;
                            break;

                        case "Oliver":
                            this.LevelType = LevelTypeEnum.OliverOne;
                            break;

                        case "OliverOne":
                            this.LevelType = LevelTypeEnum.OliverOne;
                            break;

                        case "OliverAll":
                            this.LevelType = LevelTypeEnum.OliverAll;
                            break;

                        default:
                            this.LevelType = LevelTypeEnum.Unknown;
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            string output = startDesignToolDataText + Environment.NewLine;

            if (this.Roomindex > 0)
            {
                output += RoomIndexText + " \t" + this.Roomindex.ToString() + Environment.NewLine;
            }

            if (this.DifficultyLevel > 0)
            {
                output += DifficultyLevelText + " \t" + this.DifficultyLevel.ToString() + Environment.NewLine;
            }

            if (this.LevelType > 0)
            {
                output += LevelTypeText + " \t" + this.LevelType.ToString() + Environment.NewLine;
            }

            output += endDesignToolDataText + Environment.NewLine;
            return output;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }

    /// <inheritdoc />
    /// <summary>
    /// Level Object which stores all the data related to the Level.
    /// </summary>
    public class Level : INotifyPropertyChanged
    {
        #region LevelProperties

        public string CategoryKey;

        /// <summary>
        /// Gets or sets the level name.
        /// </summary>
        private string levelName;
        public string LevelName { get => this.levelName; set => this.levelName = value; }

        public string FileName => $"{this.LevelName}.lua";

        public bool WeightEnabled => this.designToolData.LevelType == LevelTypeEnum.TimeTrial;

        // Patient Chances Objects
        public List<PatientChance> PatientChanceList = new List<PatientChance> { };


        private List<string> _treatmentOptions = new List<string> { };


        private DesignToolData designToolData = new DesignToolData();

        private List<Patient> patientDatabase = new List<Patient> { };
        public ObservableCollection<PatientChance> PatientChanceCollection
        {
            get => new ObservableCollection<PatientChance>(this.PatientChanceList);
            set
            {
                this.PatientChanceList = value.ToList<PatientChance>();
                this.OnPropertyChanged("PatientChanceCollection");
            }
        }

        // Patient Row Objects
        public ObservableCollection<Patient> PatientCollection
        {
            get
            {
                ObservableCollection<Patient> patientCollection = new ObservableCollection<Patient>();
                if (this.patientDatabase.Count > 0)
                {
                    foreach (Patient patient in this.patientDatabase)
                    {
                        patientCollection.Add(patient);
                    }
                }

                return patientCollection;
            }

            set
            {
                this.patientDatabase = value.ToList();
                this.OnPropertyChanged();
            }
        }


        #endregion LevelProperties

        public Level(string levelName)
        {
            this.LevelName = levelName;
            this.CategoryKey = Globals.GetCategoryKey(this.GetRoomIndex);

            // Ensure all possible PatientTypeChances are set in PatientChanceList from the Settings.
            this.PatientChanceList = Globals.GetSettings.GetPatientChanceList(this.CategoryKey);
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                patientChance.ParentLevel = this;
            }
            // Set ParentLevel

            // Translate the levelScript into workable variables.
            this.ParseRawText(levelName);

            this.UpdateAvailableTreatmentList();
            this.UpdateLevelOutput();
            this._canExecute = true;
        }

        #region Getters

        public Dictionary<string, int> RandomTreatmentDictionary = new Dictionary<string, int> { };


        private List<Treatment> _availableTreatmentList = new List<Treatment> { };

        private DataTable _patientSimulateDataTable = new DataTable();

        private string _treatmentsAvailableString = string.Empty;

        public List<Treatment> AvailableTreatmentList
        {
            get => this._availableTreatmentList;
            set
            {
                this._availableTreatmentList = value;
                this.OnPropertyChanged("AvailableTreatmentList");
            }
        }



        // Design Tool Data
        public string GetDesignToolData => this.designToolData.ToString();

        public double GetDifficultyModifier
        {
            get => this.designToolData.DifficultyLevel;
            set
            {
                this.designToolData.DifficultyLevel = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("GetTreatmentOptions");
                this.OnPropertyChanged("GetTreatmentsAvailableString");
                this.UpdateAvailableTreatmentList();
                Globals.GetLevelOverview.UpdateRandomRecommendations();
            }
        }

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

        public string GetLevelTypeString
        {
            get => this.designToolData.LevelTypeString;
            set
            {
                this.designToolData.LevelTypeString = value;
                this.OnPropertyChanged("GetDesignToolData");
                this.OnPropertyChanged("WeightEnabled");
                this.UpdateLevelOutput();
            }
        }


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
                    int roomIndex = -1;

                    // Generate the RoomIndex based on the naming convention of the levelName
                    if (this.LevelName != string.Empty)
                    {
                        // Assuming the room index is in r2_b04... naming convention]
                        if (this.LevelName.StartsWith("level"))
                        {
                            int levelIndex = Convert.ToInt16(this.LevelName.Replace("level", string.Empty));
                            while (roomIndex < 100)
                            {
                                int minIndex = (roomIndex - 1) * 10 + 1;
                                int maxIndex = (roomIndex * 10) + 1;

                                if (Enumerable.Range(minIndex, maxIndex).Contains(levelIndex))
                                {
                                    break;
                                }

                                roomIndex++;
                            }
                        }
                        else if (this.LevelName.StartsWith("r"))
                        {
                            roomIndex = Convert.ToInt16(Globals.FilterToNumerical(this.LevelName.Substring(1, 1)));
                        }
                        else
                        {
                            roomIndex = -1;
                        }
                    }

                    // Store the newly generated RoomIndex
                    this.GetRoomIndex = roomIndex;
                    return roomIndex;
                }
            }

            set
            {
                this.designToolData.Roomindex = value;
                this.OnPropertyChanged("GetDesignToolData");
                this.UpdateLevelOutput();
            }
        }

        public ObservableCollection<string> GetTreatmentOptions
        {
            get
            {
                List<string> treatmentListString = new List<string> { string.Empty };

                // String categoryKey = Globals.GetCategoryKey(GetRoomIndex);
                if (this.GetRoomIndex > -1 && this.CategoryKey != string.Empty)
                {
                    List<Treatment> treatmentList = Globals.GetSettings.GetTreatmentList(this.CategoryKey);

                    foreach (Treatment treatment in treatmentList)
                    {
                        // Only add treatments with a sufficient DifficultyModifer
                        if (treatment.DifficultyUnlocked <= this.GetDifficultyModifier)
                        {
                            treatmentListString.Add(treatment.TreatmentName);
                        }
                    }
                }

                return new ObservableCollection<string>(treatmentListString);
            }
        }

        public DataTable PatientSimulateDataTable => this._patientSimulateDataTable;

        public string TreatmentsAvailableString
        {
            get => this._treatmentsAvailableString;
            set
            {
                this._treatmentsAvailableString = value;
                this.OnPropertyChanged("TreatmentsAvailableString");
            }
        }

        // Patient Chance functions
        public double GetPatientChancePercentage(int Weight)
        {
            int TotalWeight = 0;
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                TotalWeight += patientChance.Weight;
            }

            double Percentage = Weight / (double)TotalWeight * 100;
            Percentage = Math.Round(Percentage, 1, MidpointRounding.AwayFromZero);
            return Percentage;
        }

        public double GetTreatmentWeightPercentage(int CustomizedWeight)
        {
            int TotalWeight = 0;
            foreach (Treatment treatment in this.AvailableTreatmentList)
            {
                TotalWeight += treatment.CustomizedWeight;
            }

            double Percentage = CustomizedWeight / (double)TotalWeight * 100;
            Percentage = Math.Round(Percentage, 1, MidpointRounding.AwayFromZero);
            return Percentage;
        }

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

        public void UpdatePatientChancePercentage()
        {
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                patientChance.UpdatePercentage();
            }

            this.UpdateLevelOutput();
        }

        public void UpdateTreatmentsAvailableString()
        {
            string output = string.Empty;

            // TODO Make this function dependant whetaher ShowAvailableTreatmentsCheck is true, currently it's not working where it is assumed false.
            if (Globals.GetLevelOverview.ShowAvailableTreatmentsCheck || true)
            {
                List<Treatment> treatmentList = this.AvailableTreatmentList;
                if (treatmentList.Count > 0)
                {
                    // Calculate totalWeight
                    int TotalWeight = 0;
                    foreach (Treatment treatment in treatmentList)
                    {
                        TotalWeight += treatment.Weight;
                    }

                    foreach (Treatment treatment in treatmentList)
                    {
                        output += treatment.TreatmentName + " (" + Math.Round(
                                      (treatment.Weight / Convert.ToDouble(TotalWeight)) * 100,
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

        public void UpdateTreatmentWeightPercentage()
        {
            foreach (Treatment treatment in this.AvailableTreatmentList)
            {
                treatment.UpdatePercentage();
            }
        }

        #endregion Getters

        public void AddPatient(string patientData = null, int index = -1)
        {
            if (index == -1)
            {
                index = this.patientDatabase.Count();
            }

            string patientName = "Patient_" + index.ToString();
            Patient patientObject = new Patient(this, patientName);

            if (patientData != null)
            {
                patientObject.SetPatientData(patientData);
            }

            this.patientDatabase.Insert(index, patientObject);
        }

        public void RandomizeTreatments(int treatmentMinValue, int treatmentMaxValue)
        {
            // Filter List by treatments with a CustomizedWeight higher than 0
            List<Treatment> treatmentOptions = this.AvailableTreatmentList.FindAll(delegate (Treatment t) { return t.CustomizedWeight > 0; });

            // https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp/wiki/Getting-Started
            IWeightedRandomizer<string> randomizer = new DynamicWeightedRandomizer<string>();

            Random random = new Random();

            foreach (Patient patient in this.PatientCollection)
            {
                // Get the number of treatments to generate.
                int treatmentNumber = random.Next(
                    Math.Min(treatmentMinValue, treatmentMaxValue),
                    treatmentMaxValue + 1);

                // Ensure that the treatment number is not highter than the available treatments.
                treatmentNumber = Math.Min(treatmentOptions.Count, treatmentNumber);
                List<string> RandomTreatmentNameList = new List<string> { };

                // Create the randomizer
                randomizer.Clear();
                for (int i = 0; i < treatmentOptions.Count; i++)
                {
                    randomizer.Add(treatmentOptions[i].TreatmentName, treatmentOptions[i].CustomizedWeight);
                }

                for (int i = 0; i < treatmentNumber; i++)
                {
                    if (randomizer.Count == 0 || randomizer.TotalWeight == 0)
                    {
                        break;
                    }

                    string randomTreatmentName = randomizer.NextWithRemoval();

                    RandomTreatmentNameList.Add(randomTreatmentName);
                }

                // Convert from String back to Treatments
                List<Treatment> randomTreatmentList = new List<Treatment> { };
                foreach (string x in RandomTreatmentNameList)
                {
                    foreach (Treatment treatment in treatmentOptions)
                    {
                        if (x == treatment.TreatmentName)
                        {
                            randomTreatmentList.Add(treatment);
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
        }

        public void SelectAllPatientChances(bool State)
        {
            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                patientChance.IsSelected = State;
            }
        }

        public void SetPatientAmount(int NewAmount)
        {
            int patientCount = this.PatientCollection.Count;

            if (NewAmount < patientCount)
            {
                int amountToSubtract = patientCount - NewAmount;

                for (int i = 1; i <= amountToSubtract; i++)
                {
                    int index = patientCount - i;

                    this.PatientCollection.RemoveAt(index);
                }
            }
            else if (NewAmount > patientCount)
            {
                int amountToAdd = NewAmount - patientCount;

                for (int i = 0; i < amountToAdd; i++)
                {
                    this.AddPatient();
                }
            }
        }



        internal void RandomizePatientChancesWeight()
        {
            Random rnd = new Random();

            foreach (PatientChance patientChance in this.PatientChanceList)
            {
                if (patientChance.IsSelected)
                {
                    patientChance.RandomizeWeight(rnd.Next(1, 100));
                }
            }
        }

        internal void UpdateMaxTreatments(int value)
        {
            foreach (Patient patient in this.PatientCollection)
            {
                patient.SetMaxTreatments(value);
            }
        }

        private void ParseRawText(string levelName)
        {
            string rawLevelText = this.GetCurrentLevelScript;

            // Clean up the text by removing tabs, enters and spaces and special characters.
            rawLevelText = rawLevelText.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("{\n", "{")
                .Replace(" ", string.Empty).Replace("\n\n", string.Empty);

            // Parse DesignToolData embeded in file.
            string startDesignToolData = "--[[HM4DesignToolData:";
            string endDesignToolData = "--]]";
            string designToolDataText;
            if (rawLevelText.Contains(startDesignToolData) && rawLevelText.Contains(endDesignToolData))
            {
                int startDesignToolDataIndex = rawLevelText.IndexOf(startDesignToolData);
                int endDesignToolDataIndex = rawLevelText.IndexOf(endDesignToolData) + endDesignToolData.Length;

                if (startDesignToolDataIndex > -1 && endDesignToolDataIndex - endDesignToolData.Length > -1
                                                  && startDesignToolDataIndex < endDesignToolDataIndex)
                {
                    designToolDataText = rawLevelText.Substring(
                        startDesignToolDataIndex,
                        endDesignToolDataIndex - startDesignToolDataIndex);
                    this.designToolData.ParseDesignData(designToolDataText);
                    rawLevelText = rawLevelText.Replace(designToolDataText, string.Empty);
                }
            }

            // Parse the PatientChances section.
            string startPatientChancesText = "levelDesc.patientChances=";
            string endPatientChancesText = "}";
            string patientsChancesRawText;
            if (rawLevelText.Contains(startPatientChancesText) && rawLevelText.Contains(endPatientChancesText))
            {
                int startPatientChancesIndex = rawLevelText.IndexOf(startPatientChancesText);
                int endPatientChancesIndex = rawLevelText.IndexOf(endPatientChancesText) + endPatientChancesText.Length;

                if (startPatientChancesIndex > -1 && endPatientChancesIndex - endPatientChancesText.Length > -1
                                                  && startPatientChancesIndex < endPatientChancesIndex)
                {
                    // Do some extra formatting and cleaning up.
                    patientsChancesRawText = rawLevelText.Substring(
                        startPatientChancesIndex,
                        endPatientChancesIndex - startPatientChancesIndex);
                    rawLevelText = rawLevelText.Remove(startPatientChancesIndex, endPatientChancesIndex);

                    patientsChancesRawText = patientsChancesRawText.Replace("levelDesc.patientChances=", string.Empty)
                        .Replace(",}", "}");
                    patientsChancesRawText = patientsChancesRawText.Trim(' ').Replace("=", ":");

                    // Translate to workable Dictionary
                    Dictionary<string, int> patientChancesDict =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(patientsChancesRawText);

                    foreach (KeyValuePair<string, int> patientChance in patientChancesDict)
                    {
                        foreach (PatientChance patientChanceListItem in this.PatientChanceList)
                        {
                            if (patientChanceListItem.PatientName == patientChance.Key)
                            {
                                patientChanceListItem.Weight = patientChance.Value;
                                break;
                            }
                        }

                        // TODO Remove this if undefined PatientTypeChances in the Settings should not be added from the LevelScript.
                        // PatientChanceList.Add(new PatientChance(patientChance.Key, patientChance.Value));
                    }
                }
            }

            // Parse the PatientList with treatments
            string startPatientTriggerText = "levelDesc.triggers=";
            string endPatientTriggerText = "},\n}";
            string patientsTriggersRawText;
            if (rawLevelText.Contains(startPatientTriggerText) && rawLevelText.Contains(endPatientTriggerText))
            {
                int startPatientTriggerIndex = rawLevelText.IndexOf(startPatientTriggerText);
                int endPatientTriggerIndex = rawLevelText.IndexOf(endPatientTriggerText) + endPatientTriggerText.Length;

                if (startPatientTriggerIndex > -1 && endPatientTriggerIndex - endPatientChancesText.Length > -1
                                                  && startPatientTriggerIndex < endPatientTriggerIndex)
                {
                    patientsTriggersRawText = rawLevelText.Substring(
                        startPatientTriggerIndex,
                        endPatientTriggerIndex - startPatientTriggerIndex);
                    rawLevelText = rawLevelText.Remove(startPatientTriggerIndex, endPatientTriggerIndex);

                    patientsTriggersRawText = patientsTriggersRawText
                        .Replace(startPatientTriggerText + "\n{", string.Empty).TrimEnd('}');

                    string[] delimiter = { "},\n" };
                    List<string> patientTriggers = patientsTriggersRawText.Split(delimiter, StringSplitOptions.None)
                        .ToList<string>();

                    foreach (string patientTrigger in patientTriggers)
                    {
                        this.AddPatient(patientTrigger);
                    }
                }
            }

            // Isolate any previous comments at the start of the text
            string startComments;
            if (rawLevelText.Contains(startDesignToolData))
            {
                int startDesignToolDataIndex = rawLevelText.IndexOf(startPatientChancesText);
                if (startDesignToolDataIndex > 0)
                {
                    startComments = rawLevelText.Substring(0, startDesignToolDataIndex);
                }
            }
            else if (rawLevelText.Contains(startDesignToolData))
            {
                int startPatientChancesIndex = rawLevelText.IndexOf(startPatientChancesText);
                if (startPatientChancesIndex > 0)
                {
                    startComments = rawLevelText.Substring(0, startPatientChancesIndex);
                }
            }
            else if (rawLevelText.Contains(startPatientTriggerText))
            {
                int startPatientTriggerIndex = rawLevelText.IndexOf(startPatientTriggerText);
                if (startPatientTriggerIndex > 0)
                {
                    startComments = rawLevelText.Substring(0, startPatientTriggerIndex);
                }
            }
        }

        /// <summary>
        /// The save level to file.
        /// </summary>
        private void SaveLevelToFile()
        {

            string levelScript = this.AllowLevelScriptUpdate ? this.GetNewLevelScript : this.LevelScriptBuffer;

            string path = Globals.GetSettings.projectPathLevel + this.FileName;
            if (File.Exists(path))
            {
                File.WriteAllText(path, levelScript);
                this.OnPropertyChanged("GetCurrentLevelScript");
            }
        }

        private void OpenLevelFromFile()
        {
            string fileName = Globals.GetSettings.projectPathLevel + this.FileName;
            if (File.Exists(fileName))
            {
                Process.Start(fileName);
            }

        }

        #region LevelScript
        private bool _allowLevelScriptUpdate = true;

        public bool AllowLevelScriptUpdate
        {
            get => this._allowLevelScriptUpdate;
            set
            {
                this._allowLevelScriptUpdate = value;
                this.UpdateLevelOutput();
            }
        }
        private string _currentLevelScript = null;
        public string GetCurrentLevelScript
        {
            get
            {
                this._currentLevelScript = LevelOverview.ReadLevelTextFromFile(this.LevelName);

                return this._currentLevelScript;
            }
        }
        public string GetNewLevelScript
        {
            get
            {
                string output = string.Empty;

                output += this.designToolData.ToString();
                output += Environment.NewLine;

                // Output the patient chances
                if (this.PatientChanceList.Count > 0)
                {
                    output += "levelDesc.patientChances = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    int i = 0;
                    foreach (PatientChance patientChance in this.PatientChanceList)
                    {
                        if (patientChance.Weight > 0)
                        {
                            output += "\t" + patientChance.PatientName + " \t= \t" + patientChance.Weight;
                            if (i < this.PatientChanceList.Count - 1)
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
                if (this.patientDatabase.Count() > 0)
                {
                    output += "levelDesc.triggers = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    foreach (Patient patient in this.patientDatabase)
                    {
                        output += patient.ToString();
                    }

                    output += "}" + Environment.NewLine;
                }

                return output;
            }
        }


        private string _levelScriptBuffer = string.Empty;
        public string LevelScriptBuffer
        {
            get => this._levelScriptBuffer;
            set
            {
                this._levelScriptBuffer = value;
                this.OnPropertyChanged(this.LevelScriptBuffer);
            }
        }

        public void UpdateLevelOutput()
        {
            if (this.AllowLevelScriptUpdate)
            {
                this.LevelScriptBuffer = this.GetNewLevelScript;
                this.OnPropertyChanged("LevelScriptBuffer");
            }
        }

        #endregion


        #region Commands

        private bool _canExecute;
        private ICommand _randomizeAllWeightCommand;
        private ICommand _openLevelFromFileCommand;
        private ICommand _saveLevelToFile;

        // ReSharper disable once UnusedMember.Global
        public ICommand RandomizeAllWeightCommand => this._randomizeAllWeightCommand ?? (this._randomizeAllWeightCommand = new CommandHandler(this.RandomizePatientChancesWeight, this._canExecute));

        // ReSharper disable once UnusedMember.Global
        public ICommand OpenLevelFromFileCommand => this._openLevelFromFileCommand ?? (this._openLevelFromFileCommand = new CommandHandler(this.OpenLevelFromFile, this._canExecute));

        // ReSharper disable once UnusedMember.Global
        public ICommand SaveLevelToFileCommand => this._saveLevelToFile ?? (this._saveLevelToFile = new CommandHandler(this.SaveLevelToFile, this._canExecute));

        #endregion Commands

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}