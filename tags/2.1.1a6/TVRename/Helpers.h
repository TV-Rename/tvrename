#pragma once

namespace TVRename
{
    using namespace System::IO;
    using namespace System;
    using namespace System::Windows::Forms;
    using namespace System::Text::RegularExpressions;

    public delegate void SetProgressDelegate(int percent);

    typedef System::Collections::Generic::List<System::String ^> StringList;

    static System::Drawing::Color WarningColor() 
    { 
        return System::Drawing::Color::FromArgb(static_cast<System::Int32>(
            static_cast<System::Byte>(255)), 
            static_cast<System::Int32>(static_cast<System::Byte>(210)), 
            static_cast<System::Int32>(static_cast<System::Byte>(210))); 
    }

    typedef System::Collections::Generic::List<System::IO::FileInfo ^> FileList;
    typedef System::Collections::Generic::List<Int64> FileLengths;

    static void GetFileLengths(FileList ^files, FileLengths ^lengths)
    {
        for each (FileInfo ^f in files)
            lengths->Add(f->Length);
    }
    static void BuildDirCache(FileList ^files, String ^folder, bool subFolders)
    {
        if (!DirectoryInfo(folder).Exists)
        {
            System::Windows::Forms::DialogResult res = MessageBox::Show("The search folder \"" + folder + " does not exist.\n",
                "Folder does not exist",MessageBoxButtons::OK, MessageBoxIcon::Warning);
        }

        try {
            if (folder->Length >= 248)
            {
                MessageBox::Show("Skipping folder that has a name longer than the Windows permitted 247 characters: " + folder,
                    "Path name too long",
                    MessageBoxButtons::OK,
                    MessageBoxIcon::Warning);
                return;
            }

            DirectoryInfo ^di = gcnew DirectoryInfo(folder);
            if (!di->Exists)
                return;

            //                DirectorySecurity ^ds = di->GetAccessControl();

            array<FileInfo ^> ^f2 = di->GetFiles();
            for (int i=0;i<f2->Length;i++)
            {
                if ((f2[i]->Name->Length + folder->Length) >= 260)
                {
                    MessageBox::Show("Skipping file that has a path+name longer than the Windows permitted 259 characters: " + f2[i]->Name + " in " + folder,
                        "File+Path name too long",
                        MessageBoxButtons::OK,
                        MessageBoxIcon::Warning);
                }
                else
                    files->Add(f2[i]);
            }

            if (subFolders)
            {
                array<DirectoryInfo ^> ^dirs = di->GetDirectories();
                for (int i=0;i<dirs->Length;i++)
                    BuildDirCache(files, dirs[i]->FullName, subFolders);
            }
        }
        catch (...)
        {
        }
    }
        static String ^SimplifyName(String ^n)
        {
            n = n->ToLower();
            n = n->Replace("the","");
            n = n->Replace("'","");
			n = n->Replace("&","");
			n = n->Replace("and","");
            n = Regex::Replace(n, "[_\\W]+"," ");
            return n;
        }

}
