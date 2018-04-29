﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SettingsNamespace;
using LevelData;
using NaturalSort.Extension;
using System.IO;
using System.Windows.Media;

namespace DataNameSpace
{

    public enum LevelType
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

        public static GameValues GameValue = new GameValues();

        //public static windowMain windowMainObject;
        public static List<String> roomCategories = new List<String> { "Room 1", "Room 2", "Room 3", "Room 4", "Room 5", "Room 6" };

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
                    LevelOverviewObject = new LevelOverview();
                }

                return LevelOverviewObject;
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
        public int difficultyModifierTreatmentsBased = 11;
        public int startLevelDuration = 110000;
        public int timeIncreasePerLevel = 4500;

        public int initialTimeBetweenPatients = 11000;
        public int decreaseTimeBetweenPatients = 250;

        public int initialTimePerTreatment = 6000;
        public int decreaseTimePerTreatment = 250;
        public int checkoutPerPatient = 2000;
        public int treatmentMinimumTime = 1600;

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
            if (Math.Round(difficultyModifier / difficultyModifierTreatmentsBased, 2) + 1 > 3.5)
            {
                return 3.5;
            }
            else
            {
                return Math.Round(difficultyModifier / difficultyModifierTreatmentsBased, 2) + 1;
            }
        }

        public Double TimePerTreatment(Double difficultyModifier)
        {
            Double x = initialTimePerTreatment - difficultyModifier * decreaseTimePerTreatment;

            if (x < treatmentMinimumTime)
            {
                return treatmentMinimumTime;
            }
            else
            {
                return x;
            }
        }

        public Double MilliSecondsPerLevel(Double difficultyModifier)
        {
            Double x = startLevelDuration + difficultyModifier * timeIncreasePerLevel;

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
            return initialTimeBetweenPatients - difficultyModifier * decreaseTimeBetweenPatients;
        }

        public Double AverageEntryTimePerPatient(Double difficultyModifier)
        {
            Double x = TreatmentPerPatient(difficultyModifier) * TimePerTreatment(difficultyModifier);
            x += TimeBetweenPatients(difficultyModifier);
            x += checkoutPerPatient;

            return x;
        }

        public Double NumberOfPatients(Double difficultyModifier)
        {
            Double x = MilliSecondsPerLevel(difficultyModifier) / AverageEntryTimePerPatient(difficultyModifier);
            return Math.Ceiling(x); //TODO Check if similair to math.ceil in python
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

    public struct Treatment
    {
        private String _treatmentName;
        private Double _difficultyUnlocked;
        private int _heartsValue;
        private int _weight;
        private bool _gesture;
        private bool _alwaysLast;
        //private Color _colorValue;
        private String _colorValueString;

        public String TreatmentName
        {
            get
            {
                return _treatmentName;
            }
            set
            {
                _treatmentName = value;
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
            }
        }
        public String ColorValueString
        {
            get
            {
                return "TestColorString";
            }
            set
            {
                _colorValueString = value;
            }
        }


        public Treatment(String treatmentName = null, Double difficultyUnlocked = 0, int heartsValue = 0, int weight = 0, bool gesture = false, bool alwaysLast = false, Color color = new Color())
        {
            _treatmentName = treatmentName;
            _difficultyUnlocked = difficultyUnlocked;
            _heartsValue = heartsValue;
            _weight = weight;
            _gesture = gesture;
            _alwaysLast = alwaysLast;
            //_colorValue = color;
            _colorValueString = "TestColorString";
        }

        #region Overloads
        //public Treatment(DataGridViewRow dataRow)
        //{
        //    DataGridViewCheckBoxCell cell = dataRow.Cells[0] as DataGridViewCheckBoxCell;

        //    //Get TreatmentName
        //    DataGridViewTextBoxCell tmpTreatmentName = dataRow.Cells[1] as DataGridViewTextBoxCell;
        //    if (tmpTreatmentName != null && tmpTreatmentName.Value != null)
        //    {
        //        TreatmentName = (String)tmpTreatmentName.Value;
        //    }
        //    else
        //    {
        //        TreatmentName = null;
        //    }

        //    //Get DifficultyUnlocked
        //    DataGridViewComboBoxCell tmpDifficultyUnlocked = dataRow.Cells[2] as DataGridViewComboBoxCell;
        //    if (tmpDifficultyUnlocked != null && tmpDifficultyUnlocked.Value != null)
        //    {
        //        DifficultyUnlocked = DataNameSpace.Globals.StringToDouble((String)tmpDifficultyUnlocked.Value);
        //    }
        //    else
        //    {
        //        DifficultyUnlocked = 0;
        //    }

        //    //Get HeartsValue
        //    DataGridViewTextBoxCell tmpHeartsValue = dataRow.Cells[3] as DataGridViewTextBoxCell;
        //    if (tmpHeartsValue != null && tmpHeartsValue.Value != null)
        //    {
        //        HeartsValue = int.Parse((String)tmpHeartsValue.Value);
        //    }
        //    else
        //    {
        //        HeartsValue = 0;
        //    }

        //    //Get Weight
        //    DataGridViewTextBoxCell tmpWeight = dataRow.Cells[4] as DataGridViewTextBoxCell;
        //    if (tmpWeight != null && tmpWeight.Value != null)
        //    {
        //        Weight = int.Parse((String)tmpWeight.Value);
        //    }
        //    else
        //    {
        //        Weight = 0;
        //    }

        //    //Get Gesture
        //    DataGridViewCheckBoxCell tmpGesture = dataRow.Cells[5] as DataGridViewCheckBoxCell;
        //    if (tmpGesture != null && tmpGesture.Value != null)
        //    {
        //        Gesture = (bool)tmpGesture.Value;
        //    }
        //    else
        //    {
        //        Gesture = false;
        //    }

        //    //Get AlwaysLast
        //    DataGridViewCheckBoxCell tmpAlwaysLast = dataRow.Cells[6] as DataGridViewCheckBoxCell;
        //    if (tmpAlwaysLast != null && tmpAlwaysLast.Value != null)
        //    {
        //        AlwaysLast = (bool)tmpAlwaysLast.Value;
        //    }
        //    else
        //    {
        //        AlwaysLast = false;
        //    }

        //    //Get Color
        //    DataGridViewTextBoxCell tmpColor = dataRow.Cells[7] as DataGridViewTextBoxCell;
        //    if (tmpColor != null && tmpColor.Value != null && false) //TODO Check for correct color to string conversion
        //    {
        //        ColorValue = (Color)tmpColor.Value;
        //    }
        //    else
        //    {
        //        ColorValue = new Color();
        //    }

        //}

        public Treatment(String treatmentName, String treatmentDataString)
        {
            List<String> treatmentData = treatmentDataString.Split(',').ToList<String>();
            _treatmentName = treatmentName;
            _difficultyUnlocked = Convert.ToDouble(treatmentData[0]);
            _heartsValue = Convert.ToInt32(treatmentData[1]);
            _weight = Convert.ToInt32(treatmentData[2]);
            _gesture = Convert.ToBoolean(treatmentData[3]);
            _alwaysLast = Convert.ToBoolean(treatmentData[4]);
            //_colorValue = (Color)ColorConverter.ConvertFromString(treatmentData[5]);
            _colorValueString = "TestColorString";
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
            return outputList;
        }

        public Dictionary<String, List<String>> ToDictionary()
        {
            Dictionary<String, List<String>> outputDict = new Dictionary<String, List<String>> { };
            outputDict.Add(TreatmentName, this.ToList());
            return outputDict;
        }

        public String ToString(bool includeTreatmentName = false)
        {
            return String.Join(",", ToList(includeTreatmentName));
        }

        public bool IsEmpty()
        {
            return TreatmentName == null || TreatmentName == "";
        }

        #region Operators
        public override bool Equals(Object obj)
        {
            return obj is Treatment && this == (Treatment)obj;
        }

        public override int GetHashCode()
        {
            var hashCode = 1917950600;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TreatmentName);
            hashCode = hashCode * -1521134295 + DifficultyUnlocked.GetHashCode();
            hashCode = hashCode * -1521134295 + HeartsValue.GetHashCode();
            hashCode = hashCode * -1521134295 + Weight.GetHashCode();
            hashCode = hashCode * -1521134295 + Gesture.GetHashCode();
            hashCode = hashCode * -1521134295 + AlwaysLast.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Treatment x, Treatment y)
        {
            return x.TreatmentName == y.TreatmentName && x.DifficultyUnlocked == y.DifficultyUnlocked && x.HeartsValue == y.HeartsValue &&
           x.Weight == y.Weight && x.Gesture == y.Gesture && x.AlwaysLast == y.AlwaysLast;
        }
        public static bool operator !=(Treatment x, Treatment y)
        {
            return !(x.TreatmentName == y.TreatmentName && x.DifficultyUnlocked == y.DifficultyUnlocked && x.HeartsValue == y.HeartsValue &&
           x.Weight == y.Weight && x.Gesture == y.Gesture && x.AlwaysLast == y.AlwaysLast);
        }
        #endregion
    }

}

