// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Patient.cs" company="Blue Giraffe">
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
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Input;

    using HM4DesignTool.Data;
    using HM4DesignTool.Forms;
    using HM4DesignTool.Utilities;

    /// <inheritdoc />
    /// <summary>
    /// Holds the patient triggers in a Level.
    /// </summary>
    public class Patient : INotifyPropertyChanged
    {
        #region Fields

        #region General

        /// <summary>
        /// true if the LevelOverview has finished loading and setting up.
        /// </summary>
        private readonly bool patientFinishedLoading;

        /// <summary>
        /// The parent level field.
        /// </summary>
        private Level parentLevel;

        #endregion

        #region PatientData


        private bool isSelected;

        /// <summary>
        /// The delay.
        /// </summary>
        private int delay = 1000;

        /// <summary>
        /// The patient name.
        /// </summary>
        private string patientName = string.Empty;

        /// <summary>
        /// The weight of this patients when in TimeTrial field.
        /// </summary>
        private int weight;

        #endregion

        #region Treatment

        /// <summary>
        /// The treatment list for this patient.
        /// </summary>
        private ObservableCollection<Treatment> treatmentList = new ObservableCollection<Treatment>();

        #endregion

        #region PatientTraits

        /// <summary>
        /// The patient trait string.
        /// </summary>
        private string patientTraitString = string.Empty;

        /// <summary>
        /// The patient trait list for this patient.
        /// </summary>
        private ObservableCollection<PatientTrait> patientTraitList = new ObservableCollection<PatientTrait>();

        #endregion

        #region Commands

        /// <summary>
        /// The open traits window commdnd fieleld.
        /// </summary>
        private ICommand openTraitsWindowCommand;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Patient"/> class.
        /// </summary>
        /// <param name="parentLevel">
        /// The parent level.
        /// </param>
        /// <param name="patientName">
        /// The patient name.
        /// </param>
        public Patient(Level parentLevel, string patientName = null)
        {
            this.ParentLevel = parentLevel;

            if (patientName != null)
            {
                this.patientName = patientName;
            }

            this.patientFinishedLoading = true;
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
        #region PatientData

        public int PatientIndex => this.ParentLevel.PatientCollection.IndexOf(this);

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
        /// Gets or sets the PatientName.
        /// </summary>
        public string PatientName
        {
            get => this.patientName;

            set
            {
                this.patientName = value;
                this.OnPropertyChanged();
                this.ParentLevel?.UpdateLevelOutput();
            }
        }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        public int Delay
        {
            get => this.delay;

            set
            {
                this.delay = value;
                this.OnPropertyChanged();

                this.ParentLevel?.UpdateLevelOutput();
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
                this.ParentLevel?.UpdateLevelOutput();
            }
        }

        #endregion

        #region Treatment

        public int TreatmentCount => this.TreatmentCollection.Count;

        /// <summary>
        /// Gets the valid treatment count.
        /// </summary>
        public int ValidTreatmentCount
        {
            get
            {
                int count = 0;
                foreach (Treatment treatment in this.TreatmentCollection)
                {
                    if (!treatment.IsEmpty)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the visible treatment count.
        /// </summary>
        public int TreatmentCollectionVisibleCount
        {
            get
            {
                int count = 0;
                foreach (Treatment treatment in this.TreatmentCollection)
                {
                    if (treatment.IsVisible)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the treatment list string.
        /// </summary>
        public string TreatmentListString
        {
            get
            {
                string output = string.Empty;

                foreach (Treatment treatment in this.TreatmentCollection)
                {
                    if (!treatment.IsEmpty)
                    {
                        output = $"{output}\"{treatment.TreatmentName}\", ";
                    }
                }

                // Remove last ,
                if (output.EndsWith(", "))
                {
                    output = output.Remove(output.Length - 2, 2);
                }

                return output;
            }
        }

        /// <summary>
        /// Gets the treatment options used for the Treatment dropdowns.
        /// </summary>
        public ObservableCollection<string> TreatmentOptions
        {
            get
            {
                if (this.ParentLevel != null)
                {
                    List<string> output = this.ParentLevel.AvailableTreatmentStringList;
                    output.Insert(0, string.Empty);

                    return new ObservableCollection<string>(output);
                }
                else
                {
                    return new ObservableCollection<string>();
                }
            }
        }
        #endregion

        #region ItemCollections

        /// <summary>
        /// Gets or sets the treatment collection used to bound to from the front-end.
        /// </summary>
        public ObservableCollection<Treatment> TreatmentCollection
        {
            get => this.treatmentList;

            set
            {
                int maxCount = Math.Max(Globals.GetLevelOverview.MaxTreatmentsVisible, value.Count);
                for (int i = 0; i < maxCount; i++)
                {
                    if (i < value.Count)
                    {
                        if (value[i] != null)
                        {
                            value[i].SetLevelParent(this.parentLevel);
                            value[i].SetPatientParent(this);
                        }
                    }
                    else
                    {
                        value.Add(new Treatment());
                    }
                }
                this.OnPropertyChanged();
                this.ParentLevel?.UpdateLevelOutput();
            }
        }

        /// <summary>
        /// Gets or sets the list with all the Patient objects in this level converted to an ObservableCollection.
        /// </summary>
        public ObservableCollection<PatientTrait> PatientTraitCollection
        {
            get => this.patientTraitList;

            set
            {
                this.patientTraitList = value;
                this.OnPropertyChanged();
                this.UpdatePatientTraitString();
            }
        }

        /// <summary>
        /// Gets or sets the patient trait string.
        /// </summary>
        public string PatientTraitString
        {
            get => this.patientTraitString;

            set
            {
                this.patientTraitString = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// The open traits window command.
        /// </summary>
        public ICommand OpenTraitsWindowCommand => this.openTraitsWindowCommand ?? (this.openTraitsWindowCommand = new CommandHandler(this.OpenTraitsWindow, this.patientFinishedLoading));

        #endregion

        #endregion

        #region General

        /// <summary>
        /// Gets the room index of this patient.
        /// </summary>
        public int RoomIndex
        {
            get
            {
                if (this.ParentLevel != null)
                {
                    return this.ParentLevel.GetRoomIndex;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent level.
        /// </summary>
        public Level ParentLevel
        {
            get => this.parentLevel;

            set
            {
                this.parentLevel = value;
                this.OnPropertyChanged();
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
        /// <param name="patient1">
        /// Patient 1.
        /// </param>
        /// <param name="patient2">
        /// Patient 2.
        /// </param>
        /// <returns>
        /// Return true if both Patient objects are equal.
        /// </returns>
        public static bool operator ==(Patient patient1, Patient patient2)
        {
            return EqualityComparer<Patient>.Default.Equals(patient1, patient2);
        }

        /// <summary>
        /// The != operator.
        /// </summary>
        /// <param name="patient1">
        /// The patient 1.
        /// </param>
        /// <param name="patient2">
        /// The patient 2.
        /// </param>
        /// <returns>
        /// Return true if both Patient objects are NOT equal.
        /// </returns>
        public static bool operator !=(Patient patient1, Patient patient2)
        {
            return !(patient1 == patient2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var patient = obj as Patient;
            return patient != null && this.PatientName == patient.PatientName && this.Delay == patient.Delay && this.Weight == patient.Weight &&
                   EqualityComparer<ObservableCollection<Treatment>>.Default.Equals(this.TreatmentCollection, patient.TreatmentCollection) &&
                   EqualityComparer<ObservableCollection<PatientTrait>>.Default.Equals(this.PatientTraitCollection, patient.PatientTraitCollection);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = 1139967511;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.PatientName);
            hashCode = (hashCode * -1521134295) + this.Delay.GetHashCode();
            hashCode = (hashCode * -1521134295) + this.Weight.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<ObservableCollection<Treatment>>.Default.GetHashCode(this.TreatmentCollection);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ObservableCollection<PatientTrait>>.Default.GetHashCode(this.PatientTraitCollection);
            return hashCode;
        }
        #endregion

        #region PatientData

        /// <summary>
        /// Set patient data from string.
        /// </summary>
        /// <param name="patientData">
        /// The patient data.
        /// </param>
        public void SetPatientData(string patientData)
        {
            this.ParsePatientData(patientData);
        }

        #endregion
        #region Treatment

        /// <summary>
        /// Set the max (visible) treatments.
        /// </summary>
        /// <param name="value">
        /// The new amount of treatments.
        /// </param>
        public void SetMaxTreatments(int value)
        {
            if (value > 0)
            {
                int treatmentCount = this.TreatmentCollectionVisibleCount;

                if (value < treatmentCount)
                {
                    int amountToSubtract = treatmentCount - value;

                    for (int i = 1; i <= amountToSubtract; i++)
                    {
                        int index = treatmentCount - i;

                        this.TreatmentCollection.ElementAt(index).IsVisible = false;
                    }
                }
                else if (value > treatmentCount)
                {
                    int amountToAdd = value - treatmentCount;

                    for (int i = 0; i < amountToAdd; i++)
                    {
                        int index = treatmentCount + i;
                        if (index < this.TreatmentCollection.Count)
                        {
                            this.TreatmentCollection.ElementAt(index).IsVisible = true;
                        }
                        else
                        {
                            this.AddTreatment();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Set the treatments from List(string Treatment Name).
        /// </summary>
        /// <param name="listTreatment">
        /// The treatment list.
        /// </param>
        public void SetTreatments(List<string> listTreatment)
        {
            List<Treatment> treatmentList = new List<Treatment>();
            foreach (string treatment in listTreatment)
            {
                treatmentList.Add(new Treatment(treatment, this));
            }

            this.SetTreatments(treatmentList);
            this.OnPropertyChanged("TreatmentCollection");
        }


        /// <summary>
        /// Set the treatments from List(Treatment).
        /// </summary>
        /// <param name="listTreatment">
        /// The treatment list.
        /// </param>
        public void SetTreatments(List<Treatment> listTreatment)
        {
            // Ensure that all treatments have empty slots up till the maxTreatments visible. 
            int maxCount = Math.Max(Globals.GetLevelOverview.MaxTreatmentsVisible, listTreatment.Count);
            for (int i = 0; i < maxCount; i++)
            {
                if (i < listTreatment.Count)
                {
                    if (listTreatment[i] != null)
                    {
                        listTreatment[i].SetLevelParent(this.parentLevel);
                        listTreatment[i].SetPatientParent(this);
                    }
                }
                else
                {
                    listTreatment.Add(new Treatment());
                }
            }

            // Make sure the maxAmount of visible treatments in visible
            this.SetMaxTreatments(Math.Max(listTreatment.Count, this.TreatmentCollection.Count));

            for (int i = 0; i < this.TreatmentCollection.Count; i++)
            {
                Treatment treatment = this.TreatmentCollection.ElementAt(i);

                if (i < listTreatment.Count)
                {
                    treatment.TreatmentName = listTreatment[i].TreatmentName;
                }
                else
                {
                    treatment.TreatmentName = string.Empty;
                }

                treatment.SetPatientParent(this);
            }
        }


        public void TreatmentsUpdated()
        {
            this.OnPropertyChanged("TreatmentCollection");
        }

        #endregion
        #region String

        /// <summary>
        /// Update the patient trait string.
        /// </summary>
        public void UpdatePatientTraitString()
        {
            string output = string.Empty;

            foreach (PatientTrait patientTrait in this.PatientTraitCollection)
            {
                output += patientTrait.ToString();
            }

            if (output.EndsWith(","))
            {
                output = output.TrimEnd(',');
            }

            this.PatientTraitString = output;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.PatientName}, {this.delay}, {this.TreatmentListString},";
        }

        /// <summary>
        /// Print this output in Level format. 
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToOutput()
        {
            if (this.ValidTreatmentCount > 0)
            {
                string output = "\t{";

                if (this.delay > -1)
                {
                    // Add extra space to line everything up
                    if (this.delay < 10000)
                    {
                        output = $"{output} delay =  {this.delay.ToString()}";
                    }
                    else
                    {
                        output = $"{output} delay = {this.delay.ToString()}";
                    }
                }

                if (this.ParentLevel.WeightEnabled && this.weight > -1)
                {
                    output = $"{output}, weight = {this.weight.ToString()}";
                }

                if (this.TreatmentCollection != null && this.TreatmentCollection.Count > 0)
                {
                    output = $"{output}, todo = {{{this.TreatmentListString}}}";
                }

                if (this.PatientTraitCollection.Count > 0)
                {
                    output += ", ";

                    foreach (PatientTrait patientTrait in this.PatientTraitCollection)
                    {
                        output += patientTrait.ToString();
                    }
                }

                output = $"{output}}},{Environment.NewLine}";

                return output;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Window

        /// <summary>
        /// The open the patient traits window.
        /// </summary>
        public void OpenTraitsWindow()
        {
            // Make sure at least 10 entries are avalible. 
            if (this.PatientTraitCollection.Count <= 10)
            {
                for (; this.PatientTraitCollection.Count <= 10;)
                {
                    this.PatientTraitCollection.Add(new PatientTrait());
                }
            }

            TraitsWindow dialog = new TraitsWindow { PatientParent = this };
            dialog.PatientParent = this;
            bool? unused = dialog.ShowDialog();
        }

        #endregion

        #endregion
        #region Private

        #region ParseData

        /// <summary>
        /// Parse patient data from string.
        /// </summary>
        /// <param name="patientData">
        /// The patient data.
        /// </param>
        private void ParsePatientData(string patientData)
        {
            patientData = patientData.Replace("\"", string.Empty);

            List<string> parameterList = new List<string> { "delay", "weight", "todo" };
            for (int i = 0; i < parameterList.Count; i++)
            {
                if (patientData.Contains(parameterList[i]))
                {
                    string text = Data.GetVariable(patientData, parameterList[i]);
                    text = Globals.RemoveFirstComma(text);

                    if (text != string.Empty)
                    {
                        switch (i)
                        {
                            // Delay
                            case 0:
                                this.Delay = Globals.StringToInt(text);
                                break;

                            // Weight
                            case 1:
                                this.Weight = Globals.StringToInt(text);
                                break;

                            // Treatments
                            case 2:
                                this.SetTreatments(text.Split(',').ToList());
                                break;

                            default:
                                break;
                        }

                    }
                    // Remove found entries to later still store any unindexed text. 
                    patientData = patientData.Replace($"{parameterList[i]}={text}", string.Empty);
                    patientData = patientData.Replace($"{parameterList[i]}={{{text}}}", string.Empty);

                    patientData = Globals.RemoveFirstComma(patientData);
                }
            }

            // If there is remaining data then it is probably traits that have been added.

            if (patientData != string.Empty && patientData.Length > 0)
            {
                List<string> traitsList = Regex.Split(patientData, ",").Where(s => s != string.Empty).ToList();
                this.PatientTraitCollection.Clear();

                foreach (string trait in traitsList)
                {
                    List<string> valueList = Regex.Split(trait, "=").Where(s => s != string.Empty).ToList();
                    if (valueList.Count == 2)
                    {
                        this.PatientTraitCollection.Add(new PatientTrait(valueList[0], valueList[1]));
                    }
                    else if (valueList.Count == 1)
                    {
                        this.PatientTraitCollection.Add(new PatientTrait(valueList[0]));

                    }
                }
            }
        }

        #endregion
        #region Treatment

        /// <summary>
        /// Add treatment to Patient.
        /// </summary>
        /// <param name="treatmentName">
        /// The treatment name.
        /// </param>
        private void AddTreatment(string treatmentName = "")
        {
            Treatment treatment;
            if (treatmentName != string.Empty)
            {
                treatment = Globals.GetSettings.GetTreatment(treatmentName, this.RoomIndex);
            }
            else
            {
                treatment = new Treatment();
            }

            treatment.SetPatientParent(this);
            treatment.IsVisible = true;
            this.TreatmentCollection.Add(treatment);
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
    }
}
