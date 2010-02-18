using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Net;
using System.IO.Compression;
using System.Xml;
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


namespace TVRename
{


	public class TVDoc
	{
		private string[] Args; // command line arguments
		private TVRenameStats mStats;
		private bool mDirty;
		private static System.Collections.Generic.List<ShowItem > ShowItems;

		private Thread mDownloaderThread;
		private bool DownloadOK;

		private TheTVDB mTVDB;

		public bool DownloadDone;
		public bool DownloadStopOnError;
		public int DownloadPct;
		public int DownloadsRemaining;
		public Semaphore WorkerSemaphore;
		public System.Collections.Generic.List<Thread > Workers;

		public TheTVDB GetTVDB(bool @lock, string whoFor)
		{
			if (@lock)
			{
				if (string.IsNullOrEmpty(whoFor))
					whoFor = "unknown";

				mTVDB.GetLock("GetTVDB : " + whoFor);
			}
			return mTVDB;
		}

		public ScanProgress ScanProgDlg;
		public bool AIOCancel;


		public System.Collections.Generic.List<AIOItem > TheAIOList;
		public System.Collections.Generic.List<IgnoreItem > Ignore;
		public System.Collections.Generic.List<AddItem > AddItems;
		public TVSettings Settings;
		public System.Collections.Generic.List<string > MonitorFolders;
		public System.Collections.Generic.List<string > IgnoreFolders;
		public System.Collections.Generic.List<string > SearchFolders;
		public RSSItemList RSSList;

		public bool LoadOK;
		public string LoadErr;

		public static FileInfo TVDBFile()
		{
			return new FileInfo(System.Windows.Forms.Application.UserAppDataPath+"\\TheTVDB.xml");
		}
		public static FileInfo TVDocSettingsFile()
		{
			return new FileInfo(System.Windows.Forms.Application.UserAppDataPath+"\\TVRenameSettings.xml");
		}

		public TVDoc(string[] args, FileInfo settingsFile, FileInfo tvdbFile)
		{
			Args = args;
			Ignore = new System.Collections.Generic.List<IgnoreItem >();

			Workers = null;
			WorkerSemaphore = null;

			mTVDB = new TheTVDB(tvdbFile, TVDBFile());

			mStats = new TVRenameStats();
			mDirty = false;
			TheAIOList = new System.Collections.Generic.List<AIOItem >();

			Settings = new TVSettings();

			MonitorFolders = new System.Collections.Generic.List<string >();
			IgnoreFolders = new System.Collections.Generic.List<string >();
			SearchFolders = new System.Collections.Generic.List<string >();
			ShowItems = new System.Collections.Generic.List<ShowItem >();
			AddItems = new System.Collections.Generic.List<AddItem >();

			DownloadDone = true;
			DownloadOK = true;

			AIOCancel = false;
			ScanProgDlg = null;

			LoadOK = ((settingsFile == null) || LoadXMLSettings(settingsFile)) && mTVDB.LoadOK;

			//    StartServer();
		}

		~TVDoc()
		{
			StopBGDownloadThread();
		}

		public string[] GetArgs()
		{
			return Args;
		}

		public bool HasArg(string which)
		{
			foreach (string s in Args)
				if (s.ToLower() == which.ToLower())
					return true;
			return false;
		}


	private static void LockShowItems()
			 {
				 return;
//C++ TO C# CONVERTER TODO TASK: C# does not allow setting or comparing #define constants:
#if DEBUG
				 System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
				 System.Diagnostics.StackFrame sf = st.GetFrame(0);
				 string msg = sf.GetMethod().DeclaringType.FullName + "::" + sf.GetMethod().Name;
				 System.Diagnostics.Debug.Print("LockShowItems " + msg);
#endif

				 Monitor.Enter(ShowItems);
			 }
	public static void UnlockShowItems()
			{
				return;
//C++ TO C# CONVERTER TODO TASK: C# does not allow setting or comparing #define constants:
#if DEBUG
				System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
				System.Diagnostics.StackFrame sf = st.GetFrame(0);
				string msg = sf.GetMethod().DeclaringType.FullName + "::" + sf.GetMethod().Name;
				System.Diagnostics.Debug.Print("UnlockShowItems " + msg);
#endif

				Monitor.Exit(ShowItems);
			}

	public TVRenameStats Stats()
			{
				mStats.NS_NumberOfShows = ShowItems.Count;
				mStats.NS_NumberOfSeasons = 0;
				mStats.NS_NumberOfEpisodesExpected = 0;

				LockShowItems();
				foreach (ShowItem si in ShowItems)
				{
					foreach (KeyValuePair<int, System.Collections.Generic.List<ProcessedEpisode > > k in si.SeasonEpisodes)
						mStats.NS_NumberOfEpisodesExpected += k.Value.Count;
					mStats.NS_NumberOfSeasons += si.SeasonEpisodes.Count;
				}
				UnlockShowItems();

				return mStats;
			}
			public void SetDirty()
			{
				mDirty = true;
			}
			public bool Dirty()
			{
				return mDirty;
			}
			public System.Collections.Generic.List<ShowItem > GetShowItems(bool @lock)
			{
				if (@lock)
					LockShowItems();

				ShowItems.Sort(new Comparison<ShowItem>(CompareShowItemNames));
				return ShowItems;
			}
			public ShowItem GetShowItem(int id)
			{
				LockShowItems();
				foreach (ShowItem si in ShowItems)
				{
					if (si.TVDBCode == id)
					{
						UnlockShowItems();
						return si;
					}
				}
				UnlockShowItems();
				return null;
			}


//            
//			void WebServer()
//			{
//			TVRenameServer ^serve = gcnew TVRenameServer(this); // all work is done in constructor which never returns
//			}
//			void StartServer()
//			{
//			mServerThread = gcnew Thread(gcnew ThreadStart(this, &TVDoc::WebServer));
//			mServerThread->Name = "Web Server";
//			mServerThread->Start();
//			}
//			void StopServer()
//			{
//			if (mServerThread != nullptr)
//			{
//			mServerThread->Abort();
//			mServerThread = nullptr;
//
//			}
//			}
//			

			public void DoRenameCheck(SetProgressDelegate prog, ShowItem specific)
			{
				Stats().RenameChecksDone++;

				// RenameList->Clear();

				prog.Invoke(0);
				//lvStatus->Items->Clear();
				int c = 0;
				int totalN = 0;

				LockShowItems();

				Generic.List<ShowItem > showlist;
				if (specific != null)
				{
					showlist = new System.Collections.Generic.List<ShowItem >();
					showlist.Add(specific);
				}
				else
					showlist = ShowItems;

				foreach (ShowItem si in showlist)
					if (si.DoRename)
						totalN += si.SeasonEpisodes.Count;

				foreach (ShowItem si in showlist)
				{
					if (AIOCancel)
					{
						UnlockShowItems();
						return;
					}
					if (!si.DoRename)
						continue;

					if (si.AllFolderLocations(Settings).Count == 0)
						continue; // skip

					c += si.SeasonEpisodes.Count;

					prog.Invoke(100*(c+1) / (totalN+1)); // +1 to always have a bit of activity in the bar when we're working

					DoRenamesFor(si);
				}

				if (Settings.KeepTogether)
					KeepTogether(TheAIOList);

				UnlockShowItems();

				if (specific == null)
				{
					//				if (Settings->ExportRenamingXML)
					//					ExportRenamingXML(Settings->ExportRenamingXMLTo);
				}

				prog.Invoke(100);
			}

			public void DoRenamesFor(ShowItem si)
			{
				int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
				si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
				foreach (int snum in numbers)
					//for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in si->SeasonEpisodes)
				{
					//int snum = kvp->Key;
					if ((si.IgnoreSeasons.Contains(snum)) || (!si.AllFolderLocations(Settings).ContainsKey(snum)))
						continue; // ignore/skip this season

					// Season ^seas = kvp->Value;
					if ((snum == 0) && (si.CountSpecials))
						continue; // skip specials

					Collections.Generic.List<string > folders = si.AllFolderLocations(Settings)[snum];
					foreach (string folder in folders)
					{
						// generate new filename info
						DirectoryInfo di;
						try
						{
							di = new DirectoryInfo(folder);
						}
						catch
						{
							return;
						}

						if (!di.Exists)
							return; // we don't care. a missing check will pick up missing folders if necessary.

						FileInfo[]fi = di.GetFiles();

						// fill renameItems
						foreach (FileInfo fiTemp in fi)
						{
							int seas;
							int ep;

							if (!Settings.UsefulExtension(fiTemp.Extension, true))
								continue; // move on

							if (FindSeasEp(fiTemp, ref seas, ref ep, si.ShowName()))
							{
								// see if we have an epinfo for this one
								if (seas == -1)
									seas = snum;

								foreach (ProcessedEpisode pe in si.SeasonEpisodes[snum])
								{
									if ((pe.EpNum == ep) && (pe.SeasonNumber == seas))
									{
										//SeriesInfo ^ser = GetTVDB(false, "")->GetSeries(si->TVDBCode);

										string nname = FilenameFriendly(Settings.NamingStyle.NameForExt(pe, fiTemp.Extension));

										if (nname != fiTemp.Name)
										{
											TheAIOList.Add(new AIOCopyMoveRename(AIOCopyMoveRename.Op.Rename, fiTemp, FileInFolder(folder, nname), pe));
										}
										break;
									}
								}
							}
						}
					}
				}
			}

