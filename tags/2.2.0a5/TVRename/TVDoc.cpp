#include "stdafx.h"

#include "TVDoc.h"

namespace TVRename
{
    bool TVDoc::MatchesSequentialNumber(String ^filename, int *seas, int *ep, ProcessedEpisode ^pe)
    {
        if (pe->OverallNumber == -1)
            return false;

        String ^num = pe->OverallNumber.ToString();

        bool found = Regex::Match("X"+filename+"X", "[^0-9]0*"+num+"[^0-9]")->Success; // need to pad to let it match non-numbers at start and end
        if (found)
        {
            *seas = pe->SeasonNumber;
            *ep = pe->EpNum;
        }
        return found;
    }

    String ^TVDoc::SEFinderSimplifyFilename(String ^filename, String ^showNameHint)
    {
        // Look at showNameHint and try to remove the first occurance of it from filename
        // This is very helpful if the showname has a >= 4 digit number in it, as that
        // would trigger the 1302 -> 13,02 matcher
        // Also, shows like "24" can cause confusion

        filename = filename->Replace("."," "); // turn dots into spaces

        if ((showNameHint == nullptr) || (String::IsNullOrEmpty(showNameHint)))
            return filename;

        bool nameIsNumber = (Regex::Match(showNameHint,"^[0-9]+$")->Success);

        int p = filename->IndexOf(showNameHint);

        if (p == 0)
        {
            filename = filename->Remove(0, showNameHint->Length);
            return filename;
        }

        if (nameIsNumber) // e.g. "24", or easy exact match of show name at start of filename
            return filename;

        for each (Match ^m in Regex::Matches(showNameHint, "(?:^|[^a-z]|\\b)([0-9]{3,})")) // find >= 3 digit numbers in show name
        {
            if (m->Groups->Count > 1) // just in case
            {
                String ^number = m->Groups[1]->Value;
                filename = Regex::Replace(filename, "(^|\\W)" + number + "\\b",""); // remove any occurances of that number in the filename
            }
        }


        return filename;
    }

    bool TVDoc::FindSeasEp(FileInfo ^fi, int *seas, int *ep, String ^showNameHint)
    {
        return FindSeasEp(fi, seas, ep, showNameHint, Settings->FNPRegexs);
    }


    bool TVDoc::FindSeasEp(FileInfo ^fi, int *seas, int *ep, String ^showNameHint, FNPRegexList ^rexps)
    {
        String ^filename = fi->Name;
        int l = filename->Length;
        int le = fi->Extension->Length;
        filename = filename->Substring(0,l-le);
        return FindSeasEp(fi->Directory->FullName, filename, seas, ep, showNameHint, rexps);
    }

    bool TVDoc::FindSeasEp(String ^directory, String ^filename, int *seas, int *ep, String ^showNameHint, FNPRegexList ^rexps)
    {
        *seas = *ep = -1;

        filename = SEFinderSimplifyFilename(filename, showNameHint);

        String ^fullPath = directory + "\\" + filename; // construct full path with sanitised filename

        if ((filename->Length > 256) || (fullPath->Length > 256))
            return false;

        int leftMostPos = filename->Length;

        filename = filename->ToLower()+" ";
        fullPath = fullPath->ToLower()+" ";

        for each (FilenameProcessorRE ^re in rexps)
        {
			if (!re->Enabled)
				continue;
            try
            {
                Match ^m = Regex::Match(re->UseFullPath ? fullPath:filename, re->RE, RegexOptions::IgnoreCase);
                if (m->Success)
                {
                    int adj = re->UseFullPath ? (fullPath->Length - filename->Length) : 0;

                    int p = Math::Min(m->Groups["s"]->Index, m->Groups["e"]->Index) - adj;
                    if (p >= leftMostPos)
                        continue;

                    *seas = int::Parse(m->Groups["s"]->ToString());
                    *ep = int::Parse(m->Groups["e"]->ToString());
                                        
                    leftMostPos = p;
                }
            }
            catch (FormatException ^)
            {
            }
        }

        return ((*seas != -1) || (*ep != -1));
    }



} // namespace
