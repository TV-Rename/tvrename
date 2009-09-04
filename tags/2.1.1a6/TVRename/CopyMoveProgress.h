#pragma once

#include "RenameItem.h"
#include "MissingEpisode.h"
#include "Statistics.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::IO;


namespace TVRename {
	enum CopyMoveResult { kCopyMoveOk, kUserCancelled, kFileError, kAlreadyExists };

	const int kArrayLength = 256*1024;

	/// <summary>
	/// Summary for CopyMoveProgress
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class CopyMoveProgress : public System::Windows::Forms::Form
	{
		BinaryReader ^msr;
		BinaryWriter ^msw;
		array<Byte>^DataArray;
		CopyMoveResult &Result;
		String ^mErrorText;
		RCList ^mSources, ^ErrorFiles;
		MissingEpisodeList ^MissingList;
		int mCurrentNum;
		TVRenameStats ^mStats;


	private: System::Windows::Forms::ProgressBar^  pbDiskSpace;

	private: System::Windows::Forms::Label^  label4;


	private: System::Windows::Forms::Label^  txtDiskSpace;
	private: System::Windows::Forms::Label^  txtTotal;
	private: System::Windows::Forms::Label^  txtFile;
	private: System::Windows::Forms::Timer^  copyTimer;
	private: System::Windows::Forms::CheckBox^  cbPause;


			 System::Threading::Thread ^mCopyThread;


	public: delegate void CopyDoneHandler();
	public: delegate void PercentHandler(int one, int two, int num);
	public: delegate void FilenameHandler(String ^newName);

			void CopyDoneFunc(/*CopyMoveResult why, String ^errorText*/)
			{
				copyTimer->Stop();
				this->Close();
			}

	public: event CopyDoneHandler ^CopyDone;
	public: event PercentHandler ^Percent;
	public: event FilenameHandler ^Filename;

			String ^ErrorText() 
			{ 
				String ^t = mErrorText;

				if (t->Length > 0) // last char will be an extra \r we don't really want
					t->Remove(mErrorText->Length-1);

				return t;
			}
			RCList ^ErrFiles() { return ErrorFiles; }

	public:
		CopyMoveProgress(RCList ^list, CopyMoveResult &res, MissingEpisodeList ^mel, TVRenameStats ^stats) :
		Result(res),
			mSources(list),
			MissingList(mel),
			mStats(stats)
		{
			InitializeComponent();
			copyTimer->Start();

			mCurrentNum = -1;
			//mLastPct = -1;
			mErrorText = "";
			ErrorFiles = gcnew RCList();

			DataArray = gcnew array<Byte>(kArrayLength);

			this->CopyDone += gcnew CopyDoneHandler(this, &CopyMoveProgress::CopyDoneFunc);
			this->Percent += gcnew PercentHandler(this, &CopyMoveProgress::SetPercentages);
			this->Filename += gcnew FilenameHandler(this, &CopyMoveProgress::SetFilename);
		}

		void SetFilename(String ^filename)
		{
			txtFilename->Text = filename;
		}
		void SetPercentages(int file, int group, int currentNum)
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

			txtFile->Text = (file/10).ToString() + "% Done";
			txtTotal->Text = (group/10).ToString() + "% Done";

			pbFile->Value = file;
			pbGroup->Value = group;
			pbFile->Update();
			pbGroup->Update();
			txtFile->Update();
			txtTotal->Update();
			Update();
		}


	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~CopyMoveProgress()
		{
			copyTimer->Stop();
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::ProgressBar^  pbFile;
	protected: 
	private: System::Windows::Forms::ProgressBar^  pbGroup;
	private: System::Windows::Forms::Button^  button1;
	private: System::Windows::Forms::Label^  label1;
	private: System::Windows::Forms::Label^  label2;
	private: System::Windows::Forms::Label^  label3;
	private: System::Windows::Forms::Label^  txtFilename;
	private: System::ComponentModel::IContainer^  components;

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>



#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(CopyMoveProgress::typeid));
			this->pbFile = (gcnew System::Windows::Forms::ProgressBar());
			this->pbGroup = (gcnew System::Windows::Forms::ProgressBar());
			this->button1 = (gcnew System::Windows::Forms::Button());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->txtFilename = (gcnew System::Windows::Forms::Label());
			this->pbDiskSpace = (gcnew System::Windows::Forms::ProgressBar());
			this->label4 = (gcnew System::Windows::Forms::Label());
			this->txtDiskSpace = (gcnew System::Windows::Forms::Label());
			this->txtTotal = (gcnew System::Windows::Forms::Label());
			this->txtFile = (gcnew System::Windows::Forms::Label());
			this->copyTimer = (gcnew System::Windows::Forms::Timer(this->components));
			this->cbPause = (gcnew System::Windows::Forms::CheckBox());
			this->SuspendLayout();
			// 
			// pbFile
			// 
			this->pbFile->Location = System::Drawing::Point(82, 34);
			this->pbFile->Maximum = 1000;
			this->pbFile->Name = L"pbFile";
			this->pbFile->Size = System::Drawing::Size(242, 23);
			this->pbFile->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbFile->TabIndex = 0;
			// 
			// pbGroup
			// 
			this->pbGroup->Location = System::Drawing::Point(82, 63);
			this->pbGroup->Maximum = 1000;
			this->pbGroup->Name = L"pbGroup";
			this->pbGroup->Size = System::Drawing::Size(242, 23);
			this->pbGroup->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbGroup->TabIndex = 0;
			// 
			// button1
			// 
			this->button1->Location = System::Drawing::Point(312, 126);
			this->button1->Name = L"button1";
			this->button1->Size = System::Drawing::Size(75, 23);
			this->button1->TabIndex = 1;
			this->button1->Text = L"Cancel";
			this->button1->UseVisualStyleBackColor = true;
			this->button1->Click += gcnew System::EventHandler(this, &CopyMoveProgress::button1_Click);
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(48, 38);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(26, 13);
			this->label1->TabIndex = 2;
			this->label1->Text = L"File:";
			this->label1->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(40, 67);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(34, 13);
			this->label2->TabIndex = 3;
			this->label2->Text = L"Total:";
			this->label2->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Location = System::Drawing::Point(22, 12);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(52, 13);
			this->label3->TabIndex = 2;
			this->label3->Text = L"Filename:";
			this->label3->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtFilename
			// 
			this->txtFilename->Location = System::Drawing::Point(82, 12);
			this->txtFilename->Name = L"txtFilename";
			this->txtFilename->Size = System::Drawing::Size(304, 16);
			this->txtFilename->TabIndex = 2;
			this->txtFilename->UseMnemonic = false;
			// 
			// pbDiskSpace
			// 
			this->pbDiskSpace->Location = System::Drawing::Point(82, 92);
			this->pbDiskSpace->Maximum = 1000;
			this->pbDiskSpace->Name = L"pbDiskSpace";
			this->pbDiskSpace->Size = System::Drawing::Size(243, 23);
			this->pbDiskSpace->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbDiskSpace->TabIndex = 0;
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Location = System::Drawing::Point(9, 95);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(65, 13);
			this->label4->TabIndex = 3;
			this->label4->Text = L"Disk Space:";
			this->label4->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtDiskSpace
			// 
			this->txtDiskSpace->AutoSize = true;
			this->txtDiskSpace->Location = System::Drawing::Point(331, 95);
			this->txtDiskSpace->Name = L"txtDiskSpace";
			this->txtDiskSpace->Size = System::Drawing::Size(55, 13);
			this->txtDiskSpace->TabIndex = 3;
			this->txtDiskSpace->Text = L"--- GB free";
			this->txtDiskSpace->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtTotal
			// 
			this->txtTotal->AutoSize = true;
			this->txtTotal->Location = System::Drawing::Point(331, 67);
			this->txtTotal->Name = L"txtTotal";
			this->txtTotal->Size = System::Drawing::Size(53, 13);
			this->txtTotal->TabIndex = 3;
			this->txtTotal->Text = L"---% Done";
			this->txtTotal->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtFile
			// 
			this->txtFile->AutoSize = true;
			this->txtFile->Location = System::Drawing::Point(331, 38);
			this->txtFile->Name = L"txtFile";
			this->txtFile->Size = System::Drawing::Size(53, 13);
			this->txtFile->TabIndex = 3;
			this->txtFile->Text = L"---% Done";
			this->txtFile->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// copyTimer
			// 
			this->copyTimer->Interval = 200;
			this->copyTimer->Tick += gcnew System::EventHandler(this, &CopyMoveProgress::copyTimer_Tick);
			// 
			// cbPause
			// 
			this->cbPause->Appearance = System::Windows::Forms::Appearance::Button;
			this->cbPause->Location = System::Drawing::Point(231, 126);
			this->cbPause->Name = L"cbPause";
			this->cbPause->Size = System::Drawing::Size(75, 23);
			this->cbPause->TabIndex = 4;
			this->cbPause->Text = L"Pause";
			this->cbPause->TextAlign = System::Drawing::ContentAlignment::MiddleCenter;
			this->cbPause->UseVisualStyleBackColor = true;
			this->cbPause->CheckedChanged += gcnew System::EventHandler(this, &CopyMoveProgress::cbPause_CheckedChanged);
			// 
			// CopyMoveProgress
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(400, 162);
			this->Controls->Add(this->cbPause);
			this->Controls->Add(this->txtFile);
			this->Controls->Add(this->txtTotal);
			this->Controls->Add(this->txtDiskSpace);
			this->Controls->Add(this->label4);
			this->Controls->Add(this->label2);
			this->Controls->Add(this->txtFilename);
			this->Controls->Add(this->label3);
			this->Controls->Add(this->label1);
			this->Controls->Add(this->button1);
			this->Controls->Add(this->pbDiskSpace);
			this->Controls->Add(this->pbGroup);
			this->Controls->Add(this->pbFile);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"CopyMoveProgress";
			this->ShowInTaskbar = false;
			this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Hide;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"File Operation Progress";
			this->Load += gcnew System::EventHandler(this, &CopyMoveProgress::CopyMoveProgress_Load);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
	private: System::Void copyTimer_Tick(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (mCurrentNum == -1)
				 {
					 pbDiskSpace->Value = 0;
					 txtDiskSpace->Text = "--- GB free";
				 }
				 else
				 {
					 RCItem  ^rci = mSources[mCurrentNum];
					 String ^toFolder = rci->ToFolder;
					 if (Regex::Match(toFolder, "^[A-Za-z]:\\\\")->Success)
					 {
						 System::IO::DriveInfo ^di = gcnew System::IO::DriveInfo(DirectoryInfo(toFolder).Root->ToString());
						 int pct = (int)((1000*di->TotalFreeSpace) / di->TotalSize);
						 pbDiskSpace->Value = 1000-pct;
						 txtDiskSpace->Text = ((int)((double)di->TotalFreeSpace/1024.0/1024.0/1024.0 + 0.5)).ToString() + " GB free";
					 }
					 else
					 {
						 txtDiskSpace->Text = "Unknown";
						 pbDiskSpace->Value = 0;
					 }

				 }

				 pbDiskSpace->Update();
				 txtDiskSpace->Update();
			 }

			 bool ContinueAfterError()
			 {
				 return true;
			 }

			 void RemoveFromML(RCItem ^rci)
			 {
				 if ((MissingList == nullptr) || (rci->TheEpisode == nullptr))
					 return;

				 for each (MissingEpisode ^me in MissingList)
				 {
					 if (me == rci->TheEpisode)
					 {
						 MissingList->Remove(me);
						 break;
					 }
				 }

			 }
			 void CopyMachine()
			 {

				 long long totalSize = 0;
				 long long totalCopiedSoFar = 0;


				 totalSize = 0;
				 totalCopiedSoFar = 0;

				 for (int j=0;j<mSources->Count;j++)
					 //if (mSources[j]->Enabled())
						 totalSize += mSources[j]->FileSize();

				 for (int i=0;i<mSources->Count;i++)
				 {
					 while (cbPause->Checked)
						 System::Threading::Thread::Sleep(100);

					 RCItem  ^rci = mSources[i];
					 //if (!rci->Enabled())
						// continue;

					 // txtFilename->Text = rci->GetToName();
					 array<Object^>^args = gcnew array<Object^>(1);
					 args[0] = gcnew String(rci->ToName);

					 this->BeginInvoke(gcnew FilenameHandler(this, &CopyMoveProgress::SetFilename), args);

					 long long thisFileSize;
					 thisFileSize = rci->FileSize();

                                         System::Security::AccessControl::FileSecurity ^security = nullptr;
                                         try 
                                         {
                                           security = FileInfo(rci->FullFromName()).GetAccessControl();
                                         }
                                         catch (...)
                                         {
                                         }

					 if ( ((rci->Operation == rcMove)||(rci->Operation == rcRename)) && // move or rename
						 (rci->FullFromName()->Substring(0,3)->ToLower() == rci->FullToName()->Substring(0,3)->ToLower()) ) // same device
					 {
						 // ask the OS to do it for us, since its easy and quick!
						 try {
							 if (rci->FullFromName()->ToLower() == rci->FullToName()->ToLower())
							 {
								 // XP won't actually do a rename if its only a case difference
								 String ^tempName = rci->FullToName()+".tvrenametemp";
								 File::Move(rci->FullFromName(),tempName);
								 File::Move(tempName, rci->FullToName());
							 }
							 else
								 File::Move(rci->FullFromName(),rci->FullToName());

							 RemoveFromML(rci);
							 if (rci->Operation == rcMove)  mStats->FilesMoved++;
							 if (rci->Operation == rcRename)  mStats->FilesRenamed++;
						 }
						 catch (System::Exception ^e)
						 {
							 mErrorText += e->Message+"\r";
							 rci->LastError = e->Message;
							 ErrorFiles->Add(rci);
							 if (!ContinueAfterError())
							 {
								 for (int k=i+1;k<mSources->Count;k++)
									 ErrorFiles->Add(mSources[k]); // also add what was skipped

								 this->BeginInvoke(gcnew CopyDoneHandler(this, &CopyMoveProgress::CopyDoneFunc), nullptr);
								 return;
							 }
							 totalCopiedSoFar += thisFileSize;
						 }
					 }
					 else
					 {
						 // do it ourself!
						 try {
							 long long thisFileCopied = 0;
							 msr = nullptr;
							 msw = nullptr;

							 String ^from = rci->FullFromName();
							 String ^to = rci->FullToName();

							 msr = gcnew BinaryReader(gcnew FileStream(from, FileMode::Open));
							 String ^tempName = to+".tvrenametemp";
							 if (File::Exists(tempName))
								 File::Delete(tempName);
							 msw = gcnew BinaryWriter(gcnew FileStream(tempName, FileMode::CreateNew));

							 int n = 0;

							 do {
								 n = msr->Read(DataArray, 0, kArrayLength);
								 if (n)
								 {
									 msw->Write(DataArray, 0, n);
								 }
								 totalCopiedSoFar += n;
								 thisFileCopied += n;

								 array<Object^>^args = gcnew array<Object^>(3);
								 args[0] = gcnew int((thisFileSize != 0) ? (int)(1000*thisFileCopied/thisFileSize) : 50);
								 args[1] = gcnew int((totalSize != 0) ? (int)(1000*totalCopiedSoFar/totalSize) : 50);
								 args[2] = gcnew int(i);

								 this->BeginInvoke(gcnew PercentHandler(this, &CopyMoveProgress::SetPercentages), args);

								 while (cbPause->Checked)
									 System::Threading::Thread::Sleep(100);
							 } while (n != 0);

							 msr->Close();
							 msw->Close();

							 // rename temp version to final name
							 if (FileInfo(to).Exists)
								 FileInfo(to).Delete(); // outta ma way!
							 FileInfo(tempName).MoveTo(to);

							 // if that was a move/rename, delete the source
							 if ((rci->Operation == rcMove)||(rci->Operation == rcRename))
								 File::Delete(rci->FullFromName());

							 if (rci->Operation == rcMove)  mStats->FilesMoved++;
							 if (rci->Operation == rcRename)  mStats->FilesRenamed++;
							 if (rci->Operation == rcCopy)  mStats->FilesCopied++;

							 RemoveFromML(rci);
						 } // try
						 catch (IOException ^e)
						 {
							 mErrorText += e->Message+"\r";
                                                         rci->LastError = e->Message;

							 Result = kAlreadyExists;
							 if (msw != nullptr)
								 msw->Close();
							 if (msr != nullptr)
								 msr->Close();
                                                       
                                                         ErrorFiles->Add(rci); 

                                                         if (!ContinueAfterError())
							 {
								 for (int k=i+1;k<mSources->Count;k++)
                                                                     ErrorFiles->Add(mSources[k]); // what was skipped
								 this->BeginInvoke(gcnew CopyDoneHandler(this, &CopyMoveProgress::CopyDoneFunc), nullptr);
								 return;
							 }

							 totalCopiedSoFar += thisFileSize;
						 }
						 catch (System::Threading::ThreadAbortException ^)
						 {
							 Result = kUserCancelled;
							 for (int k=i;k<mSources->Count;k++)
								 ErrorFiles->Add(mSources[k]); // what was skipped

							 if (msw != nullptr)
							 {
								 msw->Close();
								 if (File::Exists(rci->FullToName()+".tvrenametemp"))
									 File::Delete(rci->FullToName()+".tvrenametemp");
							 }
							 if (msr != nullptr)
								 msr->Close();
							 this->BeginInvoke(gcnew CopyDoneHandler(this, &CopyMoveProgress::CopyDoneFunc), nullptr);
							 return;
						 }
						 catch (System::Exception ^e)
						 {
							 mErrorText += "Error on file " + rci->FullToName()+"\r";

							 Result = kFileError;
                                                         rci->LastError = e->Message;
							 if (msw != nullptr)
							 {
								 msw->Close();
								 if (File::Exists(rci->FullToName()+".tvrenametemp"))
									 File::Delete(rci->FullToName()+".tvrenametemp");
							 }
							 if (msr != nullptr)
								 msr->Close();

                                                         ErrorFiles->Add(rci); 

							 if (!ContinueAfterError())
							 {
								 for (int k=i;k<mSources->Count;k++)
									 ErrorFiles->Add(mSources[k]); // what was skipped
								 this->BeginInvoke(gcnew CopyDoneHandler(this, &CopyMoveProgress::CopyDoneFunc), nullptr);
								 return;
							 }
							 totalCopiedSoFar += thisFileSize;

						 }
					 } // do it ourself
                                         try 
                                         {
                                            if (security != nullptr)
                                                FileInfo(rci->FullToName()).SetAccessControl(security);
                                         }
                                         catch (...)
                                         {
                                         }
				 }// for each source

				 Result = kCopyMoveOk;
				 this->BeginInvoke(gcnew CopyDoneHandler(this, &CopyMoveProgress::CopyDoneFunc), nullptr);
			 } // CopyMachine

	private: System::Void button1_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 mCopyThread->Abort();
				 //CopyDone(false);

			 }
	private: System::Void CopyMoveProgress_Load(System::Object^  sender, System::EventArgs^  e) 
			 {

				 mCopyThread = gcnew System::Threading::Thread(gcnew System::Threading::ThreadStart(this, &CopyMoveProgress::CopyMachine));
                                 mCopyThread->Name = "Copy Thread";
				 mCopyThread->Start();
			 }

	private: System::Void cbPause_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 bool en = !(cbPause->Checked);
				 pbFile->Enabled = en;
				 pbGroup->Enabled = en;
				 pbDiskSpace->Enabled = en;
				 txtFile->Enabled = en;
				 txtTotal->Enabled = en;
				 txtDiskSpace->Enabled = en;
				 label1->Enabled = en;
				 label2->Enabled = en;
				 label4->Enabled = en;
				 label3->Enabled = en;
				 txtFilename->Enabled = en;
			 }
	}; // class
} // namespace
