#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

namespace TVRename {

    ref class TVDoc;

	/// <summary>
	/// Summary for DownloadProgress
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class DownloadProgress : public System::Windows::Forms::Form
	{
	private:
		TVDoc ^mDoc;

	public:
	  DownloadProgress(TVDoc ^doc);

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~DownloadProgress()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::Button^  bnCancel;
	protected: 

	private: System::Windows::Forms::ProgressBar^  pbProgressBar;

	private: System::Windows::Forms::Label^  label2;

	private: System::Windows::Forms::Label^  txtCurrent;
	private: System::Windows::Forms::Timer^  tmrUpdate;
	private: System::ComponentModel::IContainer^  components;
	protected: 

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
                    this->bnCancel = (gcnew System::Windows::Forms::Button());
                    this->pbProgressBar = (gcnew System::Windows::Forms::ProgressBar());
                    this->label2 = (gcnew System::Windows::Forms::Label());
                    this->txtCurrent = (gcnew System::Windows::Forms::Label());
                    this->tmrUpdate = (gcnew System::Windows::Forms::Timer(this->components));
                    this->SuspendLayout();
                    // 
                    // bnCancel
                    // 
                    this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
                    this->bnCancel->Location = System::Drawing::Point(159, 62);
                    this->bnCancel->Name = L"bnCancel";
                    this->bnCancel->Size = System::Drawing::Size(75, 23);
                    this->bnCancel->TabIndex = 0;
                    this->bnCancel->Text = L"Cancel";
                    this->bnCancel->UseVisualStyleBackColor = true;
                    this->bnCancel->Click += gcnew System::EventHandler(this, &DownloadProgress::bnCancel_Click);
                    // 
                    // pbProgressBar
                    // 
                    this->pbProgressBar->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->pbProgressBar->Location = System::Drawing::Point(12, 35);
                    this->pbProgressBar->Name = L"pbProgressBar";
                    this->pbProgressBar->Size = System::Drawing::Size(366, 15);
                    this->pbProgressBar->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
                    this->pbProgressBar->TabIndex = 1;
                    // 
                    // label2
                    // 
                    this->label2->AutoSize = true;
                    this->label2->Location = System::Drawing::Point(12, 9);
                    this->label2->Name = L"label2";
                    this->label2->Size = System::Drawing::Size(116, 13);
                    this->label2->TabIndex = 2;
                    this->label2->Text = L"Currently Downloading:";
                    // 
                    // txtCurrent
                    // 
                    this->txtCurrent->AutoSize = true;
                    this->txtCurrent->Location = System::Drawing::Point(134, 9);
                    this->txtCurrent->Name = L"txtCurrent";
                    this->txtCurrent->Size = System::Drawing::Size(16, 13);
                    this->txtCurrent->TabIndex = 2;
                    this->txtCurrent->Text = L"---";
                    // 
                    // tmrUpdate
                    // 
                    this->tmrUpdate->Enabled = true;
                    this->tmrUpdate->Interval = 100;
                    this->tmrUpdate->Tick += gcnew System::EventHandler(this, &DownloadProgress::tmrUpdate_Tick);
                    // 
                    // DownloadProgress
                    // 
                    this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
                    this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
                    this->ClientSize = System::Drawing::Size(390, 97);
                    this->Controls->Add(this->txtCurrent);
                    this->Controls->Add(this->label2);
                    this->Controls->Add(this->pbProgressBar);
                    this->Controls->Add(this->bnCancel);
                    this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
                    this->Name = L"DownloadProgress";
                    this->ShowInTaskbar = false;
                    this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
                    this->Text = L"Download Progress";
                    this->Load += gcnew System::EventHandler(this, &DownloadProgress::DownloadProgress_Load);
                    this->ResumeLayout(false);
                    this->PerformLayout();

                }
#pragma endregion
	private: System::Void bnCancel_Click(System::Object^  sender, System::EventArgs^  e) ;
private: System::Void tmrUpdate_Tick(System::Object^  sender, System::EventArgs^  e);
private: System::Void DownloadProgress_Load(System::Object^  sender, System::EventArgs^  e) 
		 {
			 //UpdateStuff();
		 }
	  void UpdateStuff();
};

}
