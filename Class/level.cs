﻿using DataNameSpace;
using NaturalSort.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Weighted_Randomizer;

namespace LevelData
{
    public class DesignToolData : INotifyPropertyChanged
    {
        private int _roomIndex = 0;
        private double _difficultyLevel = 0;
        private LevelTypeEnum _levelType = LevelTypeEnum.Unknown;

        public int Roomindex
        {
            get
            {
                return _roomIndex;
            }
            set
            {
                _roomIndex = value;
            }
        }

        public double DifficultyLevel
        {
            get
            {
                return _difficultyLevel;
            }
            set
            {
                _difficultyLevel = value;
            }
        }

        public LevelTypeEnum LevelType
        {
            get
            {
                return _levelType;
            }
            set
            {
                _levelType = value;
                OnPropertyChanged("LevelType");
            }
        }

        public String LevelTypeString
        {
            get
            {
                return Enum.GetName(typeof(LevelTypeEnum), LevelType);
            }
            set
            {
                //Enum.TryParse(value, out LevelTypeEnum LevelType);
                LevelType = (LevelTypeEnum)Enum.Parse(typeof(LevelTypeEnum), value);
            }
        }

        private const String startDesignToolDataText = "--[[HM4DesignToolData:";
        private const String RoomIndexText = "RoomIndex:";
        private const String DifficultyLevelText = "DifficultyLevel:";
        private const String LevelTypeText = "LevelType:";
        private const String endDesignToolDataText = "--]]";

        public DesignToolData()
        {
        }

