using System;
using System.Collections.Generic;
using System.Text;

namespace TVRename
{
    abstract class Finder
    {
        protected ItemList TheActionList;
        protected bool ActionCancel = false;
        protected TVDoc mDoc;

        public Finder(TVDoc doc)
        {
            mDoc = doc;
        }

        public enum FinderDisplayType { Local, Downloading, RSS};

        public abstract void Check(SetProgressDelegate prog, int startpct, int totPct);
        
        public abstract bool Active();

        public abstract FinderDisplayType DisplayType();

        public void setActionList(ItemList actionList) { TheActionList = actionList; }

        public void interrupt() { ActionCancel = true; }
        public void reset() { ActionCancel = false; }

    }
}
