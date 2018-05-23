// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientChance.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Level
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;

    using HM4DesignTool.Data;
    using HM4DesignTool.Utilities;

    /// <inheritdoc />
    /// <summary>
    /// Object to store the PatientType with the Weight it can occur
    /// </summary>
    public class PatientChance : INotifyPropertyChanged
    {
        #region Fields

        #region General

        /// <summary>
        /// Only execute commands in this level when this is true.
        /// </summary>
        private readonly bool canExecuteCommands;

        #endregion General

        /// <summary>
        /// The patient name.
        /// </summary>
        private string patientName = string.Empty;

        /// <summary>
        /// The weight of this patient occuring, from 0 - 100.
        /// </summary>
        private int weight;

        /// <summary>
        /// The percentage this patient can occur relative to the other PatientChances.
        /// </summary>
        private double percentage;

        /// <summary>
        /// Whether this patientChance has been selected from the UI.
        /// </summary>
        private bool isSelected = true;

        #region Commands

        /// <summary>
        /// The randomize weight command field.
        /// </summary>
        private ICommand randomizeWeightCommand;

        #endregion Commands

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientChance"/> class, Object to store the PatientType with the Weight it can occur.
        /// </summary>
        /// <param name="patientName">
        /// The patient name.
        /// </param>
        /// <param name="weight">
        /// Thepatient weight.
        /// </param>
        public PatientChance(string patientName, int weight = 0)
        {
            this.PatientName = patientName;
            this.Weight = weight;
            this.canExecuteCommands = true;
        }

        #endregion Constructors

        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion Events

        #region Properties

        #region Public

        #region Data
        /// <summary>
        /// Gets or sets the parent level of this PatientChance.
        /// </summary>
        public Level ParentLevel { get; set; } = null;

        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName
        {
            get => this.patientName;

            set
            {
                this.patientName = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the weight of this patient occuring, from 0 - 100.
        /// </summary>
        public int Weight
        {
            get => this.weight;

            set
            {
                this.weight = value;
                this.OnPropertyChanged();
                if (this.ParentLevel != null)
                {
                    this.ParentLevel.UpdatePatientChancePercentage();
                }
            }
        }

        /// <summary>
        /// Gets or sets The percentage this patient can occur relative to the other PatientChances.
        /// </summary>
        public double Percentage
        {
            get => this.percentage;

            set
            {
                this.percentage = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("PercentageString");
            }
        }

        /// <summary>
        /// Gets the percentage string.
        /// </summary>
        public string PercentageString => this.Percentage.ToString("N1") + "%";

        /// <summary>
        /// Gets or sets a value indicating whether this patientChance has been selected from the UI.
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

        #endregion
        #region Commands

        /// <summary>
        /// Gets the randomize weight command.
        /// </summary>
        public ICommand RandomizeWeightCommand
        {
            get
            {
                return this.randomizeWeightCommand ?? (this.randomizeWeightCommand = new CommandHandler(() => this.RandomizeWeight(), this.canExecuteCommands));
            }
        }

        #endregion
        #endregion Public

        #endregion Properties

        #region Methods

        #region Public

        #region Operators

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="chance1">
        /// The chance 1.
        /// </param>
        /// <param name="chance2">
        /// The chance 2.
        /// </param>
        /// <returns>
        /// Return True if Equal
        /// </returns>
        public static bool operator ==(PatientChance chance1, PatientChance chance2)
        {
            return EqualityComparer<PatientChance>.Default.Equals(chance1, chance2);
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="chance1">
        /// The chance 1.
        /// </param>
        /// <param name="chance2">
        /// The chance 2.
        /// </param>
        /// <returns>
        /// Return True if not Equal
        /// </returns>
        public static bool operator !=(PatientChance chance1, PatientChance chance2)
        {
            return !(chance1 == chance2);
        }

        /// <summary>
        /// Equals operator.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var chance = obj as PatientChance;
            return chance != null && this.PatientName == chance.PatientName;
        }

        /// <summary>
        /// Get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return -140387131 + EqualityComparer<string>.Default.GetHashCode(this.PatientName);
        }
        #endregion Operators

        /// <summary>
        /// The randomize the occurance weight of this PatientChance.
        /// </summary>
        /// <param name="randomValue">
        /// The random value.
        /// </param>
        public void RandomizeWeight(int randomValue = 0)
        {
            if (randomValue == 0)
            {
                this.Weight = Globals.GetRandom.Next(1, 100);
            }
            else
            {
                this.Weight = randomValue;
            }
        }

        /// <summary>
        /// Update the percentage of this PatientChance in relation to all the other PatientChances.
        /// </summary>
        public void UpdatePercentage()
        {
            if (this.ParentLevel != null)
            {
                this.Percentage = this.ParentLevel.GetPatientChancePercentage(this.Weight);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.PatientName + " = " + this.Weight.ToString();
        }

        #endregion Public

        #endregion Methods

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