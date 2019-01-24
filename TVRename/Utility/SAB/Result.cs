using System.IO;
using System.Text;
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
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <queue><noofslots_total>0</noofslots_total> <diskspace2_norm>2.2 T</diskspace2_norm> <paused>False</paused> <finish>0</finish> <speedlimit_abs></speedlimit_abs> <slots></slots> <speed>0  </speed> <size>0 B</size> <rating_enable>False</rating_enable> <eta>unknown</eta> <refresh_rate>1</refresh_rate> <start>0</start> <version>2.3.7</version> <diskspacetotal2>2794.39</diskspacetotal2> <limit>8888</limit> <diskspacetotal1>2794.39</diskspacetotal1> <status>Idle</status> <have_warnings>0</have_warnings> <cache_art>0</cache_art> <sizeleft>0 B</sizeleft> <finishaction>None</finishaction> <paused_all>False</paused_all> <quota>0 </quota> <have_quota>False</have_quota> <mbleft>0.00</mbleft> <diskspace2>2236.57</diskspace2> <diskspace1>2236.57</diskspace1> <scripts></scripts> <categories><category>*</category> </categories> <timeleft>0:00:00</timeleft> <pause_int>0</pause_int> <noofslots>0</noofslots> <mb>0.00</mb> <loadavg></loadavg> <cache_max>536870912</cache_max> <kbpersec>0.00</kbpersec> <speedlimit>100</speedlimit> <cache_size>0 B</cache_size> <left_quota>0 </left_quota> <diskspace1_norm>2.2 T</diskspace1_norm> <queue_details>0</queue_details> </queue>"));
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
