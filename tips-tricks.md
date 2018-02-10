#### Tips and Tricks

<!-- START TIPS FROM THE DEVELOPERS --------- -->
## Tips from the developers

* Most items can be double-clicked, to do the most "useful" thing for them.

* Folders, shows, and episodes can be right-clicked for menus of appropriate actions.

* Column headers can be clicked to sort by that column.

* Folder lists support "dragging and dropping" folders from Explorer.

* You can remove items from the finding, renaming and organising lists by selecting them, and pressing "Delete" on your keyboard.

* Selecting one of the main tabs by double-clicking will automatically do the "Find", "Check", or "Refresh" action associated with that tab.

* Changes you make are not saved unless you specifically do so. TV&nbsp;Rename will prompt you to save when you exit (if it needs to). If you screw something up, just exit without saving and TV&nbsp;Rename will forget everything you have done since the last save when it is reloaded.

* TV&nbsp;Rename stores all its data in JSON  files. For more information on what there is, and where it is read the [Technical Guide - Configuration Files](/manual/technical#configuration-files "Read the Technical Guide").

* A number of TV&nbsp;Rename's functions can be accessed using the command line. You can find more details [here...](/manual/cmd-line "Read about Command Line functionality").

* Whatever manipulation TV&nbsp;Rename applies to your files, it will never change the season or episode number of a show. Even if the show and episode names get corrupted the season and episode numbers will remain unchanged. This means that the problem should be fixable after changing some settings, or adding new rules. At least, that's the theory. :sunglasses:

{:.toplink}
[Return to Top]()
<!-- END TIPS FROM THE DEVELOPERS ----------- -->

<!-- START TIPS FROM OUR USERS -------------- -->
## Tips from our users

* You can change the behavior of some earlier versions of Windows 10 regarding mapped drives. In the registry under:
  
  **HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System**
  
  Create a new **DWORD (32 Bit)** named **EnableLinkedConnections** and set the value to **"1"**.  
  
  Restart Windows after making the change. All applications should now see the same mappings regardless of being in User or Admin mode.
  
  _**Jürgen**_

{:.toplink}
[Return to Top]()
<!-- END TIPS FROM OUR USERS ---------------- -->
