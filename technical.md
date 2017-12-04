## Configuration Files
Files on your computer that TVRename creates and uses:

 **(Windows XP)** \Documents and Settings\<username>\Application Data\TVRename\TVRename\2.1\ 

**or**

 **(Vista, 7, 9, 10)** \Users\<username>\AppData\Roaming\TVRename\TVRename\2.1\ 

This folder contains TVRename's settings files:

* **TVRenameSettings.xml** - Pretty much everything. All your shows, folders, and settings. This is only overwritten when you choose "Save". Older, backup, copies of the settings file are also kept in here.
* **Layout.xml** - TVRename's window position and column sizes are saved here.
* **Statistics.xml** - TVRename's  statistics are saved here.
* **TheTVDB.xml** - Locally cached tvdb.com show information.

The only thing saved to the **registry** is uninstall information, used by the "Add/Remove" control panel.

## Source Code 
Download TVRename, program or source code, from the [GitHub page](https://github.com/TV-Rename/tvrename).

## Framework
MS .NET 2.0 framework - Used by TVRename. The TVRename installer will check for this and let you know if it is needed.
[here](http://www.microsoft.com/downloads/details.aspx?familyid=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&displaylang=en:new=true)

## Credit
TVRename makes use of data from [TheTVDB.com](http://thetvdb.com/). Please visit their site, and consider helping by contributing information and artwork for episodes and series.

It also uses
* [DotNET Zip](http://www.codeplex.com/DotNetZip), 
* [SourceGrid](http://www.codeplex.com/sourcegrid/), 
* and has had significant speed improvements as a result of using Red Gate's [ANTS profiler](http://www.red-gate.com/products/ants_profiler/index.htm), 
* and JetBrains' [dotTrace](http://www.jetbrains.com/profiler/). 
* JetBrains' [ReSharper](http://www.jetbrains.com/resharper/) is also used for improving the code. 
* AlphaFS for advanved file operations
