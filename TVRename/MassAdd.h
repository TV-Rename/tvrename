//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "FolderItem.h"
#include "TVDoc.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {
    enum { kBusted = 0, kGood = 1, kIgnoring = 2 };

	/// <summary>
	/// Summary for MassAdd
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
  ref class TVDoc;


  public ref class MassAdd : public System::Windows::Forms::Form
	{
      TVDoc ^mDoc;
    private: System::Windows::Forms::Button^  bnIgnore;
             FolderList ^mCandidates;
    private: System::Windows::Forms::ListView^  lvTheList;
    private: System::Windows::Forms::ColumnHeader^  columnHeader2;
    private: System::Windows::Forms::ColumnHeader^  columnHeader9;
    private: System::Windows::Forms::ColumnHeader^  columnHeader3;
    private: System::Windows::Forms::ColumnHeader^  columnHeader5;
    private: System::Windows::Forms::ColumnHeader^  columnHeader22;
    private: System::Windows::Forms::ColumnHeader^  columnHeader1;
    private: System::Windows::Forms::ColumnHeader^  columnHeader10;
    private: System::Windows::Forms::ColumnHeader^  columnHeader11;

             Collections::Generic::List<bool> ^mCandidatesIgnore;

	public:
      MassAdd(TVDoc ^doc, Collections::Generic::List<String ^> ^candidates)
		{
          mDoc = doc;
          mCandidates = gcnew FolderList();
          mCandidatesIgnore = gcnew Collections::Generic::List<bool>;

          for (int i=0;i<candidates->Count;i++)
          {
            FolderItem ^fi = gcnew FolderItem();
            fi->SetFolder(candidates[i]);
            mCandidates->Add(fi);
            mCandidatesIgnore->Add(false);
          }

			InitializeComponent();
			
            FillList();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~MassAdd()
		{
			if (components)
			{
				delete components;
			}
		}
    private: System::Windows::Forms::Button^  bnOK;
    protected: 









    private: System::Windows::Forms::ImageList^  ilImages;
    private: System::Windows::Forms::Button^  bnCancel;
    private: System::Windows::Forms::Button^  bnEdit;
    private: System::Windows::Forms::Button^  bnRemove;
    private: System::ComponentModel::IContainer^  components;

    protected: 

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
          System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(MassAdd::typeid));
          this->bnOK = (gcnew System::Windows::Forms::Button());
          this->ilImages = (gcnew System::Windows::Forms::ImageList(this->components));
          this->bnCancel = (gcnew System::Windows::Forms::Button());
          this->bnEdit = (gcnew System::Windows::Forms::Button());
          this->bnRemove = (gcnew System::Windows::Forms::Button());
          this->bnIgnore = (gcnew System::Windows::Forms::Button());
          this->lvTheList = (gcnew System::Windows::Forms::ListView());
          this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
          this->columnHeader9 = (gcnew System::Windows::Forms::ColumnHeader());
          this->columnHeader3 = (gcnew System::Windows::Forms::ColumnHeader());
          this->columnHeader5 = (gcnew System::Windows::Forms::ColumnHeader());
          this->columnHeader22 = (gcnew System::Windows::Forms::ColumnHeader());
          this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
          this->columnHeader10 = (gcnew System::Windows::Forms::ColumnHeader());
          this->columnHeader11 = (gcnew System::Windows::Forms::ColumnHeader());
          this->SuspendLayout();
          // 
          // bnOK
          // 
          this->bnOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
          this->bnOK->Location = System::Drawing::Point(948, 349);
          this->bnOK->Name = L"bnOK";
          this->bnOK->Size = System::Drawing::Size(75, 23);
          this->bnOK->TabIndex = 0;
          this->bnOK->Text = L"OK";
          this->bnOK->UseVisualStyleBackColor = true;
          this->bnOK->Click += gcnew System::EventHandler(this, &MassAdd::bnOK_Click);
          // 
          // ilImages
          // 
          this->ilImages->ImageStream = (cli::safe_cast<System::Windows::Forms::ImageListStreamer^  >(resources->GetObject(L"ilImages.ImageStream")));
          this->ilImages->TransparentColor = System::Drawing::Color::Transparent;
          this->ilImages->Images->SetKeyName(0, L"Busted.ico");
          this->ilImages->Images->SetKeyName(1, L"Good.ico");
          this->ilImages->Images->SetKeyName(2, L"Ignore.ico");
          // 
          // bnCancel
          // 
          this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
          this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
          this->bnCancel->Location = System::Drawing::Point(1029, 349);
          this->bnCancel->Name = L"bnCancel";
          this->bnCancel->Size = System::Drawing::Size(75, 23);
          this->bnCancel->TabIndex = 13;
          this->bnCancel->Text = L"Cancel";
          this->bnCancel->UseVisualStyleBackColor = true;
          // 
          // bnEdit
          // 
          this->bnEdit->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
          this->bnEdit->Location = System::Drawing::Point(8, 349);
          this->bnEdit->Name = L"bnEdit";
          this->bnEdit->Size = System::Drawing::Size(75, 23);
          this->bnEdit->TabIndex = 14;
          this->bnEdit->Text = L"&Edit";
          this->bnEdit->UseVisualStyleBackColor = true;
          this->bnEdit->Click += gcnew System::EventHandler(this, &MassAdd::bnEdit_Click);
          // 
          // bnRemove
          // 
          this->bnRemove->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
          this->bnRemove->Location = System::Drawing::Point(89, 349);
          this->bnRemove->Name = L"bnRemove";
          this->bnRemove->Size = System::Drawing::Size(75, 23);
          this->bnRemove->TabIndex = 14;
          this->bnRemove->Text = L"&Remove";
          this->bnRemove->UseVisualStyleBackColor = true;
          this->bnRemove->Click += gcnew System::EventHandler(this, &MassAdd::bnRemove_Click);
          // 
          // bnIgnore
          // 
          this->bnIgnore->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
          this->bnIgnore->Location = System::Drawing::Point(170, 349);
          this->bnIgnore->Name = L"bnIgnore";
          this->bnIgnore->Size = System::Drawing::Size(75, 23);
          this->bnIgnore->TabIndex = 14;
          this->bnIgnore->Text = L"&Ignore";
          this->bnIgnore->UseVisualStyleBackColor = true;
          this->bnIgnore->Click += gcnew System::EventHandler(this, &MassAdd::bnIgnore_Click);
          // 
          // lvTheList
          // 
          this->lvTheList->AllowDrop = true;
          this->lvTheList->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
            | System::Windows::Forms::AnchorStyles::Left) 
            | System::Windows::Forms::AnchorStyles::Right));
          this->lvTheList->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(8) {this->columnHeader2, this->columnHeader9, 
            this->columnHeader3, this->columnHeader5, this->columnHeader22, this->columnHeader1, this->columnHeader10, this->columnHeader11});
          this->lvTheList->FullRowSelect = true;
          this->lvTheList->Location = System::Drawing::Point(12, 12);
          this->lvTheList->Name = L"lvTheList";
          this->lvTheList->Size = System::Drawing::Size(1092, 331);
          this->lvTheList->SmallImageList = this->ilImages;
          this->lvTheList->Sorting = System::Windows::Forms::SortOrder::Ascending;
          this->lvTheList->TabIndex = 15;
          this->lvTheList->UseCompatibleStateImageBehavior = false;
          this->lvTheList->View = System::Windows::Forms::View::Details;
          this->lvTheList->DoubleClick += gcnew System::EventHandler(this, &MassAdd::lvTheList_DoubleClick);
          // 
          // columnHeader2
          // 
          this->columnHeader2->Text = L"Show";
          this->columnHeader2->Width = 196;
          // 
          // columnHeader9
          // 
          this->columnHeader9->Text = L"Season";
          this->columnHeader9->Width = 51;
          // 
          // columnHeader3
          // 
          this->columnHeader3->Text = L"tv.com code";
          this->columnHeader3->Width = 82;
          // 
          // columnHeader5
          // 
          this->columnHeader5->Text = L"Next";
          this->columnHeader5->Width = 41;
          // 
          // columnHeader22
          // 
          this->columnHeader22->Text = L"Check";
          this->columnHeader22->Width = 53;
          // 
          // columnHeader1
          // 
          this->columnHeader1->Text = L"Folder";
          this->columnHeader1->Width = 283;
          // 
          // columnHeader10
          // 
          this->columnHeader10->Text = L"Count Specials";
          this->columnHeader10->Width = 87;
          // 
          // columnHeader11
          // 
          this->columnHeader11->Text = L"Naming Style";
          this->columnHeader11->Width = 244;
          // 
          // MassAdd
          // 
          this->AcceptButton = this->bnOK;
          this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
          this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
          this->CancelButton = this->bnCancel;
          this->ClientSize = System::Drawing::Size(1116, 384);
          this->Controls->Add(this->lvTheList);
          this->Controls->Add(this->bnIgnore);
          this->Controls->Add(this->bnRemove);
          this->Controls->Add(this->bnEdit);
          this->Controls->Add(this->bnCancel);
          this->Controls->Add(this->bnOK);
          this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
          this->Name = L"MassAdd";
          this->ShowInTaskbar = false;
          this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Show;
          this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
          this->Text = L"Add Watch Items";
          this->ResumeLayout(false);

        }
