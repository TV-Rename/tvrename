#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {
    ref class UI;

    public ref class FolderMonitorProgress : public System::Windows::Forms::Form
    {
    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~FolderMonitorProgress()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::Button^  bnCancel;

    private: System::Windows::Forms::Label^  label2;
    private: System::Windows::Forms::Timer^  timer1;


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
            this->bnCancel = (gcnew System::Windows::Forms::Button());
            this->label2 = (gcnew System::Windows::Forms::Label());
            this->timer1 = (gcnew System::Windows::Forms::Timer(this->components));
            this->SuspendLayout();
            // 
            // bnCancel
            // 
            this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
            this->bnCancel->Location = System::Drawing::Point(60, 33);
            this->bnCancel->Name = L"bnCancel";
            this->bnCancel->Size = System::Drawing::Size(75, 23);
            this->bnCancel->TabIndex = 0;
            this->bnCancel->Text = L"Cancel";
            this->bnCancel->UseVisualStyleBackColor = true;
            this->bnCancel->Click += gcnew System::EventHandler(this, &FolderMonitorProgress::bnCancel_Click);
            // 
            // label2
            // 
            this->label2->AutoSize = true;
            this->label2->Location = System::Drawing::Point(39, 9);
            this->label2->Name = L"label2";
            this->label2->Size = System::Drawing::Size(117, 13);
            this->label2->TabIndex = 2;
            this->label2->Text = L"Automatic show lookup";
            // 
            // timer1
            // 
            this->timer1->Enabled = true;
            this->timer1->Tick += gcnew System::EventHandler(this, &FolderMonitorProgress::timer1_Tick);
            // 
            // FolderMonitorProgress
            // 
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->ClientSize = System::Drawing::Size(201, 68);
            this->Controls->Add(this->label2);
            this->Controls->Add(this->bnCancel);
            this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
            this->Name = L"FolderMonitorProgress";
            this->ShowInTaskbar = false;
            this->StartPosition = System::Windows::Forms::FormStartPosition::CenterScreen;
            this->Text = L"Folder Monitor";
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion

        UI ^mUI;
        public:
            bool StopNow;

         public:
             FolderMonitorProgress(UI ^theui);
             System::Void bnCancel_Click(System::Object^  sender, System::EventArgs^  e) ;


    private: System::Void timer1_Tick(System::Object^  sender, System::EventArgs^  e) ;
    };

}
