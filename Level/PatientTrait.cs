namespace HM4DesignTool.Level
{
    using System.ComponentModel;

    public class PatientTrait
    {
        private string property;

        private string value;

        public string Property
        {
            get => this.property;
            set
            {
                this.property = value;
                this.OnPropertyChanged();
            }
        }

        public string Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.OnPropertyChanged();
            }
        }


        PatientTrait()
        {

        }

        PatientTrait(string Property, string Value)
        {
            this.Property = Property;
            this.Value = Value;
        }



        #region Events

        /// <inheritdoc />
        /// <summary>
        /// This is used to notify the bound XAML Control to update its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion 


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
