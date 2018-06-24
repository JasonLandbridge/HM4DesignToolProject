// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  The main window for the Design Tool </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace HM4DesignTool.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;
    using System.Windows.Media;

    using HM4DesignTool.Data;

    using UiWindows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            Test test = new Test();

            // Set Global reference to this Window
            Globals.GetMainWindow = this;
            this.DataContext = this;
            this.levelDataLayout.DataContext = Globals.GetLevelOverview;
            this.mainLayout.DataContext = Globals.GetLevelOverview;
            this.levelControls.DataContext = Globals.GetLevelOverview;
            this.OnPropertyChanged("PatientRowDataTemplate");
        }

        public void UpdatePatientSimulateGrid()
        {

            Grid newGrid = this.PatientSimulateHeaderGrid;


            int rowCount = Globals.GetLevelOverview.GetLevelLoaded.PatientCollection.Count;
            int columnCount = rowCount + Globals.GetLevelOverview.MaxTreatmentsVisible + 1;


            if (newGrid.RowDefinitions.Count == 0)
            {
                RowDefinition r1 = new RowDefinition { Height = new GridLength(25, GridUnitType.Pixel) };
                newGrid.RowDefinitions.Add(r1);
            }

            newGrid.ColumnDefinitions.Clear();
            newGrid.Children.Clear();

            for (int i = 0; i < columnCount; i++)
            {
                ColumnDefinition c1 = new ColumnDefinition { Width = new GridLength(218, GridUnitType.Pixel) };
                newGrid.ColumnDefinitions.Add(c1);
            }

            List<string> headerList = new List<string>();

            for (int i = 0; i < columnCount; i++)
            {
                headerList.Add((i * 5).ToString() + " Seconds");
            }

            int loop = 0;
            foreach (string headerName in headerList)
            {
                Label label = new Label { Content = headerName };
                label.FontWeight = new FontWeight();
                label.Style = this.FindResource("SimulateHeaderFont") as Style;

                Grid.SetRow(label, 0);
                Grid.SetColumn(label, loop);

                newGrid.Children.Add(label);
                loop++;
            }



            string xaml = @"<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>";
            xaml += "<Grid ShowGridLines='False'>";
            xaml += "<Grid.ColumnDefinitions>";
            for (int i = 0; i < 40; i++)
            {
                xaml += @"<ColumnDefinition Width='218' />";
            }

            xaml += "</Grid.ColumnDefinitions>";

            xaml += "<Grid.RowDefinitions>";
            for (int i = 0; i < rowCount; i++)
            {
                xaml += @"<RowDefinition Height='50'/>";
            }

            xaml += "</Grid.RowDefinitions>";
            xaml += "</Grid></ItemsPanelTemplate>";


            this.PatientSimulateGrid.ItemsPanel = XamlReader.Parse(xaml) as ItemsPanelTemplate;


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
        /// Gets the status bar text.
        /// </summary>
        public string StatusbarText
        {
            get
            {
                // TODO Create Statusbar functionality
                this.OnPropertyChanged();
                return string.Empty;
            }
        }

        #endregion

        #region Private
        #region Signals

        /// <summary>
        /// Called when this window is closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WindowClosed(object sender, EventArgs e)
        {
            this.StoreWindowSettings();
        }

        /// <summary>
        /// Will open the Custom Treatment Weight window.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CustomTreatmentWeightsButtonClick(object sender, RoutedEventArgs e)
        {
            CustomTreatmentWeightsWindow dialog = new CustomTreatmentWeightsWindow();
            bool? unused = dialog.ShowDialog();
        }

        #region Menu

        /// <summary>
        /// Menu item to open settings window.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuItemOpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow dialog = new SettingsWindow();
            bool? unused = dialog.ShowDialog();
        }

        /// <summary>
        /// Menu item to create new levels window.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuItemNewLevel(object sender, RoutedEventArgs e)
        {
            NewLevelWindow dialog = new NewLevelWindow();
            bool? unused = dialog.ShowDialog();
        }

        /// <summary>
        /// Menu item to export level list window.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuItemExportLevelList(object sender, RoutedEventArgs e)
        {
            LevelListExport dialog = new LevelListExport();
            bool? unused = dialog.ShowDialog();
        }

        /// <summary>
        /// Menu item to open help window.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuItemOpenHelp(object sender, RoutedEventArgs e)
        {
            HelpWindow dialog = new HelpWindow();
            bool? unused = dialog.ShowDialog();
        }

        #endregion

        #region PatientChances

        /// <summary>
        /// The select all patient chances checkboxes.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectAllPatientChancesCheckboxClick(object sender, RoutedEventArgs e)
        {
            Globals.GetLevelOverview.GetLevelLoaded.SelectAllPatientChances((bool)this.selectAllPatientChancesCheckbox.IsChecked);
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Store the window settings.
        /// </summary>
        private void StoreWindowSettings()
        {
            Globals.GetSettings.RoomFilterDropdownIndex = this.levelListFilter.SelectedIndex;
            Globals.GetSettings.RoomFilterStoryCheck = (bool)this.levelListStoryCheck.IsChecked;
            Globals.GetSettings.RoomFilterBonusCheck = (bool)this.levelListBonusCheck.IsChecked;
            Globals.GetSettings.RoomFilterUnknownCheck = (bool)this.levelListUnknownCheck.IsChecked;

            Globals.GetSettings.ShowStationNextToTreatment = Globals.GetLevelOverview.ShowStationsNextToTreatments;
            if (this.levelListDisplay.Items.Count > 0)
            {
                TreeViewItem categoryItem = this.levelListDisplay.Items[0] as TreeViewItem;
                Globals.GetSettings.LevelListFirstCategoryOpen = categoryItem.IsExpanded;
            }
            else
            {
                Globals.GetSettings.LevelListFirstCategoryOpen = true;
            }

            Globals.GetSettings.SaveSettings();
        }

        #endregion


        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == this.sv1)
            {
                this.sv2.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                this.sv1.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

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


        #endregion INotifyPropertyChanged Members



        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollBar scrollBar = sender as ScrollBar;
            this.sv0.ScrollToHorizontalOffset(scrollBar.Value);
            this.sv2.ScrollToHorizontalOffset(scrollBar.Value);
        }

        private void PatientSimulateHorizontalScrollbar_Loaded(object sender, RoutedEventArgs e)
        {
            // Used to make the scrollbar thumb thicker
            if (this.PatientSimulateHorizontalScrollbar.Track?.Thumb != null)
            {
                this.PatientSimulateHorizontalScrollbar.Track.ViewportSize = double.NaN;
                this.PatientSimulateHorizontalScrollbar.Track.Thumb.Width = 150;
            }
        }

        private void dummy_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}