			public void DoMissingCheck(SetProgressDelegate prog, ShowItem specific)
			{
				Stats().MissingChecksDone++;
				mStats.NS_NumberOfEpisodes = 0;

				// MissingEpisodes->Clear();

				prog.Invoke(0);
				//lvStatus->Items->Clear();
				int c = 0;
				int totalN = 0;
				LockShowItems();

				Generic.List<ShowItem > showlist;
				if (specific != null)
				{
					CheckAllFoldersExist(specific);
					showlist = new System.Collections.Generic.List<ShowItem >();
					showlist.Add(specific);
				}
				else
					showlist = ShowItems;

				foreach (ShowItem si in showlist)
					if (si.DoMissingCheck)
						totalN += si.AllFolderLocations(Settings).Count;

				foreach (ShowItem si in showlist)
				{
					if (AIOCancel)
					{
						UnlockShowItems();
						return;
					}

					if (!si.DoMissingCheck)
						continue; // skip

					prog.Invoke(100*(c+1) / (totalN+1)); // +1 to always have a bit of activity in the bar when we're working

					c += si.AllFolderLocations(Settings).Count;

					if (!UpToDateCheck(si))
						break;
				}
				UnlockShowItems();

				if (specific == null)
				{
//                    
//					if (Settings->ExportMissingCSV)
//					ExportMissingCSV(Settings->ExportMissingCSVTo);
//					if (Settings->ExportMissingXML)
//					ExportMissingXML(Settings->ExportMissingXMLTo);
//					
				}

				prog.Invoke(100);
			}

			public void SetSearcher(int n)
			{
				Settings.TheSearchers.SetToNumber(n);
				SetDirty();
			}
			public bool FolderIsSubfolderOf(string thisOne, string ofThat)
			{
				// need terminating slash, otherwise "c:\abc def" will match "c:\abc"
				thisOne += "\\";
				ofThat += "\\";
				int l = ofThat.Length;
				return ((thisOne.Length >= l) && (thisOne.Substring(0,l).ToLower() == ofThat.ToLower()));

			}

			public void CheckFolder(DirectoryInfo di, System.Collections.Generic.List<AddItem > addList) // check a monitored folder for new shows
			{
				foreach (DirectoryInfo di2 in di.GetDirectories())
				{
					string seasonWord = "Season ";
					bool hasSeasonFolders = false;
					try
					{
						// keep in sync with ProcessAddItems, etc.
						hasSeasonFolders = di2.GetDirectories("*" + seasonWord + "*").Length > 0; // todo - use non specific word
					}
					catch (UnauthorizedAccessException )
					{
						// e.g. recycle bin, system volume information
						continue;
					}

					if (!hasSeasonFolders)
						CheckFolder(di2, addList); // not a season folder.. recurse!

					string theFolder = di2.FullName.ToLower();

					// if its not in the ignore list
					if (IgnoreFolders.Contains(theFolder))
						continue;

					// ..and not already a folder for one of our shows
					bool bzzt = false;
					foreach (ShowItem si in ShowItems)
					{
						if (si.AutoAddNewSeasons && !string.IsNullOrEmpty(si.AutoAdd_FolderBase) && FolderIsSubfolderOf(theFolder, si.AutoAdd_FolderBase))
						{
							// we're looking at a folder that is a subfolder of an existing show
							bzzt = true;
							break;
						}

						if (bzzt)
							break;

						Generic.Dictionary<int, System.Collections.Generic.List<string > > afl = si.AllFolderLocations(Settings);
						foreach (KeyValuePair<int, System.Collections.Generic.List<string > > kvp in afl)
						{
							foreach (string folder in kvp.Value)
							{
								if (theFolder.ToLower() == folder.ToLower())
								{
									bzzt = true;
									break;
								}
							}
						}

						if (bzzt)
							break;
					} // for each showitem
					if (!bzzt)
					{
						// ....its good!
						addList.Add(new AddItem(di2.FullName, hasSeasonFolders ? FolderModeEnum.kfmFolderPerSeason : FolderModeEnum.kfmFlat, -1));
					}
				} // for each directory
			}

			public void CheckMonitoredFolders()
			{
				AddItems.Clear();

				LockShowItems();
				foreach (string folder in MonitorFolders)
				{
					//try {
					DirectoryInfo di = new DirectoryInfo(folder);
					if (!di.Exists)
						continue;

					CheckFolder(di, AddItems);
//                     }
//					catch (...)
//					{
//					}
				}
				UnlockShowItems();
			}

			public bool RenameFilesToMatchTorrent(string torrent, string folder, TreeView tvTree, SetProgressDelegate prog, bool copyNotMove, string copyDest)
			{
				if (string.IsNullOrEmpty(folder))
					return false;
				if (string.IsNullOrEmpty(torrent))
					return false;

				if (copyNotMove)
				{
					if (string.IsNullOrEmpty(copyDest))
						return false;
					if (!Directory.Exists(copyDest))
						return false;
				}

				Stats().TorrentsMatched++;

				BTFileRenamer btp = new BTFileRenamer(prog);
				Generic.List<AIOItem > newList = new System.Collections.Generic.List<AIOItem >();
				bool r = btp.RenameFilesOnDiskToMatchTorrent(torrent, folder, tvTree, newList, prog, copyNotMove, copyDest);

				foreach (AIOItem i in newList)
					TheAIOList.Add(i);

				return r;
			}


			public string FilenameFriendly(string fn)
			{
				foreach (Replacement R in Settings.Replacements)
				{
					if (R.CaseInsensitive)
						fn = Regex.Replace(fn, Regex.Escape(R.This), Regex.Escape(R.That), RegexOptions.IgnoreCase);
					else
						fn = fn.Replace(R.This, R.That);
				}
				if (Settings.ForceLowercaseFilenames)
					fn = fn.ToLower();
				return fn;
			}

