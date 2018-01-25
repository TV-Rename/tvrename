using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TVRename.Core.Extensions;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.Core.Actions
{
    // ReSharper disable once InconsistentNaming
    public class Mede8erViewAction : IAction
    {
        private readonly FileInfo file;
        private readonly bool season;

        public string Type => "Metadata";

        public string Produces => this.file.FullName;

        public Mede8erViewAction(FileInfo file, bool season)
        {
            this.file = file;
            this.season = season;
        }

        public async Task Run(CancellationToken ct)
        {
            await Task.Factory.StartNew(() =>
            {
                XmlWriter writer = XmlWriter.Create(this.file.FullName, new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.UTF8,
                    NewLineChars = "\r\n"
                });

                writer.WriteStartDocument(true);

                writer.WriteStartElement("FolderTag");

                if (this.season) // Season
                {
                    writer.WriteNode("ViewMode", "Photo");

                    writer.WriteNode("ViewType", "Video");
                }
                else // Show
                {
                    writer.WriteNode("ViewMode", "Preview"); 
                }

                writer.WriteEndElement();
                
                writer.Close();
            }, ct);
        }
    }
}
