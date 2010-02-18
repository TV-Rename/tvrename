//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System;
using System.Xml;

namespace TVRename
{
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//	ref class ShowItem;

	public class ProcessedEpisode : Episode
		{
			public int EpNum2; // if we are a concatenation of episodes, this is the last one in the series. Otherwise, same as EpNum
			public bool Ignore;
			public bool NextToAir;
			public int OverallNumber;
			public ShowItem SI;

			public ProcessedEpisode(SeriesInfo ser, Season seas, ShowItem si) : base(ser, seas)
			{
				NextToAir = false;
				OverallNumber = -1;
				Ignore = false;
				EpNum2 = EpNum;
				SI = si;
			}

			public ProcessedEpisode(ProcessedEpisode O) : base(O)
			{
				NextToAir = O.NextToAir;
				EpNum2 = O.EpNum2;
				Ignore = O.Ignore;
				SI = O.SI;
				OverallNumber = O.OverallNumber;
			}
			public ProcessedEpisode(Episode e, ShowItem si) : base(e)
			{
				OverallNumber = -1;
				NextToAir = false;
				EpNum2 = EpNum;
				Ignore = false;
				SI = si;
			}

			public string NumsAsString()
			{
				if (EpNum == EpNum2)
					return EpNum.ToString();
				else
					return EpNum.ToString() + "-" + EpNum2.ToString();
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
			public static int EPNumberSorter(ProcessedEpisode e1, ProcessedEpisode e2)
			{
				int ep1 = e1.EpNum;
				int ep2 = e2.EpNum;

				return ep1-ep2;
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
			public static int DVDOrderSorter(ProcessedEpisode e1, ProcessedEpisode e2)
			{
				int ep1 = e1.EpNum;
				int ep2 = e2.EpNum;

				string key = "DVD_episodenumber";
				if (e1.Items.ContainsKey(key) && e2.Items.ContainsKey(key))
				{
					string n1 = e1.Items[key];
					string n2 = e2.Items[key];
					if ((!string.IsNullOrEmpty(n1)) && (!string.IsNullOrEmpty(n2)))
					{
						try
						{
							int t1 = (int)(1000.0 * double.Parse(n1));
							int t2 = (int)(1000.0 * double.Parse(n2));
							ep1 = t1;
							ep2 = t2;
						}
						catch (FormatException )
						{
						}
					}
				}

				return ep1-ep2;
			}
		}


		//enum CheckType { checkNone = 0, checkAll = 1, checkRecent = 2}; // TODO: remove this, and make a list of seasons/eps to ignore

		public class ShowItem
		{
			public TheTVDB TVDB;
			public bool UseCustomShowName;
			public string CustomShowName;
			public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ShowRule > > SeasonRules;
			public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ProcessedEpisode > > SeasonEpisodes; // built up by applying rules.
			public bool ShowNextAirdate;
			public int TVDBCode;

			public bool AutoAddNewSeasons;
			public string AutoAdd_FolderBase; // TODO: use magical renaming tokens here
			public string AutoAdd_SeasonFolderName; // TODO: use magical renaming tokens here
			public bool AutoAdd_FolderPerSeason;

			public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string > > ManualFolderLocations;

			public bool UseSequentialMatch;
			public bool DoRename;
			public bool CountSpecials;
			public bool DoMissingCheck;
			public bool PadSeasonToTwoDigits;
			public System.Collections.Generic.List<int> IgnoreSeasons;
			public bool DVDOrder; // sort by DVD order, not the default sort we get
			public bool ForceCheckAll;

			public SeriesInfo TheSeries()
			{
				return TVDB.GetSeries(TVDBCode);
			}
			public string ShowName()
			{
				if (UseCustomShowName)
					return CustomShowName;
				SeriesInfo ser = TheSeries();
				if (ser != null)
					return ser.Name;
				return "<"+TVDBCode.ToString()+ " not downloaded>";
			}
			public void SetDefaults(TheTVDB db)
			{
				TVDB = db;
				ManualFolderLocations = new GlobalMembersShowItem.Generic.Dictionary<int, System.Collections.Generic.List<string > >();
				IgnoreSeasons = new GlobalMembersShowItem.Generic.List<int>();
				UseCustomShowName = false;
				CustomShowName = "";
				UseSequentialMatch = false;
				SeasonRules = new GlobalMembersShowItem.Generic.Dictionary<int, GlobalMembersShowItem.Generic.List<ShowRule > >();
				SeasonEpisodes = new GlobalMembersShowItem.Generic.Dictionary<int, GlobalMembersShowItem.Generic.List<ProcessedEpisode > >();
				ShowNextAirdate = true;
				TVDBCode = -1;
				//                WhichSeasons = gcnew Generic::List<int>;
				//                NamingStyle = (int)NStyle::DefaultStyle();
				AutoAddNewSeasons = true;
				PadSeasonToTwoDigits = false;
				AutoAdd_FolderBase = "";
				AutoAdd_FolderPerSeason = true;
				AutoAdd_SeasonFolderName = "Season ";
				DoRename = true;
				DoMissingCheck = true;
				CountSpecials = false;
				DVDOrder = false;
				ForceCheckAll = false;
			}
			//Generic::List<int> ^WhichSeasons()
			//{
			//    Generic::List<int> ^r = gcnew Generic::List<int>();
			//    for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in SeasonEpisodes)
			//        r->Add(kvp->Key);
			//    return r;
			//}

			public ShowItem(TheTVDB db)
			{
				SetDefaults(db);
			}

			public System.Collections.Generic.List<ShowRule > RulesForSeason(int n)
			{
				if (SeasonRules.ContainsKey(n))
					return SeasonRules[n];
				else
					return null;
			}

			public string AutoFolderNameForSeason(int n, TVSettings settings)
			{
				bool leadingZero = settings.LeadingZeroOnSeason || PadSeasonToTwoDigits;
				string r = AutoAdd_FolderBase;
				if (string.IsNullOrEmpty(r))
					return "";

				if (!r.EndsWith("\\"))
					r += "\\";
				if (AutoAdd_FolderPerSeason)
				{
					if (n == 0)
						r += settings.SpecialsFolderName;
					else
					{
						r += AutoAdd_SeasonFolderName;
						if ((n < 10) && leadingZero)
							r += "0";
						r += n.ToString();
					}
				}
				return r;
			}


			public int MaxSeason()
			{
				int max = 0;
				foreach (KeyValuePair<int, GlobalMembersShowItem.Generic.List<ProcessedEpisode > > kvp in SeasonEpisodes)
					if (kvp.Key > max)
						max = kvp.Key;
				return max;
			}

			//String ^NiceName(int season)
			//{
			//    // something like "Simpsons (S3)"
			//    return String::Concat(ShowName," (S",season,")");
			//}

			public void WriteXMLSettings(XmlWriter writer)
			{
				writer.WriteStartElement("ShowItem");

				writer.WriteStartElement("UseCustomShowName");
				writer.WriteValue(UseCustomShowName);
				writer.WriteEndElement();
				writer.WriteStartElement("CustomShowName");
				writer.WriteValue(CustomShowName);
				writer.WriteEndElement();
				writer.WriteStartElement("ShowNextAirdate");
				writer.WriteValue(ShowNextAirdate);
				writer.WriteEndElement();
				writer.WriteStartElement("TVDBID");
				writer.WriteValue(TVDBCode);
				writer.WriteEndElement();
				writer.WriteStartElement("AutoAddNewSeasons");
				writer.WriteValue(AutoAddNewSeasons);
				writer.WriteEndElement();
				writer.WriteStartElement("FolderBase");
				writer.WriteValue(AutoAdd_FolderBase);
				writer.WriteEndElement();
				writer.WriteStartElement("FolderPerSeason");
				writer.WriteValue(AutoAdd_FolderPerSeason);
				writer.WriteEndElement();
				writer.WriteStartElement("SeasonFolderName");
				writer.WriteValue(AutoAdd_SeasonFolderName);
				writer.WriteEndElement();
				writer.WriteStartElement("DoRename");
				writer.WriteValue(DoRename);
				writer.WriteEndElement();
				writer.WriteStartElement("DoMissingCheck");
				writer.WriteValue(DoMissingCheck);
				writer.WriteEndElement();
				writer.WriteStartElement("CountSpecials");
				writer.WriteValue(CountSpecials);
				writer.WriteEndElement();
				writer.WriteStartElement("DVDOrder");
				writer.WriteValue(DVDOrder);
				writer.WriteEndElement();
				writer.WriteStartElement("ForceCheckAll");
				writer.WriteValue(ForceCheckAll);
				writer.WriteEndElement();
				writer.WriteStartElement("UseSequentialMatch");
				writer.WriteValue(UseSequentialMatch);
				writer.WriteEndElement();
				writer.WriteStartElement("PadSeasonToTwoDigits");
				writer.WriteValue(PadSeasonToTwoDigits);
				writer.WriteEndElement();

				writer.WriteStartElement("IgnoreSeasons");
				foreach (int i in IgnoreSeasons)
				{
					writer.WriteStartElement("Ignore");
					writer.WriteValue(i);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();

				foreach (KeyValuePair<int, GlobalMembersShowItem.Generic.List<ShowRule > > kvp in SeasonRules)
				{
					if (kvp.Value.Count > 0)
					{
						writer.WriteStartElement("Rules");
						writer.WriteStartAttribute("SeasonNumber");
						writer.WriteValue(kvp.Key);
						writer.WriteEndAttribute();

						foreach (ShowRule r in kvp.Value)
							r.WriteXML(writer);

						writer.WriteEndElement(); // Rules
					}
				}
				foreach (KeyValuePair<int, System.Collections.Generic.List<string > > kvp in ManualFolderLocations)
				{
					if (kvp.Value.Count > 0)
					{
						writer.WriteStartElement("SeasonFolders");
						writer.WriteStartAttribute("SeasonNumber");
						writer.WriteValue(kvp.Key);
						writer.WriteEndAttribute();

						foreach (string s in kvp.Value)
						{
							writer.WriteStartElement("Folder");
							writer.WriteStartAttribute("Location");
							writer.WriteValue(s);
							writer.WriteEndAttribute();
							writer.WriteEndElement(); // Folder
						}



						writer.WriteEndElement(); // Rules
					}
				}

				writer.WriteEndElement(); // ShowItem
			}

			public ShowItem(TheTVDB db, XmlReader reader, TVSettings settings)
			{
				SetDefaults(db);

				reader.Read();
				if (reader.Name != "ShowItem")
					return; // bail out

				reader.Read();
				while (!reader.EOF)
				{
					if ((reader.Name == "ShowItem") && !reader.IsStartElement())
						break; // all done

					if (reader.Name == "ShowName")
					{
						CustomShowName = reader.ReadElementContentAsString();
						UseCustomShowName = true;
					}
					if (reader.Name == "UseCustomShowName")
						UseCustomShowName = reader.ReadElementContentAsBoolean();
					if (reader.Name == "CustomShowName")
						CustomShowName = reader.ReadElementContentAsString();
					else if (reader.Name == "TVDBID")
						TVDBCode = reader.ReadElementContentAsInt();
					else if (reader.Name == "CountSpecials")
						CountSpecials = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "ShowNextAirdate")
						ShowNextAirdate = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "AutoAddNewSeasons")
						AutoAddNewSeasons = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "FolderBase")
						AutoAdd_FolderBase = reader.ReadElementContentAsString();
					else if (reader.Name == "FolderPerSeason")
						AutoAdd_FolderPerSeason = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "SeasonFolderName")
						AutoAdd_SeasonFolderName = reader.ReadElementContentAsString();
					else if (reader.Name == "DoRename")
						DoRename = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "DoMissingCheck")
						DoMissingCheck = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "DVDOrder")
						DVDOrder = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "ForceCheckAll")
						ForceCheckAll = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "PadSeasonToTwoDigits")
						PadSeasonToTwoDigits = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "UseSequentialMatch")
						UseSequentialMatch = reader.ReadElementContentAsBoolean();
					else if (reader.Name == "IgnoreSeasons")
					{
						if (!reader.IsEmptyElement)
						{
							reader.Read();
							while (reader.Name != "IgnoreSeasons")
							{
								if (reader.Name == "Ignore")
									IgnoreSeasons.Add(reader.ReadElementContentAsInt());
								else
									reader.ReadOuterXml();
							}
						}
						reader.Read();
					}
					else if (reader.Name == "Rules")
					{
						if (!reader.IsEmptyElement)
						{
							int snum = int.Parse(reader.GetAttribute("SeasonNumber"));
							SeasonRules[snum] = new GlobalMembersShowItem.Generic.List<ShowRule >();
							reader.Read();
							while (reader.Name != "Rules")
							{
								if (reader.Name == "Rule")
								{
									SeasonRules[snum].Add(new ShowRule(reader.ReadSubtree()));
									reader.Read();
								}
							}
						}
						reader.Read();
					}
					else if (reader.Name == "SeasonFolders")
					{
						if (!reader.IsEmptyElement)
						{
							int snum = int.Parse(reader.GetAttribute("SeasonNumber"));
							ManualFolderLocations[snum] = new System.Collections.Generic.List<string >();
							reader.Read();
							while (reader.Name != "SeasonFolders")
							{
								if ((reader.Name == "Folder") && reader.IsStartElement())
								{
									string ff = reader.GetAttribute("Location");
									if (AutoFolderNameForSeason(snum, settings) != ff)
										ManualFolderLocations[snum].Add(ff);
								}
								reader.Read();
							}
						}
						reader.Read();
					}

					else
						reader.ReadOuterXml();
				} // while
			}



			public static System.Collections.Generic.List<ProcessedEpisode > ProcessedListFromEpisodes(System.Collections.Generic.List<Episode > el, ShowItem si)
			{
				GlobalMembersShowItem.Generic.List<ProcessedEpisode > pel = new GlobalMembersShowItem.Generic.List<ProcessedEpisode >();
				foreach (Episode e in el)
					pel.Add(new ProcessedEpisode(e, si));
				return pel;
			}

			public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string > > AllFolderLocations(TVSettings settings)
			{
				return AllFolderLocations(settings, true);
			}
			public string TTS(string s) // trim trailing slash
			{
				return s.TrimEnd('\\');
			}
			public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string > > AllFolderLocations(TVSettings settings, bool manualToo)
			{
				GlobalMembersShowItem.Generic.Dictionary<int, System.Collections.Generic.List<string > > fld = new GlobalMembersShowItem.Generic.Dictionary<int, System.Collections.Generic.List<string > >();

				if (manualToo)
				{
					foreach (KeyValuePair<int, System.Collections.Generic.List<string > > kvp in ManualFolderLocations)
					{
						if (!fld.ContainsKey(kvp.Key))
							fld[kvp.Key] = new System.Collections.Generic.List<string >();
						foreach (string s in kvp.Value)
							fld[kvp.Key].AddTTS(s);
					}
				}

				if (AutoAddNewSeasons && (!string.IsNullOrEmpty(AutoAdd_FolderBase)))
				{
					int highestThereIs = -1;
					foreach (KeyValuePair<int, GlobalMembersShowItem.Generic.List<ProcessedEpisode > > kvp in SeasonEpisodes)
						if (kvp.Key > highestThereIs)
							highestThereIs = kvp.Key;

					for (int i =0;i<=highestThereIs;i++) // start at 0 for specials season
					{
						if (IgnoreSeasons.Contains(i))
							continue;

						string newName = AutoFolderNameForSeason(i, settings);
						if ((!string.IsNullOrEmpty(newName)) && (DirectoryInfo(newName).Exists))
						{
							if (!fld.ContainsKey(i))
								fld[i] = new System.Collections.Generic.List<string >();
							if (!fld[i].Contains(newName))
								fld[i].AddTTS(newName);
						}
					}
				}

				return fld;

			}


		} // ShowItem

        public class ShowItemList : System.Collections.Generic.List<ShowItem>
        {
        }


} // namespace