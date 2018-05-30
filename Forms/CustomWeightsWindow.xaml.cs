// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomWeightsWindow.xaml.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Forms
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using HM4DesignTool.Data;
    using HM4DesignTool.Level;

    /// <summary>
    /// Interaction logic for CustomTreatmentWeightsWindow.xaml
    /// </summary>
    public partial class CustomTreatmentWeightsWindow : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The customized treatment list field
        /// </summary>
        private List<Treatment> customizedAvailableTreatmentList = new List<Treatment>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTreatmentWeightsWindow"/> class.
        /// </summary>
        public CustomTreatmentWeightsWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.CustomizedAvailableTreatmentList = Globals.GetLevelOverview.GetLevelLoaded.AvailableTreatmentList;
            Globals.GetLevelOverview.GetLevelLoaded.UpdateTreatmentWeightPercentage();
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

        /// <summary>
        /// Gets or sets the customized available treatment list.
        /// </summary>
        public List<Treatment> CustomizedAvailableTreatmentList
        {
            get => this.customizedAvailableTreatmentList;
            set
            {
                this.customizedAvailableTreatmentList = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Save the data
        /// </summary>
        private void SaveData()
        {
            Dictionary<string, Dictionary<string, int>> customizedAvailableTreatmentWeightsDict = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, int> treatmentWeightsDict = new Dictionary<string, int>();

            foreach (Treatment treatment in this.CustomizedAvailableTreatmentList)
            {
                if (treatment.Weight != treatment.CustomizedWeight)
                {
                    treatmentWeightsDict.Add(treatment.TreatmentName, treatment.CustomizedWeight);
                }
            }

            customizedAvailableTreatmentWeightsDict.Add(
                Globals.GetLevelOverview.GetLevelLoaded.LevelName,
                treatmentWeightsDict);

            // Send the data to the SettingsObject
            Globals.GetSettings.SetCustomizedTreatmentWeightsDict(customizedAvailableTreatmentWeightsDict);

            // Store the settings to the save file
            Globals.GetSettings.SaveSettings();

            // Ensure the new settings are loaded into the level correctly
            Globals.GetLevelOverview.GetLevelLoaded.UpdateAvailableTreatmentList();
        }

        #region ClickEvents

        /// <summary>
        /// The button save event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            this.SaveData();
            this.Close();
        }

        /// <summary>
        /// The button cancel event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
