// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Xml;

// An "IgnoreItem" represents a file/episode to never ask the user about again. (Right-click->Ignore Selected / Options->Ignore List)

namespace TVRename
{
    public class IgnoreItem
    {
        public string FileAndPath;

        public IgnoreItem(XmlReader r)
        {
            if (r.Name == "Ignore")
                FileAndPath = r.ReadElementContentAsString();
        }

        public IgnoreItem(string fileAndPath)
        {
            FileAndPath = fileAndPath;
        }

        public bool SameFileAs(IgnoreItem o)
        {
            if (string.IsNullOrEmpty(FileAndPath) || string.IsNullOrEmpty(o.FileAndPath))
                return false;
            return FileAndPath == o.FileAndPath;
        }

        public void Write(XmlWriter writer)
        {
            XMLHelper.WriteElementToXML(writer,"Ignore",FileAndPath);
        }
    }
}
