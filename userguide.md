This is a fairly brief overview of TVRename and how to use it. I'm not very good at writing documentation (and it's not something that's anywhere near as fun as writing code), so this may be somewhat brief. Any suggestions for improvements, or offers to write better documentation, are welcome. :)

**Note:** Some of this user guide is written based on the older, 2.0, version of the software. Most of the features are the same, or similar. The main changes have been to how you enter in your shows ("Shows and Folders"), and the use of thetvdb.com as the database - rather than tv.com.

If you need any further help, sign up to the forums and post a message there.

# Introduction
First we'll start with what you're going to end up with. This shows a number of shows we are currently watching. The first four in the list have aired, and the first two of those are on disk locally. We can also see what's coming up in the next few days, and further ahead. There is a episode summary for the currently selected episode, and a calendar showing when shows are airing. At the very bottom, in the status bar, is a countdown to the next show that is airing.

![When To Watch](images/screenshots/WhenToWatch.PNG)

# Beginner's Overview

See the **[Quick start guide](quickstart)** for further information on how to get TV Rename up and running 

# Tips and Tricks
* Most items can be double-clicked, to do the most "useful" thing for them.
* Folders, shows, and episodes can be right-clicked for appropriate actions.
* Column headers can be clicked to sort by that column.
* Folder lists support drag and drop of folders from Explorer, for quick and easy addition.
* You cen remove items from the renaming and finding and organising lists by selecting them, and pressing "Delete" on your keyboard.
* If you double-click on a tab to select it, it will automatically do the "Find", "Check", or "Refresh" action for that tab.
* After making changes, you need to save before exiting (TVRename will prompt you). If you screw something up, just exit without saving and your settings will be restored.
* TVRename stores a lot of data in XML files. See the files section for more information on what there is, and where it is.
* Whatever manipulation TVRename does to your files, it will never change the season or episode number. If the names get screwed up, the season and episode number will be unchanged, meaning that the problem should be (fairly easily) fixable after changing some settings, or adding new rules. At least, that's the theory. :)

# My Shows
This is where you tell TV Rename which shows you are interested in, and where they are stored on your computer. It can be a local drive, or a mapped network share.
In this image, you can see a number of shows, each of which can have a folder associated with it.

![Shows and Folders](images/screenshots/MyShows.PNG)

Buttons:
 * **Add** - Add a new show to the list. Remember to add folders to it separately if you need to monitor them it on disk as well. You don't need to add a folder if you only want its airdate to show up on the "When to Watch" tab.
 * **Edit** - Edit the currently selected show.
 * **Delete** - Remove the currently selected show(s).
 * **Filter** - Filter the listed shows base don entered criteria.
 * Expand all
 * Hide Details Panel
 * **Open** - Opens the folder for the TV show.
 * **Visit tvdb.com** - Visit the tv.com page for this season of this show.

## Add/Edit Show
Clicking on the "Add" or "Edit" button for a show, in the "Shows and Folders" tab, opens this dialog.

![addeditshow](images/UserGuide/addeditshow.png)
![tvcomsearch](images/UserGuide/tvcomsearch.png)
	
In this example, we are setting up TVRename for the show "Grey's Anatomy", season 4.

The first thing is entering the **tv.com code**. If you know it, you can type it in directly. Otherwise, type part of the show's name (e.g. "Grey"). If it still doesn't show, click on "Search" and TVRename will search for what you typed on tv.com. Downloaded results from searching are cached locally for future use.

You can pre-load the cache for tv.com codes by using the "Update Codes" option on the "Tools" menu. This will download from TVRename's website data for about 14000 shows, but it is only updated very infrequently.

The **show name** is what you like to call the show yourself. You may like to remove "The" from names, or extra info like the year that tv.com puts on some shows. If you click **Copy**, it will keep it the same as tv.com's naming.

You enter the season number into the **season** field. The **timezone** is the timezone that the airdate times on the tv.com summary page are in. Most shows are Eastern USA timezone, but British shows (e.g. Jekyll) will be British Standard Time. This only has an effect on the "When to Watch" display, which then translates those times into your computer's timezone.

**Show next airdate** chooses if this show will appear in the "When to Watch" tab. tv.com also can include pilots, specials, and TV movies in their episode guide. If you don't check these three checkboxes, they will be ignored completely. If you check the checkbox, it will be counted as an episode. For example, Mythbusters often counts a special as an episode.

The Old option means that once data is downloaded from tv.com, and locally cached, it will never be downloaded again. (See the "Mark Old Shows" dialog, too.) Otherwise, TVRename re-downloads data from tv.com for a show depending on how long it was since the last episode, and if the next episode's airdate is known.

The rules section lets you manipulate the tv.com episode guide to suit how you have the episodes on your computer. The rules are applied in order, from top to bottom, it is possible to use the Up and Down buttons to re-prioritise them. Add, edit, and Delete will alter the rules list.

In this example, episodes 16 and 17 were aired as a double episode, meaning you have only one file on disk for both. The rule merges the two episodes into one. This affects the display in the "Episode Guide" tab, as well as what is checked for and how the files are renamed.

## Add/Modify Rule
This is the dialog for adding or editing a rule for a show's season. Choose the operation at the top, then enter the appropriate values below.

