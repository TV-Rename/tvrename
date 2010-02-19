//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.IO;

namespace TVRename
{
	public enum CopyMoveResult: int
	{
		kCopyMoveOk,
		kUserCancelled,
		kFileError,
		kAlreadyExists
	}

	/// <summary>
	/// Summary for CopyMoveProgress
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class CopyMoveProgress : System.Windows.Forms.Form
	{
		public const int kArrayLength = 256 *1024;

        public CopyMoveResult Result;
		// String ^mErrorText;
		private System.Collections.Generic.List<AIOItem > mToDo;
		//RCList ^mSources, ^ErrorFiles;
		//MissingEpisodeList ^MissingList;
		private int mCurrentNum;
		private TVRenameStats mStats;
		private System.Threading.Thread mCopyThread;
		private TVDoc mDoc;


	private System.Windows.Forms.ProgressBar pbDiskSpace;
	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.Label txtDiskSpace;
	private System.Windows.Forms.Label txtTotal;
	private System.Windows.Forms.Label txtFile;
	private System.Windows.Forms.Timer copyTimer;
	private System.Windows.Forms.CheckBox cbPause;

	public delegate void CopyDoneHandler();
	public delegate void PercentHandler(int one, int two, int num);
	public delegate void FilenameHandler(string newName);

//C++ TO C# CONVERTER NOTE: Embedded comments are not maintained by C++ to C# Converter
//ORIGINAL LINE: void CopyDoneFunc(/*CopyMoveResult why, String ^errorText*/)
			public void CopyDoneFunc()
			{
				copyTimer.Stop();
				this.Close();
			}

	public event CopyDoneHandler CopyDone;
	public event PercentHandler Percent;
	public event FilenameHandler Filename;
//            
//			String ^ErrorText() 
//			{ 
//			String ^t = mErrorText;
//
//			if (t->Length > 0) // last char will be an extra \r we don't really want
//			t->Remove(mErrorText->Length-1);
//
//			return t;
//			}
//			
			//			RCList ^ErrFiles() { return ErrorFiles; }

		public CopyMoveProgress(TVDoc doc, System.Collections.Generic.List<AIOItem > todo, TVRenameStats stats)
		  {
			mDoc = doc;
			mToDo = todo;
			mStats = stats;
			  InitializeComponent();
			  copyTimer.Start();

			  mCurrentNum = -1;
			  //mLastPct = -1;
			  //			mErrorText = "";
			  //			ErrorFiles = gcnew RCList();

			  this.CopyDone += CopyDoneFunc;
			  this.Percent += new PercentHandler(SetPercentages);
			  this.Filename += new FilenameHandler(SetFilename);
		  }

		  public void SetFilename(string filename)
		  {
			  txtFilename.Text = filename;
		  }
		  public void SetPercentages(int file, int group, int currentNum)
		  {
			  mCurrentNum = currentNum;
			  //mPct = group;

			  if (file > 1000)
				  file = 1000;
			  if (group > 1000)
				  group = 1000;
			  if (file < 0)
				  file = 0;
			  if (group < 0)
				  group = 0;

			  txtFile.Text = (file/10).ToString() + "% Done";
			  txtTotal.Text = (group/10).ToString() + "% Done";

			  pbFile.Value = file;
			  pbGroup.Value = group;
			  pbFile.Update();
			  pbGroup.Update();
			  txtFile.Update();
			  txtTotal.Update();
			  Update();
		  }


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~CopyMoveProgress()
		{
			copyTimer.Stop();
		}
	private System.Windows.Forms.ProgressBar pbFile;
	private System.Windows.Forms.ProgressBar pbGroup;
	private System.Windows.Forms.Button button1;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.Label txtFilename;
	private System.ComponentModel.IContainer components;

		/// <summary>
		/// Required designer variable.
		/// </summary>



#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = (new System.ComponentModel.Container());
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(CopyMoveProgress)));
			this.pbFile = (new System.Windows.Forms.ProgressBar());
			this.pbGroup = (new System.Windows.Forms.ProgressBar());
			this.button1 = (new System.Windows.Forms.Button());
			this.label1 = (new System.Windows.Forms.Label());
			this.label2 = (new System.Windows.Forms.Label());
			this.label3 = (new System.Windows.Forms.Label());
			this.txtFilename = (new System.Windows.Forms.Label());
			this.pbDiskSpace = (new System.Windows.Forms.ProgressBar());
			this.label4 = (new System.Windows.Forms.Label());
			this.txtDiskSpace = (new System.Windows.Forms.Label());
			this.txtTotal = (new System.Windows.Forms.Label());
			this.txtFile = (new System.Windows.Forms.Label());
			this.copyTimer = (new System.Windows.Forms.Timer(this.components));
			this.cbPause = (new System.Windows.Forms.CheckBox());
			this.SuspendLayout();
			// 
			// pbFile
			// 
			this.pbFile.Location = new System.Drawing.Point(82, 34);
			this.pbFile.Maximum = 1000;
			this.pbFile.Name = "pbFile";
			this.pbFile.Size = new System.Drawing.Size(242, 23);
			this.pbFile.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbFile.TabIndex = 0;
			// 
			// pbGroup
			// 
			this.pbGroup.Location = new System.Drawing.Point(82, 63);
			this.pbGroup.Maximum = 1000;
			this.pbGroup.Name = "pbGroup";
			this.pbGroup.Size = new System.Drawing.Size(242, 23);
			this.pbGroup.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbGroup.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(312, 126);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(48, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "File:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(40, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Total:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(22, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Filename:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtFilename
			// 
			this.txtFilename.Location = new System.Drawing.Point(82, 12);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.Size = new System.Drawing.Size(304, 16);
			this.txtFilename.TabIndex = 2;
			this.txtFilename.UseMnemonic = false;
			// 
			// pbDiskSpace
			// 
			this.pbDiskSpace.Location = new System.Drawing.Point(82, 92);
			this.pbDiskSpace.Maximum = 1000;
			this.pbDiskSpace.Name = "pbDiskSpace";
			this.pbDiskSpace.Size = new System.Drawing.Size(243, 23);
			this.pbDiskSpace.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbDiskSpace.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 95);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Disk Space:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtDiskSpace
			// 
			this.txtDiskSpace.AutoSize = true;
			this.txtDiskSpace.Location = new System.Drawing.Point(331, 95);
			this.txtDiskSpace.Name = "txtDiskSpace";
			this.txtDiskSpace.Size = new System.Drawing.Size(55, 13);
			this.txtDiskSpace.TabIndex = 3;
			this.txtDiskSpace.Text = "--- GB free";
			this.txtDiskSpace.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtTotal
			// 
			this.txtTotal.AutoSize = true;
			this.txtTotal.Location = new System.Drawing.Point(331, 67);
			this.txtTotal.Name = "txtTotal";
			this.txtTotal.Size = new System.Drawing.Size(53, 13);
			this.txtTotal.TabIndex = 3;
			this.txtTotal.Text = "---% Done";
			this.txtTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtFile
			// 
			this.txtFile.AutoSize = true;
			this.txtFile.Location = new System.Drawing.Point(331, 38);
			this.txtFile.Name = "txtFile";
			this.txtFile.Size = new System.Drawing.Size(53, 13);
			this.txtFile.TabIndex = 3;
			this.txtFile.Text = "---% Done";
			this.txtFile.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// copyTimer
			// 
			this.copyTimer.Interval = 200;
			this.copyTimer.Tick += new System.EventHandler(copyTimer_Tick);
			// 
			// cbPause
			// 
			this.cbPause.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbPause.Location = new System.Drawing.Point(231, 126);
			this.cbPause.Name = "cbPause";
			this.cbPause.Size = new System.Drawing.Size(75, 23);
			this.cbPause.TabIndex = 4;
			this.cbPause.Text = "Pause";
			this.cbPause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cbPause.UseVisualStyleBackColor = true;
			this.cbPause.CheckedChanged += new System.EventHandler(cbPause_CheckedChanged);
			// 
			// CopyMoveProgress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(400, 162);
			this.Controls.Add(this.cbPause);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.txtTotal);
			this.Controls.Add(this.txtDiskSpace);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtFilename);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.pbDiskSpace);
			this.Controls.Add(this.pbGroup);
			this.Controls.Add(this.pbFile);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CopyMoveProgress";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Progress";
			this.Load += new System.EventHandler(CopyMoveProgress_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
#endregion
	private void copyTimer_Tick(object sender, System.EventArgs e)
			 {
				 if (mCurrentNum == -1)
				 {
					 pbDiskSpace.Value = 0;
					 txtDiskSpace.Text = "--- GB free";
				 }
				 else
				 {
					 bool ok = false;
					 AIOItem aio = mToDo[mCurrentNum];
					 DirectoryInfo toWhere = null;

					 if (aio.Type == AIOType.kCopyMoveRename)
						 toWhere = (AIOCopyMoveRename)(aio).To.Directory;
					 else if (aio.Type == AIOType.kDownload)
						 toWhere = (AIODownload)(aio).Destination.Directory;
					 else if (aio.Type == AIOType.kRSS)
						 toWhere = FileInfo((AIORSS)(aio).TheFileNoExt).Directory;
					 else if (aio.Type == AIOType.kNFO)
						 toWhere = (AIONFO)(aio).Where.Directory;

					 DirectoryInfo toRoot = null;
					 if (toWhere.Name.StartsWith("\\\\"))
						 toRoot = null;
					 else
						 toRoot = toWhere.Root;

					 if (toRoot != null)
					 {
						 System.IO.DriveInfo di;
						 try
						 {
							 // try to get root of drive
							 di = new System.IO.DriveInfo(toRoot.ToString());
						 }
						 catch (System.ArgumentException )
						 {
							 di = null;
						 }

						 if (di != null)
						 {
							 int pct = (int)((1000 *di.TotalFreeSpace) / di.TotalSize);
							 pbDiskSpace.Value = 1000-pct;
							 txtDiskSpace.Text = ((int)((double)di.TotalFreeSpace/1024.0/1024.0/1024.0 + 0.5)).ToString() + " GB free";
							 ok = true;
						 }
					 }

					 if (!ok)
					 {
						 txtDiskSpace.Text = "Unknown";
						 pbDiskSpace.Value = 0;
					 }

				 }

				 pbDiskSpace.Update();
				 txtDiskSpace.Update();
			 }

			 private static string TempFor(FileInfo f)
			 {
				 return f.FullName+".tvrenametemp";
			 }

			 private void CopyMachine()
			 {
				 Byte[]dataArray = new Byte[kArrayLength];
				 BinaryReader msr;
				 BinaryWriter msw;

				 long totalSize = 0;
				 long totalCopiedSoFar = 0;

				 totalSize = 0;
				 totalCopiedSoFar = 0;

				 int nfoCount = 0;
				 int downloadCount = 0;

				 for (int i =0;i<mToDo.Count;i++)
				 {
					 if (mToDo[i].Type == AIOType.kCopyMoveRename)
						 totalSize += (AIOCopyMoveRename)(mToDo[i]).FileSize();
					 else if (mToDo[i].Type == AIOType.kNFO)
						 nfoCount++;
					 else if (mToDo[i].Type == AIOType.kDownload)
						 downloadCount++;
					 else if (mToDo[i].Type == AIOType.kRSS)
						 downloadCount++;
				 }

				 int extrasCount = nfoCount+downloadCount;
				 long sizePerExtra = 1;
				 if ((extrasCount > 0) && (totalSize != 0))
				   sizePerExtra = totalSize/(10 *extrasCount);
				 if (sizePerExtra == 0)
					 sizePerExtra = 1;
				 totalSize += sizePerExtra * extrasCount;

				 int extrasDone = 0;

				 for (int i =0;i<mToDo.Count;i++)
				 {
					 while (cbPause.Checked)
						 System.Threading.Thread.Sleep(100);

					 AIOItem aio1 = mToDo[i];

					 if ((aio1.Type == AIOType.kRSS) || (aio1.Type == AIOType.kRSS) || (aio1.Type == AIOType.kDownload))
					 {
						 Object[]args = new Object[1];
						 args[0] = new string(aio1.FilenameForProgress());
						 this.BeginInvoke(SetFilename, args);

						 Object[]args2 = new Object[3];
						 args2[0] = new int((extrasCount != 0) ? (int)(1000 *extrasDone/extrasCount) : 0);
						 args2[1] = new int((totalSize != 0) ? (int)(1000 *totalCopiedSoFar/totalSize) : 50);
						 args2[2] = new int(i);

						 this.BeginInvoke(SetPercentages, args2);

						 aio1.Action(mDoc);
						 extrasDone++;
						 totalCopiedSoFar += sizePerExtra;
					 }
					 else if (aio1.Type == AIOType.kCopyMoveRename)
					 {
						 AIOCopyMoveRename aio = (AIOCopyMoveRename)(aio1);
						 aio.HasError = false;
						 aio.ErrorText = "";

						 Object[]args = new Object[1];
						 args[0] = new string(aio.FilenameForProgress());
						 this.BeginInvoke(SetFilename, args);

						 long thisFileSize;
						 thisFileSize = aio.FileSize();

						 System.Security.AccessControl.FileSecurity security = null;
						 try
						 {
							 security = aio.From.GetAccessControl();
						 }
						 catch
						 {
						 }

						 if (aio.IsMoveRename() && (aio.From.Directory.Root.FullName.ToLower() == aio.To.Directory.Root.FullName.ToLower())) // same device ... TODO: UNC paths?
						 {
							 // ask the OS to do it for us, since it's easy and quick!
							 try
							 {
								 if (Same(aio.From, aio.To))
								 {
									 // XP won't actually do a rename if its only a case difference
									 string tempName = TempFor(aio.To);
									 aio.From.MoveTo(tempName);
									 FileInfo(tempName).MoveTo(aio.To.FullName);
								 }
								 else
									 aio.From.MoveTo(aio.To.FullName);

								 aio.Done = true;

								 System.Diagnostics.Debug.Assert((aio.Operation == AIOCopyMoveRename.Op.Move) || (aio.Operation == AIOCopyMoveRename.Op.Rename));
								 if (aio.Operation == AIOCopyMoveRename.Op.Move)
									 mStats.FilesMoved++;
								 else if (aio.Operation == AIOCopyMoveRename.Op.Rename)
									 mStats.FilesRenamed++;
							 }
							 catch (System.Exception e)
							 {
								 aio.Done = true;
								 aio.HasError = true;
								 aio.ErrorText = e.Message;
								 totalCopiedSoFar += thisFileSize;
							 }
						 }
						 else
						 {
							 // do it ourself!
							 try
							 {
								 long thisFileCopied = 0;
								 msr = null;
								 msw = null;

								 msr = new BinaryReader(new FileStream(aio.From.FullName, FileMode.Open));
								 string tempName = TempFor(aio.To);
								 if (FileInfo(tempName).Exists)
									 FileInfo(tempName).Delete();

								 msw = new BinaryWriter(new FileStream(tempName, FileMode.CreateNew));

								 int n = 0;

								 do
								 {
									 n = msr.Read(dataArray, 0, kArrayLength);
									 if (n != 0)
										 msw.Write(dataArray, 0, n);
									 totalCopiedSoFar += n;
									 thisFileCopied += n;

									 Object[]args = new Object[3];
									 args[0] = new int((thisFileSize != 0) ? (int)(1000 *thisFileCopied/thisFileSize) : 50);
									 args[1] = new int((totalSize != 0) ? (int)(1000 *totalCopiedSoFar/totalSize) : 50);
									 args[2] = new int(i);

									 this.BeginInvoke(SetPercentages, args);

									 while (cbPause.Checked)
										 System.Threading.Thread.Sleep(100);
								 } while (n != 0);

								 msr.Close();
								 msw.Close();

								 // rename temp version to final name
								 if (aio.To.Exists)
									 aio.To.Delete(); // outta ma way!
								 FileInfo(tempName).MoveTo(aio.To.FullName);

								 // if that was a move/rename, delete the source
								 if (aio.IsMoveRename())
									 aio.From.Delete();

								 if (aio.Operation == AIOCopyMoveRename.Op.Move)
									 mStats.FilesMoved++;
								 else if (aio.Operation == AIOCopyMoveRename.Op.Rename)
									 mStats.FilesRenamed++;
								 else if (aio.Operation == AIOCopyMoveRename.Op.Copy)
									 mStats.FilesCopied++;

								 aio.Done = true;
							 } // try
							 catch (IOException e)
							 {
								 aio.Done = true;
								 aio.HasError = true;
								 aio.ErrorText = e.Message;

								 Result = CopyMoveResult.kAlreadyExists;
								 if (msw != null)
									 msw.Close();
								 if (msr != null)
									 msr.Close();

								 totalCopiedSoFar += thisFileSize;
							 }
							 catch (System.Threading.ThreadAbortException )
							 {
								 Result = CopyMoveResult.kUserCancelled;
								 //for (int k=i;k<mSources->Count;k++)
								 // ErrorFiles->Add(mSources[k]); // what was skipped

								 if (msw != null)
								 {
									 msw.Close();
									 string tempName = TempFor(aio.To);
									 if (File.Exists(tempName))
										 File.Delete(tempName);
								 }
								 if (msr != null)
									 msr.Close();
								 this.BeginInvoke(CopyDoneFunc, null);
								 return;
							 }
							 catch (System.Exception e)
							 {
								 Result = CopyMoveResult.kFileError;
								 aio.HasError = true;
								 aio.ErrorText = e.Message;
								 if (msw != null)
								 {
									 msw.Close();
									 if (FileInfo(TempFor(aio.To)).Exists)
										 FileInfo(TempFor(aio.To)).Delete();
								 }
								 if (msr != null)
									 msr.Close();

								 totalCopiedSoFar += thisFileSize;
							 }
						 } // do it ourself
						 try
						 {
							 if (security != null)
								 aio.To.SetAccessControl(security);
						 }
						 catch
						 {
						 }
					 } // if copymoverename

				 } // for each source

				 Result = CopyMoveResult.kCopyMoveOk;
				 this.BeginInvoke(CopyDoneFunc, null);
			 } // CopyMachine

	private void button1_Click(object sender, System.EventArgs e)
			 {
				 mCopyThread.Interrupt();
				 //CopyDone(false);

			 }
	private void CopyMoveProgress_Load(object sender, System.EventArgs e)
			 {

				 mCopyThread = new System.Threading.Thread(new System.Threading.ThreadStart(this, CopyMoveProgress.CopyMachine));
				 mCopyThread.Name = "Copy Thread";
				 mCopyThread.Start();
			 }

	private void cbPause_CheckedChanged(object sender, System.EventArgs e)
			 {
				 bool en = !(cbPause.Checked);
				 pbFile.Enabled = en;
				 pbGroup.Enabled = en;
				 pbDiskSpace.Enabled = en;
				 txtFile.Enabled = en;
				 txtTotal.Enabled = en;
				 txtDiskSpace.Enabled = en;
				 label1.Enabled = en;
				 label2.Enabled = en;
				 label4.Enabled = en;
				 label3.Enabled = en;
				 txtFilename.Enabled = en;
			 }
	} // class
} // namespace