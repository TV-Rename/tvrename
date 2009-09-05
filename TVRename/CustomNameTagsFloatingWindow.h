//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "CustomName.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

	/// <summary>
	/// Summary for CustomNameTagsFloatingWindow
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class CustomNameTagsFloatingWindow : public System::Windows::Forms::Form
	{
	public:
            CustomNameTagsFloatingWindow(ProcessedEpisode ^pe)
            {
                InitializeComponent();

                for each (String ^s in CustomName::Tags())
                {
                    String ^txt = s;
                    if (pe != nullptr)
                        txt += " - " + CustomName::NameForNoExt(pe, s);

                    label1->Text += txt + "\r\n";
                }
                }

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~CustomNameTagsFloatingWindow()
		{
			if (components)
			{
				delete components;
			}
		}
        private: System::Windows::Forms::Label^  label1;
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
                    System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(CustomNameTagsFloatingWindow::typeid));
                    this->label1 = (gcnew System::Windows::Forms::Label());
                    this->SuspendLayout();
                    // 
                    // label1
                    // 
                    this->label1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
                        | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->label1->AutoSize = true;
                    this->label1->Location = System::Drawing::Point(12, 9);
                    this->label1->Name = L"label1";
                    this->label1->Size = System::Drawing::Size(0, 13);
                    this->label1->TabIndex = 0;
                    // 
                    // CustomNameTagsFloatingWindow
                    // 
                    this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
                    this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
                    this->AutoSize = true;
                    this->ClientSize = System::Drawing::Size(248, 41);
                    this->Controls->Add(this->label1);
                    this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::SizableToolWindow;
                    this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
                    this->Name = L"CustomNameTagsFloatingWindow";
                    this->ShowInTaskbar = false;
                    this->Text = L"Tags";
                    this->ResumeLayout(false);
                    this->PerformLayout();

                }
#pragma endregion
	};
}
