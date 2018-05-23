namespace UiWindows
{
    using System;
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

        /// </summary>
        private List<Treatment> _customizedAvailableTreatmentList = new List<Treatment> { };
        public List<Treatment> CustomizedAvailableTreatmentList
        {
            get
            {
                return _customizedAvailableTreatmentList;
            }
            set
            {
                _customizedAvailableTreatmentList = value;
                OnPropertyChanged("CustomizedAvailableTreatmentList");
            }
        }


        public CustomTreatmentWeightsWindow()
        {
            InitializeComponent();
            DataContext = this;

            CustomizedAvailableTreatmentList = Globals.GetLevelOverview.GetLevelLoaded.AvailableTreatmentList;
            Globals.GetLevelOverview.GetLevelLoaded.UpdateTreatmentWeightPercentage();
        }

        private void SaveData()
        {
            Dictionary<String, Dictionary<String, int>> CustomizedAvailableTreatmentWeightsDict = new Dictionary<string, Dictionary<string, int>> { };
            Dictionary<String, int> TreatmentWeightsDict = new Dictionary<string, int> { };

            foreach(Treatment treatment in CustomizedAvailableTreatmentList)
            {
                if(treatment.Weight != treatment.CustomizedWeight)
                {
                    TreatmentWeightsDict.Add(treatment.TreatmentName, treatment.CustomizedWeight);
                }
            }

            CustomizedAvailableTreatmentWeightsDict.Add(Globals.GetLevelOverview.GetLevelLoaded.LevelName, TreatmentWeightsDict);
            //Send the data to the SettingsObject
            Globals.GetSettings.SetCustomizedTreatmentWeightsDict(CustomizedAvailableTreatmentWeightsDict);
            //Store the settings to the save file
            Globals.GetSettings.SaveSettings();
            //Ensure the new settings are loaded into the level correctly
            Globals.GetLevelOverview.GetLevelLoaded.UpdateAvailableTreatmentList();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





        #endregion INotifyPropertyChanged Members

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            this.Close();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
