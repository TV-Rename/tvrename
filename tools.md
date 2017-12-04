## Exporters
"RSS Export" will save a RSS-reader compatible XML file to the location you specify. It can then be read by something like XBOX Media Center, or the Vista RSS Widget. You can limit how many days or shows are written to the file. The RSS file is updated whenever the "When to watch" tab is manually or automatically refreshed.

![preferences](images/screenshots/Preferences-Export.PNG)

Below is an example of the RSS output being displayed in a Vista sidebar gadget. Clicking on each item shows the episode summary information.

![rsswidget2](images/UserGuide/rsswidget2.png)

## Actors Grid

![Actors Grid](images/screenshots/ActorsGrid.PNG)

## Buy Me A Drink!
I'm a thirsty man, so I need lots of drinks. :)

![buymeadrink](images/UserGuide/buymeadrink.png)

## Statistics
Just how useful has TVRename been? "Episodes on disk" is the number of episodes found the last time a "missing check" was done. "Total Episodes" is the count from the tv.com episode guide for all the shows you have.

![statistics](images/UserGuide/statistics.png)

## Torrent Match
The "Torrent Match" tab lets you rename files, so their names match what is in a .torrent file. This is done using the torrent hashes, so it will work on any type of file, as long as it is big enough for a partial hash to be done on it.

Choose the .torrent file, and folder of files to rename. If you choose a "Copy To" location, the files will be copied to their new names, leaving the originals intact. If it is off, then they will be renamed in place. 

![torrentmatch1](images/UserGuide/torrentmatch1.png)

Currently, TVRename can only process single file torrents, and multi-file torrents without subdirectories in them.

After clicking "Go", the torrent file will be processed. This can take a while if the torrent file has a lot of items in it, or there are a lot of potential matches in the "Folder" you have selected.

If processing is successful, you will be taken to the rename or finding and orgnising tab, to see the suggested operations. Your files will only be modified if you click "Rename" or "Move/Copy" from there.

![torrentmatch2](images/UserGuide/torrentmatch2.png)

It's not possible to rename the files in (a multiple file) torrent, to match what you have on disk. Changing the names affects the hash, which makes it a different torrent from the tracker's point of view. Because of this, TVRename renames the files on disk instead.
