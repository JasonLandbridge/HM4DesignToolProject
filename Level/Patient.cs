using DataNameSpace;
using NaturalSort.Extension;
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

namespace LevelData
{
    using HM4DesignTool.Level;

    public class Patient : INotifyPropertyChanged
    {
        public Level ParentLevel = null;

        private ObservableCollection<Treatment> _treatmentList = new ObservableCollection<Treatment> { };

        private int delay = 1000;

        private String patientName = String.Empty;

        private Dictionary<String, String> patientTraits;

        private int weight = 0;

        private bool weightEnabled = false;

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

        public event PropertyChangedEventHandler PropertyChanged;

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
                    return new ObservableCollection<String>(ParentLevel.AvailableTreatmentStringList);
                }
                else
                {
                    return new ObservableCollection<String> { };
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

        public int GetTreatmentCount
        {
            get
            {
                int count = 0;
                foreach (Treatment treatment in TreatmentCollection)
                {
                    if (!treatment.IsEmpty)
                    {
                        count++;
                    }
                }

                return count;

            }
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

        public void SetPatientData(String patientData)
        {
            ParsePatientData(patientData);
        }

        public void SetTreatments(List<Treatment> TreatmentList)
        {
            //Make sure the maxAmount of visible treatments in visible
            SetMaxTreatments(Math.Max(TreatmentList.Count, TreatmentCollection.Count));


            for (int i = 0; i < TreatmentCollection.Count; i++)
            {
                Treatment treatment = TreatmentCollection.ElementAt<Treatment>(i);

                if (i < TreatmentList.Count)
                {
                    treatment.TreatmentName = TreatmentList[i].TreatmentName;
                }
                else
                {
                    treatment.TreatmentName = String.Empty;
                }

                treatment.SetPatientParent(this);
            }

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
                    if (!treatment.IsEmpty)
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

        private void ParsePatientData(String patientData)
        {
            // Clean up the patientData
            patientData = System.Text.RegularExpressions.Regex.Replace(patientData, @"\s+", "");

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
                catch (Exception)
                {
                    Console.WriteLine($"ERROR: Level.ParsePatientData, failed to parse patientData: {patientData}");
                }
            }
        }
        #region Events

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Events
    }

}
