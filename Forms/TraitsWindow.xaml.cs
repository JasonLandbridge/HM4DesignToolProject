using System.Windows;

namespace HM4DesignTool.Forms
{
    using System.Windows.Controls;

    using HM4DesignTool.Data;
    using HM4DesignTool.Level;

    /// <summary>
    /// Interaction logic for TraitsWindow.xaml
    /// </summary>
    public partial class TraitsWindow : Window
    {
        private Patient patientParent;

        public Patient PatientParent
        {
            get => this.patientParent;
            set
            {
                this.patientParent = value;
                this.DataContext = this.PatientParent;
                this.PatientTraitsPreview.DataContext = this.PatientParent;
                this.PatientParent.UpdatePatientTraitString();
            }
        }

        public TraitsWindow()
        {
            this.InitializeComponent();
            if (PatientParent != null)
            {
                this.DataContext = this.PatientParent;
                this.PatientTraitsPreview.DataContext = this.PatientParent;
                this.PatientParent.UpdatePatientTraitString();
            }
        }

        private void buttonTraitWindowSave_Click(object sender, RoutedEventArgs e)
        {
            Globals.GetLevelOverview.GetLevelLoaded.UpdateLevelOutput();
            this.Close();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            this.PatientParent.UpdatePatientTraitString();
        }
    }
}
