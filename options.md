#### Options and Preferences

![The Options Menu](images/options/menu-01.png){:.pic-l}
One of TV Rename's strongest (and most confusing) features is its configurability. For the newcomer especially, it's very easy to get "lost" in the plethora of options and preferences, and give up.

Luckily the "out-of-the-box" defaults work well, and should you wish to change any settings, all the stuff you can "fiddle with" is described here.

As you can see from the image above there are a number of options, each of which is discussed below.

{:.center}
[Offline Operation](#offline-operation)&nbsp;&#9670;&nbsp;[Automatic Background Download](#automatic-background-download)&nbsp;&#9670;&nbsp;[Preferences](#preferences "Delve into the preferences")

{:.center}
[Ignore List](#ignore-list)&nbsp;&#9670;&nbsp;[Filename Template Editor](#filename-template-editor)&nbsp;&#9670;&nbsp;[Search Engines](#search-engines)&nbsp;&#9670;&nbsp;[Filename Processors]()

## Offline Operation

{:.toplink}
[Return to Top]()

## Automatic Background Download

{:.toplink}
[Return to Top]()

## Preferences

![The Preferences Tabs](images/options/preferences-tabs-01.png){:.pic-l}
As can be seen from the image there are a large number of tabs in "Preferences", each of which is discussed in below.<br />

{:.center}
[Media Center](#media-center)&nbsp;&#9670;&nbsp;[Search Folders](#search-folders)&nbsp;&#9670;&nbsp;[&#181;Torrent / NZB](#&#181;torrent--nzb)&nbsp;&#9670;&nbsp;[Tree Coloring](#tree-coloring)

{:.center}
[General](#general)&nbsp;&#9670;&nbsp;[Files and Folders](#files-and-folders)&nbsp;&#9670;&nbsp;[Automatic Export](#automatic-export)&nbsp;&#9670;&nbsp;[Scan Options](#scan-options)&nbsp;&#9670;&nbsp;[Folder Deleting](#folder-deleting)

{:.toplink}
[Return to Top]()

### Media Center

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### Search Folders
![Preferences - the Search Folders tab](images/options/preferences-search-folders-02.png){:.pic-l}
The ***Search Folders*** tab is used to tell TV Rename where to look for TV show episode files BEFORE they are processed. Logical entries in here would be your downloads folder (if you download TV show episodes from the internet) or maybe your desktop (if you rip TV show episodes from DVD or Blu-ray). Or both!

Three buttons are available at the bottom of the tab. `Add` opens an explorer style window so you can browse to the folder location you wish to add and click `OK`, `Remove` removes a highlighted row from the panel and `Open` opens an explorer window targeting the row highlighted in the panel.

At the top of the tab the "Monitor folders for changes" tick box tell TV Rename to automatically check for new files in the identified locations and the "Scan Type" radio buttons tell TV Rename the type of scan to perform if anything is found by the folder monitor.

| *Defaults:* | "Folder Monitor" | ***Un-ticked*** |
|-------------|------------------|-----------------|
|             | "Scan type"      | ***Full***      |

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### &#181;Torrent / NZB

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### Tree Coloring
![Preferences - the Tree Coloring tab](images/options/preferences-tree-coloring-01.png){:.pic-l}
If you're like me and have a large catalogue of old TV Shows, the ***My&nbsp;Shows*** tab can be somewhat cluttered, but you don't want to remove anything from TV Rename. ***Tree Coloring*** to the rescue...

Tree coloring allows you to change the color of the text on the left hand pane of the ***My&nbsp;Shows*** tab depending on the status of the show or the show season.

To create a record:-
* Expand the "Status:" drop-down and select the status you wish to match from the list.
* Select a color to associate with the status by either entering a web-safe color name or code in the "Text Color:" box or clicking `Select Colour` and choosing from the pallet.
* Click the `Add` button and your new entry will appear the box above.

For example: to make finished shows less obtrusive in ***My&nbsp;Shows*** expand the "Status:" drop-down and select "Show Status: Ended", in the "Text Color:" box type "#808080" and click `Add`. Back in the  ***My&nbsp;Shows*** text for shows that have finished will be light grey and less obtrusive.

If you wish to remove a rule from the list just select it and click `Remove`.

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### General
![Preferences - the General tab](images/options/preferences-general-01.png){:.pic-l}
*When to watch* "X" *days count as recent*<br />
Specifies how many recent days are listed for the for "Aired in the last N days" section of the ***[When to Watch](userguide#when-to-watch "Read about When to Watch")*** tab.

*Default: **7 days***

*Double-click in When to Watch does:*<br />
Controls the double-click action in the ***[When to Watch](userguide#when-to-watch "Read about When to Watch")*** tab. Options are Search and Scan.

*Default: **Search***

{:.clear}
*Startup Tab:*<br />
Selects which tab you wish open when TV Rename loads. ***My&nbsp;Shows***, ***Search*** or ***When to Watch***.

*Default: **My Shows***

The *Show Notification Area* and *Show in Taskbar* interact, one of them **must** be ticked. If you try to un-tick both the option you are changing clears, but the other one automatically sets.

If *Show in Taskbar* is selected TV Rename's icon appears in the Windows taskbar.<br />
If *Show Notification Area* is selected TV Rename's icon appears in the System Tray. Right-clicking it will show an "Upcoming Shows" list, and double clicking restores the main window.<br />
Both boxes can be ticked, in which case you get both functionalities.

| *Defaults:* | "Show Notification Area" | ***Un-ticked*** |
|-------------|--------------------------|-----------------|
|             | "Show in Taskbar"        | ***Ticked***    |

*Show episode pictures in episode guides*<br />
Does what-it-says-on-the-tin, if ticked screen grabs from show episodes are displayed with the episode description in the ***My&nbsp;Shows*** tab. If un-ticked only the episode description is displayed.

*Default: **Ticked***

*Download up to "X" shows simultaneously from TheTVDB*<br />
Sets the number of concurrent connections to TheTVDB API. It can be set in the range to 1 to 8.

*Default: **4***

*Automatically select show and season in My&nbsp;Shows*<br />
If ticked this works for both the ***When to watch*** and ***Scan*** tabs. If an item is selected in either of these tabs the ***My&nbsp;Shows*** tab is automatically updated to highlight the indicated show and season.

*Default: **Ticked***

*Look for airdate in filenames*<br />
If ticked this provides a second method of identifying show episodes by looking for a date (in a number of formats) in the shows filename and comparing that against the air-date.

The supported date formats are: "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "yy-MM-dd", "dd-MM-yy" and "MM-dd-yy"

And the "date separators" can be any of: - / . , " " (a space)

*Default: **Un-ticked***

*Prefered language:*<br />
This option sets the language for returned data when requesting information from TheTVDB API. TV Rename will request "English" If the selected language is not available.

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### Files and Folders

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### Automatic Export
![Preferences - the Automatic Export tab](images/options/preferences-auto-export-01.png){:.pic-l}
Ticking the "RSS" box in the "When to watch"  section of the panel will save a RSS-reader compatible XML file to the location you specify (by typing or browsing). This file can then be read by something like XBOX Media Center, or a Windows  RSS App.

Ticking the "XML" box in the "When to watch" section of the panel will save a standard XML file to the location you specify (by typing or browsing).

In either case you can limit how many days or shows are written to the file. The files are updated whenever the "When to watch" tab is manually or automatically refreshed.

Ticking the XML" box in the "Missing" section of the panel, will save a standard XML containing the missing episodes detected during a scan to the location you specify (by typing or browsing)

*Default: **All un-ticked***

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### Scan options

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

### Folder Deleting

{:.toplink}
[Return to Preferences](#preferences)&nbsp;&#9670;&nbsp;[Return to Top]()

## Ignore List

{:.toplink}
[Return to Top]()

## Filename Template Editor
![Preferences - the Filename Template Editor tab](images/options/filename-template-editor-01.png){:.pic-l}
This is where the format of the filenames that TV Rename will rename to are defined.

To help illustrate the results the "Sample and Test:" panel contains the processed entries from the show and season selected in the ***My Shows*** tab.

The "Naming template:" text box displays a tokenised version of the filename which can be edited directly or populated using the "Tags:" drop-down, or overwritten with a record selected from the "Presets:" drop-down. Any changes in the "Naming Template:" are automatically reflected in the "Sample and Test:" panel.   

The available tags with their definitions are listed below: -

|{ShowName}   |Name of the Show                                                         |
|-------------|-------------------------------------------------------------------------|
|{Season}     |Number of the season                                                     |
|-------------|-------------------------------------------------------------------------|
|{Season:2}   |Number of the season forced to 2 characters with a leading zero          |
|-------------|-------------------------------------------------------------------------|
|{Episode}    |Number of the episode (within series), eg S01E03                         |
|-------------|-------------------------------------------------------------------------|
|{Episode2}   |Number of the second episode of a pair (within series), eg S01E04-E05    |
|             | created by the default S{Season:2}E{Episode}[-E{Episode2}])             |
|-------------|-------------------------------------------------------------------------|
|{EpisodeName}|Name of the Episode                                                      |
|-------------|-------------------------------------------------------------------------|
|{Number}     |Overall number of the episode                                            |
|-------------|-------------------------------------------------------------------------|
|{Number:2}   |Overall number of the episode forced to 2 characters with a leading zero |
|-------------|-------------------------------------------------------------------------|
|{Number:3}   |Overall number of the episode forced to 3 characters with leading zero(s)|
|-------------|-------------------------------------------------------------------------|
|{ShortDate}  |Air date in short format, eg 25/12/2017                                  |
|-------------|-------------------------------------------------------------------------|
|{LongDate}   |Air date in lomg format, eg 25 December 2017                             |
|-------------|-------------------------------------------------------------------------|
|{YMDDate}    |Air date in YMD format, eg 2017/12/25                                    |
|-------------|-------------------------------------------------------------------------|
|{AllEpisodes}|All episodes - E01E02 etc                                                |

| *Default:* | ***{ShowName} - S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}*** |
|------------|------------------------------------------------------------------------|
|            | (the second preset).                                                   |

{:.toplink}
[Return to Top]()

## Search Engines

## Filename Processors



##### Files & Folders
| Field | Explanation |
|-------|-----------------|
| x | The filename character replacements set what to use if the episode name has a character in it that isn't allowed in a Windows filename. |
| x | "Find Extensions" sets the extensions of media files to look for. Separate them each with a semicolon, don't use spaces, and make sure you put the dot in! |

##### Automatic Export

| Field | Explanation |
|-------|-----------------|
| x | y |

##### Scan Options

| Field | Explanation |
|-------|-----------------|
| x | y |

##### Folder Deleting
| Field | Explanation |
|-------|-----------------|
| x | y |


##### Media Centre
This is were you configure the type of media player (and hence what additional files you need TV Rename to download)
| Field | Explanation |
|-------|-----------------|
| x | y |

##### Search Folders
| Field | Explanation |
|-------|-----------------|
| Monitor folders for changes | SHould the system automatically run a scan when a file in one of the Search Folders is modified? |
| Scan Type | If the system is monitoring the Search Folders then what [type of scan](userguide#scan-types) should be run when a file changes?  |
| Search Folders | Where should TV Rename look for missing episodes? It is also the list of folders that are monitored |

##### uTorrent / NZB
| Field | Explanation |
|-------|-----------------|
| x | y |

##### Coloring
| Field | Explanation |
|-------|-----------------|
| x | y |

# Offline Operation
# Automatic Background Download
# Ignore List
# Filename Template Editor
# Search Engines
# Filename Processors



"Use sequential number matching" will match episodes based on their overall airing order. Because this causes a lot of false matches, it is off by default. For example, Seinfeld S08E02 is the 136th episode aired, so with this option "Seinfeld - 136 - The Soul Mate.avi" will be seen at S08E02.

The "Default Naming Style" is what is used for new folders that you add, so if you are adding a lot, set it here first!

# Season Settings

# Other Settings

## File Name Processors
This screen tells TV Rename what filenames to look out for when searchign for a missing file. To really understand it you need to understand is [Regular Expressions](https://regexone.com/). Once you understand them you can see that this is a list of regular expressions that cature the season and episode number from the filename. Each one can work on just the filename "Show S01E07.avi", or on the whole filename "C:/files/showName/S01E02.avi".

 * Each can be toggled on and off
 * A folder can be specified to test the results of the Regular Expression
 * Each can have some notes to remind the user why that expression was added

### AWAITING UPDATE
## Search Engines
### AWAITING UPDATE

