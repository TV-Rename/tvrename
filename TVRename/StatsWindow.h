#pragma once

#include "Statistics.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

	/// <summary>
	/// Summary for StatsWindow
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class StatsWindow : public System::Windows::Forms::Form
	{
		TVRenameStats ^Stats;

	public:
		StatsWindow(TVRenameStats ^s)
		{
			Stats = s;
			InitializeComponent();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~StatsWindow()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::Label^  label1;
	private: System::Windows::Forms::Label^  label2;
	private: System::Windows::Forms::Label^  label3;
	private: System::Windows::Forms::Label^  label4;
	private: System::Windows::Forms::Label^  label5;
	private: System::Windows::Forms::Label^  label6;
	private: System::Windows::Forms::Label^  label7;
	private: System::Windows::Forms::Label^  txtFM;
	private: System::Windows::Forms::Label^  txtFC;
	private: System::Windows::Forms::Label^  txtRCD;
	private: System::Windows::Forms::Label^  txtMCD;
	private: System::Windows::Forms::Label^  txtFAOD;
        private: System::Windows::Forms::Label^  txtAAS;

	private: System::Windows::Forms::Label^  txtTM;
	private: System::Windows::Forms::Button^  button1;
	private: System::Windows::Forms::Label^  label8;
	private: System::Windows::Forms::Label^  txtFR;
	private: System::Windows::Forms::Label^  label9;
	private: System::Windows::Forms::Label^  txtNOS;
	private: System::Windows::Forms::Label^  label11;
        private: System::Windows::Forms::Label^  txtNOSeas;

	private: System::Windows::Forms::Label^  label13;
	private: System::Windows::Forms::Label^  txtEOD;
	private: System::Windows::Forms::Label^  label10;
	private: System::Windows::Forms::Label^  txtTE;


	protected: 

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
                    System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(StatsWindow::typeid));
                    this->label1 = (gcnew System::Windows::Forms::Label());
                    this->label2 = (gcnew System::Windows::Forms::Label());
                    this->label3 = (gcnew System::Windows::Forms::Label());
                    this->label4 = (gcnew System::Windows::Forms::Label());
                    this->label5 = (gcnew System::Windows::Forms::Label());
                    this->label6 = (gcnew System::Windows::Forms::Label());
                    this->label7 = (gcnew System::Windows::Forms::Label());
                    this->txtFM = (gcnew System::Windows::Forms::Label());
                    this->txtFC = (gcnew System::Windows::Forms::Label());
                    this->txtRCD = (gcnew System::Windows::Forms::Label());
                    this->txtMCD = (gcnew System::Windows::Forms::Label());
                    this->txtFAOD = (gcnew System::Windows::Forms::Label());
                    this->txtAAS = (gcnew System::Windows::Forms::Label());
                    this->txtTM = (gcnew System::Windows::Forms::Label());
                    this->button1 = (gcnew System::Windows::Forms::Button());
                    this->label8 = (gcnew System::Windows::Forms::Label());
                    this->txtFR = (gcnew System::Windows::Forms::Label());
                    this->label9 = (gcnew System::Windows::Forms::Label());
                    this->txtNOS = (gcnew System::Windows::Forms::Label());
                    this->label11 = (gcnew System::Windows::Forms::Label());
                    this->txtNOSeas = (gcnew System::Windows::Forms::Label());
                    this->label13 = (gcnew System::Windows::Forms::Label());
                    this->txtEOD = (gcnew System::Windows::Forms::Label());
                    this->label10 = (gcnew System::Windows::Forms::Label());
                    this->txtTE = (gcnew System::Windows::Forms::Label());
                    this->SuspendLayout();
                    // 
                    // label1
                    // 
                    this->label1->AutoSize = true;
                    this->label1->Location = System::Drawing::Point(12, 9);
                    this->label1->Name = L"label1";
                    this->label1->Size = System::Drawing::Size(66, 13);
                    this->label1->TabIndex = 0;
                    this->label1->Text = L"Files moved:";
                    // 
                    // label2
                    // 
                    this->label2->AutoSize = true;
                    this->label2->Location = System::Drawing::Point(12, 55);
                    this->label2->Name = L"label2";
                    this->label2->Size = System::Drawing::Size(66, 13);
                    this->label2->TabIndex = 0;
                    this->label2->Text = L"Files copied:";
                    // 
                    // label3
                    // 
                    this->label3->AutoSize = true;
                    this->label3->Location = System::Drawing::Point(12, 78);
                    this->label3->Name = L"label3";
                    this->label3->Size = System::Drawing::Size(115, 13);
                    this->label3->TabIndex = 0;
                    this->label3->Text = L"Rename checks done:";
                    // 
                    // label4
                    // 
                    this->label4->AutoSize = true;
                    this->label4->Location = System::Drawing::Point(12, 101);
                    this->label4->Name = L"label4";
                    this->label4->Size = System::Drawing::Size(110, 13);
                    this->label4->TabIndex = 0;
                    this->label4->Text = L"Missing checks done:";
                    // 
                    // label5
                    // 
                    this->label5->AutoSize = true;
                    this->label5->Location = System::Drawing::Point(12, 124);
                    this->label5->Name = L"label5";
                    this->label5->Size = System::Drawing::Size(126, 13);
                    this->label5->TabIndex = 0;
                    this->label5->Text = L"Find and organises done:";
                    // 
                    // label6
                    // 
                    this->label6->AutoSize = true;
                    this->label6->Location = System::Drawing::Point(12, 147);
                    this->label6->Name = L"label6";
                    this->label6->Size = System::Drawing::Size(98, 13);
                    this->label6->TabIndex = 0;
                    this->label6->Text = L"Auto added shows:";
                    // 
                    // label7
                    // 
                    this->label7->AutoSize = true;
                    this->label7->Location = System::Drawing::Point(12, 170);
                    this->label7->Name = L"label7";
                    this->label7->Size = System::Drawing::Size(93, 13);
                    this->label7->TabIndex = 0;
                    this->label7->Text = L"Torrents matched:";
                    // 
                    // txtFM
                    // 
                    this->txtFM->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtFM->Location = System::Drawing::Point(138, 9);
                    this->txtFM->Name = L"txtFM";
                    this->txtFM->Size = System::Drawing::Size(46, 13);
                    this->txtFM->TabIndex = 0;
                    this->txtFM->Text = L"---";
                    this->txtFM->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // txtFC
                    // 
                    this->txtFC->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtFC->Location = System::Drawing::Point(138, 55);
                    this->txtFC->Name = L"txtFC";
                    this->txtFC->Size = System::Drawing::Size(46, 13);
                    this->txtFC->TabIndex = 0;
                    this->txtFC->Text = L"---";
                    this->txtFC->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // txtRCD
                    // 
                    this->txtRCD->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtRCD->Location = System::Drawing::Point(138, 78);
                    this->txtRCD->Name = L"txtRCD";
                    this->txtRCD->Size = System::Drawing::Size(46, 13);
                    this->txtRCD->TabIndex = 0;
                    this->txtRCD->Text = L"---";
                    this->txtRCD->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // txtMCD
                    // 
                    this->txtMCD->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtMCD->Location = System::Drawing::Point(138, 101);
                    this->txtMCD->Name = L"txtMCD";
                    this->txtMCD->Size = System::Drawing::Size(46, 13);
                    this->txtMCD->TabIndex = 0;
                    this->txtMCD->Text = L"---";
                    this->txtMCD->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // txtFAOD
                    // 
                    this->txtFAOD->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtFAOD->Location = System::Drawing::Point(138, 124);
                    this->txtFAOD->Name = L"txtFAOD";
                    this->txtFAOD->Size = System::Drawing::Size(46, 13);
                    this->txtFAOD->TabIndex = 0;
                    this->txtFAOD->Text = L"---";
                    this->txtFAOD->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // txtAAS
                    // 
                    this->txtAAS->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtAAS->Location = System::Drawing::Point(138, 147);
                    this->txtAAS->Name = L"txtAAS";
                    this->txtAAS->Size = System::Drawing::Size(46, 13);
                    this->txtAAS->TabIndex = 0;
                    this->txtAAS->Text = L"---";
                    this->txtAAS->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // txtTM
                    // 
                    this->txtTM->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtTM->Location = System::Drawing::Point(138, 170);
                    this->txtTM->Name = L"txtTM";
                    this->txtTM->Size = System::Drawing::Size(46, 13);
                    this->txtTM->TabIndex = 0;
                    this->txtTM->Text = L"---";
                    this->txtTM->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // button1
                    // 
                    this->button1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
                    this->button1->DialogResult = System::Windows::Forms::DialogResult::Cancel;
                    this->button1->Location = System::Drawing::Point(114, 284);
                    this->button1->Name = L"button1";
                    this->button1->Size = System::Drawing::Size(75, 23);
                    this->button1->TabIndex = 1;
                    this->button1->Text = L"Close";
                    this->button1->UseVisualStyleBackColor = true;
                    this->button1->Click += gcnew System::EventHandler(this, &StatsWindow::button1_Click);
                    // 
                    // label8
                    // 
                    this->label8->AutoSize = true;
                    this->label8->Location = System::Drawing::Point(12, 32);
                    this->label8->Name = L"label8";
                    this->label8->Size = System::Drawing::Size(75, 13);
                    this->label8->TabIndex = 0;
                    this->label8->Text = L"Files renamed:";
                    // 
                    // txtFR
                    // 
                    this->txtFR->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtFR->Location = System::Drawing::Point(138, 32);
                    this->txtFR->Name = L"txtFR";
                    this->txtFR->Size = System::Drawing::Size(46, 13);
                    this->txtFR->TabIndex = 0;
                    this->txtFR->Text = L"---";
                    this->txtFR->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // label9
                    // 
                    this->label9->AutoSize = true;
                    this->label9->Location = System::Drawing::Point(12, 193);
                    this->label9->Name = L"label9";
                    this->label9->Size = System::Drawing::Size(92, 13);
                    this->label9->TabIndex = 0;
                    this->label9->Text = L"Number of shows:";
                    // 
                    // txtNOS
                    // 
                    this->txtNOS->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtNOS->Location = System::Drawing::Point(138, 193);
                    this->txtNOS->Name = L"txtNOS";
                    this->txtNOS->Size = System::Drawing::Size(46, 13);
                    this->txtNOS->TabIndex = 0;
                    this->txtNOS->Text = L"---";
                    this->txtNOS->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // label11
                    // 
                    this->label11->AutoSize = true;
                    this->label11->Location = System::Drawing::Point(12, 216);
                    this->label11->Name = L"label11";
                    this->label11->Size = System::Drawing::Size(101, 13);
                    this->label11->TabIndex = 0;
                    this->label11->Text = L"Number of seasons:";
                    // 
                    // txtNOSeas
                    // 
                    this->txtNOSeas->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtNOSeas->Location = System::Drawing::Point(138, 216);
                    this->txtNOSeas->Name = L"txtNOSeas";
                    this->txtNOSeas->Size = System::Drawing::Size(46, 13);
                    this->txtNOSeas->TabIndex = 0;
                    this->txtNOSeas->Text = L"---";
                    this->txtNOSeas->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // label13
                    // 
                    this->label13->AutoSize = true;
                    this->label13->Location = System::Drawing::Point(12, 239);
                    this->label13->Name = L"label13";
                    this->label13->Size = System::Drawing::Size(90, 13);
                    this->label13->TabIndex = 0;
                    this->label13->Text = L"Episodes on disk:";
                    // 
                    // txtEOD
                    // 
                    this->txtEOD->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtEOD->Location = System::Drawing::Point(138, 239);
                    this->txtEOD->Name = L"txtEOD";
                    this->txtEOD->Size = System::Drawing::Size(46, 13);
                    this->txtEOD->TabIndex = 0;
                    this->txtEOD->Text = L"---";
                    this->txtEOD->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // label10
                    // 
                    this->label10->AutoSize = true;
                    this->label10->Location = System::Drawing::Point(12, 262);
                    this->label10->Name = L"label10";
                    this->label10->Size = System::Drawing::Size(79, 13);
                    this->label10->TabIndex = 0;
                    this->label10->Text = L"Total episodes:";
                    // 
                    // txtTE
                    // 
                    this->txtTE->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->txtTE->Location = System::Drawing::Point(138, 262);
                    this->txtTE->Name = L"txtTE";
                    this->txtTE->Size = System::Drawing::Size(46, 13);
                    this->txtTE->TabIndex = 0;
                    this->txtTE->Text = L"---";
                    this->txtTE->TextAlign = System::Drawing::ContentAlignment::TopRight;
                    // 
                    // StatsWindow
                    // 
                    this->AcceptButton = this->button1;
                    this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
                    this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
                    this->CancelButton = this->button1;
                    this->ClientSize = System::Drawing::Size(207, 319);
                    this->Controls->Add(this->button1);
                    this->Controls->Add(this->txtTE);
                    this->Controls->Add(this->txtEOD);
                    this->Controls->Add(this->txtTM);
                    this->Controls->Add(this->label10);
                    this->Controls->Add(this->label13);
                    this->Controls->Add(this->label7);
                    this->Controls->Add(this->txtNOSeas);
                    this->Controls->Add(this->txtAAS);
                    this->Controls->Add(this->label11);
                    this->Controls->Add(this->label6);
                    this->Controls->Add(this->txtNOS);
                    this->Controls->Add(this->txtFAOD);
                    this->Controls->Add(this->label9);
                    this->Controls->Add(this->label5);
                    this->Controls->Add(this->txtMCD);
                    this->Controls->Add(this->label4);
                    this->Controls->Add(this->txtRCD);
                    this->Controls->Add(this->label3);
                    this->Controls->Add(this->txtFR);
                    this->Controls->Add(this->label8);
                    this->Controls->Add(this->txtFC);
                    this->Controls->Add(this->label2);
                    this->Controls->Add(this->txtFM);
                    this->Controls->Add(this->label1);
                    this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
                    this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
                    this->Name = L"StatsWindow";
                    this->ShowInTaskbar = false;
                    this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Hide;
                    this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
                    this->Text = L"Statistics";
                    this->Load += gcnew System::EventHandler(this, &StatsWindow::StatsWindow_Load);
                    this->ResumeLayout(false);
                    this->PerformLayout();

                }
#pragma endregion
	private: System::Void StatsWindow_Load(System::Object^  sender, System::EventArgs^  e) 
			 {
				 txtFM->Text = Stats->FilesMoved.ToString();
				 txtFR->Text = Stats->FilesRenamed.ToString();
				 txtFC->Text = Stats->FilesCopied.ToString();
				 txtRCD->Text = Stats->RenameChecksDone.ToString();
				 txtMCD->Text = Stats->MissingChecksDone.ToString();
				 txtFAOD->Text = Stats->FindAndOrganisesDone.ToString();
				 txtAAS->Text = Stats->AutoAddedShows.ToString();
				 txtTM->Text = Stats->TorrentsMatched.ToString();
				 txtNOS->Text = Stats->NS_NumberOfShows.ToString();
				 txtNOSeas->Text = Stats->NS_NumberOfSeasons.ToString();
				 int noe = Stats->NS_NumberOfEpisodes;
				 txtEOD->Text = ((noe == -1)?"?":noe.ToString());
				 txtTE->Text = Stats->NS_NumberOfEpisodesExpected.ToString();
			 }
private: System::Void button1_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->DialogResult = ::DialogResult::OK;
			 this->Close();
		 }
};
}
