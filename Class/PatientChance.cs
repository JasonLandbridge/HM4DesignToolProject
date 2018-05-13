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
using System.Windows.Input;
using Utilities;

namespace LevelData
{
    public class PatientChance : INotifyPropertyChanged // TODO Check if this can be removed ,IEquatable<PatientChance>
    {
        public Level ParentLevel = null;
        private bool _isSelected = true;
        private String _patientName = String.Empty;

        private double _percentage = 0;

        private int _weight = 0;

        public PatientChance(String PatientName, int Weight = 0)
        {
            this.PatientName = PatientName;
            this.Weight = Weight;
            _canExecute = true;
        }

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

        public String PercentageString
        {
            get
            {
                return Percentage.ToString("N1") + "%";
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
                if (ParentLevel != null)
                {
                    ParentLevel.UpdatePatientChancePercentage();
                }
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

        public override bool Equals(object obj)
        {
            var chance = obj as PatientChance;
            return chance != null &&
                   PatientName == chance.PatientName;
        }

        public override int GetHashCode()
        {
            return -140387131 + EqualityComparer<string>.Default.GetHashCode(PatientName);
        }

        public static bool operator ==(PatientChance chance1, PatientChance chance2)
        {
            return EqualityComparer<PatientChance>.Default.Equals(chance1, chance2);
        }

        public static bool operator !=(PatientChance chance1, PatientChance chance2)
        {
            return !(chance1 == chance2);
        }

        //public static bool operator !=(PatientChance patientChance1, PatientChance patientChance2)
        //{
        //    return !(patientChance1 == patientChance2);
        //}

        //public static bool operator ==(PatientChance patientChance1, PatientChance patientChance2)
        //{
        //    return patientChance1.PatientName == patientChance2.PatientName;
        //}

        //public bool Equals(PatientChance other)
        //{
        //    return this.PatientName == other.PatientName;
        //}

        //public override int GetHashCode()
        //{
        //    return -140387131 + EqualityComparer<string>.Default.GetHashCode(PatientName);
        //}
        #endregion Operators

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        #endregion INotifyPropertyChanged Members
    }

}