![addmodifyrule](images/UserGuide/addmodifyrule.png)


* **Ignore** - Keep the specified episode in the guide, but don't check for it (or rename it) on disk locally.
* **Rename** - Manually set the name of an episode.
* **Remove** - Make a an episode disappear. All episodes above will be renumbered down to fill the gap.
* **Swap** - Swap the position of two episodes.
* **Merge** - These episodes are all on disk locally as one multi-episode file.
* **Insert** - Manually add an episode into the season. Episodes after are renumbered to accomodate it.
* **Split** - Turn one episode into many, renumbering episode after the split to accomodate them.
After adding a rule, you can go to the "Episode Guide" tab, select the show, and then click "Refresh". You will then be able to see (and check) the effects of the rules you've created.

## Episode Guide
Once you have set up your shows, you can visit the episode guide to see the information from tvdb.com, after modification by any rules you may have added.

Select the show from the combo box. If you've recently edited the show in the "shows and folders" tab, the display may be empty. In this case, click on the Refresh button.

"Visit TVDB" will open your web browser on the tv.com page for this season of this show. Clicking on the show name will take you to the show summary page.

The rightmost button lets you choose your preferred torrent search engine. This is used when you click on one of the "Search" links in the episode guide.

If TVRename has found the corresponding episode on disk, a watch link will be displayed. That will open the video file in the associated Windows movie player.

The episode guide also includes indication of whether or not the show has been aired, or how long until it airs. The "time to do" display is adjusted from the timezone on tv.com's page, to that of your computer.

![episodeguide](images/UserGuide/episodeguide.png)

# Scanning

TV Rename will scan your shows and look for missing/outdated files. For anything missing/wrong it will try and fix the issues automatically 

## Scan Types

There are 3 types of scan you can choose from :

 1. **Full** - A full scan of all shows
 2. **Recent** - Scan of all the shows that have aird in the last 7 days
 3. **Quick** - Scan just the shows that are in the last 7 days and have a missing episode on disk. Plus any shows that match a media file in the download directory.

## Scan Results

This shows you where ther are gaps in your collection. Click the check button, and TVRename will look through your folders and list what you are missing.

The "arrow-down" button in the bottom left lets you choose your preferred torrent search engine. Clicking on the button in the bottom left will search for the currently selected missing episode(s) on that site. Double-clicking an item in the list will also search.

![Scan Results](images/screenshots/ScanResults.PNG)


After doing a "missing" check, "finding and organising" will search, on your computer, for those missing files.

Add a number of "search folders". Either use the "Add" button, or drag and drop folders from Windows Explorer into this list. The "Open" button will open an Explorer window for the selected folder. Folders added to this list automatically have their subfolders searched.

Press the "Find" button to search for missing episodes. TVRename will list the episodes it found, and the appropriate move (on the same device) or copy (across different devices) operation to get them to where they should be. If you choose "Leave Originals", it will always copy the files to their new location.

Once you are satisfied with the list of things to do, click on "Move/Copy" and TVRename will do it.

While files are being copied and/or moved, the dialog below is shown. Press "Pause" to temporarily pause the copy/move operation. Click it again to resume. "Cancel" will stop immediately. The disk space shown is for the drive that the current file is being copied/moved to.

![copymoveprogress](images/UserGuide/copymoveprogress.png)

Click on the check button, and TVRename will, after downloading any needed information from tv.com, go through your folders and see if any files need to be renamed.

TVRename attempts to intelligently determine a show's season and episode number from it's filename, and handles most common naming styles.

A folder can be excluded from a rename check by setting rename files to no in the add/edit folder dialog, accessed from the shows and folders tab.

You can select items in the list and press the Delete key on your keyboard to remove them from the list. Once you are happy with the changes offered, click the Rename button at the bottom, and TVRename will make the changes.

# When to Watch
For shows which have the "Show next airdate" option set, they will be listed here if tv.com has airdate information available. The time and date are adjusted to be in the timezone that you have Windows set to be.

Click on the columns to sort by them. Right-click on items to do useful related actions. For shows that have aired, an icon is shown to indicate if it is on disk (double-clicking will open it), or needs to be searched for. Double-clicking a item that isn't on disk will open the specified torrent search engine for it.

Click on the calendar to see what is airing on a particular day. Dates with shows airing are in bold. Click on a show to see its episode summary below. The refresh button will make sure that the information is up-to-date.

"Aired in the last N days" and "Next 7 days" are self-explanatory. "Later" shows the next airing episode of any show you're interested in that isn't in the next week. "Future Episodes" are all known episodes after anything in the two preceding categories.

You can turn this grouping off by clicking on the "How Long" column header, and on by clicking on "Air Date".

The "When to Watch" display is automatically refreshed from time to time, and TVRename will download in the background any updates needed from tv.com. Background downloading can be disabled from the Options menu, and will also be disabled if you're in Offline Mode.

![watch](images/UserGuide/whentowatch.png)

## Tools

See the **[Tools](tools)** section for further information on some of the additional tools and features of the software

## Preferences

See the **[Settings Guide](settings)** for further information on how to configure TV Rename


