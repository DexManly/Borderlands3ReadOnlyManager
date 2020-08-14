using Gibbed.Borderlands3.SaveFormats;
using OakSave;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Borderlands3ReadOnlyManager.HelperClasses
{
    public class Borderlands3SaveFile
    {
        private FileInfo _fileInfo;

        #region Constructor
        public Borderlands3SaveFile(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            _fileInfo = fileInfo;

            // Use reader to parse information
            using (BinaryReader br = new BinaryReader(File.OpenRead(fileInfo.FullName)))
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                string fileHeader = ascii.GetString(br.ReadBytes(4));
                
                if (fileHeader.Equals("GVAS"))
                    IsGSAVFile = true;
                else
                    return; // no need to parse any further

                SkipSomeMetaDataWeDontCareAbout(br);

                string saveGameType = ReadUEString(br); // Hopefully of type OakSaveGame or maybe OakProfile
                MetaData = GetFileMetaData(br, saveGameType);
            }
        }
        #endregion

        #region Public Properties
        public bool IsGSAVFile { get; set; } = false;

        public SaveFileMetaData MetaData { get; set; } = new SaveFileMetaData(); // New object should have some nice defaults in case meta data can't be obtained.

        public DateTime LastWriteTime => _fileInfo.LastWriteTime;

        public string FileName => _fileInfo.Name;
        #endregion

        #region Public Methods
        public bool IsFileReadOnly(string ntAccountName)
        {
            bool isReadOnly = false;

            if (_fileInfo.IsReadOnly) // need to check if user has denied write permissions
            {
                var fileAccessRules = _fileInfo.GetAccessControl().GetAccessRules(true, true, typeof(NTAccount));
                foreach (AuthorizationRule fileRule in fileAccessRules)
                {
                    if (fileRule.IdentityReference.Value.Equals(ntAccountName, StringComparison.CurrentCultureIgnoreCase))
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
        #endregion

        #region Private Helper Methods
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