#pragma endregion

        void FillList()
        {
          lvTheList->BeginUpdate();
          lvTheList->Items->Clear();

          for (int i=0;i<mCandidates->Count;i++)
          {
            FolderItem ^fi = mCandidates[i];
            ListViewItem ^lvi = fi->AddSelf(lvTheList, i);
            lvi->ImageIndex = mCandidatesIgnore[i] ? kIgnoring : ( fi->Busted() ? kBusted : kGood );
          }
          
          lvTheList->EndUpdate();
        }
    private: System::Void bnRemove_Click(System::Object^  sender, System::EventArgs^  e) 
             {
               int c = lvTheList->SelectedItems->Count;
               if (!c)
                 return;

               System::Windows::Forms::DialogResult res = MessageBox::Show("Remove " + c + " item"+((c>1)?"s":"")+"?","Remove Item"+((c>1)?"s":""),MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
               if (res != System::Windows::Forms::DialogResult::Yes)
                 return;

               if (c)
               {
                 System::Collections::Generic::List<int> ^ll = gcnew System::Collections::Generic::List<int>;
                 for (int i=0;i<c;i++)
                   ll->Add(safe_cast<int>(lvTheList->SelectedItems[i]->Tag));
                 ll->Sort();
                 for (int i=ll->Count-1;i>=0;i--)
                 {
                   int idx = ll[i];
                   mCandidates->RemoveAt(idx);
                   mCandidatesIgnore->RemoveAt(idx);
                 }
                 FillList();
               }
             }

private: System::Void bnEdit_Click(System::Object^  sender, System::EventArgs^  e);
private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e);

private: System::Void bnIgnore_Click(System::Object^  sender, System::EventArgs^  e) 
         {
           int c = lvTheList->SelectedItems->Count;
           if (c)
           {
             System::Collections::Generic::List<int> ^ll = gcnew System::Collections::Generic::List<int>;
             for (int i=0;i<c;i++)
             {
               int idx = safe_cast<int>(lvTheList->SelectedItems[i]->Tag);
               mCandidatesIgnore[idx] = !mCandidatesIgnore[idx];
             }
             FillList();
           }
         }
private: System::Void lvTheList_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
         {
           bnEdit_Click(nullptr,nullptr);
         }

};
}
