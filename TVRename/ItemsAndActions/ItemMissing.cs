//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

using Alphaleonis.Win32.Filesystem;
using System;

public abstract class ItemMissing : Item
{
    public string TheFileNoExt;
    public string Filename;
    protected string Folder;

    public override string DestinationFile => Filename;
    public override string ScanListViewGroup => "lvgActionMissing";
    public override string DestinationFolder => Folder;
    public override string TargetFolder
    {
        get
        {
            try
            {
                return new FileInfo(TheFileNoExt).DirectoryName;
            }
            catch (NotSupportedException)
            {
                return string.Empty;
            }
        }
    }
        
    public override bool CheckedItem { get => false; set { } }
    public override int IconNumber => 1;
    public abstract bool DoRename { get; }
    public abstract MediaConfiguration Show { get; }
    public override IgnoreItem? Ignore => GenerateIgnore(TheFileNoExt);

    public void AddComment(string p0)
    {
        ErrorText += p0;
        NotifyPropertyChanged();
    }
}
