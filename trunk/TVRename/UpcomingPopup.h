#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

#include "TVDoc.h"

namespace TVRename {

	/// <summary>
	/// Summary for UpcomingPopup
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class UpcomingPopup : public System::Windows::Forms::Form
	{
      TVDoc ^mDoc;

	public:
		UpcomingPopup(TVDoc ^doc)
		{
          mDoc = doc;
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~UpcomingPopup()
		{
			if (components)
			{
				delete components;
			}
		}
    private: System::Windows::Forms::ListView^  lvUpcoming;
    protected: 
    private: System::Windows::Forms::ColumnHeader^  columnHeader1;
    private: System::Windows::Forms::ColumnHeader^  columnHeader2;
    private: System::Windows::Forms::ColumnHeader^  columnHeader3;
    private: System::Windows::Forms::ColumnHeader^  columnHeader4;
    private: System::Windows::Forms::Timer^  TimerOfDeath;
    private: System::Windows::Forms::Button^  hiddenButton;
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
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(UpcomingPopup::typeid));
			this->lvUpcoming = (gcnew System::Windows::Forms::ListView());
			this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader3 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader4 = (gcnew System::Windows::Forms::ColumnHeader());
			this->TimerOfDeath = (gcnew System::Windows::Forms::Timer(this->components));
			this->hiddenButton = (gcnew System::Windows::Forms::Button());
			this->SuspendLayout();
			// 
			// lvUpcoming
			// 
			this->lvUpcoming->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(4) {this->columnHeader1, this->columnHeader2, 
				this->columnHeader3, this->columnHeader4});
			this->lvUpcoming->Dock = System::Windows::Forms::DockStyle::Fill;
			this->lvUpcoming->HeaderStyle = System::Windows::Forms::ColumnHeaderStyle::None;
			this->lvUpcoming->Location = System::Drawing::Point(0, 0);
			this->lvUpcoming->MultiSelect = false;
			this->lvUpcoming->Name = L"lvUpcoming";
			this->lvUpcoming->Size = System::Drawing::Size(458, 90);
			this->lvUpcoming->TabIndex = 1;
			this->lvUpcoming->UseCompatibleStateImageBehavior = false;
			this->lvUpcoming->View = System::Windows::Forms::View::Details;
			this->lvUpcoming->Enter += gcnew System::EventHandler(this, &UpcomingPopup::lvUpcoming_Enter);
			this->lvUpcoming->SelectedIndexChanged += gcnew System::EventHandler(this, &UpcomingPopup::lvUpcoming_SelectedIndexChanged);
			// 
			// columnHeader4
			// 
			this->columnHeader4->Width = 228;
			// 
			// TimerOfDeath
			// 
			this->TimerOfDeath->Enabled = true;
			this->TimerOfDeath->Interval = 10000;
			this->TimerOfDeath->Tick += gcnew System::EventHandler(this, &UpcomingPopup::TimerOfDeath_Tick);
			// 
			// hiddenButton
			// 
			this->hiddenButton->Location = System::Drawing::Point(276, 31);
			this->hiddenButton->Name = L"hiddenButton";
			this->hiddenButton->Size = System::Drawing::Size(97, 23);
			this->hiddenButton->TabIndex = 0;
			this->hiddenButton->Text = L"hiddenButton";
			this->hiddenButton->UseVisualStyleBackColor = true;
			this->hiddenButton->Visible = false;
			// 
			// UpcomingPopup
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(458, 90);
			this->Controls->Add(this->hiddenButton);
			this->Controls->Add(this->lvUpcoming);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedToolWindow;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"UpcomingPopup";
			this->ShowIcon = false;
			this->ShowInTaskbar = false;
			this->StartPosition = System::Windows::Forms::FormStartPosition::Manual;
			this->Text = L"Upcoming Shows";
			this->TopMost = true;
			this->Load += gcnew System::EventHandler(this, &UpcomingPopup::UpcomingPopup_Load);
			this->ResumeLayout(false);

		}
#pragma endregion
    private: System::Void UpcomingPopup_Load(System::Object^  sender, System::EventArgs^  e) 
             {
               int screenWidth = Screen::PrimaryScreen->WorkingArea.Width;
               int screenHeight = Screen::PrimaryScreen->WorkingArea.Height;
               this->Left = screenWidth - this->Width;
               this->Top = screenHeight - this->Height;

               FillSelf();
             }
    private: System::Void TimerOfDeath_Tick(System::Object^  sender, System::EventArgs^  e) 
             {
               this->Close();
             }

             void FillSelf()
             {
               lvUpcoming->BeginUpdate();
               lvUpcoming->Items->Clear();

               const int kN = 5;

               ProcessedEpisodeList ^next5 = mDoc->NextNShows(kN, 9999);

               if (next5 != nullptr)
               {
                 for each (ProcessedEpisode ^ei in next5)
                 {
                   ListViewItem ^lvi = gcnew ListViewItem;
                   lvi->Text = ei->HowLong();
                   lvi->SubItems->Add(ei->DayOfWeek());
                   lvi->SubItems->Add(ei->TimeOfDay());
                   lvi->SubItems->Add(mDoc->Settings->NamingStyle->NameForExt(ei,nullptr));
                   lvUpcoming->Items->Add(lvi);
                 }
				 if (lvUpcoming->Items->Count > 0)
				 {
					 int h1 = lvUpcoming->Items[0]->GetBounds(ItemBoundsPortion::Entire).Height + 6;
					 this->Height = (h1 * lvUpcoming->Items->Count);
				 }
               }
			   
               int w = 0;
               for (int i=0;i<lvUpcoming->Columns->Count;i++)
               {
                 lvUpcoming->Columns[i]->AutoResize(ColumnHeaderAutoResizeStyle::ColumnContent);
                 w += lvUpcoming->Columns[i]->Width;
               }

               lvUpcoming->Width = w;
               lvUpcoming->SelectedIndices->Clear();
               hiddenButton->Select();

               lvUpcoming->EndUpdate();
             }

    private: System::Void lvUpcoming_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
             {
               lvUpcoming->SelectedIndices->Clear();
               hiddenButton->Select();
             }
private: System::Void lvUpcoming_Enter(System::Object^  sender, System::EventArgs^  e) 
         {
           hiddenButton->Select();
         }
};
}
