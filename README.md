# Borderlands3ReadOnlyManager
A Borderlands 3 file manager that allows for one click set/unset read only.

## General
Applying read-only to Borderlands 3 files requires slightly more effort than in previous games. I created a GUI interface that allows you to apply and remove read only in one simple click...as opposed to the lengthy process of finding the file, opening properties, ticking the 'Read-only' box, hitting apply, going to the Security tab, selecting the target user, clicking 'Edit...', checking the Deny box, and finally clicking Apply/OK.

## How To Use
Setup is easy! Open the app and fill out the initial settings you want to use. Then, use the main screen to Deny/Allow write access for the configured settings.
### Config
1) Folder - Path to your save folder. You can either set it in by hand or look for a folder in the folder explorer.
2) User - The user account you intend to deny write access to. The options will be provided based on the accounts that have permissions to the folder.
3) Hide settings - A handly setting if you intend to do some streaming and want to prevent people from know the inner workings of your PC (user name or file path information).
4) Click Save.
### Main Screen
Use check boxes to deny write (checked) or allow write (unchecked). Only .sav files are shown in the grid. Grid is updated on application focus.
