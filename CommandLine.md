# TV Rename can be run from the command line too.

These are the options you need to know!

You can also send a commad line TV Rename to an already running TV Reanem to initiate activity

# Main Options
"/scan"
 * Asks TV Rename to do a scan

"/doall"
 * Asks TV Rename to do all actions it can (will also initiate a scan)

"/quit"
 * Can send a message to a 'headeless' TV Rename to ask it to quit

# Hidden Behaviour

"/hide"
 * Hides the UI and associated messageboxes from view 
 * defaults to not add missing directories (assuming below is not set)
 * exits once actions complete
 
"/unattended"
* Don't think it should really be needed anymore as hide does the same job

# Override options

"/createmissing"  

"/ignoremissing"  
 * gives options to ignore or create folders if they are missing                  
                  
"/norenamecheck"
 * allows user to request an already running TV Rename to do a scan with no renaming

 
# Settings Files
"/recover"
 * Force Recover will put up a dialog box that enables the user to recover a prior TVDB.xml or prior settiing file. (normally this error recovery dialog only comes up if the settings are corrupted)

"/userfilepath:BLAH"
* used to set a custom location for the settings files
