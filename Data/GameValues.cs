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
    public static class GameValues
    {
        // TODO Make sure to import the constant values from GSettings
        #region Properties
        /// <summary>
        /// Gets or sets the base amount of duration a level lasts in milliseconds.
        /// </summary>
        public static int StartLevelDuration { get; set; } = 110000;

        /// <summary>
        /// Gets or sets the time increase per difficulty modifier level in milliseconds.
        /// </summary>
        public static int TimeIncreasePerLevel { get; set; } = 4500;

        /// <summary>
        /// Gets or sets the initial time in milliseconds between patients.
        /// </summary>
        public static int InitialTimeBetweenPatients { get; set; } = 11000;

        /// <summary>
        /// Gets or sets the decrease time in milliseconds between patients.
        /// </summary>
        public static int DecreaseTimeBetweenPatients { get; set; } = 250;

        /// <summary>
        /// Gets or sets the checkout per patient.
        /// </summary>
        public static int CheckoutPerPatient { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the initial time per treatment.
        /// </summary>
        public static int InitialTimePerTreatment { get; set; } = 6000;

        /// <summary>
        /// Gets or sets the treatment minimum time.
        /// </summary>
        public static int TreatmentMinimumTime { get; set; } = 1600;

        /// <summary>
        /// Gets or sets the decrease time per treatment.
        /// </summary>
        public static int DecreaseTimePerTreatment { get; set; } = 250;

        /// <summary>
        /// Gets or sets the treatment based difficulty modifier.
        /// </summary>
        public static int TreatmentBasedDifficultyModifier { get; set; } = 11;

        #endregion

        #region Methods

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
        public static Dictionary<string, double> GetBalancingData(double difficultyModifier)
        {
            Dictionary<string, double> balancingData = new Dictionary<string, double>();

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

        #endregion

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
        public static double TreatmentPerPatient(double difficultyModifier)
        {
            double treatmentPerPatient = Math.Round(difficultyModifier / TreatmentBasedDifficultyModifier, 2) + 1;

            // Never return a value higher than 3.5.
            return Math.Min(treatmentPerPatient, 3.5);
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
        public static int TreatmentPerPatientToInt(double difficultyModifier)
        {
            double x = Math.Round(TreatmentPerPatient(difficultyModifier), MidpointRounding.AwayFromZero);
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
        public static double TimePerTreatment(double difficultyModifier)
        {
            double x = InitialTimePerTreatment - (difficultyModifier * DecreaseTimePerTreatment);
            
            // Never return a value lower than the TreatmentMinimumTime.
            return Math.Max(x, TreatmentMinimumTime);
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
        public static double MilliSecondsPerLevel(double difficultyModifier)
        {
            double x = StartLevelDuration + (difficultyModifier * TimeIncreasePerLevel);

            // Never return a value higher than 400000 ms.
            return Math.Min(x, 400000);
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
        public static double MinutesPerLevel(double difficultyModifier)
        {
            return MilliSecondsPerLevel(difficultyModifier) / 60000;
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
        public static double TimeBetweenPatients(double difficultyModifier)
        {
            return InitialTimeBetweenPatients - (difficultyModifier * DecreaseTimeBetweenPatients);
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
        public static double AverageEntryTimePerPatient(double difficultyModifier)
        {
            double x = TreatmentPerPatient(difficultyModifier) * TimePerTreatment(difficultyModifier);
            x += TimeBetweenPatients(difficultyModifier) + CheckoutPerPatient;

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
        public static double NumberOfPatients(double difficultyModifier)
        {
            double x = MilliSecondsPerLevel(difficultyModifier) / AverageEntryTimePerPatient(difficultyModifier);
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
        public static int NumberOfPatientsToInt(double difficultyModifier)
        {
            return Convert.ToInt32(NumberOfPatients(difficultyModifier));
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
        public static Dictionary<string, double> GetBalancingData(string difficultyModifier)
        {
            return GetBalancingData(Globals.StringToDouble(difficultyModifier));
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
        public static double TreatmentPerPatient(string difficultyModifier)
        {
            return TreatmentPerPatient(Globals.StringToDouble(difficultyModifier));
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
        public static double TimePerTreatment(string difficultyModifier)
        {
            return TimePerTreatment(Globals.StringToDouble(difficultyModifier));
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
        public static double MilliSecondsPerLevel(string difficultyModifier)
        {
            return MilliSecondsPerLevel(Globals.StringToDouble(difficultyModifier));
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
        public static double MinutesPerLevel(string difficultyModifier)
        {
            return MinutesPerLevel(Globals.StringToDouble(difficultyModifier));
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
        public static double TimeBetweenPatients(string difficultyModifier)
        {
            return TimeBetweenPatients(Globals.StringToDouble(difficultyModifier));
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
        public static double AverageEntryTimePerPatient(string difficultyModifier)
        {
            return AverageEntryTimePerPatient(Globals.StringToDouble(difficultyModifier));
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
        public static double NumberOfPatients(string difficultyModifier)
        {
            return NumberOfPatients(Globals.StringToDouble(difficultyModifier));
        }

        #endregion
    }
}
