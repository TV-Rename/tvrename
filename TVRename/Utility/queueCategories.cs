using System.Xml.Serialization;

namespace TVRename.SAB
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public class queueCategories : object, System.ComponentModel.INotifyPropertyChanged
    {
        private string categoryField;

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string category
        {
            get => categoryField;
            set
            {
                categoryField = value;
                RaisePropertyChanged("category");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}