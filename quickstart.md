# Quickstart Guide

## Welcome to TVRename!

This guide will help you through the basics setting up TVRename.

To return to this page click `Quickstart Guide` in TVRename's **Help**, or browse to [www.tvrename.com/quickstart](http://www.tvrename.com/quickstart "Browse the TVRename Quickstart Guide online").

> This guide assumes you already have the beginings of a "Media Library" - *a directory (or directories) somewhere on your PC or NAS or whatever, with a bunch of TV Show files in it (them). TVRename doesn't care how these directories are organised, however to us mere humans probably one directory per show or one directory per show with one sub-directory per season makes sense in terms of being able to find stuff later.*

## Basic Setup

### 1. Add content from your "Media Library"...

 1. Go to **Tools>Folder Monitor** and whilst on the ***Folder*** tab click `Add`. Browse to the root directory of your "Media Library" and click `OK`. The path will be added to the ***Monitor Folders*** list.

 2. Click the `Check >>` button, the selected path will be scanned and any recognised content automatically displayed in the ***Scan Results*** tab.
 
 3. Whilst displaying the ***Scan Results*** tab click `Auto ID All` and TVRename will attempt to match the found content against [The TVDB](http://thetvdb.com "Visit thetvdb.com"), and try to guess if the show is part of a "flat" structure with all the episodes in one directory, or a "tree" structure with sub directories for each season and specials.
 Any shows not identified will not have an entry in the **Show** column or the **thetvdb code** column; these can be fixed manually by highlighting the relevant row in the table and clicking `Edit` and using `Search`. 

 4. Once the match process has finished, click `Add & Close` and all the identified shows will be added to the ***My Shows*** tab.
 ***My Shows*** should now be populated with the TV shows that have been recognised.
 
 5. You can manually add shows by clicking the `Add` button in the ***My Shows*** tab. Enter the show name, and click `Search` to find it. 
 Enter the base directory for the show in your Media Library, and select whether or not you have a sub-directory for each season.

 6. Go back to the ***My Shows*** tab and browse the series information. Here, by right clicking on a show, you can edit the show itself to control how TVRename manages it, or edit a season to override episode information fetched from TheTVDB.

### 2. Set your Preferences...

 1. In **Options>Preferences**, go to the ***Search Folders*** tab and set the download location for new episodes. Also visit **Options>Filename Template Editor** to set how TVRename will rename your files.

 2. Take a look at the other tabs in **Options>Preferences** to get an idea of the changes that can be made to TVRenames behaviour.
 
### 3. Run a Scan...

 1. Go to the ***Scan*** tab, and click `Full Scan`. TVRename will download any needed show information from [The TVDB](http://thetvdb.com "Visit thetvdb.com"), and examine the contents of the Media Library. Settings in **Options...** will change the behaviour here.

 2. When the scan is complete, review TVRename's suggested changes and other information. Any ticked item will be actioned, so untick things you're not sure about, or use the checkboxes in the bottom-right of the screen. Right-click on selected items for more actions.

 3. Click `Do Checked` and TVRename will process the ticked actions, moving and renaming files as necessary.

### 4. Ongoing Monitoring...

 1. Go to the ***When to watch*** tab to see all known future information (including airdates) of shows you are following.
 
 2. In the ***Search Folders*** tab of **Options>Preferences** ticking the "Monitor folders for changes" box will tell TVRename to automatically scan whenever a new file is made available.

### 5. Sit back, relax, and let TV Rename do all the hard work for you!

***Whatever changes TVRename makes to your recording names the season and episode details will remain untouched.***
*It is highly unlikely that the names of the recordings will get screwed up, but, should it happen, the problem should be easily fixable after changing some settings, or adding new rules. At least, that's the theory. :)*

*If all else fails using something like the (free for personal use) **[Bulk Rename Utility](http://www.bulkrenameutility.co.uk "Visit www.bulkrenameutility.co.uk")** should do the trick.*
