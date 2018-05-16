using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

using DataNameSpace;

using NaturalSort.Extension;

namespace LevelData
{
    using System.Windows.Input;

    using HM4DesignTool.Forms;
    using HM4DesignTool.Level;
    using HM4DesignTool.Utilities;

    public class Patient : INotifyPropertyChanged
    {




        #region Fields


        #region General
        /// <summary>
        /// true if the LevelOverview has finished loading and setting up.
        /// </summary>
        private bool patientFinishedLoading;

        public Level ParentLevel = null;


        private int delay = 1000;

        private string patientName = string.Empty;

        private Dictionary<string, string> patientTraits;

        private int weight = 0;

        private bool weightEnabled = false;


        #endregion

        #region Treatment

        private ObservableCollection<Treatment> _treatmentList = new ObservableCollection<Treatment> { };


        #endregion

        #region PatientTraits

        private List<PatientTrait> patientTraitList = new List<PatientTrait>();

        #endregion

        #region Commands

        private ICommand openTraitsWindowCommand;

        #endregion


        #endregion


        #region Constructors
        public Patient()
        {
            this.patientFinishedLoading = true;
        }

        public Patient(Level ParentLevel, string patientName = null)
        {
            this.ParentLevel = ParentLevel;

            if (patientName != null)
            {
                this.patientName = patientName;
            }

            this.patientFinishedLoading = true;

        }

        #endregion












        #region Events



        #endregion

        #region Properties

        #region Public
        #region PatientData
        public string PatientName
        {
            get => this.patientName;

            set
            {
                this.patientName = value;
                this.OnPropertyChanged("PatientName");
                if (this.ParentLevel != null)
                {
                    this.ParentLevel.UpdateLevelOutput();
                }
            }
        }

        public int Delay
        {
            get
            {
                return this.delay;
            }

            set
            {
                this.delay = value;
                this.OnPropertyChanged("Delay");

                if (this.ParentLevel != null)
                {
                    this.ParentLevel.UpdateLevelOutput();
                }
            }
        }

        public int Weight
        {
            get
            {
                return this.weight;
            }

            set
            {
                this.weight = value;
                this.OnPropertyChanged("Weight");
                if (this.ParentLevel != null)
                {
                    this.ParentLevel.UpdateLevelOutput();
                }
            }
        }

        #endregion



        public List<PatientTrait> PatientTraitList { get; set; }



