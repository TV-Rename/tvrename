// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    internal abstract class Exporter
    {
        public abstract bool Active();
        public abstract void Run();
        protected abstract string Location();
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
    }
}
