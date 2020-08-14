# Borderlands3ReadOnlyManager
A Borderlands 3 file manager that allows for one click set/unset read only.

## General
Applying read-only to Borderlands 3 files requires slightly more effort than in previous games. I created a GUI interface that allows you to apply and remove read only in one simple click...as opposed to the lengthy process of finding the file, opening properties, ticking the 'Read-only' box, hitting apply, going to the Security tab, selecting the target user, clicking 'Edit...', checking the Deny box, and finally clicking Apply/OK.

## How To Use
1) Download a release from [Releases](https://github.com/DexManly/Borderlands3ReadOnlyManager/tree/master/Releases)
2) Unzip
3) Open Borderlands3ReadOnlyManager.exe and fill out the initial settings you want to use. 
4) Use the main screen to Deny/Allow write access.

### Config
1) Folder - Path to your save folder. You can either set it in by hand or look for a folder in the folder explorer.
2) User - The user account you intend to deny write access to. The options will be provided based on the accounts that have permissions to the folder.
3) Hide settings - A handly setting if you intend to do some streaming and want to prevent people from know the inner workings of your PC (user name or file path information).
4) Click Save.

### Main Screen
- Use check boxes to deny write (checked) or allow write (unchecked). Only .sav files are shown in the grid. 
- Grid is updated on application focus. (This *may* need to change if bad performance is a thing for folks with a larger number of save files)
- Shows save file information meta data to help organize and find save files. Metadata includes:
    - Nick Name
    - Level
    - Class

![Main Screen Example](Images/bl3ReadOnlyManager_MainForm.jpg)

# Credits
* [FromDarkHell](https://github.com/FromDarkHell) for looking at how they read .sav files in [BL3ProfileEditor](https://github.com/FromDarkHell/BL3ProfileEditor). Specifically my Borderlands3SaveFile class closely resembles GVASData but rewritten a bit to use a BinaryReader because IO couldn't read metadata from a readonly file.
* [Gibbed](https://github.com/Gibbed) and [apocalyptech](https://github.com/apocalyptech) for providing the protobufs and encryption/decryption for character save files in [Borderlands3Protos](https://github.com/gibbed/Borderlands3Protos)