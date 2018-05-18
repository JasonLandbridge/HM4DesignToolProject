// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameValues.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  Contains all the important Game Values for calculations </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Game Values of HM4.
    /// </summary>
    public class GameValues
    {
        // TODO Make sure to import the constant values from GSettings
        #region Properties

        /// <summary>
        /// Gets or sets the difficulty modifier treatments based.
        /// </summary>
        public int DifficultyModifierTreatmentsBased { get; set; } = 11;

        /// <summary>
        /// Gets or sets the start level duration.
        /// </summary>
        public int StartLevelDuration { get; set; } = 110000;

        /// <summary>
        /// Gets or sets the time increase per level.
        /// </summary>
        public int TimeIncreasePerLevel { get; set; } = 4500;

        /// <summary>
        /// Gets or sets the initial time between patients.
        /// </summary>
        public int InitialTimeBetweenPatients { get; set; } = 11000;

        /// <summary>
        /// Gets or sets the decrease time between patients.
        /// </summary>
        public int DecreaseTimeBetweenPatients { get; set; } = 250;

        /// <summary>
        /// Gets or sets the initial time per treatment.
        /// </summary>
        public int InitialTimePerTreatment { get; set; } = 6000;

        /// <summary>
        /// Gets or sets the decrease time per treatment.
        /// </summary>
        public int DecreaseTimePerTreatment { get; set; } = 250;

        /// <summary>
        /// Gets or sets the checkout per patient.
        /// </summary>
        public int CheckoutPerPatient { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the treatment minimum time.
        /// </summary>
        public int TreatmentMinimumTime { get; set; } = 1600;

        #endregion

        /// <summary>
        /// Gets a dictionary from all the GameValues.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, double> GetBalancingData(double difficultyModifier)
        {
            Dictionary<string, double> balancingData = new Dictionary<string, double>();

            if (difficultyModifier > 0.5)
            {
                balancingData.Add("averageEntryTimePerPatient", this.AverageEntryTimePerPatient(difficultyModifier));
                balancingData.Add("timeBetweenPatients", this.TimeBetweenPatients(difficultyModifier));
                balancingData.Add("numberOfPatients", this.NumberOfPatients(difficultyModifier));
                balancingData.Add("treatmentPerPatient", this.TreatmentPerPatient(difficultyModifier));
                balancingData.Add("timePerTreatment", this.TimePerTreatment(difficultyModifier));
                balancingData.Add("milliSecondsPerLevel", this.MilliSecondsPerLevel(difficultyModifier));
                balancingData.Add("minutesPerLevel", this.MinutesPerLevel(difficultyModifier));

                return balancingData;
            }
            else
            {
                return null;
            }
        }

        #region GameValues

        /// <summary>
        /// The treatment per patient.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double TreatmentPerPatient(double difficultyModifier)
        {
            if (Math.Round(difficultyModifier / this.DifficultyModifierTreatmentsBased, 2) + 1 > 3.5)
            {
                return 3.5;
            }
            else
            {
                return Math.Round(difficultyModifier / this.DifficultyModifierTreatmentsBased, 2) + 1;
            }
        }

        /// <summary>
        /// The treatment per patient to int.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int TreatmentPerPatientToInt(double difficultyModifier)
        {
            double x = Math.Round(this.TreatmentPerPatient(difficultyModifier), MidpointRounding.AwayFromZero);
            return Convert.ToInt32(x);
        }

        /// <summary>
        /// The time per treatment.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double TimePerTreatment(double difficultyModifier)
        {
            double x = this.InitialTimePerTreatment - (difficultyModifier * this.DecreaseTimePerTreatment);

            if (x < this.TreatmentMinimumTime)
            {
                return this.TreatmentMinimumTime;
            }
            else
            {
                return x;
            }
        }

        /// <summary>
        /// The milli seconds per level.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double MilliSecondsPerLevel(double difficultyModifier)
        {
            double x = this.StartLevelDuration + (difficultyModifier * this.TimeIncreasePerLevel);

            if (x > 400000)
            {
                return 400000;
            }
            else
            {
                return x;
            }
        }

        /// <summary>
        /// The minutes per level.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double MinutesPerLevel(double difficultyModifier)
        {
            return this.MilliSecondsPerLevel(difficultyModifier) / 60000;
        }

        /// <summary>
        /// The time between patients.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double TimeBetweenPatients(double difficultyModifier)
        {
            return this.InitialTimeBetweenPatients - (difficultyModifier * this.DecreaseTimeBetweenPatients);
        }

        /// <summary>
        /// The average entry time per patient.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double AverageEntryTimePerPatient(double difficultyModifier)
        {
            double x = this.TreatmentPerPatient(difficultyModifier) * this.TimePerTreatment(difficultyModifier);
            x += this.TimeBetweenPatients(difficultyModifier);
            x += this.CheckoutPerPatient;

            return x;
        }

        /// <summary>
        /// The number of patients per level.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double NumberOfPatients(double difficultyModifier)
        {
            double x = this.MilliSecondsPerLevel(difficultyModifier) / this.AverageEntryTimePerPatient(difficultyModifier);
            return Math.Ceiling(x); // TODO Check if similair to math.ceil in python
        }

        /// <summary>
        /// The number of patients per level to int.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int NumberOfPatientsToInt(double difficultyModifier)
        {
            return Convert.ToInt32(this.NumberOfPatients(difficultyModifier));
        }

        #endregion

        #region Overloads

        /// <summary>
        /// Get balancing data from string difficulty modifier.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, double> GetBalancingData(string difficultyModifier)
        {
            return this.GetBalancingData(Globals.StringToDouble(difficultyModifier));
        }

        /// <summary>
        /// The treatment per patient.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double TreatmentPerPatient(string difficultyModifier)
        {
            return this.TreatmentPerPatient(Globals.StringToDouble(difficultyModifier));
        }

        /// <summary>
        /// The time per treatment.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double TimePerTreatment(string difficultyModifier)
        {
            return this.TimePerTreatment(Globals.StringToDouble(difficultyModifier));
        }

        /// <summary>
        /// The milli seconds per level.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double MilliSecondsPerLevel(string difficultyModifier)
        {
            return this.MilliSecondsPerLevel(Globals.StringToDouble(difficultyModifier));
        }

        /// <summary>
        /// The minutes per level.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double MinutesPerLevel(string difficultyModifier)
        {
            return this.MinutesPerLevel(Globals.StringToDouble(difficultyModifier));
        }

        /// <summary>
        /// The time between patients.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double TimeBetweenPatients(string difficultyModifier)
        {
            return this.TimeBetweenPatients(Globals.StringToDouble(difficultyModifier));
        }

        /// <summary>
        /// The average entry time per patient.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double AverageEntryTimePerPatient(string difficultyModifier)
        {
            return this.AverageEntryTimePerPatient(Globals.StringToDouble(difficultyModifier));
        }

        /// <summary>
        /// The number of patients.
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double NumberOfPatients(string difficultyModifier)
        {
            return this.NumberOfPatients(Globals.StringToDouble(difficultyModifier));
        }

        #endregion
    }
}
