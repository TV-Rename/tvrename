#pragma once

using namespace System::Collections;
using System::String;
using namespace System;

namespace TVRename
{
    enum FolderModeEnum {kfmFlat, kfmFolderPerSeason, kfmSpecificSeason };

    public ref class AddItem
    {
        // represents a folder in the Folder Monitor, where the user is entering what show+season it is
    public:
        String ^Folder;
        String ^ShowName;
        SeriesInfo ^TheSeries;
        FolderModeEnum FolderMode;
        int SpecificSeason;

        AddItem(String ^folder, FolderModeEnum folderMode, int season)
        {
            Folder = folder;
            SpecificSeason = season;
            FolderMode = folderMode;
            TheSeries = nullptr;
        }
    };

    typedef Generic::List<AddItem ^> AddItemList;

} // namespace
