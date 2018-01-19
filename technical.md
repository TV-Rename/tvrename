#### The Technical Guide
***Oily rag and spanners at the ready? Right then, here we go!***

## [TV Rename and the Command&nbsp;Line](cmd-line "Read about Command Line functionality")
A number of TV Rename's functions can be accessed using the command line. You can find more details [here...](cmd-line "Read about Command Line functionality")

## The Registry
The only information TV Rename stores in the registry is installation information which is used by Windows "Apps and Features" to uninstall the program. But hey, why would you want to do that?

## Configuration Files
By default, all the configuration information and locally cached data from [TheTVDB](http://thetvdb.com "Visit TheTVDB.com") is stored in a series of XML files under the users Application folder (unless overridden using the CLI */userfilepath*{:.cli} option).

On Windows XP this is: -

> \\Documents and Settings\\\<user-name\>\\Application Data\\TVRename\\TVRename\\2.1\\

And on Windows Vista and later: -

> \\Users\\\<user-name\>\\AppData\\Roaming\\TVRename\\TVRename\\2.1\\

In either case the 2.1 folder contains the same set of files.

**If you wish to make a backup of your TV Rename setup a copy of this folder is all you need!**

It contains: -

**Layout.xml** - TV Rename's window position and size as well as column widths.

**Statistics.xml** - TV Rename's historical statistics.

**TheTVDB.xml** - The locally cached tvdb.com show season and episode information for everything listed in the ***My Shows*** tab.

**TheTVDB.xml.0 - TheTVDB.xml.9** - A maximum of ten backup copies of theTVDB.xml file. A new file gets created here every time you click ***File>Save*** on a first-in-first-out (FIFO) basis.

**TVRenameSettings.xml** - Everything else not mentioned is stored in here. All your shows, Media Library paths, folder structures and settings. As with TheTVDB.xml this is only overwritten when you choose **File>Save**.

**TVRenameSettings.xml.0 - TVRenameSettings.xml.9** - backup copies of TV RenameSettings.xml (FIFO).

## Log Files
Logging was added in version 2.3. The main log file (TVRename.log) can be found in: -
> \\Documents and Settings\\\<username\>\\AppData\\Roaming\\TVRename\\log\\

on Windows XP or: -

> \\Users\\\<username\>\\AppData\\Roaming\\TVRename\\log\\

On Windows Vista and later.

Log files are rotated into an "archive" folder (in the same location as TVRename.log) every time TV Rename is run or every 24 hours, whichever is sooner.

As with **TheTVDB.xml** and **TVRenameSettings.xml** a maximum of ten backup copies of the log file are kept (**TVRename00.log** - **TVRename09.log**) on a first-in-first-out (FIFO) basis.

Logging uses NLog. The configuration file ([NLog.config](https://github.com/TV-Rename/tvrename/blob/master/TVRename%23/NLog.config "Look at NLog.config in the TV-Rename Repo")) is stored in the TV Rename program folder.

You can read the [NLog Wiki](https://github.com/nlog/NLog/wiki/Configuration-file "Visit the NLog Wiki") for guidance on how to adjust the configuration file to collect more information.

When raising a bug please include a log file that illustrates the issue if you can, it will help us find a solution quickly.

## Source Code
You can find TV Rename's source code (along with executables and this website) in [The TV Rename GitHub Repository](https://github.com/TV-Rename/tvrename "Visit The repository").

## Development Links
 * You can find the Developers Wiki [here]({{ site.github.wiki_url }} "Read the Wiki")...
 * In addition there is a [Developers Forum in Google Groups](https://groups.google.com/forum/#!forum/tv-rename-development) which you can request access to. 
 * The legacy forum can be [accessed](http://old.tvrename.com/bbold/) in read-only mode for background and history about the project
 * If you select "Beta" mode in in **Options>Preferences** ***General*** you will be automatically notified of new Beta and Release Candidate releases as they are made available. Additionally all the releases can be found on [TV Rename's GitHub Releases page]({{ site.github.releases_url }} "Visit the TV Rename Releases page"). 

## Framework
TV Rename uses the Microsoft .NET Framework. The installer will check for its presence and let you know if any action is needed. It's a free download from [Microsoft](https://www.microsoft.com/net/download/windows "Get .NET").

## Credits
TV Rename pulls data from [TheTVDB.com](http://thetvdb.com/ "Visit TheTVDB.com") using their API. **Please visit their site, register, and help out by contributing information and artwork for TV Series and Episodes.**

It also uses: -
* [Json.NET](https://www.newtonsoft.com/json),
* [AlphaFS](http://alphafs.alphaleonis.com/) for advanced .NET file operations,
* [NLog](http://nlog-project.org/) open-source logging for .NET,
* [SourceGrid](https://sourcegrid.codeplex.com/),
* TVRename has seen significant speed improvements as a result of using: -
  * Red Gate's [ANTS Performance Profiler](https://www.red-gate.com/products/dotnet-development/ants-performance-profiler/ "Visit ANTS Performance Profiler pages")
  * JetBrains' [dotTrace](https://www.jetbrains.com/profiler  "Visit the Jetbrains Profiler pages")
  * JetBrains' [ReSharper](https://www.jetbrains.com/resharper "Visit the Jetbrains Resharper pages")

And thanks to:<br />
Vecteezy! Vector Art for our cool [Retro Television Logo](https://www.vecteezy.com/vector-art/73089-retro-television) - Vectors by [www.vecteezy.com](https://www.vecteezy.com).
