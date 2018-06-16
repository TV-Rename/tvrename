using System.Xml.Serialization;

namespace TVRename.SAB
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public class queueSlotsSlot : object, System.ComponentModel.INotifyPropertyChanged
    {
        private string statusField;
        private string indexField;
        private string etaField;
        private string missingField;
        private string avg_ageField;
        private string scriptField;
        private string msgidField;
        private string verbosityField;
        private double mbField;
        private string sizeleftField;
        private string filenameField;
        private string priorityField;
        private string catField;
        private double mbleftField;
        private string timeleftField;
        private string percentageField;
        private string nzo_idField;
        private string unpackoptsField;
        private string sizeField;

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
        public string index
        {
            get => indexField;
            set
            {
                indexField = value;
                RaisePropertyChanged("index");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string eta
        {
            get => etaField;
            set
            {
                etaField = value;
                RaisePropertyChanged("eta");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string missing
        {
            get => missingField;
            set
            {
                missingField = value;
                RaisePropertyChanged("missing");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string avg_age
        {
            get => avg_ageField;
            set
            {
                avg_ageField = value;
                RaisePropertyChanged("avg_age");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string script
        {
            get => scriptField;
            set
            {
                scriptField = value;
                RaisePropertyChanged("script");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string msgid
        {
            get => msgidField;
            set
            {
                msgidField = value;
                RaisePropertyChanged("msgid");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string verbosity
        {
            get => verbosityField;
            set
            {
                verbosityField = value;
                RaisePropertyChanged("verbosity");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double mb
        {
            get => mbField;
            set
            {
                mbField = value;
                RaisePropertyChanged("mb");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string sizeleft
        {
            get => sizeleftField;
            set
            {
                sizeleftField = value;
                RaisePropertyChanged("sizeleft");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string filename
        {
            get => filenameField;
            set
            {
                filenameField = value;
                RaisePropertyChanged("filename");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string priority
        {
            get => priorityField;
            set
            {
                priorityField = value;
                RaisePropertyChanged("priority");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string cat
        {
            get => catField;
            set
            {
                catField = value;
                RaisePropertyChanged("cat");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double mbleft
        {
            get => mbleftField;
            set
            {
                mbleftField = value;
                RaisePropertyChanged("mbleft");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string timeleft
        {
            get => timeleftField;
            set
            {
                timeleftField = value;
                RaisePropertyChanged("timeleft");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string percentage
        {
            get => percentageField;
            set
            {
                percentageField = value;
                RaisePropertyChanged("percentage");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nzo_id
        {
            get => nzo_idField;
            set
            {
                nzo_idField = value;
                RaisePropertyChanged("nzo_id");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string unpackopts
        {
            get => unpackoptsField;
            set
            {
                unpackoptsField = value;
                RaisePropertyChanged("unpackopts");
            }
        }

        [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string size
        {
            get => sizeField;
            set
            {
                sizeField = value;
                RaisePropertyChanged("size");
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