﻿namespace HM4DesignTool.Level
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    using HM4DesignTool.Data;

    public class Station
    {
        #region Fields
        private bool _isSelected = false;

        private string stationName = string.Empty;

        /// <summary>
        /// The difficulty unlocked.
        /// </summary>
        private double difficultyUnlocked;
        #endregion



        #region Constructors

        public Station()
        {

        }

        public Station(string stationName)
        {
            this.StationName = stationName;

        }

        public Station(string stationName, string stationDataString)
        {
            List<string> stationData = stationDataString.Split(',').ToList();
            this.StationName = stationName;

            // Used to ensure only the avaliable options are set. 
            for (int index = 0; index < stationData.Count; index++)
            {
                switch (index)
                {
                    case 0:
                        {
                            if (stationData[0] != string.Empty)
                            {
                                this.DifficultyUnlocked = Convert.ToDouble(stationData[0]);
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Change this Treatment by retrieving the data from the new TreatmentName.
        /// </summary>
        private void ChangeTreatment()
        {
            if (this.StationName != string.Empty)
            {
                // int roomIndex = this.parentPatient.RoomIndex;
                Station newStation = Globals.GetSettings.GetStation(this.StationName);
                this.DifficultyUnlocked = newStation.DifficultyUnlocked;
            }
            else
            {
                this.DifficultyUnlocked = 0;
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
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string StationName
        {
            get => this.stationName;
            set
            {
                this.stationName = value;
                OnPropertyChanged();

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
            get => this.difficultyUnlocked.ToString("N1");

            set
            {
                this.DifficultyUnlocked = Convert.ToDouble(value);
                this.OnPropertyChanged();
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
            return this.ToOutput(true);
        }


        /// <summary>
        /// Convert this Station to a String
        /// </summary>
        /// <param name="includeStationName">
        /// Include the treatment name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToOutput(bool includeStationName = false)
        {
            return string.Join(",", this.ToList(includeStationName));
        }

        /// <summary>
        /// Convert all Station Data to string format to be stored.
        /// </summary>
        /// <param name="includeStationName">
        /// Whether to include the Station name.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> ToList(bool includeStationName = false)
        {
            List<string> outputList = new List<string>();
            if (includeStationName)
            {
                outputList.Add(this.StationName);
            }

            outputList.Add(this.DifficultyUnlocked.ToString(CultureInfo.InvariantCulture));
            return outputList;
        }
        #endregion

        #region Private



        #endregion

        #endregion

        /// <summary>
        /// Check if this Treatment is empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(this.StationName) || this.StationName == string.Empty;

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
