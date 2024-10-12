using GameServerCore.Content;
using LeaguePackets.Game;
using LeagueSandbox.GameServer.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServerLib.Content;

//I'll be using the 1.0.0.126 version of this class, as it appears the 4.20 version is way too over-engineered for my liking
//I'll keep what I did regarding the 4.20 at the bottom of this file, it isn't much though
internal class Cache
{
    internal static Cache Instance { get; } = new();
    internal readonly Dictionary<string, ContentFile> FileNameToFile = [];
    internal ContentFile LastAccessedFile;
    internal string LastAccessedFileName;

    private Cache()
    {
        FileNameToFile = [];
        LastAccessedFile = null!;
        LastAccessedFileName = "";
    }

    //Check
    private string CreateFullPath(string filename)
    {
        string path = "";
        if (FileNameToFile is not null) //?
        {
            path = Directory.GetCurrentDirectory();
            FileSysHelper.RelativeToAbsolutePath(path, out path);
        }
        FileSysHelper.FormatPath(ref path);
        return $"{path}/{filename}";
    }
    internal void PreloadFile(string fileName)
    {
        GetFile(fileName, false);
    }
    internal ContentFile? GetFile(string fileName, bool skipCache /*unused*/ = false)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        ContentFile? file;

        if (!skipCache) //Custom? Maybe I overlooked this check in the disassembly
        {
            if (fileName == LastAccessedFileName)
            {
                file = LastAccessedFile;
            }
            else if (FileNameToFile.TryGetValue(fileName, out file))
            {
                LastAccessedFile = file;
                LastAccessedFileName = fileName;
            }

            if (file is not null)
            {
                if (!file.m_fileHasBeenModified)
                {
                    return file;
                }
            }
        }

        string fullPath = CreateFullPath(fileName);
        FileSysHelper.ParseFileSpecification(fullPath, out string folder, out string name, out string extension);
        string keyVal = $"{folder}/defaults/{name}.{extension}";
        file = new(Path.GetFileName(fileName), fullPath, keyVal);
        FileNameToFile[fileName] = file;

        LastAccessedFile = file;
        LastAccessedFileName = fileName;
        return file;
    }
    internal void GetValue(out string returnValue, string pFileName, string pSection, string pName, string pDefault, bool skipCache)
    {
        ContentFile? eax = GetFile(pFileName, skipCache);
        if (eax is null)
        {
            returnValue = pDefault;
            return;
        }
        eax.GetValue(out returnValue, pFileName, pName, HashFunctions.HashStringSdbm(pSection, pName), pDefault);
    }
    internal void GetValue(out bool returnValue, string pFileName, string pSection, string pName, bool pDefault, bool skipCache)
    {
        ContentFile? eax = GetFile(pFileName, skipCache);
        if (eax is null)
        {
            returnValue = pDefault;
            return;
        }
        eax.GetValue(out returnValue, pFileName, pName, HashFunctions.HashStringSdbm(pSection, pName), pDefault);
    }
    internal void GetValue(out Vector4 returnValue, string pFileName, string pSection, string pName, Vector4 pDefault, bool skipCache)
    {
        ContentFile? eax = GetFile(pFileName, skipCache);
        if (eax is null)
        {
            returnValue = pDefault;
            return;
        }
        eax.GetValue(out returnValue, pFileName, pName, HashFunctions.HashStringSdbm(pSection, pName), pDefault);
    }
    internal void GetValue(out Vector3 returnValue, string pFileName, string pSection, string pName, Vector3 pDefault, bool skipCache)
    {
        ContentFile? eax = GetFile(pFileName, skipCache);
        if (eax is null)
        {
            returnValue = pDefault;
            return;
        }
        eax.GetValue(out returnValue, pFileName, pName, HashFunctions.HashStringSdbm(pSection, pName), pDefault);
    }
    internal void GetValue(out Vector2 returnValue, string pFileName, string pSection, string pName, Vector2 pDefault, bool skipCache)
    {
        ContentFile? eax = GetFile(pFileName, skipCache);
        if (eax is null)
        {
            returnValue = pDefault;
            return;
        }
        eax.GetValue(out returnValue, pFileName, pName, HashFunctions.HashStringSdbm(pSection, pName), pDefault);
    }
    internal void GetValue(out float returnValue, string pFileName, string pSection, string pName, float pDefault, bool skipCache)
    {
        ContentFile? eax = GetFile(pFileName, skipCache);
        if (eax is null)
        {
            returnValue = pDefault;
            return;
        }
        eax.GetValue(out returnValue, pFileName, pName, HashFunctions.HashStringSdbm(pSection, pName), pDefault);
    }
    internal void GetValue(out int returnValue, string pFileName, string pSection, string pName, int pDefault, bool skipCache)
    {
        ContentFile? eax = GetFile(pFileName, skipCache);
        if (eax is null)
        {
            returnValue = pDefault;
            return;
        }
        eax.GetValue(out returnValue, pFileName, pName, HashFunctions.HashStringSdbm(pSection, pName), pDefault);
    }
}

internal static class FileSysHelper
{
    internal static void ParseFileSpecification(string pIn, out string path, out string name, out string extension)
    {
        path = Path.GetDirectoryName(pIn) ?? "";
        name = Path.GetFileNameWithoutExtension(pIn);
        extension = Path.GetExtension(pIn)[1..];
    }

    internal static void RelativeToAbsolutePath(string relativePath, out string fullPath)
    {
        fullPath = Path.GetFullPath(relativePath);
        FormatPath(ref fullPath);
    }

    internal static void FormatPath(ref string path)
    {
        path = path.Replace('\\', '/');
    }

    internal static void ReplaceCharacter(ref string path, char from, char to)
    {
        path = path.Replace(from, to);
    }

    internal static void CreatePathDirectories(string path)
    {
        FormatPath(ref path);
        Directory.CreateDirectory(path);
    }
}


//internal class Cache
//{
//    Dictionary<string, LocalizedFilePair> FileNameToFilePairs;
//    LocalizedFilePair LastAccessedFilePair;
//    string LastAccessedFileName;
//    string PathBuf;
//    string NameBuf;
//    string ExtBuf;
//    string FullPathBuf;
//    string LocalizedFileNameBuf;
//    string FullFileNameBuf;
//    string LocalizedFullFileNameBuf;
//    string CurrentDirectory;
//    //Thread::Id m_owningThreadId;

//    class LocalizedFilePair
//    {
//        internal ContentFile m_defaultFile;
//        internal ContentFile m_localizedFile;
//    };

//    bool PreloadFile(string fileName)
//    {
//        ContentFile defaultFile = null;
//        LocalizedFilePair filePair = GetLocalizedFilePair(fileName, false);
//        if(filePair.m_localizedFile is not null)
//        {
//            defaultFile = filePair.m_defaultFile;
//            if (!defaultFile.m_TextFileExists)
//            {
//                return defaultFile.binaryCached;
//            }
//        }
//        return true;
//    }

//    LocalizedFilePair GetLocalizedFilePair(string fileName, bool unk)
//    {

//    }
//}