        #region ItemCollections
        /// <summary>
        /// Gets or sets the list with all the Patient objects in this level converted to an ObservableCollection.
        /// </summary>
        public ObservableCollection<PatientTrait> PatientTraitCollection
        {
            get => new ObservableCollection<PatientTrait>(this.PatientTraitList);

            set
            {
                this.PatientTraitList = value.ToList();
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Commands
        public ICommand OpenTraitsWindowCommand
        {
            get
            {
                return this.openTraitsWindowCommand ?? (this.openTraitsWindowCommand = new CommandHandler(this.OpenTraitsWindow, this.patientFinishedLoading));
            }
        }



        #endregion

        #endregion

        #region General
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

        #endregion

        #region Private



        #endregion

        #endregion



        #region Methods

        #region Public



        #endregion

        #region Private

        private void ParsePatientData(string patientData)
        {
            // Clean up the patientData
            patientData = System.Text.RegularExpressions.Regex.Replace(patientData, @"\s+", string.Empty);
            patientData = patientData.Replace("\t", String.Empty);
            patientData = RemoveFirstComma(patientData);

            // Parse the delay
            string delayText = "delay=";
            if (patientData.Contains(delayText))
            {
                int startIndex = patientData.IndexOf(delayText) + delayText.Length;
                int endIndex = patientData.IndexOf(",");
                if (startIndex > -1 && endIndex > -1)
                {
                    string delayString = patientData.Substring(startIndex, endIndex - startIndex);
                    delayString = Globals.FilterToNumerical(delayString);
                    if (delayString != string.Empty)
                    {
                        this.delay = Convert.ToInt32(delayString);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Level.ParsePatientData, Failed to parse delayString, was none!");
                    }

                    patientData = patientData.Remove(startIndex - delayText.Length, endIndex);
                }
            }

            // Parse the weight
            patientData = RemoveFirstComma(patientData);
            string weightText = "weight=";
            if (patientData.Contains(weightText))
            {
                int startIndex = patientData.IndexOf(weightText) + weightText.Length;
                int endIndex = patientData.IndexOf(",");
                if (startIndex > -1 && endIndex > 0)
                {
                    string weightString = patientData.Substring(startIndex, endIndex - startIndex);
                    weightString = Globals.FilterToNumerical(weightString);
                    if (weightString != string.Empty)
                    {
                        this.weight = Convert.ToInt32(weightString);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Level.ParsePatientData, Failed to parse weightString, was none!");
                        this.weight = -1;
                    }

                    patientData = patientData.Remove(startIndex - weightText.Length, endIndex);
                    this.weightEnabled = true;
                }
            }

            // Parse the treatment list
            patientData = RemoveFirstComma(patientData);
            string treatmentStartText = "todo={";
            string treatmentEndText = "}";
            if (patientData.Contains(treatmentStartText))
            {
                int startIndex = patientData.IndexOf(treatmentStartText) + treatmentStartText.Length;
                int endIndex = patientData.IndexOf(treatmentEndText, startIndex);
                if (startIndex > -1 && endIndex > -1)
                {
                    string rawTreatments = patientData.Substring(startIndex, endIndex - startIndex);
                    rawTreatments = rawTreatments.Replace("\"", string.Empty);
                    List<string> TreatmentList = rawTreatments
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

                    // Convert found treatments to Treatment Objects
                    for (int i = 0;
                         i < Math.Max(TreatmentList.Count, Globals.GetLevelOverview.MaxTreatmentsVisible);
                         i++)
                    {
                        if (i < TreatmentList.Count)
                        {
                            this.AddTreatment(TreatmentList[i]);
                        }
                        else
                        {
                            this.AddTreatment();
                        }
                    }

                    patientData = patientData.Remove(startIndex - treatmentStartText.Length, endIndex);

                }
            }

            // If there is remaining data then it is probably traits that have been added.
            patientData = RemoveFirstComma(patientData);
            if (patientData.Length > 5)
            {

                patientData = patientData.Trim().Replace("=", ":");
                try
                {
                    this.patientTraits =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(patientData);
                }
                catch (Exception)
                {
                    Console.WriteLine($"ERROR: Level.ParsePatientData, failed to parse patientData: {patientData}");
                }
            }
        }

        public void OpenTraitsWindow()
        {
            TraitsWindow dialog = new TraitsWindow();
            bool? dialogResult = dialog.ShowDialog();

        }

        #endregion

        #endregion


        #region Operators


        #endregion



        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
        public int GetTreatmentCount
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
        public ObservableCollection<Treatment> TreatmentCollection
        {
            get
            {
                return this._treatmentList;
            }

            set
            {
                this._treatmentList = value;

                this.OnPropertyChanged("TreatmentCollection");
                if (this.ParentLevel != null)
                {
                    this.ParentLevel.UpdateLevelOutput();
                }
            }
        }

        public int TreatmentCollectionVisibleCount
        {
            get
            {
                int count = 0;
                foreach (Treatment treatment in this.TreatmentCollection)
                {
                    if (treatment.IsVisible == true)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

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
                    return new ObservableCollection<string> { };
                }
            }
        }
        public void SetMaxTreatments(int Value)
        {
            if (Value > 0)
            {
                int treatmentCount = this.TreatmentCollectionVisibleCount;

                if (Value < treatmentCount)
                {
                    int amountToSubtract = treatmentCount - Value;

                    for (int i = 1; i <= amountToSubtract; i++)
                    {
                        int index = treatmentCount - i;

                        this.TreatmentCollection.ElementAt(index).IsVisible = false;

                    }

                }
                else if (Value > treatmentCount)
                {
                    int amountToAdd = Value - treatmentCount;

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

        public void SetPatientData(string patientData)
        {
            this.ParsePatientData(patientData);
        }
        public void SetTreatments(List<Treatment> TreatmentList)
        {
            // Make sure the maxAmount of visible treatments in visible
            this.SetMaxTreatments(Math.Max(TreatmentList.Count, this.TreatmentCollection.Count));


            for (int i = 0; i < this.TreatmentCollection.Count; i++)
            {
                Treatment treatment = this.TreatmentCollection.ElementAt<Treatment>(i);

                if (i < TreatmentList.Count)
                {
                    treatment.TreatmentName = TreatmentList[i].TreatmentName;
                }
                else
                {
                    treatment.TreatmentName = string.Empty;
                }

                treatment.SetPatientParent(this);
            }

        }

        public string ToOutput()
        {
            if (this.GetTreatmentCount > 0)
            {
                string output = "\t{";

                if (this.delay > -1)
                {
                    //Add extra space to line everything up
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

                if (this.TreatmentCollection != null && this.TreatmentCollection.Count() > 0)
                {
                    output = $"{output}, todo = {{{this.TreatmentListString}}}";

                }

                if (this.patientTraits != null && this.patientTraits.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> trait in this.patientTraits)
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

                output = $"{output}}},{Environment.NewLine}";

                return output;
            }
            else
            {
                return string.Empty;
            }

        }

        public override string ToString()
        {
            return $"{this.PatientName}, {this.delay}, {this.TreatmentListString},";
        }
        private static string RemoveFirstComma(string patientString)
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

        private void AddTreatment(string treatmentName = "")
        {
            Treatment treatment = null;
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
