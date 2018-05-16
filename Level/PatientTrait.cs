namespace HM4DesignTool.Level
{
    using System.ComponentModel;

    public class PatientTrait
    {
        private string property = string.Empty;

        private string value = string.Empty;

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


        public PatientTrait()
        {

        }

        public PatientTrait(string Property, string Value)
        {
            this.Property = Property;
            this.Value = Value;
        }

        public override string ToString()
        {
            if (this.Property != string.Empty && this.Value != string.Empty)
            {
                return $"{this.Property} = {this.Value},";
            }
            else
            {
                return string.Empty;
            }
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
