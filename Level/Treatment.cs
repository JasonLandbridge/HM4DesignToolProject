// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Treatment.cs" company="Blue Giraffe">
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
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;

    using HM4DesignTool.Data;

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