        public void ParseDesignData(String designToolData)
        {
            designToolData = designToolData.Replace("\t", "").Replace("\r", "").Replace(" ", "");
            designToolData.Replace(startDesignToolDataText, "").Replace(endDesignToolDataText, "");
            String[] delimiter = { "\n" };
            List<String> designToolList = designToolData.Split(delimiter, StringSplitOptions.None).ToList<String>();

            foreach (String entry in designToolList)
            {
                String textItem = entry;

                if (textItem.Contains(RoomIndexText))
                {
                    textItem = textItem.Replace(RoomIndexText, "");
                    textItem = Globals.FilterToNumerical(textItem);
                    Roomindex = Convert.ToInt32(textItem);
                }

                if (textItem.Contains(DifficultyLevelText))
                {
                    textItem = textItem.Replace(DifficultyLevelText, "");
                    DifficultyLevel = (float)Convert.ToDouble(textItem);
                }
                if (textItem.Contains(LevelTypeText))
                {
                    textItem = textItem.Replace(LevelTypeText, "");
                    switch (textItem)
                    {
                        case "Bonus":
                            LevelType = LevelTypeEnum.Bonus;
                            break;

                        case "Story":
                            LevelType = LevelTypeEnum.Story;
                            break;

                        case "MiniGame":
                            LevelType = LevelTypeEnum.MiniGame;
                            break;

                        case "TimeTrial":
                            LevelType = LevelTypeEnum.TimeTrial;
                            break;

                        case "Oliver":
                            LevelType = LevelTypeEnum.OliverOne;
                            break;

                        case "OliverOne":
                            LevelType = LevelTypeEnum.OliverOne;
                            break;

                        case "OliverAll":
                            LevelType = LevelTypeEnum.OliverAll;
                            break;

                        default:
                            LevelType = LevelTypeEnum.Unknown;
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            String output = startDesignToolDataText + Environment.NewLine;

            if (Roomindex > 0)
            {
                output += RoomIndexText + " \t" + Roomindex.ToString() + Environment.NewLine;
            }

            if (DifficultyLevel > 0)
            {
                output += DifficultyLevelText + " \t" + DifficultyLevel.ToString() + Environment.NewLine;
            }

            if (LevelType > 0)
            {
                output += LevelTypeText + " \t" + LevelType.ToString() + Environment.NewLine;
            }

            output += endDesignToolDataText + Environment.NewLine;
            return output;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }

    public class Level : INotifyPropertyChanged
    {
        #region LevelProperties

        public String LevelName = String.Empty;
        public String CategoryKey = String.Empty;

        //General Properties
        private bool _allowLevelScriptUpdate = true;

        public bool AllowLevelScriptUpdate
        {
            get
            {
                return _allowLevelScriptUpdate;
            }
            set
            {
                _allowLevelScriptUpdate = value;
                UpdateLevelOutput();
            }
        }

        public bool WeightEnabled
        {
            get
            {
                return designToolData.LevelType == LevelTypeEnum.TimeTrial;
            }
        }

        private List<String> _treatmentOptions = new List<String> { };
        private List<Patient> patientDatabase = new List<Patient> { };

        //private Dictionary<String, int> patientChancesDict = new Dictionary<String, int>();
        private DesignToolData designToolData = new DesignToolData();

        private String currentLevelScript = null;

        //Patient Row Objects
        public ObservableCollection<Patient> PatientCollection
        {
            get
            {
                ObservableCollection<Patient> patientCollection = new ObservableCollection<Patient>();
                if (patientDatabase.Count > 0)
                {
                    foreach (Patient patient in patientDatabase)
                    {
                        patientCollection.Add(patient);
                    }
                }
                return patientCollection;
            }
            set
            {
                patientDatabase = value.ToList();
                OnPropertyChanged();
            }
        }

        //Patient Chances Objects
        public List<PatientChance> PatientChanceList = new List<PatientChance> { };

        public ObservableCollection<PatientChance> PatientChanceCollection
        {
            get
            {
                return new ObservableCollection<PatientChance>(PatientChanceList);
            }
            set
            {
                PatientChanceList = value.ToList<PatientChance>();
                OnPropertyChanged("PatientChanceCollection");
            }
        }

        #endregion LevelProperties

        public Level(string levelName)
        {
            LevelName = levelName;
            CategoryKey = Globals.GetCategoryKey(GetRoomIndex);
            //Ensure all possible PatientTypeChances are set in PatientChanceList from the Settings.
            PatientChanceList = Globals.GetSettings.GetPatientChanceList(CategoryKey);
            foreach (PatientChance patientChance in PatientChanceList)
            {
                patientChance.ParentLevel = this;
            } //Set ParentLevel

            //Translate the levelScript into workable variables.
            ParseRawText(levelName);
            _canExecute = true;
        }

        #region Getters

        //Design Tool Data
        public String GetDesignToolData
        {
            get
            {
                return designToolData.ToString();
            }
        }

        public int GetRoomIndex
        {
            get
            {
                if (designToolData.Roomindex != 0)
                {
                    return designToolData.Roomindex;
                }
                else
                {
                    int roomIndex = -1;

                    //Generate the RoomIndex based on the naming convention of the levelName
                    if (LevelName != String.Empty)
                    {
                        //Assuming the room index is in r2_b04... naming convention]
                        if (LevelName.StartsWith("level"))
                        {
                            int levelIndex = Convert.ToInt16(LevelName.Replace("level", ""));
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
                        else if (LevelName.StartsWith("r"))
                        {
                            roomIndex = Convert.ToInt16(Globals.FilterToNumerical(LevelName.Substring(1, 1)));
                        }
                        else
                        {
                            roomIndex = -1;
                        }
                    }
                    //Store the newly generated RoomIndex
                    GetRoomIndex = roomIndex;
                    return roomIndex;
                }
            }
            set
            {
                designToolData.Roomindex = value;
                OnPropertyChanged("GetDesignToolData");
                UpdateLevelOutput();
            }
        }

        public double GetDifficultyModifier
        {
            get
            {
                return designToolData.DifficultyLevel;
            }
            set
            {
                designToolData.DifficultyLevel = value;
                OnPropertyChanged("GetDifficultyModifier");
                Globals.GetLevelOverview.UpdateRandomRecommendations();
            }
        }

        public String GetDifficultyModifierString
        {
            get
            {
                return GetDifficultyModifier.ToString("0.0");
            }
            set
            {
                if (value != null)
                {
                    GetDifficultyModifier = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    GetDifficultyModifier = 1;
                }
                UpdateLevelOutput();
            }
        }

        public Dictionary<String, int> RandomTreatmentDictionary = new Dictionary<string, int> { };
        public ObservableCollection<Treatment> RandomTreatmentList
        {
            get
            {

            }
            set
            {

            }
        }

        //Level Script output
        public String GetLevelTypeString
        {
            get
            {
                return designToolData.LevelTypeString;
            }
            set
            {
                designToolData.LevelTypeString = value;
                OnPropertyChanged("GetDesignToolData");
                OnPropertyChanged("WeightEnabled");
                UpdateLevelOutput();
            }
        }

        public ObservableCollection<String> GetTreatmentOptions
        {
            get
            {
                List<String> treatmentListString = new List<String> { String.Empty };

                //String categoryKey = Globals.GetCategoryKey(GetRoomIndex);
                if (GetRoomIndex > -1 && CategoryKey != String.Empty)
                {
                    List<Treatment> treatmentList = Globals.GetSettings.GetTreatmentList(CategoryKey);

                    foreach (Treatment treatment in treatmentList)
                    {
                        treatmentListString.Add(treatment.TreatmentName);
                    }
                }

                return new ObservableCollection<String>(treatmentListString);
            }
        }

        public String GetCurrentLevelScript
        {
            get
            {
                if (currentLevelScript == null || currentLevelScript == "")
                {
                    currentLevelScript = LevelOverview.ReadLevelTextFromFile(LevelName);
                }
                return currentLevelScript;
            }
        }

        public String GetNewLevelScript
        {
            get
            {
                string output = "";

                output += designToolData.ToString();
                output += Environment.NewLine;
                //Output the patient chances
                if (PatientChanceList.Count > 0)
                {
                    output += "levelDesc.patientChances = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    int i = 0;
                    foreach (PatientChance patientChance in PatientChanceList)
                    {
                        if (patientChance.Weight > 0)
                        {
                            output += "\t" + patientChance.PatientName + " \t= \t" + patientChance.Weight;
                            if (i < PatientChanceList.Count - 1)
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
                //Output the patient treatments
                if (patientDatabase.Count() > 0)
                {
                    output += "levelDesc.triggers = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    foreach (Patient patient in patientDatabase)
                    {
                        output += patient.ToString();
                    }
                    output += "}" + Environment.NewLine;
                }
                return output;
            }
        }

        //Patient Chance functions
        public double GetPatientChancePercentage(int Weight)
        {
            int TotalWeight = 0;
            foreach (PatientChance patientChance in PatientChanceList)
            {
                TotalWeight += patientChance.Weight;
            }

            double Percentage = Weight / (double)TotalWeight * 100;
            Percentage = Math.Round(Percentage, 1, MidpointRounding.AwayFromZero);
            return Percentage;
        }

        public void UpdatePatientChancePercentage()
        {
            foreach (PatientChance patientChance in PatientChanceList)
            {
                patientChance.UpdatePercentage();
            }
            UpdateLevelOutput();
        }

        #endregion Getters

        public void UpdateLevelOutput()
        {
            if (AllowLevelScriptUpdate)
            {
                OnPropertyChanged("GetNewLevelScript");
            }
        }

        public void AddPatient(String patientData = null, int index = -1)
        {
            if (index == -1)
            {
                index = patientDatabase.Count();
            }
            String patientName = "Patient_" + index.ToString();
            Patient patientObject = new Patient(this, patientName);

            if (patientData != null)
            {
                patientObject.SetPatientData(patientData);
            }

            patientDatabase.Insert(index, patientObject);
        }

        private void ParseRawText(String levelName)
        {
            String rawLevelText = GetCurrentLevelScript;
            // Clean up the text by removing tabs, enters and spaces and special characters.
            rawLevelText = rawLevelText.Replace("\t", "").Replace("\r", "").Replace("{\n", "{").Replace(" ", "").Replace("\n\n", "");

            // Parse DesignToolData embeded in file.
            String startDesignToolData = "--[[HM4DesignToolData:";
            String endDesignToolData = "--]]";
            String designToolDataText;
            if (rawLevelText.Contains(startDesignToolData) && rawLevelText.Contains(endDesignToolData))
            {
                int startDesignToolDataIndex = rawLevelText.IndexOf(startDesignToolData);
                int endDesignToolDataIndex = rawLevelText.IndexOf(endDesignToolData) + endDesignToolData.Length;

                if (startDesignToolDataIndex > -1 && endDesignToolDataIndex - endDesignToolData.Length > -1 && startDesignToolDataIndex < endDesignToolDataIndex)
                {
                    designToolDataText = rawLevelText.Substring(startDesignToolDataIndex, endDesignToolDataIndex - startDesignToolDataIndex);
                    designToolData.ParseDesignData(designToolDataText);
                    rawLevelText = rawLevelText.Replace(designToolDataText, "");
                }
            }

            // Parse the PatientChances section.
            String startPatientChancesText = "levelDesc.patientChances=";
            String endPatientChancesText = "}";
            String patientsChancesRawText;
            if (rawLevelText.Contains(startPatientChancesText) && rawLevelText.Contains(endPatientChancesText))
            {
                int startPatientChancesIndex = rawLevelText.IndexOf(startPatientChancesText);
                int endPatientChancesIndex = rawLevelText.IndexOf(endPatientChancesText) + endPatientChancesText.Length;

                if (startPatientChancesIndex > -1 && endPatientChancesIndex - endPatientChancesText.Length > -1 && startPatientChancesIndex < endPatientChancesIndex)
                {
                    // Do some extra formatting and cleaning up.
                    patientsChancesRawText = rawLevelText.Substring(startPatientChancesIndex, endPatientChancesIndex - startPatientChancesIndex);
                    rawLevelText = rawLevelText.Remove(startPatientChancesIndex, endPatientChancesIndex);

                    patientsChancesRawText = patientsChancesRawText.Replace("levelDesc.patientChances=", "").Replace(",}", "}");
                    patientsChancesRawText = patientsChancesRawText.Trim(' ').Replace("=", ":");

                    //Translate to workable Dictionary
                    Dictionary<String, int> patientChancesDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<String, int>>(patientsChancesRawText);

                    foreach (KeyValuePair<String, int> patientChance in patientChancesDict)
                    {
                        foreach (PatientChance patientChanceListItem in PatientChanceList)
                        {
                            if (patientChanceListItem.PatientName == patientChance.Key)
                            {
                                patientChanceListItem.Weight = patientChance.Value;
                                break;
                            }
                        }
                        //TODO Remove this if undefined PatientTypeChances in the Settings should not be added from the LevelScript.
                        //PatientChanceList.Add(new PatientChance(patientChance.Key, patientChance.Value));
                    }
                }
            }

            // Parse the PatientList with treatments
            String startPatientTriggerText = "levelDesc.triggers=";
            String endPatientTriggerText = "},\n}";
            String patientsTriggersRawText;
            if (rawLevelText.Contains(startPatientTriggerText) && rawLevelText.Contains(endPatientTriggerText))
            {
                int startPatientTriggerIndex = rawLevelText.IndexOf(startPatientTriggerText);
                int endPatientTriggerIndex = rawLevelText.IndexOf(endPatientTriggerText) + endPatientTriggerText.Length;

                if (startPatientTriggerIndex > -1 && endPatientTriggerIndex - endPatientChancesText.Length > -1 && startPatientTriggerIndex < endPatientTriggerIndex)
                {
                    patientsTriggersRawText = rawLevelText.Substring(startPatientTriggerIndex, endPatientTriggerIndex - startPatientTriggerIndex);
                    rawLevelText = rawLevelText.Remove(startPatientTriggerIndex, endPatientTriggerIndex);

                    patientsTriggersRawText = patientsTriggersRawText.Replace(startPatientTriggerText + "\n{", "").TrimEnd('}');

                    String[] delimiter = { "},\n" };
                    List<String> patientTriggers = patientsTriggersRawText.Split(delimiter, StringSplitOptions.None).ToList<String>();

                    foreach (String patientTrigger in patientTriggers)
                    {
                        AddPatient(patientTrigger);
                    }
                }
            }

            // Isolate any previous comments at the start of the text
            String startComments;
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

        internal void RandomizePatientChancesWeight()
        {
            Random rnd = new Random();

            foreach (PatientChance patientChance in PatientChanceList)
            {
                if (patientChance.IsSelected)
                {
                    patientChance.RandomizeWeight(rnd.Next(1, 100));
                }
            }
        }

        public void SelectAllPatientChances(bool State)
        {
            foreach (PatientChance patientChance in PatientChanceList)
            {
                patientChance.IsSelected = State;
            }
        }
        internal void UpdateMaxTreatments(int value)
        {

            foreach (Patient patient in PatientCollection)
            {
                patient.SetMaxTreatments(value);
            }

        }


        public void RandomizeTreatments(int treatmentMinValue, int treatmentMaxValue)
        {

            List<String> treatmentOptions = GetTreatmentOptions.ToList();

            IWeightedRandomizer<int> randomizer = new DynamicWeightedRandomizer<int>();
            //Start from 1, 0 is none
            for (int i = 1; i < treatmentOptions.Count; i++)
            {
                randomizer.Add(i, Globals.GetSettings.GetTreatment(treatmentOptions[i]).Weight);
            }

            Random random = new Random();

            foreach (Patient patient in PatientCollection)
            {

                //Get the number of treatments to generate. 
                int treatmentNumber = random.Next(treatmentMinValue, treatmentMaxValue + 1);
                bool searching = true;
                int loop = 0;
                List<int> RandomIndexArray = new List<int> { };

                //Get random unique list of indexes. 
                while (searching)
                {
                    int randomIndex = randomizer.NextWithReplacement();
                    if (!RandomIndexArray.Contains(randomIndex))
                    {
                        RandomIndexArray.Add(randomIndex);

                    }
                    //Break if enough have been found
                    if (treatmentNumber == RandomIndexArray.Count)
                    {
                        break;
                    }
                    //or when max loop have been reached. 
                    if (loop >= 100)
                    {
                        if (RandomIndexArray.Count < treatmentNumber)
                        {
                            //TODO Fill up any remaining slots with an empty treatment. 
                        }
                        break;
                    }
                    loop++;
                }

                //Convert from Index back to String Names
                List<String> randomTreatmentList = new List<string> { };
                foreach (int x in RandomIndexArray)
                {
                    randomTreatmentList.Add(treatmentOptions[x]);
                }
                patient.SetTreatments(randomTreatmentList);
            }
        }

        public void SetPatientAmount(int NewAmount)
        {
            int patientCount = PatientCollection.Count;

            if (NewAmount < patientCount)
            {
                int amountToSubtract = patientCount - NewAmount;

                for (int i = 1; i <= amountToSubtract; i++)
                {
                    int index = patientCount - i;

                    PatientCollection.RemoveAt(index);
                }

            }
            else if (NewAmount > patientCount)
            {
                int amountToAdd = NewAmount - patientCount;

                for (int i = 0; i < amountToAdd; i++)
                {
                    AddPatient();
                }
            }

        }

        #region Commands

        private bool _canExecute;
        private ICommand _randomizeAllWeightCommand;

        public ICommand RandomizeAllWeightCommand
        {
            get
            {
                return _randomizeAllWeightCommand ?? (_randomizeAllWeightCommand = new CommandHandler(() => RandomizePatientChancesWeight(), _canExecute));
            }
        }

        #endregion Commands

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }

    public class Patient : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Level ParentLevel = null;
        private bool weightEnabled = false;

        private String patientName = String.Empty;
        private int delay = 1000;
        private int weight = 0;

        private ObservableCollection<Treatment> _treatmentList = new ObservableCollection<Treatment> { };

        public ObservableCollection<Treatment> TreatmentCollection
        {
            get
            {
                return _treatmentList;
            }
            set
            {

                _treatmentList = value;

                OnPropertyChanged("TreatmentCollection");
                if (ParentLevel != null)
                {
                    ParentLevel.UpdateLevelOutput();
                }
            }
        }

        public int TreatmentCollectionVisibleCount
        {
            get
            {
                int count = 0;
                foreach (Treatment treatment in TreatmentCollection)
                {
                    if (treatment.IsVisible == true)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public ObservableCollection<String> TreatmentOptions
        {
            get
            {
                if (ParentLevel != null)
                {
                    return ParentLevel.GetTreatmentOptions;
                }
                else
                {
                    return new ObservableCollection<String> { };
                }
            }
        }

        private Dictionary<String, String> patientTraits;

        public int RoomIndex
        {
            get
            {
                if (ParentLevel != null)
                {
                    return ParentLevel.GetRoomIndex;
                }
                else
                {
                    return -1;
                }
            }
        }

        public String PatientName
        {
            get
            {
                return patientName;
            }
            set
            {
                patientName = value;
                OnPropertyChanged("PatientName");
                if (ParentLevel != null)
                {
                    ParentLevel.UpdateLevelOutput();
                }
            }
        }

        public int Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = value;
                OnPropertyChanged("Delay");

                if (ParentLevel != null)
                {
                    ParentLevel.UpdateLevelOutput();
                }
            }
        }

        public int Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
                if (ParentLevel != null)
                {
                    ParentLevel.UpdateLevelOutput();
                }
            }
        }

        public Patient()
        {
        }

        public Patient(Level ParentLevel, String patientName = null)
        {
            this.ParentLevel = ParentLevel;

            if (patientName != null)
            {
                this.patientName = patientName;
            }
        }

        public void SetPatientData(String patientData)
        {
            ParsePatientData(patientData);
        }

        public override string ToString()
        {
            String output = "";
            output += "\t{";

            if (delay > -1)
            {
                output += " delay = " + delay.ToString() + ",";
            }
            if (weightEnabled && weight > -1)
            {
                output += " weight = " + weight.ToString() + ",";
            }

            if (TreatmentCollection != null && TreatmentCollection.Count() > 0)
            {
                output += " todo = {";
                int i = 0;
                foreach (Treatment treatment in TreatmentCollection)
                {
                    if (!treatment.IsEmpty())
                    {
                        output += "\"" + treatment.TreatmentName + "\"";

                        //Only add a comma when the element is not last in List
                        if (i < TreatmentCollection.Count - 1)
                        {
                            output += ",";
                        }
                        i++;
                    }
                }
                output += "}," + Environment.NewLine;
            }

            if (patientTraits != null && patientTraits.Count() > 0)
            {
                foreach (KeyValuePair<string, string> trait in patientTraits)
                {
                    output += trait.Key + " = ";
                    if (trait.Value == "true")
                    {
                        output += trait.Value;
                    }
                    else
                    {
                        output += "\"" + trait.Value + "\"";
                    }
                }
            }

            return output;
        }

        private void ParsePatientData(String patientData)
        {
            // Clean up the patientData
            patientData = Regex.Replace(patientData, @"\s+", "");

            patientData = RemoveFirstComma(patientData);
            //Parse the delay
            String delayText = "delay=";
            if (patientData.Contains(delayText))
            {
                int startIndex = patientData.IndexOf(delayText) + delayText.Length;
                int endIndex = patientData.IndexOf(",");
                if (startIndex > -1 && endIndex > -1)
                {
                    String delayString = patientData.Substring(startIndex, endIndex - startIndex);
                    delayString = Globals.FilterToNumerical(delayString);
                    if (delayString != String.Empty)
                    {
                        delay = Convert.ToInt32(delayString);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Level.ParsePatientData, Failed to parse delayString, was none!");
                    }
                    patientData = patientData.Remove(startIndex - delayText.Length, endIndex);
                }
            }
            //Parse the weight
            patientData = RemoveFirstComma(patientData);
            String weightText = "weight=";
            if (patientData.Contains(weightText))
            {
                int startIndex = patientData.IndexOf(weightText) + weightText.Length;
                int endIndex = patientData.IndexOf(",");
                if (startIndex > -1 && endIndex > 0)
                {
                    String weightString = patientData.Substring(startIndex, endIndex - startIndex);
                    weightString = Globals.FilterToNumerical(weightString);
                    if (weightString != String.Empty)
                    {
                        weight = Convert.ToInt32(weightString);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Level.ParsePatientData, Failed to parse weightString, was none!");
                        weight = -1;
                    }
                    patientData = patientData.Remove(startIndex - weightText.Length, endIndex);
                    weightEnabled = true;
                }
            }
            //Parse the treatment list
            patientData = RemoveFirstComma(patientData);
            String treatmentText = "todo={";
            if (patientData.Contains(treatmentText))
            {
                int startIndex = patientData.IndexOf(treatmentText) + treatmentText.Length;
                int endIndex = patientData.IndexOf('}');
                if (startIndex > -1 && endIndex > -1)
                {
                    String rawTreatments = patientData.Substring(startIndex, endIndex - startIndex);
                    rawTreatments = rawTreatments.Replace("\"", "");
                    List<String> TreatmentList = rawTreatments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<String>();
                    //Convert found treatments to Treatment Objects

                    for (int i = 0; i < Math.Max(TreatmentList.Count, Globals.GetLevelOverview.MaxTreatmentsVisible); i++)
                    {
                        if (i < TreatmentList.Count)
                        {
                            AddTreatment(TreatmentList[i]);
                        }
                        else
                        {
                            AddTreatment();

                        }
                    }

                    patientData = patientData.Remove(startIndex - treatmentText.Length, endIndex);
                }
            }
            // If there is remaining data then it is probably traits that have been added.
            patientData = RemoveFirstComma(patientData);
            if (patientData.Length > 5)
            {
                try
                {
                    patientTraits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<String, String>>(patientData);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Level.ParsePatientData, failed to parse patientData: " + patientData);
                }
            }
        }

        private static String RemoveFirstComma(String patientString)
        {
            if (patientString.StartsWith(","))
            {
                return patientString.Substring(1, patientString.Count() - 1);
            }
            else
            {
                return patientString;
            }
        }

        private void AddTreatment(String treatmentName = "")
        {
            Treatment treatment = null;
            if (treatmentName != "")
            {
                treatment = Globals.GetSettings.GetTreatment(treatmentName, RoomIndex);
            }
            else
            {
                treatment = new Treatment();
            }
            treatment.SetPatientParent(this);
            treatment.IsVisible = true;
            TreatmentCollection.Add(treatment);

        }

        public void SetMaxTreatments(int Value)
        {
            if (Value > 0)
            {
                int treatmentCount = TreatmentCollectionVisibleCount;

                if (Value < treatmentCount)
                {
                    int amountToSubtract = treatmentCount - Value;

                    for (int i = 1; i <= amountToSubtract; i++)
                    {
                        int index = treatmentCount - i;

                        TreatmentCollection.ElementAt(index).IsVisible = false;

                    }

                }
                else if (Value > treatmentCount)
                {
                    int amountToAdd = Value - treatmentCount;

                    for (int i = 0; i < amountToAdd; i++)
                    {
                        int index = treatmentCount + i;
                        if (index < TreatmentCollection.Count)
                        {
                            TreatmentCollection.ElementAt(index).IsVisible = true;
                        }
                        else
                        {
                            AddTreatment();
                        }
                    }
                }

            }
        }


        public void SetTreatments(List<String> TreatmentList)
        {

            SetMaxTreatments(Math.Max(TreatmentList.Count, TreatmentCollection.Count));


            for (int i = 0; i < TreatmentCollection.Count; i++)
            {
                if (i < TreatmentList.Count)
                {
                    TreatmentCollection.ElementAt<Treatment>(i).TreatmentName = TreatmentList[i];
                }
                else
                {
                    TreatmentCollection.ElementAt<Treatment>(i).TreatmentName = String.Empty;
                }
            }

        }

        #region Events

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Events
    }

    public class PatientChance : INotifyPropertyChanged, IEquatable<PatientChance>
    {
        public Level ParentLevel = null;
        private String _patientName = String.Empty;

        public String PatientName
        {
            get
            {
                return _patientName;
            }
            set
            {
                _patientName = value;
                OnPropertyChanged("PatientName");
            }
        }

        private int _weight = 0;

        public int Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                OnPropertyChanged("Weight");
                if (ParentLevel != null)
                {
                    ParentLevel.UpdatePatientChancePercentage();
                }
            }
        }

        private double _percentage = 0;

        public double Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                OnPropertyChanged("Percentage");
                OnPropertyChanged("PercentageString");
            }
        }

        private bool _isSelected = false;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }



        public String PercentageString
        {
            get
            {
                return Percentage.ToString("N1") + "%";
            }
        }

        public PatientChance(String PatientName, int Weight = 0)
        {
            this.PatientName = PatientName;
            this.Weight = Weight;
            _canExecute = true;
        }

        public override string ToString()
        {
            return PatientName + " = " + Weight.ToString();
        }

        public void UpdatePercentage()
        {
            if (ParentLevel != null)
            {
                Percentage = ParentLevel.GetPatientChancePercentage(Weight);
            }
        }

        public void RandomizeWeight(int randomValue = 0)
        {
            if (randomValue == 0)
            {
                Random rnd = new Random();
                Weight = rnd.Next(1, 100);
            }
            else
            {
                Weight = randomValue;
            }
        }

        #region Commands

        private bool _canExecute;
        private ICommand _randomizeWeightCommand;

        public ICommand RandomizeWeightCommand
        {
            get
            {
                return _randomizeWeightCommand ?? (_randomizeWeightCommand = new CommandHandler(() => RandomizeWeight(), _canExecute));
            }
        }

        #endregion Commands

        #region Operators

        public bool Equals(PatientChance other)
        {
            return this.PatientName == other.PatientName;
        }

        public override int GetHashCode()
        {
            return -140387131 + EqualityComparer<string>.Default.GetHashCode(PatientName);
        }

        public static bool operator ==(PatientChance patientChance1, PatientChance patientChance2)
        {
            return patientChance1.PatientName == patientChance2.PatientName;
        }

        public static bool operator !=(PatientChance patientChance1, PatientChance patientChance2)
        {
            return !(patientChance1 == patientChance2);
        }

        #endregion Operators

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }

    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;

        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}