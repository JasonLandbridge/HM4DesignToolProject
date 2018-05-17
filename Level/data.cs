﻿using System;
using System.Collections.Generic;
using System.Linq;
using SettingsNamespace;

using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HM4DesignTool;

namespace DataNameSpace
{
    using HM4DesignTool.Level;

    public enum LevelTypeEnum
    {
        Unknown = 0,
        Story = 1,
        Bonus = 2,
        TimeTrial = 3,
        MiniGame = 4,
        OliverOne = 5,
        OliverAll = 6

    }

    public static class Globals
    {
        private static Settings SettingsObject;
        private static Data DataObject;
        private static LevelOverview LevelOverviewObject;
        private static GameValues GameValueObject = new GameValues();
        private static MainWindow MainWindowObject;
        private static Random random;

        public static Settings GetSettings
        {
            get
            {
                if (SettingsObject == null)
                {
                    SettingsObject = new Settings();
                }
                return SettingsObject;
            }
        }
        public static Data GetData
        {
            get
            {
                if (DataObject == null)
                {
                    DataObject = new Data();
                }
                return DataObject;
            }
        }
        public static LevelOverview GetLevelOverview
        {
            get
            {
                if (LevelOverviewObject == null)
                {
                    // This is seperated as the setup of all children in the LevelOverview need a reference of this as object as well. 
                    LevelOverviewObject = new LevelOverview();
                    LevelOverviewObject.SetupLevelOverview();
                }

                return LevelOverviewObject;
            }
        }
        public static GameValues GetGameValues
        {
            get
            {
                if (GameValueObject == null)
                {
                    GameValueObject = new GameValues();
                }

                return GameValueObject;
            }
        }
        public static MainWindow GetMainWindow
        {
            get
            {
                return MainWindowObject;
            }
            set
            {
                MainWindowObject = value;
            }
        }

        public static Random GetRandom
        {
            get
            {
                if (random == null)
                {
                    random = new Random();
                }

                return random;
            }

        }

        public static List<String> roomCategories = new List<String> { "Room 1", "Room 2", "Room 3", "Room 4", "Room 5", "Room 6", "Room 7" };
        public static List<String> GetLevelTypes
        {
            get
            {
                IEnumerable<LevelTypeEnum> output = Enum.GetValues(typeof(LevelTypeEnum)).Cast<LevelTypeEnum>();
                List<String> LevelTypeString = new List<string> { };
                foreach (LevelTypeEnum leveltype in output)
                {
                    LevelTypeString.Add(leveltype.ToString());
                }
                return LevelTypeString;
            }
        }


