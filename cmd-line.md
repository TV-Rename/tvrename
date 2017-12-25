#### Command Line Interface.

### These are the options you need to know!

It TV Rename is already running using the cli will instruct that session to initiate new activity.

## Main Options

{:.cli}
/scan
* Tells TV Rename to run a scan.

{:.cli}
/doall
* Tells TV Rename execute all the actions it can (including a scan).

{:.cli}
/quit
* Tells a hidden TV Rename session to exit.

## Hidden Behaviour

{:.cli}
/hide
* Hides the User Interface and associated message boxes from view. 
* Defaults to not add missing directories (providing "/createmissing" is not set).
* Exits once actions complete.
 
{:.cli}
/unattended
* Depreciated - "/hide" has the same functionality.

## Override options

{:.cli}
/createmissing  
* Creates directories if they are missing.

{:.cli}
/ignoremissing  
* Ignore missing directories.                  
                  
{:.cli}
/norenamecheck
* Allows a request to an existing TV Rename session to scan without renaming.

 
# Settings Files

{:.cli}
/recover
* Recover will load a dialog box that enables the user to recover a prior TVDB.xml or TVRenameSettings.xml file. (Normally this dialog would only appear if the current settings are corrupted.)

{:.cli}
/userfilepath:BLAH
* Sets a custom directory path for the settings files.
