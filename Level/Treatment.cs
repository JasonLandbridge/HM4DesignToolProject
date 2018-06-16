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
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;

    using HM4DesignTool.Data;

    /// <inheritdoc />
    /// <summary>
    /// The Treatment object used to hold data about treatments.
    /// </summary>
    public class Treatment : object, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The parent patient of this treatment.
        /// </summary>
        private Patient parentPatient;

        /// <summary>
        /// The parent level of this treatment.
        /// </summary>
        private Level parentLevel;
        #region TreatmentDataFields

        /// <summary>
        /// The treatment name.
        /// </summary>
        private string treatmentName = string.Empty;

        /// <summary>
        /// The treatment type.
        /// </summary>
        private TreatmentTypeEnum treatmentType = TreatmentTypeEnum.Unknown;

        /// <summary>
        /// The difficulty unlocked.
        /// </summary>
        private double difficultyUnlocked;

        /// <summary>
        /// Part of this station
        /// </summary>
        private Station stationOwner;

        /// <summary>
        /// The weight.
        /// </summary>
        private int weight;

        /// <summary>
        /// The customized weight.
        /// </summary>
        private int customizedWeight;

        /// <summary>
        /// The hearts value.
        /// </summary>
        private int heartsValue;

        /// <summary>
        /// The always last.
        /// </summary>
        private bool alwaysLast;

        /// <summary>
        /// The gesture.
        /// </summary>
        private bool gesture;

        /// <summary>
        /// The treatment color.
        /// </summary>
        private Color treatmentColor = Colors.White;
        #endregion

        /// <summary>
        /// The weight percentage.
        /// </summary>
        private double weightPercentage;

        /// <summary>
        /// If this treatment is currently visible in the Patient Overview
        /// </summary>
        private bool isVisible;

        /// <summary>
        /// If this treatment is currently selected in the Patient Overview or Settings Window
        /// </summary>
        private bool isSelected;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Treatment"/> class.
        /// </summary>
        public Treatment()
        {
            this.TreatmentName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Treatment"/> class.
        /// </summary>
        /// <param name="treatmentName">
        /// The treatment name.
        /// </param>
        /// <param name="parentPatient">
        /// The parent patient.
        /// </param>
        public Treatment(string treatmentName, Patient parentPatient = null)
        {
            this.TreatmentName = treatmentName;
            if (parentPatient != null)
            {
                this.parentPatient = parentPatient;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Treatment"/> class.
        /// </summary>
        /// <param name="treatmentName">
        /// The treatment name.
        /// </param>
        /// <param name="treatmentDataString">
        /// The treatment data string.
        /// </param>
        public Treatment(string treatmentName, string treatmentDataString)
        {
            List<string> treatmentData = treatmentDataString.Split(',').ToList();
            this.TreatmentName = treatmentName;

            // Used to ensure only the avaliable options are set. 
            for (int index = 0; index < treatmentData.Count; index++)
            {
                switch (index)
                {
                    case 0:
                        {
                            if (treatmentData[index] != string.Empty)
                            {
                                this.TreatmentTypeString = treatmentData[index];
                            }
                            break;
                        }


                    case 1:
                        {
                            if (treatmentData[index] != string.Empty)
                            {
                                this.DifficultyUnlockedString = treatmentData[index];
                            }

                            break;
                        }

                    case 2:
                        {
                            if (treatmentData[index] != string.Empty)
                            {
                                this.StationOwner = new Station(treatmentData[index]);
                            }

                            break;
                        }

                    case 3:
                        {
                            if (treatmentData[index] != string.Empty)
                            {

                                this.HeartsValue = Globals.StringToInt(treatmentData[index]);
                            }

                            break;
                        }

                    case 4:
                        {
                            if (treatmentData[index] != string.Empty)
                            {
                                this.Weight = Globals.StringToInt(treatmentData[index]);
                                this.CustomizedWeight = this.Weight;
                            }
                            break;
                        }

                    case 5:
                        {
                            if (treatmentData[index] != string.Empty && Globals.IsBooleanValue(treatmentData[index]))
                            {
                                this.Gesture = Globals.StringToBool(treatmentData[index]);
                            }
                            break;
                        }

                    case 6:
                        {
                            if (treatmentData[index] != string.Empty && Globals.IsBooleanValue(treatmentData[index]))
                            {
                                this.AlwaysLast = Globals.StringToBool(treatmentData[index]);
                            }
                            break;
                        }

                    case 7:
                        {
                            if (treatmentData[index] != string.Empty)
                            {
                                this.TreatmentColorString = treatmentData[index];
                            }
                            break;
                        }

                    default:
                        break;
                }
            }
        }

        #endregion

        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion Events

        #region Properties

        #region Public

        /// <summary>
        /// Gets the weight percentage in string format.
        /// </summary>
        public string WeightPercentageString => this.WeightPercentage.ToString("N1") + "%";

        /// <summary>
        /// Check if this Treatment is empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(this.TreatmentName) || this.TreatmentName == string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this Treatment is selected.
        /// </summary>
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                this.isSelected = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the treatment name.
        /// </summary>
        public string TreatmentName
        {
            get => this.treatmentName;
            set
            {
                if (this.treatmentName != value)
                {
                    this.treatmentName = value;
                    this.OnPropertyChanged();

                    this.ChangeTreatment();
                    if (this.parentPatient != null)
                    {
                        this.parentPatient.ParentLevel?.UpdateLevelOutput();
                    }

                    this.OnPropertyChanged("TreatmentColorBrush");
                    this.OnPropertyChanged("TreatmentType");
                    this.OnPropertyChanged("TreatmentTypeColorBrush");
                }
            }
        }

        /// <summary>
        /// Gets or sets The treatment type.
        /// </summary>
        public TreatmentTypeEnum TreatmentType
        {
            get => this.treatmentType;
            set
            {
                this.treatmentType = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("TreatmentTypeColorBrush");
            }
        }


        public string TreatmentTypeString
        {
            get => Enum.GetName(typeof(TreatmentTypeEnum), this.TreatmentType);
            set
            {
                this.TreatmentType = (TreatmentTypeEnum)Enum.Parse(typeof(TreatmentTypeEnum), value);
                this.OnPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets the difficulty unlocked.
        /// </summary>
        public double DifficultyUnlocked
        {
            get => this.difficultyUnlocked;

            set
            {
                this.difficultyUnlocked = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the difficulty unlocked string.
        /// </summary>
        public string DifficultyUnlockedString
        {
            get => this.DifficultyUnlocked.ToString("N1");

            set
            {
                this.DifficultyUnlocked = Globals.StringToDouble(value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the hearts value.
        /// </summary>
        public int HeartsValue
        {
            get => this.heartsValue;

            set
            {
                this.heartsValue = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        public int Weight
        {
            get => this.weight;

            set
            {
                this.weight = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the customized weight.
        /// </summary>
        public int CustomizedWeight
        {
            get => this.customizedWeight;

            set
            {
                this.customizedWeight = value;
                this.OnPropertyChanged();
                if (this.parentPatient != null && this.parentPatient.ParentLevel != null)
                {
                    this.parentPatient.ParentLevel.UpdateTreatmentWeightPercentage();
                }
                else
                {
                    this.parentLevel?.UpdateTreatmentWeightPercentage();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this Treatment is a gesture.
        /// </summary>
        public bool Gesture
        {
            get => this.gesture;

            set
            {
                this.gesture = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this treatment should always be last in a generated treatment list for patients.
        /// </summary>
        public bool AlwaysLast
        {
            get => this.alwaysLast;

            set
            {
                this.alwaysLast = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the treatment color.
        /// </summary>
        public Color TreatmentColor
        {
            get => this.treatmentColor;

            set
            {
                this.treatmentColor = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("TreatmentColorBrush");
                this.OnPropertyChanged("TreatmentFontColorBrush");
                this.OnPropertyChanged("TreatmentColorString");
            }
        }

        /// <summary>
        /// Gets the treatment color brush.
        /// </summary>
        public SolidColorBrush TreatmentColorBrush => new SolidColorBrush(this.TreatmentColor);

        /// <summary>
        /// Gets the treatment font color brush.
        /// </summary>
        public SolidColorBrush TreatmentFontColorBrush
        {
            get
            {
                int brightness = (int)Math.Sqrt(
                    ((this.TreatmentColor.R * this.TreatmentColor.R) * .299) +
                    ((this.TreatmentColor.G * this.TreatmentColor.G) * .587) +
                    ((this.TreatmentColor.B * this.TreatmentColor.B) * .114));
                return brightness > 130 ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// Gets or sets the treatment color from and to string.
        /// </summary>
        public string TreatmentColorString
        {
            get => Globals.ColorToHex(this.TreatmentColor);
            set => this.TreatmentColor = Globals.HexToColor(value);
        }

        /// <summary>
        /// Gets the treatment Type color brush.
        /// </summary>
        public SolidColorBrush TreatmentTypeColorBrush
        {
            get
            {
                switch (this.TreatmentType)
                {
                    case TreatmentTypeEnum.Unknown:
                        return new SolidColorBrush(Colors.White);
                    case TreatmentTypeEnum.Quick:
                        return new SolidColorBrush(Colors.Aqua);
                    case TreatmentTypeEnum.Gesture:
                        return new SolidColorBrush(Colors.Blue);
                    case TreatmentTypeEnum.SingleProduct:
                        return new SolidColorBrush(Colors.DarkOrange);
                    case TreatmentTypeEnum.ComboProduct:
                        return new SolidColorBrush(Colors.Red);
                    case TreatmentTypeEnum.Ingredient:
                        return new SolidColorBrush(Colors.DarkViolet);
                    case TreatmentTypeEnum.CookProduct:
                        return new SolidColorBrush(Colors.Gold);
                    case TreatmentTypeEnum.Minigame:
                        return new SolidColorBrush(Colors.DarkGreen);
                    default:
                        return new SolidColorBrush(Colors.White);
                }
            }
        }


        /// <summary>
        /// Gets or sets the weight percentage.
        /// </summary>
        public double WeightPercentage
        {
            get => this.weightPercentage;

            set
            {
                this.weightPercentage = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("WeightPercentageString");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this treatment is currently visible in the Patient Overview.
        /// </summary>
        public bool IsVisible
        {
            get => this.isVisible;
            set
            {
                this.isVisible = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets this Treatment's Station owner
        /// </summary>
        public Station StationOwner
        {
            get => this.stationOwner;
            set
            {
                this.stationOwner = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("StationString");
                this.OnPropertyChanged("StationName");

            }
        }

        public string StationName
        {
            get
            {
                return this.StationOwner.StationName;
            }
            set
            {
                this.StationOwner.StationName = value;
                this.OnPropertyChanged();
            }
        }

        public string StationString
        {
            get
            {
                if (this.StationOwner != null)
                {
                    return $"{this.StationOwner.DisplayStationName} => ";
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (this.StationOwner != null)
                {
                    this.StationOwner.StationName = value;
                }
                else
                {
                    this.StationOwner = new Station(value);
                }
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Public

        #region Operators

        /// <summary>
        /// The == operator.
        /// </summary>
        /// <param name="treatment1">
        /// The treatment 1.
        /// </param>
        /// <param name="treatment2">
        /// The treatment 2.
        /// </param>
        /// <returns>
        /// Return TRUE if both Treatments are EQUAL.
        /// </returns>
        public static bool operator ==(Treatment treatment1, Treatment treatment2)
        {
            return EqualityComparer<Treatment>.Default.Equals(treatment1, treatment2);
        }

        /// <summary>
        /// The != operator.
        /// </summary>
        /// <param name="treatment1">
        /// The treatment 1.
        /// </param>
        /// <param name="treatment2">
        /// The treatment 2.
        /// </param>
        /// <returns>
        /// Return TRUE if both Treatments are NOT EQUAL.
        /// </returns>
        public static bool operator !=(Treatment treatment1, Treatment treatment2)
        {
            return !(treatment1 == treatment2);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// Return TRUE if both Treatments are EQUAL.
        /// </returns>
        public override bool Equals(object obj)
        {
            var treatment = obj as Treatment;
            return treatment != null && this.TreatmentName == treatment.TreatmentName;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return 1629960752 + EqualityComparer<string>.Default.GetHashCode(this.TreatmentName);
        }
        #endregion

        /// <summary>
        /// Convert all Treatment Data to string format to be stored.
        /// </summary>
        /// <param name="includeTreatmentName">
        /// The include treatment name.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> ToList(bool includeTreatmentName = false)
        {
            List<string> outputList = new List<string>();
            if (includeTreatmentName)
            {
                outputList.Add(this.TreatmentName);
            }
            outputList.Add(this.TreatmentTypeString);
            outputList.Add(this.DifficultyUnlocked.ToString("N1"));
            outputList.Add(this.StationOwner?.StationName);
            outputList.Add(this.HeartsValue.ToString());
            outputList.Add(this.Weight.ToString());
            outputList.Add(this.Gesture.ToString());
            outputList.Add(this.AlwaysLast.ToString());
            outputList.Add(this.TreatmentColorString);
            return outputList;
        }

        /// <summary>
        /// Return the Treatment as a Dictionary entry
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, List<string>> ToDictionary()
        {
            return new Dictionary<string, List<string>> { { this.TreatmentName, this.ToList() } };
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.ToOutput(true);
        }

        /// <summary>
        /// Updates the Treatment Weight percentage relative to the other available treatments. 
        /// </summary>
        public void UpdatePercentage()
        {
            if (this.parentPatient != null && this.parentPatient.ParentLevel != null)
            {
                this.WeightPercentage = this.parentPatient.ParentLevel.GetTreatmentWeightPercentage(this.CustomizedWeight);
            }
            else if (this.parentLevel != null)
            {
                this.WeightPercentage = this.parentLevel.GetTreatmentWeightPercentage(this.CustomizedWeight);
            }
        }


        /// <summary>
        /// Convert this treament to a String
        /// </summary>
        /// <param name="includeTreatmentName">
        /// Include the treatment name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToOutput(bool includeTreatmentName = false)
        {
            return string.Join(",", this.ToList(includeTreatmentName));
        }


        /// <summary>
        /// Set the Patient parent.
        /// </summary>
        /// <param name="patientParent">
        /// The parent patient.
        /// </param>
        public void SetPatientParent(Patient patientParent)
        {
            this.parentPatient = patientParent;
        }

        /// <summary>
        /// Set the level parent.
        /// </summary>
        /// <param name="levelParent">
        /// The parent level.
        /// </param>
        public void SetLevelParent(Level levelParent)
        {
            this.parentLevel = levelParent;
        }
        #endregion

        #region Private

        /// <summary>
        /// Change this Treatment by retrieving the data from the new TreatmentName.
        /// </summary>
        private void ChangeTreatment()
        {
            if (this.TreatmentName != string.Empty && this.parentPatient != null && this.parentPatient.ParentLevel != null)
            {
                int roomIndex = this.parentPatient.RoomIndex;
                Treatment newTreatment = Globals.GetSettings.GetTreatment(this.TreatmentName, roomIndex);
                this.treatmentType = newTreatment.TreatmentType;
                this.DifficultyUnlocked = newTreatment.HeartsValue = newTreatment.HeartsValue;
                this.StationOwner = newTreatment.StationOwner;
                this.Weight = newTreatment.Weight;
                this.CustomizedWeight = this.Weight; // Default to the base weight from settings
                this.Gesture = newTreatment.Gesture;
                this.AlwaysLast = newTreatment.AlwaysLast;
                this.TreatmentColor = newTreatment.TreatmentColor;
            }
            else
            {
                this.treatmentType = TreatmentTypeEnum.Unknown;
                this.DifficultyUnlocked = 0;
                this.StationOwner = null;
                this.Weight = 0;
                this.Gesture = false;
                this.AlwaysLast = false;
                this.TreatmentColor = Colors.White;
            }
        }

        #endregion

        #endregion
        #region INotifyPropertyChanged Members

        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        /// <param name="propertyName">
        /// The property Name.
        /// </param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}