			// consider each of files, see if it is suitable for series "ser" and episode "epi"
			// if so, add a rcitem for copy to "fi"
			public bool FindMissingEp(System.Collections.Generic.List<DirCacheEntry > files, AIOMissing me, System.Collections.Generic.List<AIOItem > addTo, AIOCopyMoveRename.Op whichOp)
			{
				string showname = me.PE.SI.ShowName();
				int season = me.PE.SeasonNumber;

				//String ^toName = FilenameFriendly(Settings->NamingStyle->NameFor(me->PE));
				int epnum = me.PE.EpNum;

				// TODO: find a 'best match', or use first ?

				showname = SimplifyName(showname);

				foreach (DirCacheEntry dce in files)
				{
					if (AIOCancel)
						return true;

					bool matched = false;

					try
					{
						if (!dce.HasUsefulExtension_NotOthersToo) // not a usefile file extension
							continue;
						if (Settings.IgnoreSamples && dce.LowerName.Contains("sample") && ((dce.Length/(1024 *1024)) < Settings.SampleFileMaxSizeMB))
							continue;

						matched = Regex.Match(dce.SimplifiedFullName,"\\b"+showname+"\\b",RegexOptions.IgnoreCase).Success;

						if (matched)
						{
							int seasF;
							int epF;
							// String ^fn = file->Name;

							if ((FindSeasEp(dce.TheFile, ref seasF, ref epF, me.PE.TheSeries.Name) && (seasF == season) && (epF == epnum)) || (me.PE.SI.UseSequentialMatch && MatchesSequentialNumber(dce.TheFile.Name, seasF, epF, me.PE) && (seasF == season) && (epF == epnum)))
							{
								addTo.Add(new AIOCopyMoveRename(whichOp, dce.TheFile, new FileInfo(me.TheFileNoExt+dce.TheFile.Extension), me.PE));
								return true;
							}
						}
					}
					catch (System.IO.PathTooLongException e)
					{
						string t = "Path too long. " + dce.TheFile.FullName + ", " + e.Message;
						t += ".  Try to display more info?";
						DialogResult dr = MessageBox.Show(t,"Path too long", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
						if (dr ==DialogResult.Yes)
						{
							t = "DirectoryName " + dce.TheFile.DirectoryName + ", File name: " + dce.TheFile.Name;
							t += matched ? ", matched.  " : ", no match.  ";
							if (matched)
							{
								t += "Show: " + me.PE.TheSeries.Name + ", Season " + season.ToString() + ", Ep " + epnum.ToString() + ".  ";
								t += "To: " + me.TheFileNoExt;
							}

							MessageBox.Show(t, "Path too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
				}

				return false;
			}

			public void KeepTogether(System.Collections.Generic.List<AIOItem > aiolist)
			{
				// for each of the items in rcl, do the same copy/move if for other items with the same
				// base name, but different extensions

				Generic.List<AIOItem > extras = new System.Collections.Generic.List<AIOItem >();

				foreach (AIOItem aio1 in aiolist)
				{
					if (aio1.Type != AIOType.kCopyMoveRename)
						continue;

					AIOCopyMoveRename aio = (AIOCopyMoveRename)(aio1);

					try
					{
						DirectoryInfo sfdi = aio.From.Directory;
						string basename = aio.From.Name;
						int l = basename.Length;
						basename = basename.Substring(0, l - aio.From.Extension.Length);

						string toname = aio.To.Name;
						int l2 = toname.Length;
						toname = toname.Substring(0, l2 - aio.To.Extension.Length);

						FileInfo[] flist = sfdi.GetFiles(basename+".*");
						foreach (FileInfo fi in flist)
						{
							// do case insensitive replace
							string n = fi.Name;
							int p = n.ToUpper().IndexOf(basename.ToUpper());
							string newName = n.Substring(0, p) + toname + n.Substring(p + basename.Length);
							if ((Settings.RenameTxtToSub) && (newName.EndsWith(".txt")))
								newName = newName.Substring(0, newName.Length - 4) + ".sub";

							AIOCopyMoveRename newitem = new AIOCopyMoveRename(aio.Operation, fi, FileInFolder(aio.To.Directory, newName), aio.PE);

							// check this item isn't already in our to-do list
							bool doNotAdd = false;
							foreach (AIOItem ai2 in aiolist)
							{
								if (ai2.Type != AIOType.kCopyMoveRename)
									continue;

								if ((AIOCopyMoveRename)(ai2).SameSource(newitem))
								{
									doNotAdd = true;
									break;
								}
							}

							if (!doNotAdd)
							{
								if (!newitem.SameAs(aio)) // don't re-add ourself
									extras.Add(newitem);
							}
						}
					}
					catch (System.IO.PathTooLongException e)
					{
						string t = "Path or filename too long. " + aio.From.FullName + ", " + e.Message;
						MessageBox.Show(t,"Path or filename too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}

				foreach (AIOItem aio in extras)
				{
					// check we don't already have this in our list and, if we don't add it!
					bool have = false;
					foreach (AIOItem aio2 in aiolist)
					{
						if (aio2.SameAs(aio))
						{
							have = true;
							break;
						}
					}

					if (!have)
						aiolist.Add(aio);
				}
			}

			public void LookForMissingEps(SetProgressDelegate prog)
			{
				// for each ep we have noticed as being missing
				// look through the monitored folders for it

				Stats().FindAndOrganisesDone++;

				prog.Invoke(0);

				Generic.List<AIOItem > newList = new System.Collections.Generic.List<AIOItem >();
				Generic.List<AIOItem > toRemove = new System.Collections.Generic.List<AIOItem >();

				int fileCount = 0;
				foreach (string s in SearchFolders)
					fileCount += CountFiles(s, true);

				int c = 0;

				Collections.Generic.List<DirCacheEntry > files = new System.Collections.Generic.List<DirCacheEntry >();
				for (int k =0;k<SearchFolders.Count;k++)
				{
					if (AIOCancel)
						return;

					c = BuildDirCache(prog, c, fileCount, files, SearchFolders[k], true, Settings);
				}

				c = 0;
				int totalN = TheAIOList.Count;
				foreach (AIOItem aio1 in TheAIOList)
				{
					if (AIOCancel)
						return;

					prog.Invoke(50 + 50*(++c) / (totalN+1)); // second 50% of progress bar

					if (aio1.Type == AIOType.kMissing)
						if (FindMissingEp(files, (AIOMissing)(aio1), newList, AIOCopyMoveRename.Op.Copy))
							toRemove.Add(aio1);
				}

				if (Settings.KeepTogether)
					KeepTogether(newList);

				prog.Invoke(100);

				if (!Settings.LeaveOriginals)
				{
					// go through and change last of each operation on a given source file to a 'Move'
					// ideally do that move within same filesystem

					// sort based on source file, and destination drive, putting last if destdrive == sourcedrive
					newList.Sort(new AIOSorter()); // was SortSmartly

					// sort puts all the CopyMoveRenames together				

					// then set the last of each source file to be a move
					for (int i =0;i<newList.Count;i++)
					{
						bool ok1 = newList[i].Type == AIOType.kCopyMoveRename;
						bool last = i == (newList.Count-1);
						bool ok2 = !last && (newList[i+1].Type == AIOType.kCopyMoveRename);

						if (ok1 && !ok2)
						{
							// last item, or last copymoverename item in the list
							AIOCopyMoveRename a1 = (AIOCopyMoveRename)(newList[i]);
							a1.Operation = AIOCopyMoveRename.Op.Move;
						}
						else if (ok1 && ok2)
						{
							AIOCopyMoveRename a1 = (AIOCopyMoveRename)(newList[i]);
							AIOCopyMoveRename a2 = (AIOCopyMoveRename)(newList[i+1]);
							if (!Same(a1.From, a2.From))
								a1.Operation = AIOCopyMoveRename.Op.Move;
						}
					}
				}

				foreach (AIOItem i in toRemove)
					TheAIOList.Remove(i);

				foreach (AIOItem i in newList)
					TheAIOList.Add(i);

//                 if (Settings->ExportFOXML)
//				ExportFOXML(Settings->ExportFOXMLTo);
			}


			// -----------------------------------------------------------------------------


			public void GetThread(Object codeIn)
			{
				Diagnostics.Debug.Assert(WorkerSemaphore != null);

				WorkerSemaphore.WaitOne(); // don't start until we're allowed to

				int code = (int)(codeIn);

				Diagnostics.Debug.Print("  Downloading " + code);
				bool r = GetTVDB(false, "").EnsureUpdated(code);
				Diagnostics.Debug.Print("  Finished " + code);
				if (!r)
				{
					DownloadOK = false;
					if (DownloadStopOnError)
						DownloadDone = true;
				}
				WorkerSemaphore.Release(1);
			}

			public void WaitForAllThreadsAndTidyUp()
			{
				if (Workers != null)
				{
					foreach (Thread t in Workers)
						if (t.IsAlive)
							t.Join();
				}

				Workers = null;
				WorkerSemaphore = null;
			}

			public void Downloader()
			{
				// do background downloads of webpages

				try
				{
					if (ShowItems.Count == 0)
					{
						DownloadDone = true;
						DownloadOK = true;
						return;
					}

					if (!GetTVDB(false, "").GetUpdates())
					{
						DownloadDone = true;
						DownloadOK = false;
						return;
					}

					// for eachs of the ShowItems, make sure we've got downloaded data for it

					int n2 = ShowItems.Count;
					int n = 0;
					Generic.List<int> codes = new System.Collections.Generic.List<int>();
					LockShowItems();
					foreach (ShowItem si in ShowItems)
						codes.Add(si.TVDBCode);
					UnlockShowItems();

					int numWorkers = Settings.ParallelDownloads;
					Workers = new System.Collections.Generic.List<Thread >();

					WorkerSemaphore = new Semaphore(numWorkers, numWorkers); // allow up to numWorkers working at once

					foreach (int code in codes)
					{
						DownloadPct = 100*(n+1)/(n2+1);
						DownloadsRemaining = n2-n;
						n++;

						WorkerSemaphore.WaitOne(); // blocks until there is an available slot
						Thread t = new Thread(new ParameterizedThreadStart(this, TVDoc.GetThread));
						Workers.Add(t);
						t.Name = "GetThread:" + code.ToString();
						t.Start(code); // will grab the semaphore as soon as we make it avialable
						int n = WorkerSemaphore.Release(1); // release our hold on the semaphore, so that worker can grab it
						Diagnostics.Debug.Print("Started " + code + " pool has " + n + " free");
						Thread.Sleep(0); // allow the other thread a chance to run and grab

						// tidy up any finished workers
						for (int i =Workers.Count-1;i>=0;i--)
							if (!Workers[i].IsAlive)
								Workers.RemoveAt(i); // remove dead worker

						if (DownloadDone)
							break;
					}

					WaitForAllThreadsAndTidyUp();

					GetTVDB(false, "").UpdatesDoneOK();
					DownloadDone = true;
					DownloadOK = true;
					return;
				}
				catch (ThreadAbortException )
				{
					DownloadDone = true;
					DownloadOK = false;
					return;
				}

				DownloadOK = false;
				DownloadDone = true;
				return;
			}


			public void StartBGDownloadThread(bool stopOnError)
			{
				if (!DownloadDone)
					return;
				DownloadStopOnError = stopOnError;
				DownloadPct = 0;
				DownloadDone = false;
				DownloadOK = true;
				mDownloaderThread = new Thread(new ThreadStart(this, TVDoc.Downloader));
				mDownloaderThread.Name = "Downloader";
				mDownloaderThread.Start();
			}

			public void WaitForBGDownloadDone()
			{
				if ((mDownloaderThread != null) && (mDownloaderThread.IsAlive))
					mDownloaderThread.Join();
				mDownloaderThread = null;
			}

			public void StopBGDownloadThread()
			{
				if (mDownloaderThread != null)
				{
					if (Workers != null)
					{
						foreach (Thread t in Workers)
							t.Interrupt();
					}

					WaitForAllThreadsAndTidyUp();

					if (mDownloaderThread.IsAlive)
					{
						mDownloaderThread.Interrupt();
						mDownloaderThread = null;
					}
				}
			}

			public bool DoDownloadsFG()
			{
				if (Settings.OfflineMode)
					return true; // don't do internet in offline mode!

				StartBGDownloadThread(true);

				const int delayStep = 100;
				int count = 1000/delayStep; // one second
				while ((count--) && (!DownloadDone))
					Threading.Thread.Sleep(delayStep);

				if (!DownloadDone && !HasArg("/hide")) // downloading still going on, so time to show the dialog if we're not in /hide mode
				{
					DownloadProgress dp = new DownloadProgress(this);
					dp.ShowDialog();
					dp.Update();
				}

				WaitForBGDownloadDone();

				GetTVDB(false, "").SaveCache();

				GenDict();

				if (!DownloadOK)
				{
					MessageBox.Show(mTVDB.LastError, "Error while downloading", MessageBoxButtons.OK, MessageBoxIcon.Error);
					mTVDB.LastError = "";
				}

				return DownloadOK;
			}

			public bool GenDict()
			{
				bool res = true;
				LockShowItems();
				foreach (ShowItem si in ShowItems)
					if (!GenerateEpisodeDict(si))
						res = false;
				UnlockShowItems();
				return res;
			}

			public Searchers GetSearchers()
			{
				return Settings.TheSearchers;
			}

			public void TidyTVDB()
			{
				// remove any shows from thetvdb that aren't in My Shows
				TheTVDB db = GetTVDB(true, "TidyTVDB");
				Generic.List<int> removeList = new System.Collections.Generic.List<int>();

				foreach (KeyValuePair<int, SeriesInfo> kvp in mTVDB.GetSeriesDict())
				{
					bool found = false;
					foreach (ShowItem si in ShowItems)
					{
						if (si.TVDBCode == kvp.Key)
						{
							found = true;
							break;
						}
					}
					if (!found)
						removeList.Add(kvp.Key);
				}

				foreach (int i in removeList)
					db.ForgetShow(i, false);

				db.Unlock("TheTVDB");
				db.SaveCache();
			}
			public void Closing()
			{
				StopBGDownloadThread();
				Stats().Save();
			}

			public void DoBTSearch(ProcessedEpisode ep)
			{
				if (ep == null)
					return;
				TVDoc.SysOpen(Settings.BTSearchURL(ep));
			}

			public void DoWhenToWatch(bool cachedOnly)
			{
				if (!cachedOnly && !DoDownloadsFG())
				{
					return;
				}
				if (cachedOnly)
					GenDict();
			}

			public System.Collections.Generic.List<System.IO.FileInfo > FindEpOnDisk(ProcessedEpisode pe)
			{
				return FindEpOnDisk(pe.SI, pe);
			}
			public System.Collections.Generic.List<System.IO.FileInfo > FindEpOnDisk(ShowItem si, Episode epi)
			{
				System.Collections.Generic.List<System.IO.FileInfo > ret = new System.Collections.Generic.List<System.IO.FileInfo >();

				int seasWanted = epi.TheSeason.SeasonNumber;
				int epWanted = epi.EpNum;

				int snum = epi.TheSeason.SeasonNumber;

				if (!si.AllFolderLocations(Settings).ContainsKey(snum))
					return ret;

				foreach (string folder in si.AllFolderLocations(Settings)[snum])
				{
					DirectoryInfo di;
					try
					{
						di = new DirectoryInfo(folder);
					}
					catch
					{
						return ret;
					}
					if (!di.Exists)
						return ret;

					FileInfo[]files = di.GetFiles();
					foreach (FileInfo fiTemp in files)
					{
						int seasFound;
						int epFound;

						if (!Settings.UsefulExtension(fiTemp.Extension, false))
							continue; // move on

						if (FindSeasEp(fiTemp, ref seasFound, ref epFound, si.ShowName()))
						{
							if (seasFound == -1)
								seasFound = seasWanted;
							if ((seasFound == seasWanted) && (epFound == epWanted))
								ret.Add(fiTemp);
						}
					}
				}

				return ret;
			}

			public bool HasAnyAirdates(ShowItem si, int snum)
			{
				bool r = false;
				TheTVDB db = GetTVDB(false, "");
				SeriesInfo ser = db.GetSeries(si.TVDBCode);
				if ((ser != null) && (ser.Seasons.ContainsKey(snum)))
				{
					foreach (Episode e in ser.Seasons[snum].Episodes)
						if (e.FirstAired != null)
						{
							r = true;
							break;
						}
				}
				return r;
			}

			public void TVShowNFOCheck(ShowItem si)
			{
				// check there is a TVShow.nfo file in the root folder for the show
				if (string.IsNullOrEmpty(si.AutoAdd_FolderBase)) // no base folder defined
					return;

				if (si.AllFolderLocations(Settings).Count == 0) // no seasons enabled
					return;

				FileInfo tvshownfo = FileInFolder(si.AutoAdd_FolderBase, "tvshow.nfo");

				bool needUpdate = !tvshownfo.Exists || (si.TheSeries().Srv_LastUpdated > TZMagic.Epoch(tvshownfo.LastWriteTime));
				// was it written before we fixed the bug in <episodeguideurl> ?
				needUpdate = needUpdate || (tvshownfo.LastWriteTime.ToUniversalTime().CompareTo(DateTime(2009,9,13,7,30,0,0, DateTimeKind.Utc)) < 0);
				if (needUpdate)
					TheAIOList.Add(new AIONFO(tvshownfo, si));
			}



			public bool UpToDateCheck(ShowItem si)
			{
				int maxEpNum = 0;
				int missingCount = -1;

				if (!si.DoMissingCheck)
					return true;

				Generic.Dictionary<int, System.Collections.Generic.List<string > > flocs = si.AllFolderLocations(Settings);

				if (Settings.NFOs)
					TVShowNFOCheck(si);

				int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
				si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
				foreach (int snum in numbers)
				{
//C++ TO C# CONVERTER NOTE: Embedded comments are not maintained by C++ to C# Converter
//ORIGINAL LINE: if (si->IgnoreSeasons->Contains(snum/*kvp->Key*/))
					if (si.IgnoreSeasons.Contains(snum))
						continue; // ignore this season

					//int snum = kvp->Key;
					if ((snum == 0) && (si.CountSpecials))
						continue; // no specials season, they're merged into the seasons themselves

					Collections.Generic.List<string > folders = new System.Collections.Generic.List<string >();

					if (flocs.ContainsKey(snum))
						folders = flocs[snum];

					if ((folders.Count == 0) && (!si.AutoAddNewSeasons))
						continue; // no folders defined or found, autoadd off, so onto the next

					foreach (string folder in folders)
					{
						DirectoryInfo di = new DirectoryInfo(folder);
						if (!di.Exists)
							continue; // Skip non-existant folders, previous call to CheckAllFoldersExist will have made any wanted

						FileInfo[]fi = di.GetFiles();

						// make up a list of all the epsiodes in this season that we have locally
						Generic.List<FileInfo > localEps = new System.Collections.Generic.List<FileInfo >();

						Generic.List<ProcessedEpisode > eps = si.SeasonEpisodes[snum];

						foreach (FileInfo fiTemp in fi)
						{
							if (AIOCancel)
								return true;

							int seas;
							int ep;

							if (!Settings.UsefulExtension(fiTemp.Extension, false))
								continue; // move on

							if (FindSeasEp(fiTemp, ref seas, ref ep, si.ShowName()))
							{
								if (seas == -1)
									seas = snum;
								if (seas == snum)
								{
									while (ep >= localEps.Count)
										localEps.Add(null);

									localEps[ep] = fiTemp;
									if (ep > maxEpNum)
										maxEpNum = ep;
								}
							}
						}

						// now look at EPInfos and see if we're up to date or not
						DateTime today = DateTime.Now;
						missingCount = 0;

						TheTVDB db = GetTVDB(true, "UpToDateCheck");
						foreach (ProcessedEpisode pe in eps)
						{
							if (AIOCancel)
							{
								db.Unlock("UpToDateCheck");
								return true;
							}


							bool ok = true;
							int n = pe.EpNum;
							if ((n >= localEps.Count) || (localEps[n] == null)) // not here locally
							{
								DateTime dt = pe.GetAirDateDT(true);

								bool notFuture = (dt != null) && (dt.CompareTo(today) < 0); // isn't an episode yet to be aired
								bool anyAirdates = HasAnyAirdates(si, snum);
								bool lastSeasAirdates = (snum > 1) ? HasAnyAirdates(si, snum-1) : true; // this might be a new season, so check the last one as well
								if (si.ForceCheckAll || (!(anyAirdates || lastSeasAirdates)) || notFuture) // not in the future (i.e. its aired)
								{
									// then add it as officially missing
									TheAIOList.Add(new AIOMissing(pe, folder+"\\"+ FilenameFriendly(Settings.NamingStyle.NameForExt(pe, null))));
									missingCount++;
									ok = false;
								}
							}
							else
							{
								FileInfo fi = localEps[n];
								// do NFO and thumbnail checks if needed
								if (Settings.EpImgs)
								{
									string ban = pe.GetItem("filename");
									if (!string.IsNullOrEmpty(ban))
									{
										string fn = fi.Name;
										fn = fn.Substring(0, fn.Length - fi.Extension.Length);
										fn += ".tbn";
										FileInfo img = FileInFolder(fi.Directory,fn);
										if (!img.Exists)
											TheAIOList.Add(new AIODownload(si, pe, img, ban));
									}
								}
								if (Settings.NFOs)
								{
									string fn = fi.Name;
									fn = fn.Substring(0, fn.Length - fi.Extension.Length);
									fn += ".nfo";
									FileInfo nfo = FileInFolder(fi.Directory, fn);

									if (!nfo.Exists || (pe.Srv_LastUpdated > TZMagic.Epoch(nfo.LastWriteTime)))
										TheAIOList.Add(new AIONFO(nfo, pe));
								}
							}

							if (ok)
								mStats.NS_NumberOfEpisodes++;
						}
						db.Unlock("UpToDateCheck");

						localEps = null;
					} // for each folder
				} // for each seas

				return true;
			}


			public bool GenerateEpisodeDict(ShowItem si)
			{
				si.SeasonEpisodes.Clear();

				// copy data from tvdb
				// process as per rules
				// done!

				TheTVDB db = GetTVDB(true, "GenerateEpisodeDict");

				SeriesInfo ser = db.GetSeries(si.TVDBCode);

				if (ser == null)
					return false; // TODO: warn user

				bool r = true;
				foreach (KeyValuePair<int, Season > kvp in ser.Seasons)
				{
					Generic.List<ProcessedEpisode > pel = GenerateEpisodes(si, ser, kvp.Key, true);
					si.SeasonEpisodes[kvp.Key] = pel;
					if (pel == null)
						r = false;
				}

				Generic.List<int> theKeys = new System.Collections.Generic.List<int>();
				// now, go through and number them all sequentially
				foreach (int snum in ser.Seasons.Keys)
					theKeys.Add(snum);

				theKeys.Sort();

				int overallCount = 1;
				foreach (int snum in theKeys)
				{
					if (snum != 0)
					{
						foreach (ProcessedEpisode pe in si.SeasonEpisodes[snum])
						{
							pe.OverallNumber = overallCount;
							overallCount += 1 + pe.EpNum2 - pe.EpNum;
						}
					}
				}

				db.Unlock("GenerateEpisodeDict");

				return r;
			}

			public static System.Collections.Generic.List<ProcessedEpisode > GenerateEpisodes(ShowItem si, SeriesInfo ser, int snum, bool applyRules)
			{
				Generic.List<ProcessedEpisode > eis = new System.Collections.Generic.List<ProcessedEpisode >();

				if ((ser == null) || !ser.Seasons.ContainsKey(snum))
					return null; // todo.. something?

				Season seas = ser.Seasons[snum];

				if (seas == null)
					return null; // TODO: warn user

				foreach (Episode e in seas.Episodes)
					eis.Add(new ProcessedEpisode(e, si)); // add a copy

				if (si.DVDOrder)
				{
					eis.Sort(new System.Comparison<ProcessedEpisode >(ProcessedEpisode.DVDOrderSorter));
					Renumber(eis);
				}
				else
					eis.Sort(new System.Comparison<ProcessedEpisode >(ProcessedEpisode.EPNumberSorter));

				if (si.CountSpecials && ser.Seasons.ContainsKey(0))
				{
					// merge specials in
					foreach (Episode ep in ser.Seasons[0].Episodes)
					{
						if (ep.Items.ContainsKey("airsbefore_season") && ep.Items.ContainsKey("airsbefore_episode"))
						{
							string seasstr = ep.Items["airsbefore_season"];
							string epstr = ep.Items["airsbefore_episode"];
							if ((string.IsNullOrEmpty(seasstr)) || (string.IsNullOrEmpty(epstr)))
								continue;
							int seas = int.Parse(seasstr);
							if (seas != snum)
								continue;
							int epnum = int.Parse(epstr);
							for (int i =0;i<eis.Count;i++)
								if ((eis[i].SeasonNumber == seas) && (eis[i].EpNum == epnum))
								{
									ProcessedEpisode pe = new ProcessedEpisode(ep, si);
									pe.TheSeason = eis[i].TheSeason;
									pe.SeasonID = eis[i].SeasonID;
									eis.Insert(i, pe);
									break;
								}
						}
					}
					// renumber to allow for specials
					int epnum = 1;
					for (int j =0;j<eis.Count;j++)
					{
						eis[j].EpNum2 = epnum + (eis[j].EpNum2 - eis[j].EpNum);
						eis[j].EpNum = epnum;
						epnum++;
					}
				}

				if (applyRules)
				{
					Generic.List<ShowRule > rules = si.RulesForSeason(snum);
					if (rules != null)
						ApplyRules(eis, rules, si);
				}

				return eis;
			}


			public static void ApplyRules(System.Collections.Generic.List<ProcessedEpisode > eis, System.Collections.Generic.List<ShowRule > rules, ShowItem si)
			{
				foreach (ShowRule sr in rules)
				{
					int nn1 = sr.First;
					int nn2 = sr.Second;

					int n1 = -1;
					int n2 = -1;
					// turn nn1 and nn2 from ep number into position in array
					for (int i =0;i<eis.Count;i++)
					{
						if (eis[i].EpNum == nn1)
						{
							n1 = i;
							break;
						}
					}
					for (int i =0;i<eis.Count;i++)
					{
						if (eis[i].EpNum == nn2)
						{
							n2 = i;
							break;
						}
					}

					string txt = sr.UserSuppliedText;
					int ec = eis.Count;

					switch (sr.DoWhatNow)
					{
					case RuleAction.kRename:
						{
							if ((n1 < ec) && (n1 >= 0))
								eis[n1].Name = txt;
							break;
						}
					case RuleAction.kRemove:
						{
							if ((n1 < ec) && (n1 >= 0) && (n2 < ec) && (n2 >= 0))
								eis.RemoveRange(n1,1+n2-n1);
							else if ((n1 < ec) && (n1 >= 0) && (n2 == -1))
								eis.RemoveAt(n1);
							break;
						}
					case RuleAction.kIgnoreEp:
						{
							if (n2 == -1)
								n2 = n1;
							for (int i =n1;i<=n2;i++)
								if ((i < ec) && (i >= 0))
									eis[i].Ignore = true;
							break;
						}
					case RuleAction.kSplit:
						{
							// split one episode into a multi-parter
							if ((n1 < ec) && (n1 >= 0))
							{
								ProcessedEpisode ei = eis[n1];
								string nameBase = ei.Name;
								eis.RemoveAt(n1); // remove old one
								for (int i =0;i<nn2;i++) // make n2 new parts
								{
									ProcessedEpisode pe2 = new ProcessedEpisode(ei, si);
									pe2.Name = nameBase + " (Part " + (i+1).ToString() + ")";
									pe2.EpNum = -2;
									pe2.EpNum2 = -2;
									eis.Insert(n1+i, pe2);
								}
							}
							break;
						}
					case RuleAction.kMerge:
					case RuleAction.kCollapse:
						{
							if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec) && (n1 < n2))
							{
								ProcessedEpisode oldFirstEI = eis[n1];
								string combinedName = eis[n1].Name + " + ";
								string combinedSummary = eis[n1].Overview + "<br/><br/>";
								//int firstNum = eis[n1]->TVcomEpCount();
								for (int i =n1+1;i<=n2;i++)
								{
									combinedName += eis[i].Name;
									combinedSummary += eis[i].Overview;
									if (i != n2)
									{
										combinedName += " + ";
										combinedSummary += "<br/><br/>";
									}
								}

								eis.RemoveRange(n1,n2-n1);

								eis.RemoveAt(n1);

								ProcessedEpisode pe2 = new ProcessedEpisode(oldFirstEI, si);
								pe2.Name = ((string.IsNullOrEmpty(txt)) ? combinedName : txt);
								pe2.EpNum = -2;
								if (sr.DoWhatNow == RuleAction.kMerge)
									pe2.EpNum2 = -2 + n2-n1;
								else
									pe2.EpNum2 = -2;

								pe2.Overview = combinedSummary;
								eis.Insert(n1, pe2);
							}
							break;
						}
					case RuleAction.kSwap:
						{
							if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec))
							{
								ProcessedEpisode t = eis[n1];
								eis[n1] = eis[n2];
								eis[n2] = t;
							}
							break;
						}
					case RuleAction.kInsert:
						{
							if ((n1 < ec) && (n1 >= 0))
							{
								ProcessedEpisode t = eis[n1];
								ProcessedEpisode n = new ProcessedEpisode(t.TheSeries, t.TheSeason, si);
								n.Name = txt;
								n.EpNum = -2;
								n.EpNum2 = -2;
								eis.Insert(n1, n);
								break;
							}
						}
						break;
					} // switch DoWhatNow

					Renumber(eis);
				} // for each rule
				// now, go through and remove the ignored ones (but don't renumber!!)
				for (int i =eis.Count-1;i>=0;i--)
				{
					if (eis[i].Ignore)
						eis.RemoveAt(i);
				}
			}


			public static void Renumber(System.Collections.Generic.List<ProcessedEpisode > eis)
			{
				if (!eis.Count)
					return; // nothing to do

				// renumber 
				// pay attention to specials etc.
				int n = (eis[0].EpNum == 0) ? 0 : 1;

				for (int i =0;i<eis.Count;i++)
					if (eis[i].EpNum != -1) // is -1 if its a special or other ignored ep
					{
						int num = eis[i].EpNum2 - eis[i].EpNum;
						eis[i].EpNum = n;
						eis[i].EpNum2 = n+num;
						n += num+1;
					}
			}

			public static void GuessShowName(AddItem ai)
			{
				// see if we can guess a season number and show name, too
				// Assume is blah\blah\blah\show\season X
				string showName = "";

				string sp = ai.Folder;
				string seasonFinder = ".*season[ _\\.]+([0-9]+).*";
				if (Regex.Matches(sp,seasonFinder,RegexOptions.IgnoreCase).Count)
				{
					// String ^s = Regex::Replace(sp, seasonFinder, "$1",RegexOptions::IgnoreCase);
					try
					{
						// seasNum = Convert::ToInt32(s);
						// remove season folder from end of the path
						sp = Regex.Replace(sp, "(.*)\\\\"+seasonFinder,"$1",RegexOptions.IgnoreCase);
					}
					catch
					{
					}
				}
				// assume last folder element is the show name
				showName = sp.Substring(sp.LastIndexOf("\\")+1);

				ai.ShowName = showName;
			}

			public System.Collections.Generic.List<ProcessedEpisode > NextNShows(int nshows, int ndays)
			{
				DateTime notBefore = DateTime.Now;
				Generic.List<ProcessedEpisode > found = new System.Collections.Generic.List<ProcessedEpisode >();

				LockShowItems();
				for (int i =0;i<nshows;i++)
				{
					ProcessedEpisode nextAfterThat = null;
					TimeSpan howClose = null;
					foreach (ShowItem si in GetShowItems(false))
					{
						if (!si.ShowNextAirdate)
							continue;
						foreach (KeyValuePair<int, System.Collections.Generic.List<ProcessedEpisode > > v in si.SeasonEpisodes)
						{
							if (si.IgnoreSeasons.Contains(v.Key))
								continue; // ignore this season

							foreach (ProcessedEpisode ei in v.Value)
							{
								if (found.Contains(ei))
									continue;

								DateTime dt = ei.GetAirDateDT(true);
								if ((dt == null) || (dt == DateTime.MaxValue))
									continue;

								TimeSpan ts = (dt.Subtract(*notBefore));
								TimeSpan timeUntil = dt.Subtract(DateTime.Now);
								if (((howClose == null) || (ts.CompareTo(howClose) <= 0) && (ts.TotalHours >= 0)) && (ts.TotalHours >= 0) && (timeUntil.TotalDays <= ndays))
								{
									howClose = ts;
									nextAfterThat = ei;
								}
							}
						}
					}
					if (nextAfterThat == null)
						return found;
					notBefore = nextAfterThat.GetAirDateDT(true);
					found.Add(nextAfterThat);
				}
				UnlockShowItems();

				return found;
			}

			public static void WriteStringsToXml(System.Collections.Generic.List<string > strings, XmlWriter writer, string elementName, string stringName)
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

	public static void Rotate(string filenameBase)
			{
				if (File.Exists(filenameBase))
				{
					for (int i =8;i>=0;i--)
					{
						string fn = filenameBase + "." + i.ToString();
						if (File.Exists(fn))
						{
							string fn2 = filenameBase + "." + (i+1).ToString();
							if (File.Exists(fn2))
								File.Delete(fn2);
							File.Move(fn, fn2);
						}
					}

					File.Copy(filenameBase, filenameBase+".0");
				}
			}

			public void WriteXMLSettings()
			{
				// backup old settings before writing new ones

				Rotate(TVDocSettingsFile().FullName);

				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.NewLineOnAttributes = true;
				XmlWriter writer = XmlWriter.Create(TVDocSettingsFile().FullName, settings);

				writer.WriteStartDocument();
				writer.WriteStartElement("TVRename");
				writer.WriteStartAttribute("Version");
				writer.WriteValue("2.1");
				writer.WriteEndAttribute(); // version

				Settings.WriteXML(writer); // <Settings>

				writer.WriteStartElement("MyShows");
				foreach (ShowItem si in ShowItems)
					si.WriteXMLSettings(writer);
				writer.WriteEndElement(); // myshows

				WriteStringsToXml(MonitorFolders, writer, "MonitorFolders", "Folder");
				WriteStringsToXml(IgnoreFolders, writer, "IgnoreFolders", "Folder");
				WriteStringsToXml(SearchFolders, writer, "FinderSearchFolders", "Folder");

				writer.WriteStartElement("IgnoreItems");
				foreach (IgnoreItem ii in Ignore)
					ii.Write(writer);
				writer.WriteEndElement(); // IgnoreItems

				writer.WriteEndElement(); // tvrename
				writer.WriteEndDocument();
				writer.Close();
				writer = null;

				mDirty = false;

				Stats().Save();

			}

			public static System.Collections.Generic.List<string > ReadStringsFromXml(XmlReader reader, string elementName, string stringName)
			{
				Collections.Generic.List<string > r = new System.Collections.Generic.List<string >();

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

			public bool LoadXMLSettings(FileInfo from)
			{
				if (from == null)
					return true;

				try
				{
					XmlReaderSettings settings = new XmlReaderSettings();
					settings.IgnoreComments = true;
					settings.IgnoreWhitespace = true;


					if (!from.Exists)
					{
						//LoadErr = from->Name + " : File does not exist";
						//return false;
						return true; // that's ok
					}

					XmlReader reader = XmlReader.Create(from.FullName, settings);

					reader.Read();
					if (reader.Name != "xml")
					{
						LoadErr = from.Name + " : Not a valid XML file";
						return false;
					}


					reader.Read();

					if (reader.Name != "TVRename")
					{
						LoadErr = from.Name + " : Not a TVRename settings file";
						return false;
					}


					if (reader.GetAttribute("Version") != "2.1")
					{
						LoadErr = from.Name + " : Incompatible version";
						return false;
					}


					reader.Read(); // move forward one

					while (!reader.EOF)
					{
						if (reader.Name == "TVRename" && !reader.IsStartElement())
							break; // end of it all

						if (reader.Name == "Settings")
						{
							Settings = new TVSettings(reader.ReadSubtree());
							reader.Read();
						}
						else if (reader.Name == "MyShows")
						{
							XmlReader r2 = reader.ReadSubtree();
							r2.Read();
							r2.Read();
							while (!r2.EOF)
							{
								if ((r2.Name == "MyShows") && (!r2.IsStartElement()))
									break;
								if (r2.Name == "ShowItem")
								{
									ShowItem si = new ShowItem(mTVDB, r2.ReadSubtree(), this.Settings);

									if (si.UseCustomShowName) // see if custom show name is actually the real show name
									{
										SeriesInfo ser = si.TheSeries();
										if ((ser != null) && (si.CustomShowName == ser.Name))
										{
											// then, turn it off
											si.CustomShowName = "";
											si.UseCustomShowName = false;
										}
									}
									ShowItems.Add(si);

									r2.Read();
								}
								else
									r2.ReadOuterXml();
							}
							reader.Read();
						}
						else if (reader.Name == "MonitorFolders")
							MonitorFolders = ReadStringsFromXml(reader, "MonitorFolders", "Folder");
						else if (reader.Name == "IgnoreFolders")
							IgnoreFolders = ReadStringsFromXml(reader, "IgnoreFolders", "Folder");
						else if (reader.Name == "FinderSearchFolders")
							SearchFolders = ReadStringsFromXml(reader, "FinderSearchFolders", "Folder");
						else if (reader.Name == "IgnoreItems")
						{
							XmlReader r2 = reader.ReadSubtree();
							r2.Read();
							r2.Read();
							while (r2.Name == "Ignore")
								Ignore.Add(new IgnoreItem(r2));
							reader.Read();
						}
						else
							reader.ReadOuterXml();
					}

					reader.Close();
					reader = null;
				}
				catch (Exception e)
				{
					LoadErr = from.Name + " : " + e.Message;
					return false;
				}

				try
				{
					Stats().Load();
				}
				catch (Exception )
				{
					// not worried if stats loading fails
				}
				return true;
			}


			public static bool SysOpen(string what)
			{
				try
				{
					System.Diagnostics.Process.Start(what);
					return true;
				}
				catch
				{
					return false;
				}
			}
//            
//			void ExportMissingCSV(String ^filename)
//			{
//			TextWriter ^f = gcnew StreamWriter(filename);
//			String ^line;
//			line = "Show Name,Season,Episode,Episode Name,Air Date,Folder,Nice Name,thetvdb.com Code";
//			f->WriteLine(line);
//			for each (MissingEpisode ^me in MissingEpisodes)
//			{
//			DateTime ^dt = me->GetAirDateDT(true);
//			String ^line = "\"" + me->TheSeries->Name + "\"" + "," + 
//			me->SeasonNumber + "," + me->EpNum + 
//			((me->EpNum != me->EpNum2) ? "-"+me->EpNum2 : "") +
//			"," +
//			"\"" + me->Name + "\"" + "," +
//			((dt != nullptr) ? dt->ToString("G") : "") + "," +
//			"\"" + me->WhereItBelongs + "\"" + "," +
//			"\"" + FilenameFriendly(Settings->NamingStyle->NameFor(me)) + "\"" + "," +
//			me->SeriesID;
//            //(NStyle::Style)me->SI->NamingStyle
//
//			f->WriteLine(line);
//			}
//			f->Close();
//			}
//			
//            
//			void ExportRCXML(String ^name, String ^filename, RCList ^list)
//			{
//			XmlWriterSettings ^settings = gcnew XmlWriterSettings();
//			settings->Indent = true;
//			settings->NewLineOnAttributes = true;
//			XmlWriter ^writer = XmlWriter::Create(filename, settings);
//
//			writer->WriteStartDocument();
//			writer->WriteStartElement("TVRename");
//			writer->WriteStartAttribute("Version");
//			writer->WriteValue("2.1");
//			writer->WriteEndAttribute(); // version
//			writer->WriteStartElement(name);
//
//
//			for each (RCItem ^r in list)
//			{
//			writer->WriteStartElement("Item");
//
//			writer->WriteStartElement("Operation");
//			writer->WriteValue(r->GetOperationName());
//			writer->WriteEndElement();
//			writer->WriteStartElement("FromFolder");
//			writer->WriteValue(r->FromFolder);
//			writer->WriteEndElement();
//			writer->WriteStartElement("FromName");
//			writer->WriteValue(r->FromName);
//			writer->WriteEndElement();
//			writer->WriteStartElement("ToFolder");
//			writer->WriteValue(r->ToFolder);
//			writer->WriteEndElement();
//			writer->WriteStartElement("ToName");
//			writer->WriteValue(r->ToName);
//			writer->WriteEndElement();
//			writer->WriteStartElement("ShowName");
//			writer->WriteValue(r->ShowName);
//			writer->WriteEndElement();	
//			writer->WriteStartElement("Season");
//			if (r->TheEpisode != nullptr)
//			writer->WriteValue(r->TheEpisode->SeasonNumber);
//			writer->WriteEndElement();
//			writer->WriteStartElement("EpNum");
//			if (r->TheEpisode != nullptr)
//			writer->WriteValue(r->TheEpisode->EpNum);
//			writer->WriteEndElement();
//			writer->WriteStartElement("EpNum2");
//			if ((r->TheEpisode != nullptr) && (r->TheEpisode->EpNum != r->TheEpisode->EpNum2) )
//			writer->WriteValue(r->TheEpisode->EpNum2);
//			writer->WriteEndElement();
//
//			writer->WriteEndElement(); //Item
//			}
//
//			writer->WriteEndElement(); // "name"
//			writer->WriteEndElement(); // tvrename
//			writer->WriteEndDocument();
//			writer->Close();
//			}
//			void ExportRenamingXML(String ^filename)
//			{
//			ExportRCXML("Renaming", filename, RenameList);
//			}
//			void ExportFOXML(String ^filename)
//			{
//			ExportRCXML("FindingAndOrganising", filename, CopyMoveList);
//			}
//			void ExportMissingXML(String ^filename)
//			{
//			XmlWriterSettings ^settings = gcnew XmlWriterSettings();
//			settings->Indent = true;
//			settings->NewLineOnAttributes = true;
//			XmlWriter ^writer = XmlWriter::Create(filename, settings);
//
//			writer->WriteStartDocument();
//			writer->WriteStartElement("TVRename");
//			writer->WriteStartAttribute("Version");
//			writer->WriteValue("2.1");
//			writer->WriteEndAttribute(); // version
//			writer->WriteStartElement("MissingItems");
//
//			for each (MissingEpisode ^me in MissingEpisodes)
//			{
//			writer->WriteStartElement("MissingItem");
//			writer->WriteStartElement("ShowName");
//			writer->WriteValue(me->SI->ShowName());
//			writer->WriteEndElement();
//			writer->WriteStartElement("Season");
//			writer->WriteValue(me->Season);
//			writer->WriteEndElement();
//			writer->WriteStartElement("Episode");
//			String ^epl = me->EpNum.ToString();
//			if (me->EpNum != me->EpNum2)
//			epl += "-"+me->EpNum2.ToString();
//			writer->WriteValue(epl);
//			writer->WriteEndElement();
//			writer->WriteStartElement("EpisodeName");
//			writer->WriteValue(me->Name);
//			writer->WriteEndElement();
//			writer->WriteStartElement("AirDate");
//			DateTime ^dt = me->GetAirDateDT(true);
//			if (dt != nullptr)
//			writer->WriteValue(dt);
//			writer->WriteEndElement();
//			writer->WriteStartElement("Folder");
//			writer->WriteValue(me->WhereItBelongs);
//			writer->WriteEndElement();
//			writer->WriteStartElement("NiceName");
//			writer->WriteValue(FilenameFriendly(Settings->NamingStyle->NameFor(me)));
//			writer->WriteEndElement();
//			writer->WriteStartElement("thetvdbID");
//			writer->WriteValue( me->SI->TVDBCode);
//			writer->WriteEndElement();
//			writer->WriteEndElement(); // MissingItem
//			}
//
//			writer->WriteEndElement(); // MissingItems
//			writer->WriteEndElement(); // tvrename
//			writer->WriteEndDocument();
//			writer->Close();
//			}
//			
			public bool GenerateUpcomingXML(Stream str, System.Collections.Generic.List<ProcessedEpisode > elist)
			{
				if (elist == null)
					return false;

				try
				{
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;
					settings.NewLineOnAttributes = true;
					settings.Encoding = Encoding.ASCII;
					XmlWriter writer = XmlWriter.Create(str, settings);

					writer.WriteStartDocument();
					writer.WriteStartElement("rss");
					writer.WriteStartAttribute("version");
					writer.WriteValue("2.0");
					writer.WriteEndAttribute();
					writer.WriteStartElement("channel");
					writer.WriteStartElement("title");
					writer.WriteValue("Upcoming Shows");
					writer.WriteEndElement();
					writer.WriteStartElement("title");
					writer.WriteValue("http://tvrename.com");
					writer.WriteEndElement();
					writer.WriteStartElement("description");
					writer.WriteValue("Upcoming shows, exported by TVRename");
					writer.WriteEndElement();

					foreach (ProcessedEpisode ei in elist)
					{
						string niceName = Settings.NamingStyle.NameForExt(ei,null);

						writer.WriteStartElement("item");
						writer.WriteStartElement("title");
						writer.WriteValue(ei.HowLong()+" "+ei.DayOfWeek()+" "+ei.TimeOfDay()+" "+niceName);
						writer.WriteEndElement();
						writer.WriteStartElement("link");
						writer.WriteValue(GetTVDB(false, "").WebsiteURL(ei.TheSeries.TVDBCode, ei.SeasonID, false));
						writer.WriteEndElement();
						writer.WriteStartElement("description");
						writer.WriteValue(niceName + "<br/>" + ei.Overview);
						writer.WriteEndElement();
						writer.WriteStartElement("pubDate");
						DateTime DT = ei.GetAirDateDT(true);
						if (DT != null)
							writer.WriteValue(DT.ToString("r"));
						writer.WriteEndElement();
						writer.WriteEndElement(); // item
					}
					writer.WriteEndElement();
					writer.WriteEndElement();
					writer.WriteEndDocument();
					writer.Close();
					return true;
				} // try
				catch
				{
					return false;
				}
			}


			public void WriteUpcomingRSS()
			{
				if (!Settings.ExportWTWRSS)
					return;

				try
				{
					// dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
					// when windows restarts, the share isn't "back" before this timer times out and fires
					// windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
					// just me :P
					MemoryStream ms = new MemoryStream();
					if (GenerateUpcomingXML(ms, NextNShows(Settings.ExportRSSMaxShows, Settings.ExportRSSMaxDays)))
					{
						StreamWriter sw = new StreamWriter(Settings.ExportWTWRSSTo);
						sw.Write(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
						sw.Close();
					}
				}
				catch
				{
				}
			}

			// see if showname is somewhere in filename
			public bool SimplifyAndCheckFilename(string filename, string showname, bool simplifyfilename, bool simplifyshowname)
			{
				return Regex.Match(simplifyfilename ? SimplifyName(filename) : filename, "\\b"+(simplifyshowname ? SimplifyName(showname) : showname)+"\\b", RegexOptions.IgnoreCase).Success;
			}

			public void CheckAgainstuTorrent(SetProgressDelegate prog)
			{
				// get list of files being downloaded by uTorrent
				string resDatFile = Settings.ResumeDatPath;
				if (string.IsNullOrEmpty(resDatFile) || !File.Exists(resDatFile))
					return;

				BTResume btr = new BTResume(prog, resDatFile);
				if (!btr.LoadResumeDat())
					return;

				Generic.List<TorrentEntry > downloading = btr.AllFilesBeingDownloaded();

				Generic.List<AIOItem > newList = new System.Collections.Generic.List<AIOItem >();
				Generic.List<AIOItem > toRemove = new System.Collections.Generic.List<AIOItem >();
				int c = TheAIOList.Count + 2;
				int n = 1;
				prog.Invoke(100 *n/c);
				foreach (AIOItem aio1 in TheAIOList)
				{
					if (AIOCancel)
						return;

					n++;
					prog.Invoke(100 *n/c);

					if (aio1.Type != AIOType.kMissing)
						continue;

					AIOMissing aio = (AIOMissing)(aio1);

					string showname = SimplifyName(aio.PE.SI.ShowName());

					foreach (TorrentEntry te in downloading)
					{
						FileInfo file = new FileInfo(te.DownloadingTo);
						if (!Settings.UsefulExtension(file.Extension, false)) // not a usefile file extension
							continue;

						// String ^simplifiedfname = SimplifyName(file->FullName);

						if (SimplifyAndCheckFilename(file.FullName, showname, true, false)) // if (Regex::Match(simplifiedfname,"\\b"+showname+"\\b",RegexOptions::IgnoreCase)->Success)
						{
							int seasF;
							int epF;
							if (FindSeasEp(file, ref seasF, ref epF, aio.PE.TheSeries.Name) && (seasF == aio.PE.SeasonNumber) && (epF == aio.PE.EpNum))
							{
								toRemove.Add(aio1);
								newList.Add(new AIOuTorrenting(te, aio.PE, aio.TheFileNoExt));
								break;
							}
						}
					}
				}

				foreach (AIOItem i in toRemove)
					TheAIOList.Remove(i);

				foreach (AIOItem aio in newList)
					TheAIOList.Add(aio);

				prog.Invoke(100);
			}

			public void RSSSearch(SetProgressDelegate prog)
			{
				int c = TheAIOList.Count + 2;
				int n = 1;
				prog.Invoke(100 *n/c);
				RSSList = new RSSItemList();
				foreach (string s in Settings.RSSURLs)
					RSSList.DownloadRSS(s, Settings.FNPRegexs);

				Generic.List<AIOItem > newItems = new System.Collections.Generic.List<AIOItem >();
				Generic.List<AIOItem > toRemove = new System.Collections.Generic.List<AIOItem >();

				foreach (AIOItem aio1 in TheAIOList)
				{
					if (AIOCancel)
						return;

					n++;
					prog.Invoke(100 *n/c);

					if (aio1.Type != AIOType.kMissing)
						continue;

					AIOMissing aio = (AIOMissing)(aio1);

					ProcessedEpisode pe = aio.PE;
					string simpleShowName = SimplifyName(pe.SI.ShowName());
					string simpleSeriesName = SimplifyName(pe.TheSeries.Name);

					foreach (RSSItem rss in RSSList)
					{
						if ((SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) || (string.IsNullOrEmpty(rss.ShowName) && SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false))) && (rss.Season == pe.SeasonNumber) && (rss.Episode == pe.EpNum))
						{
							newItems.Add(new AIORSS(rss, aio.TheFileNoExt, pe));
							toRemove.Add(aio1);
						}
					}
				}
				foreach (AIOItem i in toRemove)
					TheAIOList.Remove(i);
				foreach (AIOItem aio in newItems)
					TheAIOList.Add(aio);

				prog.Invoke(100);
			}




			public void AIOFolderJPGCheck(SetProgressDelegate prog, ShowItem specific)
			{
				prog.Invoke(0);
				int c = 0;
				int totalN = 0;
				LockShowItems();

				if (specific == null)
				{
					foreach (ShowItem si in ShowItems)
						totalN += si.AllFolderLocations(Settings).Count;
				}
				else
					totalN = 1;

				Collections.Generic.List<string > doneFolders = new System.Collections.Generic.List<string >();

				foreach (ShowItem si in ShowItems)
				{
					if (AIOCancel)
					{
						UnlockShowItems();
						return;
					}

					if ((specific != null) && (si != specific))
						continue;

					prog.Invoke(100*(c+1) / (totalN+1)); // +1 to always have a bit of activity in the bar when we're working
					c += si.AllFolderLocations(Settings).Count;

					if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && si.AllFolderLocations(Settings, false).Count)
					{
						// folder base is set, and not all seasons ignored
						FileInfo fi = FileInFolder(si.AutoAdd_FolderBase, "folder.jpg");
						if (!fi.Exists)
							TheAIOList.Add(new AIODownload(si, null, fi, si.TheSeries().GetItem(Settings.ItemForFolderJpg())));
						doneFolders.Add(si.AutoAdd_FolderBase);
					}


					foreach (KeyValuePair<int, System.Collections.Generic.List<ProcessedEpisode > > kvp in si.SeasonEpisodes)
					{
						int snum = kvp.Key;
						if ((si.IgnoreSeasons.Contains(kvp.Key)) || (!si.AllFolderLocations(Settings).ContainsKey(snum)))
							continue; // ignore/skip this season
						Collections.Generic.List<string > folders = si.AllFolderLocations(Settings)[snum];
						foreach (string folder in folders)
						{
							if (doneFolders.Contains(folder)) // some folders may come up multiple times
								continue;
							doneFolders.Add(folder);

							FileInfo fi = FileInFolder(folder,"folder.jpg");
							if (!fi.Exists)
								TheAIOList.Add(new AIODownload(si, null, fi, si.TheSeries().GetItem(Settings.ItemForFolderJpg())));
						}
					}

				}
				UnlockShowItems();
				prog.Invoke(100);
			}

			public void AIOAction(SetProgressDelegate prog, System.Collections.Generic.List<AIOItem > theList)
			{
				// first pass to CopyMoveProgress
				// then, fire Action(TVDoc ^doc) on whatever is left (!Done)

				CopyMoveResult res;
				CopyMoveProgress cmp = new CopyMoveProgress(this, theList, res, Stats());
				cmp.ShowDialog();

				prog.Invoke(0);
				int c = 0;
				foreach (AIOItem item in theList)
				{
					prog.Invoke((100*(c+1)) / (theList.Count+1));
					if (!item.Done)
					{
						item.Action(this);
					}
				}

				theList.RemoveAll(new Predicate<AIOItem>(AIOItem.DoneOK));

				prog.Invoke(0);
			}

			public bool MissingItemsInList(System.Collections.Generic.List<AIOItem > l)
			{
				foreach (AIOItem i in l)
					if (i.Type == AIOType.kMissing)
						return true;
				return false;
			}

			public void AIOGo(SetProgressDelegate prog, ShowItem specific)
			{
				if (Settings.MissingCheck && !CheckAllFoldersExist(specific)) // only check for folders existing for missing check
					return;

				if (!DoDownloadsFG())
					return;

				Thread aioWork = new Thread(new ParameterizedThreadStart(this, TVDoc.AIOGoWorker));
				aioWork.Name = "AIOGo";

				AIOCancel = false;

				ScanProgDlg = new ScanProgress(Settings.RenameCheck, Settings.MissingCheck, Settings.MissingCheck && Settings.SearchLocally, Settings.MissingCheck && Settings.CheckuTorrent, Settings.MissingCheck && Settings.SearchRSS, Settings.FolderJpg);

				aioWork.Start(specific);

				if (ScanProgDlg.ShowDialog() ==DialogResult.Cancel)
				{
					AIOCancel = true;
					aioWork.Interrupt();
				}
				else
					aioWork.Join();

				ScanProgDlg = null;
			}

			public bool CheckAllFoldersExist(ShowItem specific)
			{
				// show MissingFolderAction for any folders that are missing
				// return false if user cancels

				LockShowItems();
				Generic.List<ShowItem > showlist;

				if (specific != null)
				{
					showlist = new System.Collections.Generic.List<ShowItem >();
					showlist.Add(specific);
				}
				else
					showlist = ShowItems;

				foreach (ShowItem si in showlist)
				{
					if (!si.DoMissingCheck && !si.DoRename)
						continue; // skip

					Generic.Dictionary<int, System.Collections.Generic.List<string > > flocs = si.AllFolderLocations(Settings);


					// TODO: this is a duplicate of code in UpToDateCheck
					int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
					si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
					foreach (int snum in numbers)
					{
//C++ TO C# CONVERTER NOTE: Embedded comments are not maintained by C++ to C# Converter
//ORIGINAL LINE: if (si->IgnoreSeasons->Contains(snum/*kvp->Key*/))
						if (si.IgnoreSeasons.Contains(snum))
							continue; // ignore this season

						//int snum = kvp->Key;
						if ((snum == 0) && (si.CountSpecials))
							continue; // no specials season, they're merged into the seasons themselves

						Collections.Generic.List<string > folders = new System.Collections.Generic.List<string >();

						if (flocs.ContainsKey(snum))
							folders = flocs[snum];

						if ((folders.Count == 0) && (!si.AutoAddNewSeasons))
							continue; // no folders defined or found, autoadd off, so onto the next

						if (folders.Count == 0)
						{
							// no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
							folders.Add(si.AutoFolderNameForSeason(snum, Settings));
						}

						foreach (string folder in folders)
						{

							// generate new filename info
							bool goAgain = false;
							bool nextSeas = false;
							DirectoryInfo di = null;
							do
							{
								goAgain = false;
								if (!string.IsNullOrEmpty(folder))
								{
									try
									{
										di = new DirectoryInfo(folder);
									}
									catch
									{
										return false;
									}
								}
								if ((di == null) || (!di.Exists))
								{
									string sn = si.ShowName();
									string text = snum.ToString() + " of " + si.MaxSeason().ToString();
									string theFolder = folder;
									string otherFolder = null;

									int whatToDo = (int)FAResult.kfaNotSet;

									if (HasArg("/createmissing"))
										whatToDo = (int)FAResult.kfaCreate;
									else if (HasArg("/ignoremissing"))
										whatToDo = (int)FAResult.kfaIgnoreOnce;


									if (HasArg("/hide") && (whatToDo == (int)FAResult.kfaNotSet))
										whatToDo = (int)FAResult.kfaIgnoreOnce; // default in /hide mode is to ignore

									if (whatToDo == (int)FAResult.kfaNotSet)
									{
										// no command line guidance, so ask the user
										// 									MissingFolderAction ^mfa = gcnew MissingFolderAction(sn, text, theFolder);
										// 									mfa->ShowDialog();
										// 									whatToDo = mfa->Result;
										// 									otherFolder = mfa->FolderName;


										MissingFolderAction mfa = new MissingFolderAction(sn, text, theFolder);
										mfa.ShowDialog();
										whatToDo = mfa.Result;
										otherFolder = mfa.FolderName;
									}

									if (whatToDo == (int)FAResult.kfaCancel)
									{
										UnlockShowItems();
										return false;
									}
									else if (whatToDo == (int)FAResult.kfaCreate)
									{
										Directory.CreateDirectory(folder);
										goAgain = true;
									}
									else if (whatToDo == (int)FAResult.kfaIgnoreAlways)
									{
										nextSeas = true;
										si.IgnoreSeasons.Add(snum);
										SetDirty();
										break;
									}
									else if (whatToDo == (int)FAResult.kfaIgnoreOnce)
									{
										nextSeas = true;
										break;
									}
									else if (whatToDo == (int)FAResult.kfaRetry)
										goAgain = true;
									else if (whatToDo == (int)FAResult.kfaDifferentFolder)
									{
										folder = otherFolder;
										di = new DirectoryInfo(folder);
										goAgain = !di.Exists;
										if (di.Exists && (si.AutoFolderNameForSeason(snum, Settings).ToLower() != folder.ToLower()))
										{
											if (!si.ManualFolderLocations.ContainsKey(snum))
												si.ManualFolderLocations[snum] = new System.Collections.Generic.List<string >();
											si.ManualFolderLocations[snum].Add(folder);
											SetDirty();
										}
									}
								}
							} while (goAgain);
						} // for each folder
					} // for each snum


				} // for each show
				UnlockShowItems();

				return true;
			} // CheckAllFoldersExist

			public void RemoveIgnored()
			{
				Generic.List<AIOItem > toRemove = new System.Collections.Generic.List<AIOItem >();
				foreach (AIOItem aio in TheAIOList)
				{
					foreach (IgnoreItem ii in Ignore)
					{
						if (ii == aio.GetIgnore())
						{
							toRemove.Add(aio);
							break;
						}
					}
				}
				foreach (AIOItem aio in toRemove)
					TheAIOList.Remove(aio);
			}

			public void AIOGoWorker(Object o)
			{
				ShowItem specific = (ShowItem)(o);

				while ((ScanProgDlg == null) || (!ScanProgDlg.Ready))
					Thread.Sleep(10); // wait for thread to create the dialog

				TheAIOList = new System.Collections.Generic.List<AIOItem >();

				if (Settings.RenameCheck)
				{
					DoRenameCheck(new SetProgressDelegate(ScanProgDlg, ScanProgress.RenameProg), specific);
					RemoveIgnored();
				}

				if (Settings.MissingCheck)
				{
					if (AIOCancel)
						return;

					DoMissingCheck(new SetProgressDelegate(ScanProgDlg, ScanProgress.MissingProg), specific);
					RemoveIgnored();

					if (AIOCancel)
						return;

					if (Settings.SearchLocally && MissingItemsInList(TheAIOList))
					{
						LookForMissingEps(new SetProgressDelegate(ScanProgDlg, ScanProgress.LocalSearchProg));
						RemoveIgnored();
					}

					if (AIOCancel)
						return;

					if (Settings.CheckuTorrent && MissingItemsInList(TheAIOList))
					{
						CheckAgainstuTorrent(new SetProgressDelegate(ScanProgDlg, ScanProgress.uTorrentProg));
						RemoveIgnored();
					}

					if (AIOCancel)
						return;

					if (Settings.SearchRSS && MissingItemsInList(TheAIOList))
					{
						RSSSearch(new SetProgressDelegate(ScanProgDlg, ScanProgress.RSSProg));
						RemoveIgnored();
					}
				}
				if (AIOCancel)
					return;
				if (Settings.FolderJpg)
					AIOFolderJPGCheck(new SetProgressDelegate(ScanProgDlg, ScanProgress.FolderThumbsProg), specific);
				// NFO and Episode images are done inside Missing Check

				// sort AIO list by type
				TheAIOList.Sort(new AIOSorter());

				ScanProgDlg.Done();
			}




//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//			static string SEFinderSimplifyFilename(string name, string showNameHint);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//			static bool MatchesSequentialNumber(string filename, ref int seas, ref int ep, ProcessedEpisode pe);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//			bool FindSeasEp(FileInfo fi, ref int seas, ref int ep, string showNameHint);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//			static bool FindSeasEp(FileInfo fi, ref int seas, ref int ep, string showNameHint, Generic::List<FilenameProcessorRE > rexps);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//			static bool FindSeasEp(string directory, string filename, ref int seas, ref int ep, string showNameHint, Generic::List<FilenameProcessorRE > rexps);
	} // class TVDoc
} // namespace