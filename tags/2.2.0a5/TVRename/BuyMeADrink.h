#pragma once

#include "TVDoc.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Text::RegularExpressions;
using namespace System::Globalization;

namespace TVRename {

	/// <summary>
	/// Summary for BuyMeADrink
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class BuyMeADrink : public System::Windows::Forms::Form
	{
	public:
		BuyMeADrink(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//

            label1->Text = "If this program has saved you time, and you use it regularly, then please consider buying me a drink to say thanks!\r\r" \
              "Type in (or choose) an amount, then hit the button to go to Paypal.";
            comboBox1->Items->Add("$"+double(1).ToString(".00"));
            comboBox1->Items->Add("$"+double(2).ToString(".00"));
            comboBox1->Items->Add("$"+double(5).ToString(".00"));
            comboBox1->Items->Add("$"+double(10).ToString(".00"));
            comboBox1->Items->Add("$"+double(20).ToString(".00"));

            comboBox2->Items->Add("AUD");
            comboBox2->Items->Add("USD");

            comboBox1->SelectedIndex = 2;
            comboBox2->SelectedIndex = 0;
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~BuyMeADrink()
		{
			if (components)
			{
				delete components;
			}
		}
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::Button^  button1;
    private: System::Windows::Forms::Button^  bnClose;
    private: System::Windows::Forms::ComboBox^  comboBox1;
    private: System::Windows::Forms::ComboBox^  comboBox2;
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
          System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(BuyMeADrink::typeid));
          this->label1 = (gcnew System::Windows::Forms::Label());
          this->button1 = (gcnew System::Windows::Forms::Button());
          this->bnClose = (gcnew System::Windows::Forms::Button());
          this->comboBox1 = (gcnew System::Windows::Forms::ComboBox());
          this->comboBox2 = (gcnew System::Windows::Forms::ComboBox());
          this->SuspendLayout();
          // 
          // label1
          // 
          this->label1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
            | System::Windows::Forms::AnchorStyles::Left) 
            | System::Windows::Forms::AnchorStyles::Right));
          this->label1->Location = System::Drawing::Point(12, 9);
          this->label1->Name = L"label1";
          this->label1->Size = System::Drawing::Size(321, 61);
          this->label1->TabIndex = 0;
          // 
          // button1
          // 
          this->button1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
          this->button1->Location = System::Drawing::Point(173, 82);
          this->button1->Name = L"button1";
          this->button1->Size = System::Drawing::Size(75, 23);
          this->button1->TabIndex = 4;
          this->button1->Text = L"Paypal";
          this->button1->UseVisualStyleBackColor = true;
          this->button1->Click += gcnew System::EventHandler(this, &BuyMeADrink::button1_Click);
          // 
          // bnClose
          // 
          this->bnClose->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
          this->bnClose->DialogResult = System::Windows::Forms::DialogResult::Cancel;
          this->bnClose->Location = System::Drawing::Point(258, 82);
          this->bnClose->Name = L"bnClose";
          this->bnClose->Size = System::Drawing::Size(75, 23);
          this->bnClose->TabIndex = 1;
          this->bnClose->Text = L"Close";
          this->bnClose->UseVisualStyleBackColor = true;
          // 
          // comboBox1
          // 
          this->comboBox1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
          this->comboBox1->FormattingEnabled = true;
          this->comboBox1->Location = System::Drawing::Point(12, 82);
          this->comboBox1->Name = L"comboBox1";
          this->comboBox1->Size = System::Drawing::Size(92, 21);
          this->comboBox1->TabIndex = 2;
          // 
          // comboBox2
          // 
          this->comboBox2->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
          this->comboBox2->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
          this->comboBox2->FormattingEnabled = true;
          this->comboBox2->Location = System::Drawing::Point(110, 82);
          this->comboBox2->Name = L"comboBox2";
          this->comboBox2->Size = System::Drawing::Size(57, 21);
          this->comboBox2->TabIndex = 3;
          // 
          // BuyMeADrink
          // 
          this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
          this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
          this->CancelButton = this->bnClose;
          this->ClientSize = System::Drawing::Size(343, 115);
          this->Controls->Add(this->comboBox2);
          this->Controls->Add(this->comboBox1);
          this->Controls->Add(this->bnClose);
          this->Controls->Add(this->button1);
          this->Controls->Add(this->label1);
          this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
          this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
          this->Name = L"BuyMeADrink";
          this->ShowInTaskbar = false;
          this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Hide;
          this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
          this->Text = L"Buy Me A Drink";
          this->ResumeLayout(false);

        }
#pragma endregion
    private: System::Void button1_Click(System::Object^  sender, System::EventArgs^  e) 
             {
               double amount = 5.00; // default amount

               try 
               {
                 String ^s = Regex::Replace(comboBox1->Text, "\\$", "");
                 amount = double::Parse(s);
               }
               catch (...)
               {
               }

               String ^currency = comboBox2->Text;

               CultureInfo ^usCI = gcnew CultureInfo( "en-US",false );

               String ^paypalURL = "https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=paypal%40tvrename%2ecom&item_name=TVRename%20thank-you%20drink&no_shipping=0&no_note=1&amount="+amount.ToString("N",usCI)+"&tax=0&currency_code="+currency+"&lc=AU&bn=PP%2dDonationsBF&charset=UTF%2d8";

               TVDoc::SysOpen(paypalURL);
             }
    };
}
