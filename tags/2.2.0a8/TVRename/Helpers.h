//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
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

	static String ^SimplifyName(String ^n)
	{
		n = n->ToLower();
		n = n->Replace("the","");
		n = n->Replace("'","");
		n = n->Replace("&","");
		n = n->Replace("and","");
		n = n->Replace("!","");
		n = Regex::Replace(n, "[_\\W]+"," ");
		return n;
	}

	static bool Same(FileInfo ^a, FileInfo ^b)
	{
		return String::Compare(a->FullName, b->FullName, true) == 0; // true->ignore case
	}

	static bool Same(DirectoryInfo ^a, DirectoryInfo ^b)
	{
		String ^n1 = a->FullName;
		String ^n2 = b->FullName;
		if (!n1->EndsWith("\\"))
			n1 = n1 + "\\";
		if (!n2->EndsWith("\\"))
			n2 = n2 + "\\";

		return String::Compare(n1, n2, true) == 0; // true->ignore case
	}
	static FileInfo ^FileInFolder(String ^dir, String ^fn)
	{
		return gcnew FileInfo(String::Concat(dir, dir->EndsWith("\\")?"":"\\", fn));
	}
	static FileInfo ^FileInFolder(DirectoryInfo ^di, String ^fn)
	{
		return FileInFolder(di->FullName, fn);
	}

}
