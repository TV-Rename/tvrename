# Under the Hood
***Right then, spanners at the ready, here we go!***

## The Registry
The only thing TVRename saves to the registry is installation information which is used by Windows "Apps and Features" to uninstall the program. But Hey, why would you want to?

## Configuration Files
**All** the configuration information and locally cached data from [The TVDB](http://thetvdb.com "Visit thetvdb.com") is stored in a series of (mainly XML) files under the users Application Directory.

On Windows XP this is: -

> \\Documents and Settings\\\<username\>\\Application Data\\TVRename\\TVRename\\2.1\\ 

And on Windows 7 and later: -

> \\Users\\\<username\>\\AppData\\Roaming\\TVRename\\TVRename\\2.1\\

In either case the 2.1 directory contains the same set of files.

**If you wish to make a backup your TVRename setup a copy of this directory is all you need!**

It contains: -

**Layout.xml** - contains TVRename's window position and size as well as column widths.

**Statistics.xml** - contains TVRename's historical statistics.

**TheTVDB.xml** -  contains the locally cached tvdb.com show information.

**TVRenameSettings.xml** - Everything else not mentioned is stored in here. All your shows, Media Library paths, directory structures and settings. As with TheTVDB.xml this is only overwritten when you choose **File>Save**.

**TheTVDB.xml.0 - TheTVDB.xml.9** - A maximum of ten backup copies of the TVDB.xml file. A new file gets created here every time you click ***File>Save*** (FIFO).

**TVRenameSettings.xml.0 - TVRenameSettings.xml.9** - backup copies of TVRenameSettings.xml (FIFO).


## Log Files

Log files were added in version 2.3:
* Location is at \\Users\\\<username\>\\AppData\\Roaming\\TVRename\\TVRename\\log\\
* Log files are saved off into the archive directory every time the app is run or every 24 hours
* Log archive is at \\Users\\\<username\>\\AppData\\Roaming\\TVRename\\TVRename\\log\\archive
* Logging setting are based on nlog and the configuration file [nlog.config](https://github.com/TV-Rename/tvrename/blob/master/TVRename%23/NLog.config) stored in the TV Rename program directory
  * See [here](https://github.com/nlog/NLog/wiki/Configuration-file) for guidance on how to adjust the log fie to get more information 

When raising a bug then please upload a log file that illustrates the issue (where possible)


# Source Code 
You can find TVRename's source code (along with executables and this website) in [The TVRename GitHub Repository](https://github.com/TV-Rename/tvrename).

# Framework
TVRename uses the Microsoft .NET Framework. The installer will check for this and let you know if it is needed. It can be downloaded [here](https://www.microsoft.com/net/download/windows).

# Credits
TVRename pulls data from [TheTVDB.com](http://thetvdb.com/) using their API. Please visit their site, register, and help by contributing information and artwork for TV Series and Episodes.

It also uses
* [DotNET Zip](http://www.codeplex.com/DotNetZip), 
* [SourceGrid](http://www.codeplex.com/sourcegrid/), 
* TVRename has seen significant speed improvements as a result of using Red Gate's [ANTS profiler](http://www.red-gate.com/products/ants_profiler/index.htm), and JetBrain's [dotTrace](http://www.jetbrains.com/profiler/). 
* JetBrains' [ReSharper](http://www.jetbrains.com/resharper/) is also used for improving code performance. 
* [AlphaFS](http://alphafs.alphaleonis.com/) is used for advanced .NET file operations.

and

* [Retro TV Logo Vector Graphics by Vecteezy!](https://www.vecteezy.com/vector-art/73089-retro-television) for our great logo
