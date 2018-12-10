using System.IO;
using System.Xml.Serialization;
// ReSharper disable All

namespace TVRename.SAB
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public class Result : object, System.ComponentModel.INotifyPropertyChanged
    {
        public static Result Deserialize(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            XmlSerializer serializer = new XmlSerializer(typeof(Result));
            try
            {
                Result r = (Result) serializer.Deserialize(ms);
                return r;
            }
            catch
            {
                return null;
            }
        }

        private string statusField;
        private string errorField;

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string status
        {
            get => statusField;
            set
            {
                statusField = value;
                RaisePropertyChanged("status");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string error
        {
            get => errorField;
            set
            {
                errorField = value;
                RaisePropertyChanged("error");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
