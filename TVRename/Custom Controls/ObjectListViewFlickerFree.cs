//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;
using System.Drawing;

namespace TVRename;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

// Thanks to http://stackoverflow.com/questions/442817/c-flickering-listview-on-update
public class ObjectListViewFlickerFree<T> : ObjectListView
{
    public ObjectListViewFlickerFree()
    {
        //Activate double buffering
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

        //Enable the OnNotifyMessage event so we get a chance to filter out
        // Windows messages before they get to the form's WndProc
        SetStyle(ControlStyles.EnableNotifyMessage, true);
    }

    protected override void OnNotifyMessage(Message m)
    {
        //Filter out the WM_ERASEBKGND message
        if (m.Msg != 0x14)
        {
            base.OnNotifyMessage(m);
        }
    }

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        base.ScaleControl(factor, specified);
        this.ScaleListViewColumns(factor);
    }


    public List<T> Selected()
    {
        return this.SelectedObjects.OfType<T>().ToList();
    }

    public T? FirstSelected()
    {
        return this.Selected().FirstOrDefault();
    }

    public bool AnySelected() => SelectedObjects.Count != 0;

    internal List<T> AllObjects()
    {
        return Objects.OfType<T>().ToList();
    }
}
