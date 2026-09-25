using System.IO;
using System.Text;
using System.Xml.Serialization;
// ReSharper disable All

namespace TVRename.Utility.SAB
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public class Result : System.ComponentModel.INotifyPropertyChanged
    {
        public static Result? Deserialize(byte[] data)
        {
            MemoryStream ms = new(data);
            XmlSerializer serializer = new (typeof(Result));
            try
            {
                Result? r = (Result?) serializer.Deserialize(ms);
                return r;
            }
            catch
            {
                return null;
            }
        }

        private string statusField = string.Empty;
        private string errorField = string.Empty;

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Status
        {
            get => statusField;
            set
            {
                statusField = value;
                RaisePropertyChanged(nameof(Status));
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Error
        {
            get => errorField;
            set
            {
                errorField = value;
                RaisePropertyChanged(nameof(Error));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
