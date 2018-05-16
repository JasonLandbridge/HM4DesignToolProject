using System.Windows;

namespace HM4DesignTool.Forms
{
    /// <summary>
    /// Interaction logic for TraitsWindow.xaml
    /// </summary>
    public partial class TraitsWindow : Window
    {
        public TraitsWindow()
        {
            InitializeComponent();
            PatientTraitsPreview.DataContext = DataContext;
            PatientTraitsPreview.Text = "test";
        }

        private void buttonTraitWindowSave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
