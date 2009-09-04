#include "StdAfx.h"
#include "BugReport.h"

#include "Version.h"
#include "TVDoc.h"
#include "UI.h"

namespace TVRename
{
    System::Void BugReport::bnCreate_Click(System::Object^  sender, System::EventArgs^  e) 
    {
        txtEmailText->Text = "Working... This may take a while.";
        txtEmailText->Update();

        String ^txt = "";
        txt += "From: " + txtName->Text + " <" + txtEmail->Text + ">" + "\r\n";
        txt += "Subject: TVRename bug report" + "\r\n";
        txt += "\r\n";
        txt += "TVRename version: " + DisplayVersionString() + "\r\n";
        txt += "Experimental features: " + (UI::IncludeExperimentalStuff() ? "On" : "Off") + "\r\n";
        txt += "UserAppDataPath is " + System::Windows::Forms::Application::UserAppDataPath + "\r\n";
        txt += "EpGuidePath is " + UI::EpGuidePath() + "\r\n";
        txt += "\r\n";
        txt += "==== Brief Description ====" + "\r\n";
        txt += txtDesc1->Text + "\r\n";
        txt += "\r\n";
        txt += "==== Description ====" + "\r\n";
        txt += txtDesc2->Text + "\r\n";
        txt += "\r\n";
        txt += "==== Frequency ====" + "\r\n";
        txt += txtFreq->Text + "\r\n";
        txt += "\r\n";
        txt += "==== Notes and Comments ====" + "\r\n";
        txt += txtComments->Text + "\r\n";
        txt += "\r\n";


        if (cbSettings->Checked)
        {
            txt += "==== Settings Files ====" + "\r\n";
            txt += "\r\n";
            txt += "---- TVRenameSettings.xml" + "\r\n";
            txt += "\r\n";
            try {
                StreamReader ^sr = gcnew StreamReader(System::Windows::Forms::Application::UserAppDataPath+"\\TVRenameSettings.xml");
                txt += sr->ReadToEnd();
                sr->Close();
                txt += "\r\n";
            }
            catch (...)
            {
                txt += "Error reading TVRenameSettings.xml\r\n";
            }
            txt += "\r\n";
        }

        if (cbFOScan->Checked || cbFolderScan->Checked)
		{
			txt += "==== Filename processors ====\r\n";
			for each (FilenameProcessorRE ^s in mDoc->Settings->FNPRegexs)
				txt += (s->Enabled ? "Enabled":"Disabled") + " \""+s->RE+"\" "+(s->UseFullPath?"(FullPath)":"")+"\r\n";
			txt += "\r\n";
		}

        if (cbFOScan->Checked)
        {
            txt += "==== Finding & Organising Directory Scan ====" + "\r\n";
            txt += "\r\n";
            FileList ^files = gcnew FileList;
            for each (String ^efi in mDoc->SearchFolders)
                BuildDirCache(files, efi, true);

            for each (FileInfo ^fi in files)
            {
                int seas, ep;
                bool r = mDoc->FindSeasEp(fi, &seas, &ep, nullptr);
                bool useful = mDoc->Settings->UsefulExtension(fi->Extension,false);
                txt += fi->FullName + " ("+(r?"OK":"No")+" " + seas.ToString()+","+ep.ToString()+" "+(useful?fi->Extension:"-")+")" + "\r\n";
            }
            txt += "\r\n";
        }

        if (cbFolderScan->Checked)
        {
            txt += "==== Media Folders Directory Scan ====" + "\r\n";

            for each (ShowItem ^si in mDoc->GetShowItems(true))
            {
                for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in si->SeasonEpisodes)
                {
                    int snum = kvp->Key;
                    if (((snum == 0) && (si->CountSpecials))  || !si->AllFolderLocations(mDoc->Settings)->ContainsKey(snum))
                        continue; // skip specials


                    for each (String ^folder in si->AllFolderLocations(mDoc->Settings)[snum])
                    {
                        txt += si->TVDBCode + " : " + si->ShowName() + " : S" + snum.ToString() + "\r\n";
                        txt += "Folder: " + folder;
                        txt += "\r\n";
                        FileList ^files = gcnew FileList;
                        if (DirectoryInfo(folder).Exists)
                            BuildDirCache(files, folder, true);
                        for each (FileInfo ^fi in files)
                        {
                            int seas, ep;
                            bool r = mDoc->FindSeasEp(fi, &seas, &ep, si->ShowName());
                            bool useful = mDoc->Settings->UsefulExtension(fi->Extension,false);
                            txt += fi->FullName + " ("+(r?"OK":"No")+" " + seas.ToString()+","+ep.ToString()+" "+(useful?fi->Extension:"-")+")" + "\r\n";
                        }
                        txt += "\r\n";
                    }
                }
                txt += "\r\n";
            }
            mDoc->UnlockShowItems();

            txt += "\r\n";
        }

        txtEmailText->Text = txt;
    }


} // namespace