        public static Double StringToDouble(String difficultyModifier = null)
        {
            if (difficultyModifier != null)
            {
                return double.Parse(difficultyModifier, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }

        public static String FilterToNumerical(String str)
        {
            return Regex.Replace(str, @"[^\d]", "");
        }

        /// <summary>
        /// Remove first comma.
        /// </summary>
        /// <param name="stringObject">
        /// The patient string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveFirstComma(string stringObject)
        {
            if (stringObject.StartsWith(","))
            {
                return stringObject.Substring(1, stringObject.Count() - 1);
            }
            else if (stringObject.StartsWith("{,"))
            {

                return stringObject.Remove(1, 1);
            }
            else
            {
                return stringObject;
            }
        }

        public static String GetCategoryKey(int RoomIndex)
        {
            //Convert RoomNumber to RoomIndex, [1,2,3,4..] => [0,1,2,3..] 
            RoomIndex--;
            if (-1 < RoomIndex && RoomIndex < roomCategories.Count)
            {
                return roomCategories[RoomIndex];
            }
            else
            {
                return String.Empty;
            }
        }

        public static String ColorToHex(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static Color HexToColor(String hexCode)
        {
            if (!hexCode.StartsWith("#"))
            {
                hexCode = $"#{hexCode}";
            }

            if (hexCode.Count() == 7)
            {
                return (Color)ColorConverter.ConvertFromString(hexCode);
            }
            else
            {
                return Colors.White;
            }
        }
    }

    public class Data
    {


        private List<String> patientTypeList = new List<String> { };
        public String StatusbarText = "";

        public Data()
        {
        }

        public List<String> GetPatientTypesFromDisk(bool reload = false)
        {
            if (patientTypeList == null || patientTypeList.Count() == 0 || reload)
            {
                String projectPath = Globals.GetSettings.projectPathImages + "patients\\";

                if (System.IO.Directory.Exists(projectPath))
                {
                    List<String> rawPatientTypeList = new List<String>(System.IO.Directory.GetDirectories(projectPath));
                    List<String> rawPatientList = new List<String> { };

                    foreach (String patientType in rawPatientTypeList)
                    {
                        rawPatientList.Add(patientType.Replace(projectPath, ""));
                    }

                    patientTypeList = rawPatientList;
                }

            }

            return patientTypeList;

        }



    }

    public class GameValues
    {
        // TODO Make sure to import the constant values from GSettings
        private int difficultyModifierTreatmentsBased = 11;
        private int startLevelDuration = 110000;
        private int timeIncreasePerLevel = 4500;

        private int initialTimeBetweenPatients = 11000;
        private int decreaseTimeBetweenPatients = 250;

        private int initialTimePerTreatment = 6000;
        private int decreaseTimePerTreatment = 250;
        private int checkoutPerPatient = 2000;
        private int treatmentMinimumTime = 1600;

        public int DifficultyModifierTreatmentsBased { get => difficultyModifierTreatmentsBased; set => difficultyModifierTreatmentsBased = value; }
        public int StartLevelDuration { get => startLevelDuration; set => startLevelDuration = value; }
        public int TimeIncreasePerLevel { get => timeIncreasePerLevel; set => timeIncreasePerLevel = value; }
        public int InitialTimeBetweenPatients { get => initialTimeBetweenPatients; set => initialTimeBetweenPatients = value; }
        public int DecreaseTimeBetweenPatients { get => decreaseTimeBetweenPatients; set => decreaseTimeBetweenPatients = value; }
        public int InitialTimePerTreatment { get => initialTimePerTreatment; set => initialTimePerTreatment = value; }
        public int DecreaseTimePerTreatment { get => decreaseTimePerTreatment; set => decreaseTimePerTreatment = value; }
        public int CheckoutPerPatient { get => checkoutPerPatient; set => checkoutPerPatient = value; }
        public int TreatmentMinimumTime { get => treatmentMinimumTime; set => treatmentMinimumTime = value; }

        public GameValues()
        {

        }

        public Dictionary<String, Double> GetBalancingData(Double difficultyModifier)
        {
            Dictionary<String, Double> balancingData = new Dictionary<String, Double> { };

            if (difficultyModifier > 0.5)
            {
                balancingData.Add("averageEntryTimePerPatient", AverageEntryTimePerPatient(difficultyModifier));
                balancingData.Add("timeBetweenPatients", TimeBetweenPatients(difficultyModifier));
                balancingData.Add("numberOfPatients", NumberOfPatients(difficultyModifier));
                balancingData.Add("treatmentPerPatient", TreatmentPerPatient(difficultyModifier));
                balancingData.Add("timePerTreatment", TimePerTreatment(difficultyModifier));
                balancingData.Add("milliSecondsPerLevel", MilliSecondsPerLevel(difficultyModifier));
                balancingData.Add("minutesPerLevel", MinutesPerLevel(difficultyModifier));

                return balancingData;
            }
            else
            {
                return null;
            }

        }


        #region GameValues

        public Double TreatmentPerPatient(Double difficultyModifier)
        {
            if (Math.Round(difficultyModifier / DifficultyModifierTreatmentsBased, 2) + 1 > 3.5)
            {
                return 3.5;
            }
            else
            {
                return Math.Round(difficultyModifier / DifficultyModifierTreatmentsBased, 2) + 1;
            }
        }

        public int TreatmentPerPatientToInt(Double difficultyModifier)
        {
            double x = Math.Round(TreatmentPerPatient(difficultyModifier), MidpointRounding.AwayFromZero);
            return Convert.ToInt32(x);
        }

        public Double TimePerTreatment(Double difficultyModifier)
        {
            Double x = InitialTimePerTreatment - difficultyModifier * DecreaseTimePerTreatment;

            if (x < TreatmentMinimumTime)
            {
                return TreatmentMinimumTime;
            }
            else
            {
                return x;
            }
        }

        public Double MilliSecondsPerLevel(Double difficultyModifier)
        {
            Double x = StartLevelDuration + difficultyModifier * TimeIncreasePerLevel;

            if (x > 400000)
            {
                return 400000;
            }
            else
            {
                return x;
            }
        }

        public Double MinutesPerLevel(Double difficultyModifier)
        {
            return MilliSecondsPerLevel(difficultyModifier) / 60000;

        }

        public Double TimeBetweenPatients(Double difficultyModifier)
        {
            return InitialTimeBetweenPatients - difficultyModifier * DecreaseTimeBetweenPatients;
        }

        public Double AverageEntryTimePerPatient(Double difficultyModifier)
        {
            Double x = TreatmentPerPatient(difficultyModifier) * TimePerTreatment(difficultyModifier);
            x += TimeBetweenPatients(difficultyModifier);
            x += CheckoutPerPatient;

            return x;
        }

        public Double NumberOfPatients(Double difficultyModifier)
        {
            Double x = MilliSecondsPerLevel(difficultyModifier) / AverageEntryTimePerPatient(difficultyModifier);
            return Math.Ceiling(x); //TODO Check if similair to math.ceil in python
        }
        public int NumberOfPatientsToInt(Double difficultyModifier)
        {
            return Convert.ToInt32(NumberOfPatients(difficultyModifier));
        }

        #endregion

        #region Overloads
        public Dictionary<String, Double> GetBalancingData(String difficultyModifier)
        {
            return GetBalancingData(Globals.StringToDouble(difficultyModifier));
        }
        public Double TreatmentPerPatient(String difficultyModifier)
        {
            return TreatmentPerPatient(Globals.StringToDouble(difficultyModifier));
        }
        public Double TimePerTreatment(String difficultyModifier)
        {
            return TimePerTreatment(Globals.StringToDouble(difficultyModifier));
        }
        public Double MilliSecondsPerLevel(String difficultyModifier)
        {
            return MilliSecondsPerLevel(Globals.StringToDouble(difficultyModifier));
        }
        public Double MinutesPerLevel(String difficultyModifier)
        {
            return MinutesPerLevel(Globals.StringToDouble(difficultyModifier));
        }
        public Double TimeBetweenPatients(String difficultyModifier)
        {
            return TimeBetweenPatients(Globals.StringToDouble(difficultyModifier));
        }
        public Double AverageEntryTimePerPatient(String difficultyModifier)
        {
            return AverageEntryTimePerPatient(Globals.StringToDouble(difficultyModifier));
        }
        public Double NumberOfPatients(String difficultyModifier)
        {
            return NumberOfPatients(Globals.StringToDouble(difficultyModifier));
        }

        #endregion




    }

    public class Treatment : Object, INotifyPropertyChanged
    {
        private Patient ParentPatient = null;
        private Level ParentLevel = null;
        private String _treatmentName = String.Empty;
        private Double _difficultyUnlocked = 0;
        private int _heartsValue = 0;
        private int _weight = 0;
        private int _customizedWeight = 0;
        private bool _gesture = false;
        private bool _alwaysLast = false;
        private Color _treatmentColor = Colors.White;
        private double _weightPercentage = 0;

        private bool _isVisible = false;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public String TreatmentName
        {
            get
            {
                return _treatmentName;
            }
            set
            {
                if (_treatmentName != value)
                {
                    _treatmentName = value;
                    OnPropertyChanged("TreatmentName");
                    OnPropertyChanged("TreatmentColorBrush");
                    ChangeTreatment();
                    if (ParentPatient != null && ParentPatient.ParentLevel != null)
                    {
                        ParentPatient.ParentLevel.UpdateLevelOutput();
                    }
                }
            }
        }
        public Double DifficultyUnlocked
        {
            get
            {
                return _difficultyUnlocked;
            }
            set
            {
                _difficultyUnlocked = value;
                OnPropertyChanged("DifficultyUnlocked");

            }
        }
        public String DifficultyUnlockedString
        {
            get
            {
                String removeMe = _difficultyUnlocked.ToString("N1");
                return removeMe;
            }
            set
            {
                DifficultyUnlocked = Convert.ToDouble(value);
                OnPropertyChanged("DifficultyUnlockedString");
            }
        }
        public int HeartsValue
        {
            get
            {
                return _heartsValue;
            }
            set
            {
                _heartsValue = value;
                OnPropertyChanged("HeartsValue");

            }
        }
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

            }
        }
        public int CustomizedWeight
        {
            get
            {
                return _customizedWeight;
            }
            set
            {
                _customizedWeight = value;
                OnPropertyChanged("CustomizedWeight");
                if (ParentPatient != null && ParentPatient.ParentLevel != null)
                {
                    ParentPatient.ParentLevel.UpdateTreatmentWeightPercentage();
                }
                else if (ParentLevel != null)
                {
                    ParentLevel.UpdateTreatmentWeightPercentage();
                }
            }
        }


        public bool Gesture
        {
            get
            {
                return _gesture;
            }
            set
            {
                _gesture = value;
                OnPropertyChanged("Gesture");
            }
        }
        public bool AlwaysLast
        {
            get
            {
                return _alwaysLast;
            }
            set
            {
                _alwaysLast = value;
                OnPropertyChanged("AlwaysLast");

            }
        }
        public Color TreatmentColor
        {
            get
            {
                return _treatmentColor;
            }
            set
            {
                _treatmentColor = value;
                OnPropertyChanged("TreatmentColor");
                OnPropertyChanged("TreatmentColorBrush");
                OnPropertyChanged("TreatmentFontColorBrush");
                OnPropertyChanged("TreatmentColorString");

            }
        }
        public SolidColorBrush TreatmentColorBrush
        {
            get
            {
                return new SolidColorBrush(TreatmentColor);
            }
        }

        public SolidColorBrush TreatmentFontColorBrush
        {
            get
            {
                int brightness = (int)Math.Sqrt(
                    this.TreatmentColor.R * TreatmentColor.R * .299 +
                    TreatmentColor.G * TreatmentColor.G * .587 +
                    TreatmentColor.B * TreatmentColor.B * .114);

                if (brightness > 130)
                {
                    return new SolidColorBrush(Colors.Black);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
        }

        public String TreatmentColorString
        {
            get
            {
                return Globals.ColorToHex(TreatmentColor);
            }
            set
            {
                TreatmentColor = Globals.HexToColor(value);
            }
        }


        public double WeightPercentage
        {
            get
            {
                return _weightPercentage;
            }
            set
            {
                _weightPercentage = value;
                OnPropertyChanged("WeightPercentage");
                OnPropertyChanged("WeightPercentageString");
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        public Treatment()
        {
            this.TreatmentName = String.Empty;
        }

        public Treatment(String treatmentName, Patient ParentPatient = null)
        {
            TreatmentName = treatmentName;
            if (ParentPatient != null)
            {
                this.ParentPatient = ParentPatient;
            }
        }

        private void ChangeTreatment()
        {
            if (TreatmentName != String.Empty && ParentPatient != null && ParentPatient.ParentLevel != null)
            {
                int RoomIndex = ParentPatient.RoomIndex;
                Treatment newTreatment = Globals.GetSettings.GetTreatment(TreatmentName, RoomIndex);

                DifficultyUnlocked = newTreatment.
                HeartsValue = newTreatment.HeartsValue;
                Weight = newTreatment.Weight;
                CustomizedWeight = Weight; //Default to the base weight from settings
                Gesture = newTreatment.Gesture;
                AlwaysLast = newTreatment.AlwaysLast;
                TreatmentColor = newTreatment.TreatmentColor;

            }
            else
            {
                DifficultyUnlocked = 0;
                Weight = 0;
                Gesture = false;
                AlwaysLast = false;
                TreatmentColor = Colors.White;

            }
        }
        #region Overloads

        public Treatment(String treatmentName, String treatmentDataString)
        {
            List<String> treatmentData = treatmentDataString.Split(',').ToList<String>();
            TreatmentName = treatmentName;

            //Used to ensure only the avaliable options are set. 
            for (int index = 0; index < treatmentData.Count; index++)
            {
                switch (index)
                {
                    case 0:
                        {
                            this.DifficultyUnlocked = Convert.ToDouble(treatmentData[0]);
                            break;

                        }
                    case 1:
                        {
                            this.HeartsValue = Convert.ToInt32(treatmentData[1]);
                            break;

                        }
                    case 2:
                        {
                            this.Weight = Convert.ToInt32(treatmentData[2]);
                            this.CustomizedWeight = this.Weight;
                            break;

                        }
                    case 3:
                        {
                            this.Gesture = Convert.ToBoolean(treatmentData[3]);
                            break;

                        }
                    case 4:
                        {
                            this.AlwaysLast = Convert.ToBoolean(treatmentData[4]);
                            break;

                        }
                    case 5:
                        {
                            this.TreatmentColorString = treatmentData[5].ToString();
                            break;
                        }

                    default:
                        break;
                }

            }

        }
        #endregion


        public List<String> ToList(bool includeTreatmentName = false)
        {
            List<String> outputList = new List<String> { };
            if (includeTreatmentName)
            {
                outputList.Add(TreatmentName);
            }
            outputList.Add(DifficultyUnlocked.ToString());
            outputList.Add(HeartsValue.ToString());
            outputList.Add(Weight.ToString());
            outputList.Add(Gesture.ToString());
            outputList.Add(AlwaysLast.ToString());
            outputList.Add(TreatmentColorString);
            return outputList;
        }

        public Dictionary<String, List<String>> ToDictionary()
        {
            Dictionary<String, List<String>> outputDict = new Dictionary<String, List<String>> { };
            outputDict.Add(TreatmentName, this.ToList());
            return outputDict;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public String WeightPercentageString
        {
            get
            {
                return WeightPercentage.ToString("N1") + "%";
            }
        }

        public void UpdatePercentage()
        {
            if (ParentPatient != null && ParentPatient.ParentLevel != null)
            {
                WeightPercentage = ParentPatient.ParentLevel.GetTreatmentWeightPercentage(CustomizedWeight);
            }
            else if (ParentLevel != null)
            {
                WeightPercentage = ParentLevel.GetTreatmentWeightPercentage(CustomizedWeight);
            }

        }

        public String ToString(bool includeTreatmentName = false)
        {
            return String.Join(",", ToList(includeTreatmentName));
        }

        public bool IsEmpty => TreatmentName == null || TreatmentName == "" || TreatmentName == String.Empty;

        public void SetPatientParent(Patient ParentPatient)
        {
            this.ParentPatient = ParentPatient;
        }

        public void SetLevelParent(Level ParentLevel)
        {
            this.ParentLevel = ParentLevel;
        }


        #region Operators
        public override bool Equals(object obj)
        {
            var treatment = obj as Treatment;
            return treatment != null &&
                   TreatmentName == treatment.TreatmentName;
        }

        public override int GetHashCode()
        {
            return 1629960752 + EqualityComparer<string>.Default.GetHashCode(TreatmentName);
        }

        public static bool operator ==(Treatment treatment1, Treatment treatment2)
        {
            return EqualityComparer<Treatment>.Default.Equals(treatment1, treatment2);
        }

        public static bool operator !=(Treatment treatment1, Treatment treatment2)
        {
            return !(treatment1 == treatment2);
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

