# Under the Hood
***Right then, spanners at the ready, here we go!***

## The Registry
The only thing TV Rename saves to the registry is installation information which is used by Windows "Apps and Features" to uninstall the program. But Hey, why would you want to do that?

## Configuration Files
All the configuration information and locally cached data from [The TVDB](http://thetvdb.com "Visit thetvdb.com") is stored in a series of (mainly XML) files under the users Application Directory.

On Windows XP this is: -

> \\Documents and Settings\\\<username\>\\Application Data\\TVRename\\TVRename\\2.1\\

And on Windows 7 and later: -

> \\Users\\\<username\>\\AppData\\Roaming\\TVRename\\TVRename\\2.1\\

In either case the 2.1 directory contains the same set of files.

**If you wish to make a backup of your TV Rename setup a copy of this directory is all you need!**

It contains: -

**Layout.xml** - TV Rename's window position and size as well as column widths.

**Statistics.xml** - TV Rename's historical statistics.

**TheTVDB.xml** -  The locally cached tvdb.com show season and episode information for everythin listed in the ***My Shows*** tab.

**TheTVDB.xml.0 - TheTVDB.xml.9** - A maximum of ten backup copies of the TVDB.xml file. A new file gets created here every time you click ***File>Save*** (FIFO).

**TVRenameSettings.xml** - Everything else not mentioned is stored in here. All your shows, Media Library paths, directory structures and settings. As with TheTVDB.xml this is only overwritten when you choose **File>Save**.

**TVRenameSettings.xml.0 - TVRenameSettings.xml.9** - backup copies of TV RenameSettings.xml (FIFO).

## Log Files
Log files were added in version 2.3:
* Location is at \\Users\\\<username\>\\AppData\\Roaming\\TVRename\\TVRename\\log\\
* Log files are saved off into the archive directory every time the app is run or every 24 hours.
* Log archive is at \\Users\\\<username\>\\AppData\\Roaming\\TVRename\\TVRename\\log\\archive.
* Logging setting are based on NLog and the configuration file [NLog.config](https://github.com/TV-Rename/tvrename/blob/master/TVRename%23/NLog.config "Look at NLog.config in the TVRename Repo") stored in the TV Rename program directory.
* Read the [NLog Wiki](https://github.com/nlog/NLog/wiki/Configuration-file "Visit the NLog Wiki") for guidance on how to adjust the log fie to get more information.

When raising a bug please upload a log file that illustrates the issue (where possible).

# Source Code
You can find TV Rename's source code (along with executables and this website) in [The TV Rename GitHub Repository](https://github.com/TV-Rename/tvrename "Visit The repository").

# Framework
TV Rename uses the Microsoft .NET Framework. The installer will check for this and let you know if it is needed. It can be downloaded from [Microsoft](https://www.microsoft.com/net/download/windows "Get .NET").

# Credits
TV Rename pulls data from [TheTVDB.com](http://thetvdb.com/ "Visit TheTVDB.com") using their API. **Please visit their site, register, and help by contributing information and artwork for TV Series and Episodes.**

It also uses
* [DotNET Zip](http://www.codeplex.com/DotNetZip),
* [SourceGrid](http://www.codeplex.com/sourcegrid/),
* TV Rename has seen significant speed improvements as a result of using Red Gate's [ANTS profiler](http://www.red-gate.com/products/ants_profiler/index.htm "Visit Red Gate"), and JetBrain's [dotTrace](http://www.jetbrains.com/profiler/). 
* JetBrains' [ReSharper](http://www.jetbrains.com/resharper/) is also used for improving code performance.
* [AlphaFS](http://alphafs.alphaleonis.com/) is used for advanced .NET file operations.

and

* [Retro TV Logo Vector Graphics by Vecteezy!](https://www.vecteezy.com/vector-art/73089-retro-television) for our great logo.
