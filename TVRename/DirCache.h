//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "Settings.h"

namespace TVRename
{
	public ref class DirCacheEntry
	{
	public:
		FileInfo ^TheFile;
		String ^SimplifiedFullName;
		String ^LowerName;
		bool HasUsefulExtension_NotOthersToo;
		bool HasUsefulExtension_OthersToo;
		Int64 Length;

		DirCacheEntry(FileInfo ^f, TVSettings ^theSettings)
		{
			TheFile = f;
			SimplifiedFullName = SimplifyName(f->FullName);
			LowerName = f->Name->ToLower();
			Length = f->Length;
			if (theSettings != nullptr)
			{
			 HasUsefulExtension_NotOthersToo = theSettings->UsefulExtension(f->Extension, false);
			 HasUsefulExtension_OthersToo = HasUsefulExtension_NotOthersToo | theSettings->UsefulExtension(f->Extension, true);
			}
		}
	};

	typedef Collections::Generic::List<DirCacheEntry ^> DirCache;

	static int CountFiles(String ^folder, bool subFolders)
	{
		int n = 0;
		if (!DirectoryInfo(folder).Exists)
			return n;
		if (folder->Length >= 248)
			return n;
		DirectoryInfo ^di = gcnew DirectoryInfo(folder);
		n = di->GetFiles()->Length;

		if (subFolders)
		{
			array<DirectoryInfo ^> ^dirs = di->GetDirectories();
			for each (DirectoryInfo ^di in dirs)
				n += CountFiles(di->FullName, subFolders);
		}
		return n;
	}

	static int BuildDirCache(SetProgressDelegate ^prog, int initialCount, int totalFiles, DirCache ^fileCache, String ^folder, bool subFolders, TVSettings ^theSettings)
	{
		int filesDone = initialCount;
	
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
				return filesDone;
			}

			DirectoryInfo ^di = gcnew DirectoryInfo(folder);
			if (!di->Exists)
				return filesDone; 

			//                DirectorySecurity ^ds = di->GetAccessControl();

			
			array<FileInfo ^> ^f2 = di->GetFiles();
			for each (FileInfo ^ff in f2)
			{
				filesDone++;
				if ((ff->Name->Length + folder->Length) >= 260)
				{
					MessageBox::Show("Skipping file that has a path+name longer than the Windows permitted 259 characters: " + ff->Name + " in " + folder,
						"File+Path name too long",
						MessageBoxButtons::OK,
						MessageBoxIcon::Warning);
				}
				else
					fileCache->Add(gcnew DirCacheEntry(ff, theSettings));
					if (prog != nullptr)
			prog->Invoke(100*(filesDone) / totalFiles);
			}

			if (subFolders)
			{
				array<DirectoryInfo ^> ^dirs = di->GetDirectories();
				for each (DirectoryInfo ^di in dirs)
					filesDone = BuildDirCache(prog,filesDone,totalFiles,fileCache, di->FullName, subFolders, theSettings);
			}
		}
		catch (...)
		{
		}
		return filesDone;
	}
} // namespace