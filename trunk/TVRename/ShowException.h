//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

	/// <summary>
	/// Summary for ShowException
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class ShowException : public System::Windows::Forms::Form
	{
      Exception ^mException;
	public:
		ShowException(Exception ^e)
		{
			InitializeComponent();

			mException = e;
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~ShowException()
		{
			if (components)
			{
				delete components;
			}
		}
    private: System::Windows::Forms::TextBox^  txtText;
    protected: 

    protected: 
    private: System::Windows::Forms::Button^  button1;
    private: System::Windows::Forms::Label^  label1;

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
          System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(ShowException::typeid));
          this->txtText = (gcnew System::Windows::Forms::TextBox());
          this->button1 = (gcnew System::Windows::Forms::Button());
          this->label1 = (gcnew System::Windows::Forms::Label());
          this->SuspendLayout();
          // 
          // txtText
          // 
          this->txtText->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
            | System::Windows::Forms::AnchorStyles::Left) 
            | System::Windows::Forms::AnchorStyles::Right));
          this->txtText->Location = System::Drawing::Point(12, 58);
          this->txtText->Multiline = true;
          this->txtText->Name = L"txtText";
          this->txtText->ReadOnly = true;
          this->txtText->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
          this->txtText->Size = System::Drawing::Size(652, 300);
          this->txtText->TabIndex = 0;
          // 
          // button1
          // 
          this->button1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
          this->button1->Location = System::Drawing::Point(589, 364);
          this->button1->Name = L"button1";
          this->button1->Size = System::Drawing::Size(75, 23);
          this->button1->TabIndex = 1;
          this->button1->Text = L"Close";
          this->button1->UseVisualStyleBackColor = true;
          this->button1->Click += gcnew System::EventHandler(this, &ShowException::button1_Click);
          // 
          // label1
          // 
          this->label1->Location = System::Drawing::Point(12, 9);
          this->label1->Name = L"label1";
          this->label1->Size = System::Drawing::Size(652, 46);
          this->label1->TabIndex = 2;
          this->label1->Text = resources->GetString(L"label1.Text");
          // 
          // ShowException
          // 
          this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
          this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
          this->ClientSize = System::Drawing::Size(676, 399);
          this->Controls->Add(this->label1);
          this->Controls->Add(this->button1);
          this->Controls->Add(this->txtText);
          this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
          this->Name = L"ShowException";
          this->ShowInTaskbar = false;
          this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
          this->Text = L"Exception";
          this->Load += gcnew System::EventHandler(this, &ShowException::ShowException_Load);
          this->ResumeLayout(false);
          this->PerformLayout();

        }
#pragma endregion
    private: System::Void ShowException_Load(System::Object^  sender, System::EventArgs^  e) 
             {
               String ^t;
               t =  mException->Message + "\r\n\r\n" + mException->StackTrace;
               txtText->Text = t;
             }
    private: System::Void button1_Click(System::Object^  sender, System::EventArgs^  e) 
             {
               this->Close();
             }
    };
}
