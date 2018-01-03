// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Drawing;
using System.Globalization;
using Alphaleonis.Win32.Filesystem;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Web;
using FileSystemInfo = Alphaleonis.Win32.Filesystem.FileSystemInfo;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using System.Security;

// Helpful functions and classes

namespace TVRename
{
    public delegate void SetProgressDelegate(int percent);

    public static class XMLHelper
    {
        public static void WriteStringsToXml(List<string> strings, XmlWriter writer, string elementName, string stringName)
        {
            writer.WriteStartElement(elementName);
            foreach (string ss in strings)
            {
                writer.WriteStartElement(stringName);
                writer.WriteValue(ss);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        
        public static List<string> ReadStringsFromXml(XmlReader reader, string elementName, string stringName)
        {
            List<string> r = new List<String>();

            if (reader.Name != elementName)
                return r; // uhoh

            if (!reader.IsEmptyElement)
            {
                reader.Read();
                while (!reader.EOF)
                {
                    if ((reader.Name == elementName) && !reader.IsStartElement())
                        break;
                    if (reader.Name == stringName)
                        r.Add(reader.ReadElementContentAsString());
                    else
                        reader.ReadOuterXml();
                }
            }
            reader.Read();
            return r;
        }

        public static string ReadStringFixQuotesAndSpaces(XmlReader r)
        {
            string res = r.ReadElementContentAsString();
            res = res.Replace("\\'", "'");
            res = res.Replace("\\\"", "\"");
            res = res.Trim();
            return res;
        }

        public static void WriteElementToXML(XmlWriter writer, string elementName, string value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value??"");
            writer.WriteEndElement();
        }
        public static void WriteElementToXML(XmlWriter writer, string elementName, double value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        public static void WriteElementToXML(XmlWriter writer, string elementName, int value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        public static void WriteElementToXML(XmlWriter writer, string elementName, bool value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
        public static void WriteElementToXML(XmlWriter writer, string attributeName, DateTime? value)
        {
            writer.WriteStartElement(attributeName);
            if (!(value == null))
                writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteAttributeToXML(XmlWriter writer, string attributeName, string value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXML(XmlWriter writer, string attributeName, DateTime?  value)
        {
            writer.WriteStartAttribute(attributeName);
            if (!(value == null))
                writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXML(XmlWriter writer, string attributeName, int value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXML(XmlWriter writer, string attributeName, bool value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
        public static void WriteAttributeToXML(XmlWriter writer, string attributeName, long value)
        {
            writer.WriteStartAttribute(attributeName);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        public static void WriteInfo(XmlWriter writer, string elemName, string attribute, string attributeVal, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteStartElement(elemName);
                if (!String.IsNullOrEmpty(attribute) && !String.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteValue(value);
                writer.WriteEndElement();
            }
        }

        public static void WriteInfo(XmlWriter writer, string elemName, string attribute, string attributeVal)
        {
            if (!string.IsNullOrEmpty(attributeVal))
            {
                writer.WriteStartElement(elemName);
                if (!String.IsNullOrEmpty(attribute) && !String.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteEndElement();
            }
        }
    }

    public static class FileHelper
    {
        public static bool FolderIsSubfolderOf(string thisOne, string ofThat)
        {
            // need terminating slash, otherwise "c:\abc def" will match "c:\abc"
            thisOne += System.IO.Path.DirectorySeparatorChar.ToString();
            ofThat += System.IO.Path.DirectorySeparatorChar.ToString();
            int l = ofThat.Length;
            return ((thisOne.Length >= l) && (thisOne.Substring(0, l).ToLower() == ofThat.ToLower()));
        }

        public static void Rotate(string filenameBase)
        {
            if (File.Exists(filenameBase))
            {
                for (int i = 8; i >= 0; i--)
                {
                    string fn = filenameBase + "." + i;
                    if (File.Exists(fn))
                    {
                        string fn2 = filenameBase + "." + (i + 1);
                        if (File.Exists(fn2))
                            File.Delete(fn2);
                        File.Move(fn, fn2);
                    }
                }

                File.Copy(filenameBase, filenameBase + ".0");
            }
        }

        public static bool Same(FileInfo a, FileInfo b)
        {
            return String.Compare(a.FullName, b.FullName, true) == 0; // true->ignore case
        }

        public static bool Same(DirectoryInfo a, DirectoryInfo b)
        {
            string n1 = a.FullName;
            string n2 = b.FullName;
            if (!n1.EndsWith(Path.DirectorySeparatorChar.ToString()))
                n1 = n1 + Path.DirectorySeparatorChar;
            if (!n2.EndsWith(Path.DirectorySeparatorChar.ToString()))
                n2 = n2 + Path.DirectorySeparatorChar;

            return String.Compare(n1, n2, true) == 0; // true->ignore case
        }

        public static FileInfo FileInFolder(string dir, string fn)
        {
            return new FileInfo(String.Concat(dir, dir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? "" : Path.DirectorySeparatorChar.ToString(), fn));
        }

        public static FileInfo FileInFolder(DirectoryInfo di, string fn)
        {
            return FileInFolder(di.FullName, fn);
        }

        // see if showname is somewhere in filename
        public static bool SimplifyAndCheckFilename(string filename, string showname, bool simplifyfilename, bool simplifyshowname)
        {
            return Regex.Match(simplifyfilename ? Helpers.SimplifyName(filename) : filename, "\\b" + (simplifyshowname ? Helpers.SimplifyName(showname) : showname) + "\\b", RegexOptions.IgnoreCase).Success;
        }


        public static bool SimplifyAndCheckFilename(string filename, string showname)
        {
            return SimplifyAndCheckFilename(filename, showname,true,true);
        }

        internal static string TempPath(string v) => Path.GetTempPath() + v;

    
}

    public static class HTTPHelper
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static String HTTPRequest(String method, String url,String json, String contentType,String authToken = "", String lang = "") {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = method;
            if (authToken != "")
            {
                httpWebRequest.Headers.Add("Authorization", "Bearer " + authToken);
            }
            if (lang != "")
            {
                httpWebRequest.Headers.Add("Accept-Language",lang);
            }

            logger.Trace("Obtaining {0}", url);

            if (method == "POST") { 
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            String result;
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            logger.Trace("Returned {0}", result);
            return result;
        }

        public static JObject JsonHTTPPOSTRequest( String url, JObject request)
        {
            String response = HTTPHelper.HTTPRequest("POST",url, request.ToString(), "application/json");

            return JObject.Parse(response);
            
        }

        public static JObject JsonHTTPGETRequest(String url, Dictionary<string, string> parameters, String authToken, String lang="")
        {
            String response = HTTPHelper.HTTPRequest("GET", url + getHTTPParameters(parameters), null, "application/json", authToken,lang);

            return JObject.Parse(response);

        }

        public static string getHTTPParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("?");

            foreach (KeyValuePair<string,string>  item in parameters)
            {
                sb.Append(string.Format("{0}={1}&", item.Key, item.Value));
            }
            string finalUrl = sb.ToString();
            return finalUrl.Remove(finalUrl.LastIndexOf("&"));
        }

    }

    public static class JSONHelper {
        public static String flatten(JToken ja,String delimiter = ",")
        {
            if (ja == null) return "";

            
            if (ja.Type == JTokenType.Array)
            {
                JArray ja2 = (JArray)ja;
                string[] values = ja2.ToObject<string[]>();
                return String.Join(delimiter, values);
            }
            else { return ""; }

                
            
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }

    public static class RegistryHelper {
        //From https://www.cyotek.com/blog/configuring-the-emulation-mode-of-an-internet-explorer-webbrowser-control THANKS
        //Needed to ensure webBrowser renders HTML 5 content

        private const string InternetExplorerRootKey = @"Software\Microsoft\Internet Explorer";
        private const string BrowserEmulationKey = InternetExplorerRootKey + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private enum BrowserEmulationVersion
        {
            Default = 0,
            Version7 = 7000,
            Version8 = 8000,
            Version8Standards = 8888,
            Version9 = 9000,
            Version9Standards = 9999,
            Version10 = 10000,
            Version10Standards = 10001,
            Version11 = 11000,
            Version11Edge = 11001
        }

        private static int GetInternetExplorerMajorVersion()
        {
            int result;

            result = 0;

            try
            {
                RegistryKey key;

                key = Registry.LocalMachine.OpenSubKey(InternetExplorerRootKey);

                if (key != null)
                {
                    object value;

                    value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);

                    if (value != null)
                    {
                        string version;
                        int separator;

                        version = value.ToString();
                        separator = version.IndexOf('.');
                        if (separator != -1)
                        {
                            int.TryParse(version.Substring(0, separator), out result);
                        }
                    }
                }
            }
            catch (SecurityException se)
            {
                // The user does not have the permissions required to read from the registry key.
                logger.Error(se);
            }
            catch (UnauthorizedAccessException uae)
            {
                // The user does not have the necessary registry rights.
                logger.Error(uae);
            }

            return result;
        }
        
        private static BrowserEmulationVersion GetBrowserEmulationVersion()
        {
            BrowserEmulationVersion result;

            result = BrowserEmulationVersion.Default;

            try
            {
                RegistryKey key;

                key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
                if (key != null)
                {
                    string programName;
                    object value;

                    programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                    value = key.GetValue(programName, null);

                    if (value != null)
                    {
                        result = (BrowserEmulationVersion)Convert.ToInt32(value);
                    }
                }
            }
            catch (SecurityException se)
            {
                // The user does not have the permissions required to read from the registry key.
                logger.Error(se);
            }
            catch (UnauthorizedAccessException uae)
            {
                // The user does not have the necessary registry rights.
                logger.Error(uae);
            }

            return result;
        }

        private static bool IsBrowserEmulationSet()
        {
            return GetBrowserEmulationVersion() != BrowserEmulationVersion.Default;
        }

        private static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion)
        {
            bool result;

            result = false;

            try
            {
                RegistryKey key;

                key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);

                if (key != null)
                {
                    string programName;

                    programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

                    if (browserEmulationVersion != BrowserEmulationVersion.Default)
                    {
                        // if it's a valid value, update or create the value
                        key.SetValue(programName, (int)browserEmulationVersion, RegistryValueKind.DWord);
                        logger.Warn("SETTING REGISTRY:{0}-{1}-{2}-{3}",key.Name,programName, (int)browserEmulationVersion, RegistryValueKind.DWord.ToString());
                    }
                    else
                    {
                        // otherwise, remove the existing value
                        key.DeleteValue(programName, false);
                        logger.Warn("DELETING REGISTRY KEY:{0}-{1}", key.Name, programName);
                    }

                    result = true;
                }
            }
            catch (SecurityException se)
            {
                // The user does not have the permissions required to read from the registry key.
                logger.Error(se);
            }
            catch (UnauthorizedAccessException uae)
            {
                // The user does not have the necessary registry rights.
                logger.Error(uae);
            }

            return result;
        }

        private static bool SetBrowserEmulationVersion()
        {
            int ieVersion;
            BrowserEmulationVersion emulationCode;

            ieVersion = GetInternetExplorerMajorVersion();
            logger.Warn("IE Version {0} is identified",ieVersion );

            if (ieVersion >= 11)
            {
                emulationCode = BrowserEmulationVersion.Version11;
            }
            else
            {
                switch (ieVersion)
                {
                    case 10:
                        emulationCode = BrowserEmulationVersion.Version10;
                        break;
                    case 9:
                        emulationCode = BrowserEmulationVersion.Version9;
                        break;
                    case 8:
                        emulationCode = BrowserEmulationVersion.Version8;
                        break;
                    default:
                        emulationCode = BrowserEmulationVersion.Version7;
                        break;
                }
            }

            return SetBrowserEmulationVersion(emulationCode);
        }

        public static void UpdateBrowserEmulationVersion()
        {
            if (!IsBrowserEmulationSet())
            {
                logger.Warn("Updating the registry to ensure that the latest browser version is used");
                SetBrowserEmulationVersion();
            }
        }


    }

    public static class Helpers
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a value indicating whether application is running under Mono.
        /// </summary>
        /// <value>
        ///   <c>true</c> if application is running under Mono; otherwise, <c>false</c>.
        /// </value>
        public static bool OnMono => Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Gets the application display version from the current assemblies <see cref="AssemblyInformationalVersionAttribute"/>.
        /// </summary>
        /// <value>
        /// The application display version.
        /// </value>
        public static string DisplayVersion
        {
            get
            {
                string v = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().First().InformationalVersion;

#if DEBUG
                v += " ** Debug Build **";
#endif

                return v;
            }
        }

        public static string pad(int i)
        {
            if (i.ToString().Length > 1)
            {
                return (i.ToString());
            }
            else
            {
                return ("0" + i);
            }
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool SysOpen(string what, string arguments = null)
        {
            try
            {
                System.Diagnostics.Process.Start(what, arguments);
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }
        }

        public static Color WarningColor() => Color.FromArgb(255, 210, 210);

        public static bool Contains(string source, string toCheck, StringComparison comp) => source.IndexOf(toCheck, comp) >= 0;
        
        public static string TranslateColorToHtml(Color c) =>String.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        
        public static string SimplifyName(string n)
        {
            n = n.ToLower();
            n = n.Replace("the", "");
            n = n.Replace("'", "");
            n = n.Replace("&", "");
            n = n.Replace("and", "");
            n = n.Replace("!", "");
            n = Regex.Replace(n, "[_\\W]+", " ");
            return n.Trim();
        }

        public static string RemoveDiacritics(string stIn)
        {
            // From http://blogs.msdn.com/b/michkap/archive/2007/05/14/2629747.aspx
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}
