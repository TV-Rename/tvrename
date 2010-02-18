//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System.Windows.Forms;
using System.IO;
using System;
using System.Xml;

namespace TVRename
{
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//	ref class TVDoc;

	public enum AIOType: int
	{
		kMissing,
		kCopyMoveRename,
		kRSS,
		kDownload,
		kNFO,
		kuTorrenting
		}

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
        /*
         * TODO
         * 
        public static bool operator !=(IgnoreItem ImpliedObject, IgnoreItem o)
        {
            return !(this == o);
        }
		public static bool operator ==(IgnoreItem ImpliedObject, IgnoreItem o)
		{
			if (string.IsNullOrEmpty(FileAndPath) || string.IsNullOrEmpty(o.FileAndPath))
				return false;
			return FileAndPath == o.FileAndPath;
		}
*/

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("Ignore");
			writer.WriteValue(FileAndPath);
			writer.WriteEndElement(); // Ignore
		}
		public void Read(XmlReader r)
		{

		}

	}


	public abstract class AIOItem
	{
		public ProcessedEpisode PE; // can be null if not applicable or known

		public bool Done;
		public string ErrorText;
		public bool HasError;

		public AIOType Type;

		public abstract IgnoreItem GetIgnore();

		public abstract ListViewItem GetLVI(ListView lv);

		public abstract string FilenameForProgress();

		public abstract string TargetFolder(); // nullptr if none, otherwise folder "of interest" for this item
												   // e.g. where file is missing from, or downloader is downloading to

		public virtual bool Action(TVDoc doc)
		{
			// default is to do nothing
			// also set Done
			Done = true;
			return true; // all ok
		}

		public abstract bool SameAs(AIOItem o);
		public virtual int IconNumber()
		{
			return -1;
		}

		// Search predicate 
		public static bool DoneOK(AIOItem i)
		{
			return i.Done && !i.HasError;
		}

		protected AIOItem(AIOType t, ProcessedEpisode pe)
		{
			PE = pe;
			Done = false;
			Type = t;
			HasError = false;
			ErrorText = "";
		}

	}

	public class AIODownload : AIOItem
	{
		public ShowItem SI;
		public FileInfo Destination;
		public string BannerPath;

		public AIODownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string bannerPath) : base(AIOType.kDownload, pe)
		{
			Destination = dest;
			SI = si;
			BannerPath = bannerPath;
		}
		public override IgnoreItem GetIgnore()
		{
			if (Destination == null)
				return null;
			else
				return new IgnoreItem(Destination.FullName);
		}
		public override string TargetFolder()
		{
			if (Destination == null)
				return null;
			else
			  return Destination.DirectoryName;
		}
		public override string FilenameForProgress()
		{
			return Destination.Name;
		}
		public bool SameAs2(AIODownload o)
		{
			return (o.Destination == Destination);
		}
		public override bool SameAs(AIOItem o)
		{
			return (this.Type == o.Type) && SameAs2((AIODownload)(o));
		}
		public override int IconNumber()
		{
			return 5;
		}
		public override ListViewItem GetLVI(ListView lv)
		{
			ListViewItem lvi = new ListViewItem();

			lvi.Text = SI.ShowName();
			lvi.SubItems.Add(PE != null ? PE.SeasonNumber.ToString() : "");
			lvi.SubItems.Add(PE != null ? PE.NumsAsString() : "");

			if (PE != null)
			{
				DateTime dt = PE.GetAirDateDT(true);
				if ((dt != null) && (dt.CompareTo(DateTime.MaxValue)))
					lvi.SubItems.Add(PE.GetAirDateDT(true).ToShortDateString());
				else
					lvi.SubItems.Add("");
			}
			else
				lvi.SubItems.Add("");

			lvi.SubItems.Add(Destination.DirectoryName);
			lvi.SubItems.Add(BannerPath);

			if (string.IsNullOrEmpty(BannerPath))
				lvi.BackColor = WarningColor();

			lvi.SubItems.Add(Destination.Name);

			lvi.Tag = this;
			lvi.Group = lv.Groups[5]; // download image
			// lv->Items->Add(lvi);

			return lvi;
		}

		public override bool Action(TVDoc doc)
		{
			byte[] theData = SI.TVDB.GetPage(BannerPath, false, TVRename.tmBanner, false);
			if (theData == null)
			{
				ErrorText = "Unable to download " + BannerPath;
				HasError = true;
				return false;
			}

			FileStream fs = new FileStream(Destination.FullName, FileMode.Create);
			fs.Write(theData, 0, theData.Length);
			fs.Close();

			Done = true;
			return true;
		}
	}

	public class AIOCopyMoveRename : AIOItem
	{
		public FileInfo From;
		public FileInfo To;

		public enum Op: int
		{
			Copy,
			Move,
			Rename
		}

		public Op Operation;

		public bool IsMoveRename() // same thing to the OS
		{
			return ((Operation == Op.Move) || (Operation == Op.Rename));
		}

				public override IgnoreItem GetIgnore()
		{
			if (To == null)
				return null;
			else
				return new IgnoreItem(To.FullName);
		}

		public AIOCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep) : base(AIOType.kCopyMoveRename, ep)
		{
			Operation = operation;
			From = from;
			To = to;
		}
		public override int IconNumber()
		{
			return (IsMoveRename() ? 4 : 3);
		}
		public override string TargetFolder()
		{
			if (To == null)
				return null;
			else
			  return To.DirectoryName;
		}
		public override string FilenameForProgress()
		{
			return To.Name;
		}
		public override ListViewItem GetLVI(ListView lv)
		{
			ListViewItem lvi = new ListViewItem();

			if (PE == null)
			{
				lvi.Text = "";
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
			}
			else
			{
				lvi.Text = PE.TheSeries.Name;
				lvi.SubItems.Add(PE.SeasonNumber.ToString());
				lvi.SubItems.Add(PE.NumsAsString());
				DateTime dt = PE.GetAirDateDT(true);
				if ((dt != null) && (dt.CompareTo(DateTime.MaxValue)))
					lvi.SubItems.Add(PE.GetAirDateDT(true).ToShortDateString());
				else
					lvi.SubItems.Add("");
			}

			lvi.SubItems.Add(From.DirectoryName);
			lvi.SubItems.Add(From.Name);
			lvi.SubItems.Add(To.DirectoryName);
			lvi.SubItems.Add(To.Name);

			if (Operation == Op.Rename)
				lvi.Group = lv.Groups[1];
			else if (Operation == Op.Copy)
				lvi.Group = lv.Groups[2];
			else if (Operation == Op.Move)
				lvi.Group = lv.Groups[3];

			//lv->Items->Add(lvi);
			return lvi;
		}

		public bool SameSource(AIOCopyMoveRename o)
		{
			return (Same(From,o.From));
		}
		public bool SameAs2(AIOCopyMoveRename o)
		{
			return ((Operation == o.Operation) && Same(From,o.From) && Same(To,o.To));
		}
		public override bool SameAs(AIOItem o)
		{
			return (this.Type == o.Type) && SameAs2((AIOCopyMoveRename)(o));
		}

		public long FileSize()
		{
			try
			{
				return From.Length;
			}
			catch
			{
				return 1;
			}
		}



	}

	public class AIOMissing : AIOItem
	{
		public string TheFileNoExt;

		public AIOMissing(ProcessedEpisode pe, string whereItShouldBeNoExt) : base(AIOType.kMissing, pe)
		{
			TheFileNoExt = whereItShouldBeNoExt;
		}

		public override IgnoreItem GetIgnore()
		{
			if (string.IsNullOrEmpty(TheFileNoExt))
				return null;
			else
				return new IgnoreItem(TheFileNoExt);
		}

		public override string TargetFolder()
		{
			if (string.IsNullOrEmpty(TheFileNoExt))
				return null;
			else
				return FileInfo(TheFileNoExt).DirectoryName;
		}
		public override string FilenameForProgress()
		{
			return PE.Name;
		}
		public override bool Action(TVDoc doc)
		{
			return true; // return success, but don't set as Done
		}
		public override int IconNumber()
		{
			return 1;
		}
		public bool SameAs2(AIOMissing o)
		{
			return string.Compare(o.TheFileNoExt, TheFileNoExt) == 0;
		}
		public override bool SameAs(AIOItem o)
		{
			return (this.Type == o.Type) && SameAs2((AIOMissing)(o));
		}

		public override ListViewItem GetLVI(ListView lv)
		{
			ListViewItem lvi = new ListViewItem();

			lvi.Text = PE.SI.ShowName();
			lvi.SubItems.Add(PE.SeasonNumber.ToString());
			lvi.SubItems.Add(PE.NumsAsString());

			DateTime dt = PE.GetAirDateDT(true);
			if ((dt != null) && (dt.CompareTo(DateTime.MaxValue)))
				lvi.SubItems.Add(PE.GetAirDateDT(true).ToShortDateString());
			else
				lvi.SubItems.Add("");

			lvi.SubItems.Add(FileInfo(TheFileNoExt).DirectoryName);
			lvi.SubItems.Add(FileInfo(TheFileNoExt).Name);

			lvi.Tag = this;
			lvi.Group = lv.Groups[0];

			//lv->Items->Add(lvi);
			return lvi;
		}

	}


	public class AIORSS : AIOItem
	{
		public RSSItem RSS;
		public string TheFileNoExt;

		public AIORSS(RSSItem rss, string toWhereNoExt, ProcessedEpisode pe) : base(AIOType.kRSS, pe)
		{
			PE = pe;
			RSS = rss;
			TheFileNoExt = toWhereNoExt;
		}
				public override IgnoreItem GetIgnore()
		{
					if (string.IsNullOrEmpty(TheFileNoExt))
				return null;
			else
				return new IgnoreItem(TheFileNoExt);
		}
		public override string TargetFolder()
		{
			if (string.IsNullOrEmpty(TheFileNoExt))
				return null;
			else
				return FileInfo(TheFileNoExt).DirectoryName;
		}
		public override string FilenameForProgress()
		{
			return RSS.Title;
		}
		public bool SameAs2(AIORSS o)
		{
			return (o.RSS == RSS);
		}
		public override bool SameAs(AIOItem o)
		{
			return (this.Type == o.Type) && SameAs2((AIORSS)(o));
		}
		public override int IconNumber()
		{
			return 6;
		}
		public override ListViewItem GetLVI(ListView lv)
		{
			ListViewItem lvi = new ListViewItem();

			lvi.Text = PE.SI.ShowName();
			lvi.SubItems.Add(PE.SeasonNumber.ToString());
			lvi.SubItems.Add(PE.NumsAsString());
			DateTime dt = PE.GetAirDateDT(true);
			if ((dt != null) && (dt.CompareTo(DateTime.MaxValue)))
				lvi.SubItems.Add(PE.GetAirDateDT(true).ToShortDateString());
			else
				lvi.SubItems.Add("");

			lvi.SubItems.Add(TheFileNoExt);
			lvi.SubItems.Add(RSS.Title);

			lvi.Group = lv.Groups[4];
			lvi.Tag = this;

			// lv->Items->Add(lvi);
			return lvi;
		}

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//		override bool Action(TVDoc doc);
	}


	public class AIONFO : AIOItem
	{
		public FileInfo Where;
		public ShowItem SI; // if for an entire show, rather than specific episode

		public AIONFO(FileInfo nfo, ProcessedEpisode pe) : base(AIOType.kNFO, pe)
		{
			SI = null;
			Where = nfo;
		}
		public override IgnoreItem GetIgnore()
		{

			if (Where == null)
				return null;
			else
				return new IgnoreItem(Where.FullName);
		}
		public AIONFO(FileInfo nfo, ShowItem si) : base(AIOType.kNFO, null)
		{
			SI = si;
			Where = nfo;
		}
		public override string TargetFolder()
		{
			if (Where == null)
				return null;
			else
				return Where.DirectoryName;
		}
		public override string FilenameForProgress()
		{
			return Where.Name;
		}
		public bool SameAs2(AIONFO o)
		{
			return (o.Where == Where);
		}
		public override bool SameAs(AIOItem o)
		{
			return (this.Type == o.Type) && SameAs2((AIONFO)(o));
		}
		public override int IconNumber()
		{
			return 7;
		}

		public override ListViewItem GetLVI(ListView lv)
		{
			ListViewItem lvi = new ListViewItem();

			if (PE != null)
			{
				lvi.Text = PE.SI.ShowName();
				lvi.SubItems.Add(PE.SeasonNumber.ToString());
				lvi.SubItems.Add(PE.NumsAsString());
				DateTime dt = PE.GetAirDateDT(true);
				if ((dt != null) && (dt.CompareTo(DateTime.MaxValue)))
					lvi.SubItems.Add(PE.GetAirDateDT(true).ToShortDateString());
				else
					lvi.SubItems.Add("");
			}
			else
			{
				lvi.Text = SI.ShowName();
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
			}

			lvi.SubItems.Add(Where.DirectoryName);
			lvi.SubItems.Add(Where.Name);

			lvi.Group = lv.Groups[6];
			lvi.Tag = this;

			//lv->Items->Add(lvi);
			return lvi;
		}

		public static void WriteInfo(XmlWriter writer, ShowItem si, string whichItem, string @as)
		{
			string t = si.TheSeries().GetItem(whichItem);
			if (!string.IsNullOrEmpty(t))
			{
				writer.WriteStartElement(@as);
				writer.WriteValue(t);
				writer.WriteEndElement();
			}
		}

		public override bool Action(TVDoc doc)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			XmlWriter writer = XmlWriter.Create(Where.FullName, settings);

			if (PE != null) // specific episode
			{
				// See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
				writer.WriteStartElement("episodedetails");
				writer.WriteStartElement("title");
				writer.WriteValue(PE.Name);
				writer.WriteEndElement();
				writer.WriteStartElement("season");
				writer.WriteValue(PE.SeasonNumber);
				writer.WriteEndElement();
				writer.WriteStartElement("episode");
				writer.WriteValue(PE.EpNum);
				writer.WriteEndElement();
				writer.WriteStartElement("plot");
				writer.WriteValue(PE.Overview);
				writer.WriteEndElement();
				writer.WriteStartElement("aired");
				if (PE.FirstAired != null)
					writer.WriteValue(PE.FirstAired.ToString("yyyy-MM-dd"));
				writer.WriteEndElement();
				writer.WriteEndElement(); // episodedetails
			}
			else if (SI != null) // show overview (tvshow.nfo)
			{
				// http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

				writer.WriteStartElement("tvshow");

				writer.WriteStartElement("title");
				writer.WriteValue(SI.ShowName());
				writer.WriteEndElement();

				writer.WriteStartElement("episodeguideurl");
				writer.WriteValue(SI.TVDB.BuildURL(true, true, SI.TVDBCode, SI.TVDB.PreferredLanguage(SI.TVDBCode)));
				writer.WriteEndElement();

				WriteInfo(writer, SI, "Overview", "plot");

					string genre = SI.TheSeries().GetItem("Genre");
				if (!string.IsNullOrEmpty(genre))
				{
					genre = genre.Trim('|');
					genre = genre.Replace("|"," / ");
					writer.WriteStartElement("genre");
					writer.WriteValue(genre);
					writer.WriteEndElement();
				}

				WriteInfo(writer, SI, "FirstAired", "premiered");
				WriteInfo(writer, SI, "Rating", "rating");

				// actors...
				string actors = SI.TheSeries().GetItem("Actors");
				if (!string.IsNullOrEmpty(actors))
				{
					bool first = true;
					foreach (string aa in actors.Split('|'))
					{
						if (!string.IsNullOrEmpty(aa))
						{
							writer.WriteStartElement("actor");
							writer.WriteStartElement("name");
							writer.WriteValue(aa);
							writer.WriteEndElement(); // name
							writer.WriteEndElement(); // actor
						}
					}
				}

				WriteInfo(writer, SI, "ContentRating", "mpaa");
				WriteInfo(writer, SI, "IMDB_ID", "id");

				writer.WriteStartElement("tvdbid");
				writer.WriteValue(SI.TheSeries().TVDBCode);
				writer.WriteEndElement();

				string rt = SI.TheSeries().GetItem("Runtime");
				if (!string.IsNullOrEmpty(rt))
				{
					writer.WriteStartElement("runtime");
					writer.WriteValue(rt+" minutes");
					writer.WriteEndElement();
				}

				writer.WriteEndElement(); // tvshow
			}

			writer.Close();
			Done = true;
			return true;
		}
	}

	public class TorrentEntry
	{
		public string TorrentFile;
		public string DownloadingTo;
		public int PercentDone;

		public TorrentEntry(string torrentfile, string to, int percent)
		{
			TorrentFile = torrentfile;
			DownloadingTo = to;
			PercentDone = percent;
		}
	}


	public class AIOuTorrenting : AIOItem
	{
		public TorrentEntry Entry;
		public string DesiredLocationNoExt;

		public AIOuTorrenting(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt) : base(AIOType.kuTorrenting, pe)
		{
			DesiredLocationNoExt = desiredLocationNoExt;
			Entry = te;
		}

		public override IgnoreItem GetIgnore()
		{
					if (string.IsNullOrEmpty(DesiredLocationNoExt))
				return null;
			else
				return new IgnoreItem(DesiredLocationNoExt);
		}
		public override string TargetFolder()
		{
			if (string.IsNullOrEmpty(Entry.DownloadingTo))
				return null;
			else
				return FileInfo(Entry.DownloadingTo).DirectoryName;
		}
		public override string FilenameForProgress()
		{
			return "";
		}
		public bool SameAs2(AIOuTorrenting o)
		{
			return (o.Entry == Entry);
		}
		public override bool SameAs(AIOItem o)
		{
			return (this.Type == o.Type) && SameAs2((AIOuTorrenting)(o));
		}
		public override int IconNumber()
		{
			return 2;
		}

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = SI.ShowName();
            lvi.SubItems.Add(PE != null ? PE.SeasonNumber.ToString() : "");
            lvi.SubItems.Add(PE != null ? PE.NumsAsString() : "");

            if (PE != null)
            {
                DateTime dt = PE.GetAirDateDT(true);
                if ((dt != null) && (dt.CompareTo(DateTime.MaxValue)))
                    lvi.SubItems.Add(PE.GetAirDateDT(true).ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(Destination.DirectoryName);
            lvi.SubItems.Add(BannerPath);

            if (string.IsNullOrEmpty(BannerPath))
                lvi.BackColor = WarningColor();

            lvi.SubItems.Add(Destination.Name);

            lvi.Tag = this;
            lvi.Group = lv.Groups[5]; // download image
            // lv->Items->Add(lvi);

            return lvi;
        }

		public override bool Action(TVDoc doc)
		{
			Done = true;
			return true;
		}
	}



	public class AIOSorter: System.Collections.Generic.IComparer<AIOItem>
	{
		public virtual int Compare(AIOItem x, AIOItem y)
		{
			if (x.Type == y.Type)
			{
				if (x.Type == AIOType.kCopyMoveRename)
				{
					AIOCopyMoveRename xx = (AIOCopyMoveRename)(x);
					AIOCopyMoveRename yy = (AIOCopyMoveRename)(y);

					string s1 = xx.From.FullName+(xx.From.Directory.Root.FullName != xx.To.Directory.Root.FullName ? "0" : "1");
					string s2 = yy.From.FullName+(yy.From.Directory.Root.FullName != yy.To.Directory.Root.FullName ? "0" : "1");

					return s1.CompareTo(s2);
				}
				if (x.Type == AIOType.kDownload)
				{
					AIODownload xx = (AIODownload)(x);
					AIODownload yy = (AIODownload)(y);

					return xx.Destination.FullName.CompareTo(yy.Destination.FullName);
				}
				if (x.Type == AIOType.kRSS)
				{
					AIORSS xx = (AIORSS)(x);
					AIORSS yy = (AIORSS)(y);

					return xx.RSS.URL.CompareTo(yy.RSS.URL);
				}
				if (x.Type == AIOType.kMissing)
				{
					AIOMissing xx = (AIOMissing)(x);
					AIOMissing yy = (AIOMissing)(y);

					return (xx.TheFileNoExt+xx.PE.Name).CompareTo(yy.TheFileNoExt+yy.PE.Name);
				}
				if (x.Type == AIOType.kNFO)
				{
					AIONFO xx = (AIONFO)(x);
					AIONFO yy = (AIONFO)(y);

					if (xx.PE == null)
						return 1;
					else if (yy.PE == null)
						return -1;
					else
						return (xx.Where.FullName+xx.PE.Name).CompareTo(yy.Where.FullName+yy.PE.Name);
				}
				if (x.Type == AIOType.kuTorrenting)
				{
					AIOuTorrenting xx = (AIOuTorrenting)(x);
					AIOuTorrenting yy = (AIOuTorrenting)(y);

					if (xx.PE == null)
						return 1;
					else if (yy.PE == null)
						return -1;
					else
						return (xx.DesiredLocationNoExt).CompareTo(yy.DesiredLocationNoExt);
				}
				Diagnostics.Debug.Fail("Unknown type in AIOItem::Compare"); // uhoh
				return 1;
			}
			else
			{
				// different types
				return ((int)x.Type - (int)y.Type);
			}
		}

	}


	public class LVResults
	{
		public System.Collections.Generic.List<AIOuTorrenting > uTorrenting;
		public System.Collections.Generic.List<AIOMissing > Missing;
		public System.Collections.Generic.List<AIORSS > RSS;
		public System.Collections.Generic.List<AIOCopyMoveRename > Rename;
		public System.Collections.Generic.List<AIOCopyMoveRename > CopyMove;
		public System.Collections.Generic.List<AIODownload > Download;
		public System.Collections.Generic.List<AIONFO > NFO;
		public System.Collections.Generic.List<AIOItem > FlatList;
		public bool AllSame;
		public int Count;

		public LVResults(ListView lv, bool @checked) // if not checked, then selected items
		{
			uTorrenting = new System.Collections.Generic.List<AIOuTorrenting >();
			Missing = new System.Collections.Generic.List<AIOMissing >();
			RSS = new System.Collections.Generic.List<AIORSS >();
			CopyMove = new System.Collections.Generic.List<AIOCopyMoveRename >();
			Rename = new System.Collections.Generic.List<AIOCopyMoveRename >();
			Download = new System.Collections.Generic.List<AIODownload >();
			NFO = new System.Collections.Generic.List<AIONFO >();
			FlatList = new System.Collections.Generic.List<AIOItem >();

			Generic.List<ListViewItem > sel = new System.Collections.Generic.List<ListViewItem >();
			if (@checked)
			{
				ListView.CheckedListViewItemCollection ss = lv.CheckedItems;
				foreach (ListViewItem lvi in ss)
					sel.Add(lvi);
			}
			else
			{
				ListView.SelectedListViewItemCollection ss = lv.SelectedItems;
				foreach (ListViewItem lvi in ss)
					sel.Add(lvi);
			}

			Count = sel.Count;

			if (sel.Count == 0)
				return;

			AIOType t = (AIOItem)(sel[0].Tag).Type;

			AllSame = true;
			foreach (ListViewItem lvi in sel)
			{
				AIOItem aio = (AIOItem)(lvi.Tag);
				FlatList.Add(aio);
				AIOType t2 = aio.Type;
				if (t2 != t)
					AllSame = false;

				switch (t2)
				{
				case AIOType.kCopyMoveRename:
					{
						AIOCopyMoveRename cmr = (AIOCopyMoveRename)(aio);
						if (cmr.Operation == AIOCopyMoveRename.Op.Rename)
							Rename.Add((AIOCopyMoveRename)(aio));
						else // copy/move
							CopyMove.Add((AIOCopyMoveRename)(aio));
						break;
					}
				case AIOType.kDownload:
					Download.Add((AIODownload)(aio));
					break;
				case AIOType.kRSS:
					RSS.Add((AIORSS)(aio));
					break;
				case AIOType.kMissing:
					Missing.Add((AIOMissing)(aio));
					break;
				case AIOType.kNFO:
					NFO.Add((AIONFO)(aio));
					break;
				case AIOType.kuTorrenting:
					uTorrenting.Add((AIOuTorrenting)(aio));
					break;
				}
			}
		}
	}
} // namespace