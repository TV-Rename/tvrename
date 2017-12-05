# Preferences Dialog

## General

The filename character replacements set what to use if the episode name has a character in it that isn't allowed in a Windows filename.

The "days count as recent" specifies how many days to show for "Aired in the last N days" in the "When to Watch" tab. The "Startup tab" is what TVRename shows when you open it. The "Default Naming Style" is what is used for new folders that you add, so if you are adding a lot, set it here first!

"Use sequential number matching" will match episodes based on their overall airing order. Because this causes a lot of false matches, it is off by default. For example, Seinfeld S08E02 is the 136th episode aired, so with this option "Seinfeld - 136 - The Soul Mate.avi" will be seen at S08E02.

"Find Extensions" sets the extensions of media files to look for. Separate them each with a semicolon, don't use spaces, and make sure you put the dot in!

![General Preferences](images/screenshots/Preferences-General.PNG)

| Field | Explanation |
|-------|-----------------|
| x | y |

## Files & Folders
![Files & Folders Preferences](images/screenshots/Preferences-FilesAndFolders.PNG)

| Field | Explanation |
|-------|-----------------|
| x | y |

## Automatic Export
"RSS Export" will save a RSS-reader compatible XML file to the location you specify. It can then be read by something like XBOX Media Center, or the Vista RSS Widget. You can limit how many days or shows are written to the file. The RSS file is updated whenever the "When to watch" tab is manually or automatically refreshed.

![Export Preferences](images/screenshots/Preferences-Export.PNG)

| Field | Explanation |
|-------|-----------------|
| x | y |

## Scan Options
![Scan Preferences](images/screenshots/Preferences-Scan.PNG)

| Field | Explanation |
|-------|-----------------|
| x | y |

## Folder Deleting
![Delete Preferences](images/screenshots/Preferences-Delete.PNG)

| Field | Explanation |
|-------|-----------------|
| x | y |


## Media Centre
This is were you configure the type of media player (and hence what additional files you need TV Rename to download)

![Media Centre Preferences](images/screenshots/Preferences-MediaCentre.PNG)

| Field | Explanation |
|-------|-----------------|
| x | y |

## Search Folders
![Search Preferences](images/screenshots/Preferences-Search.PNG)

| Field | Explanation |
|-------|-----------------|
| Monitor folders for changes | SHould the system automatically run a scan when a file in one of the Search Folders is modified? |
| Scan Type | If the system is monitoring the Search Folders then what [type of scan](userguide#scan-types) should be run when a file changes?  |
| Search Folders | Where should TV Rename look for missing episodes? It is also the list of folders that are monitored |

## uTorrent / NZB
![Torrent Preferences](images/screenshots/Preferences-Torrent.PNG)
| Field | Explanation |
|-------|-----------------|
| x | y |

## Colouring
![Tree Colouring Preferences](images/screenshots/Preferences-TreeColour.PNG)

| Field | Explanation |
|-------|-----------------|
| x | y |

# Other Settings
## File Name Processors
This screen tells TV Rename what filenames to look out for when searchign for a missing file. To really understand it you need to understand is [Regular Expressions](https://regexone.com/). Once you understand them you can see that this is a list of regular expressions that cature the season and episode number from the filename. Each one can work on just the filename "Show S01E07.avi", or on the whole filename "C:/files/showName/S01E02.avi".
  
 * Each can be toggled on and off
 * A folder can be specified to test the results of the Regular Expression
 * Each can have some notes to remind the user why that expression was added

![File Name Processors](images/screenshots/FileNameProcessors.PNG)
## Search Engines
![SearchEngines](images/screenshots/SearchEngines.PNG)

* {ShowName} - Name of the Show
* {Season} - Number of the season
* {Season:2} - Number of the season forced to 2 characters with a leading zero
* {Episode} - Number of the episode (within series)
* {Episode2} - Number of the episode (within series)
* {EpisodeName} - Name of the Episode
* {Number} - Overall number of the episode
* {Number:2} - Overall number of the episode forced to 2 characters with a leading zero
* {Number:3} - Overall number of the episode forced to 3 characters with leading zero(s)
* {ShortDate} - Airdate in d format
* {LongDate} - Airdate in D format
* {YMDDate} - Airdate in yyyy/MM/dd format
* {AllEpisodes} - ALl episodes - E01E02 etc
                           
## Template Editor
This is where we define the format of the filenames we rename to. It uses the same codes as above.

![Template Editor](images/screenshots/TemplateEditor.PNG)
