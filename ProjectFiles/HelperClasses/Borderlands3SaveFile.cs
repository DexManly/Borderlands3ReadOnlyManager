using Gibbed.Borderlands3.SaveFormats;
using OakSave;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borderlands3ReadOnlyManager.HelperClasses
{
    public class Borderlands3SaveFile
    {
        private FileInfo _fileInfo;
        private string _ntAccountName;
        private SaveFileMetaData _metaData = new SaveFileMetaData();
        private bool _isReadOnly;

        #region Constructor
        public Borderlands3SaveFile(FileInfo fileInfo, string ntAccountName, List<string> isHotkeyList)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            _fileInfo = fileInfo;

            if (string.IsNullOrWhiteSpace(ntAccountName)) throw new ArgumentNullException(nameof(ntAccountName));
            _ntAccountName = ntAccountName;

            _isReadOnly = IsFileReadOnly();

            HotKeyEnabled = (isHotkeyList != null && isHotkeyList.Contains(fileInfo.Name));

            ReadFileLoop();
        }
        #endregion

        #region Public Properties
        public string FileName => _fileInfo.Name;

        public bool IsGSAVFile { get; set; } = false;

        public string NickName => _metaData.NickName;

        public int? PlayerLevel => _metaData.PlayerLevel;

        public string ClassName => _metaData.ClassName;

        public DateTime LastWriteTime => _fileInfo.LastWriteTime;

        public bool IsReadOnly {
            get { return _isReadOnly; }
            set 
            { 
                if (value != _isReadOnly)
                {
                    FlipReadOnly();
                }
            }
        }

        public bool HotKeyEnabled { get; set; } = false;
        #endregion

        #region Public Methods
        public void ReReadFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            ReadFileLoop();
        }
        #endregion

        #region Private Helper Methods
        private void FlipReadOnly()
        {
            if (_isReadOnly)
            {
                RemoveReadOnlyOnFile();
            }
            else
            {
                SetReadOnlyForFile();
            }

            _isReadOnly = !_isReadOnly;
        }

        private void SetReadOnlyForFile()
        {
            try
            {
                _fileInfo.IsReadOnly = true;
                FileSecurity fs = new FileSecurity();
                fs.AddAccessRule(new FileSystemAccessRule(_ntAccountName, FileSystemRights.Write, AccessControlType.Deny));
                _fileInfo.SetAccessControl(fs);
            }
            catch
            {
            }
        }

        private void RemoveReadOnlyOnFile()
        {
            try
            {
                FileSecurity fs = new FileSecurity();
                fs.RemoveAccessRule(new FileSystemAccessRule(_ntAccountName, FileSystemRights.Write, AccessControlType.Deny));
                _fileInfo.SetAccessControl(fs);
                _fileInfo.IsReadOnly = false;
            }
            catch
            {
            }
        }
        
        private bool IsFileReadOnly()
        {
            bool isReadOnly = false;

            if (_fileInfo.IsReadOnly) // need to check if user has denied write permissions
            {
                var fileAccessRules = _fileInfo.GetAccessControl().GetAccessRules(true, true, typeof(NTAccount));
                foreach (AuthorizationRule fileRule in fileAccessRules)
                {
                    if (fileRule.IdentityReference.Value.Equals(_ntAccountName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        //Cast to a FileSystemAccessRule to check for access rights
                        var filesystemAccessRule = (FileSystemAccessRule)fileRule;

                        if (filesystemAccessRule.FileSystemRights == FileSystemRights.Write && filesystemAccessRule.AccessControlType == AccessControlType.Deny)
                        {
                            isReadOnly = true;
                        }
                    }
                }
            }

            return isReadOnly;
        }

        // Loop because some times the file is in use by other applications and we can't read
        private void ReadFileLoop()
        {
            int retryTimes = 3;
            TimeSpan sleepyTime = TimeSpan.FromSeconds(.25);

            for (int i = 0; i < retryTimes; i++)
            {
                try
                {
                    // Use reader to parse information
                    using (BinaryReader br = new BinaryReader(File.OpenRead(_fileInfo.FullName)))
                    {
                        ASCIIEncoding ascii = new ASCIIEncoding();
                        string fileHeader = ascii.GetString(br.ReadBytes(4));

                        if (fileHeader.Equals("GVAS"))
                            IsGSAVFile = true;
                        else
                            return; // no need to parse any further

                        SkipSomeMetaDataWeDontCareAbout(br);

                        string saveGameType = ReadUEString(br); // Hopefully of type OakSaveGame or maybe OakProfile
                        _metaData = GetFileMetaData(br, saveGameType);
                    }
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("sleepyTime");
                    Thread.Sleep(sleepyTime);
                }
            }
        }

        private void SkipSomeMetaDataWeDontCareAbout(BinaryReader br)
        {
            br.ReadBytes(18); // Skip some version info
            ReadUEString(br); // buildId
            ReadCustomFormats(br);
        }
        
        private string ReadUEString(BinaryReader br)
        {
            if (br.PeekChar() < 0) return null;
            int length = br.ReadInt32();
            if (length == 0) return null;
            if (length == 1) return "";
            var valueBytes = br.ReadBytes(length);
            Encoding Utf8 = new UTF8Encoding(false);
            return Utf8.GetString(valueBytes, 0, valueBytes.Length - 1);
        }

        private void ReadCustomFormats(BinaryReader br)
        {
            br.ReadInt32(); // Custom Format Version
            int custFormatCount = br.ReadInt32();
            for (int i = 0; i < custFormatCount; i++)
            {
                br.ReadBytes(16); // Guid
                br.ReadInt32(); // int
            }
        }

        private SaveFileMetaData GetFileMetaData(BinaryReader br, string sgType)
        {
            SaveFileMetaData saveFileMetaData = new SaveFileMetaData();
            // if we have a GSAV but it isn't a OakSaveGame...return unassigned SaveFileMetaData. Covers case of profile.sav
            if (!sgType.Equals("OakSaveGame")) return saveFileMetaData;

            int remainingDataLength = br.ReadInt32();

            byte[] buf = br.ReadBytes(remainingDataLength);
            SaveBogoCrypt.Decrypt(buf, 0, remainingDataLength);
            Character characterSave = Serializer.Deserialize<Character>(new MemoryStream(buf));

            saveFileMetaData.PlayerLevel = characterSave.GameStatsDatas.FirstOrDefault(a => a.StatPath.Equals(@"/Game/PlayerCharacters/_Shared/_Design/Stats/Character/Stat_Character_Level.Stat_Character_Level")).StatValue;
            saveFileMetaData.NickName = characterSave.PreferredCharacterName;
            //characterSave.GameStatsDatas.ForEach(a => Console.WriteLine(a.StatPath + " " + a.StatValue));
            switch (characterSave.PlayerClassData.PlayerClassPath)
            {
                case @"/Game/PlayerCharacters/Operative/PlayerClassId_Operative.PlayerClassId_Operative":
                    saveFileMetaData.ClassName = "Operative";
                    break;
                case @"/Game/PlayerCharacters/Beastmaster/PlayerClassId_Beastmaster.PlayerClassId_Beastmaster":
                    saveFileMetaData.ClassName = "Beastmaster";
                    break;
                case @"/Game/PlayerCharacters/SirenBrawler/PlayerClassId_Siren.PlayerClassId_Siren":
                    saveFileMetaData.ClassName = "Siren";
                    break;
                case @"/Game/PlayerCharacters/Gunner/PlayerClassId_Gunner.PlayerClassId_Gunner":
                    saveFileMetaData.ClassName = "Gunner";
                    break;
                default:
                    break;
            }

            return saveFileMetaData;
        } 
        #endregion
    }
}
