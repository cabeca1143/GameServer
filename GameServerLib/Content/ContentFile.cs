using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GameServerCore.Content;
using INIParser;
using LeagueSandbox.GameServer.Logging;
using log4net;

namespace LeagueSandbox.GameServer.Content;

public class ContentFile
{
    private static readonly ILog _logger = LoggerProvider.GetLogger();
    public string Name { get => Path.GetFileNameWithoutExtension(m_MainFileName); }

    internal Dictionary<ulong, string> m_SectionAndNameToValue;
    internal string m_MainFileName;
    internal string m_DefaultsFileName;
    internal long m_modifiedTime;
    internal bool m_fileHasBeenModified;

    internal List<List<ulong>> mHashMaps;
    internal List<int> Int32List;
    internal List<float> Float32List;
    internal List<byte> FixedPointFloatList;
    internal List<short> Int16List;
    internal List<byte> Int8List;
    internal List<bool> BitList;
    internal List<ByteListVec3> FixedPointFloatListVec3;
    internal List<Vector3> Float32ListVec3;
    internal List<ByteListVec2> FixedPointFloatListVec2;
    internal List<Vector2> Float32ListVec2;
    internal List<ByteListVec4> FixedPointFloatListVec4;
    internal List<Vector4> Float32ListVec4;
    internal List<ushort> StringOffsetList;
    internal List<BinaryINIEntryHeader> entryHeaders;
    internal List<char> entryData;

    internal bool binaryCached;
    internal bool binaryIsDefault;
    internal bool m_TextFileExists;
    string mRelativePath;
    string mRelativeDefaultPath;
    uint mFlags;

    public IniFile? INI { get; private set; }
    internal ContentFile(string path) : this(Path.GetFileName(path), path, $"{Path.GetDirectoryName(path)}/defaults/{Path.GetFileName(path)}") { }
    internal ContentFile(string fileName, string path, string defaultsPath)
    {
        //?
        m_MainFileName = fileName;
        m_DefaultsFileName = path;

        m_fileHasBeenModified = false;
        m_SectionAndNameToValue = [];
        mHashMaps = [];
        Int32List = [];
        Float32List = [];
        FixedPointFloatList = [];
        Int16List = [];
        Int8List = [];
        BitList = [];
        FixedPointFloatListVec3 = [];
        Float32ListVec3 = [];
        FixedPointFloatListVec2 = [];
        Float32ListVec2 = [];
        FixedPointFloatListVec4 = [];
        Float32ListVec4 = [];
        StringOffsetList = [];
        entryHeaders = [];
        entryData = [];
        //var_4 = 0x12;
        binaryIsDefault = false;
        m_TextFileExists = false;

        for (int i = 0; i < (int)TypeGroupIndex.TYPE_INDEX_NumOf; i++)
        {
            mHashMaps.Add([]);
        }

        bool binaryFileV2 = TryToLoadBinaryFileV2(defaultsPath);
        binaryCached = binaryFileV2;
        if (!binaryFileV2)
        {
            bool v10 = TryToLoadBinaryFileV2(path);
            binaryIsDefault = v10;
            binaryCached = v10;

            m_modifiedTime = new FileInfo(m_MainFileName).LastWriteTime.Ticks;
            bool textFile = TryToLoadTextFile(defaultsPath, 0);
            bool v14 = TryToLoadTextFile(path, 1);
            m_TextFileExists = textFile || v14;
            mRelativePath = defaultsPath;
            mRelativeDefaultPath = path;
        }
    }

    #region Custom Stuff so I don't need to refactor the functions used all around the project
    internal bool TryToLoadTextFile(string path, uint flags)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        INI = new(path);

        foreach (string section in INI.Sections)
        {
            foreach (string entry in INI.GetKeys(section))
            {
                m_SectionAndNameToValue.Add(HashFunctions.HashStringSdbm(section, entry), INI[section, entry]!);
            }
        }

        return true;
    }
    internal bool HasMentionOf(string section, string name)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        return mHashMaps.Any(x => x.Any(y => y == hash));
    }
    internal int[] GetIntArray(string section, string name, int[] defaultValue)
    {
        GetValue(section, name, out string? str, null!);
        if (str == null)
        {
            return defaultValue;
        }
        return Array.ConvertAll(str.Split(" "), int.Parse);
    }
    internal float[] GetFloatArray(string section, string name, float[] defaultValue)
    {
        GetValue(section, name, out string? str, null!);
        if (str == null)
        {
            return defaultValue;
        }
        return Array.ConvertAll(str.Split(" "), float.Parse);
    }
    internal float[] GetMultiFloat(string section, string name, int size, float defaultValue)
    {
        GetValue(section, name, out string? str, null!);
        if (str == null)
        {
            float[] arr = new float[size];
            Array.Fill(arr, defaultValue);
            return arr;
        }
        float[] toReturn = Array.ConvertAll(str.Split(" "), float.Parse);
        Array.Resize(ref toReturn, size);
        return toReturn;
    }
    internal int[] GetMultiInt(string section, string name, int size, int defaultValue)
    {
        GetValue(section, name, out string? str, null!);
        if (str == null)
        {
            int[] arr = new int[size];
            Array.Fill(arr, defaultValue);
            return arr;
        }
        int[] toReturn = Array.ConvertAll(str.Split(" "), int.Parse);
        Array.Resize(ref toReturn, size);
        return toReturn;
    }
    internal bool GetBool(string section, string name, bool defaultValue = false)
    {
        GetValue(section, name, out bool returnValue, defaultValue);
        return returnValue;
    }
    internal float GetFloat(string section, string name, float defaultValue = 0)
    {
        GetValue(section, name, out float returnValue, defaultValue);
        return returnValue;
    }
    internal int GetInt(string section, string name, int defaultValue = 0)
    {
        GetValue(section, name, out int returnValue, defaultValue);
        return returnValue;
    }
    internal string GetString(string section, string name, string defaultValue = "")
    {
        GetValue(section, name, out string returnValue, defaultValue);
        return returnValue;
    }

    internal string GetValue(string section, string name, string defaultValue = "")
    {
        GetValue(section, name, out string returnValue, defaultValue);
        return returnValue;
    }
    internal Vector4 GetValue(string section, string name, Vector4 defaultValue = default)
    {
        GetValue(section, name, out Vector4 returnValue, defaultValue);
        return returnValue;
    }
    internal Vector3 GetValue(string section, string name, Vector3 defaultValue = default)
    {
        GetValue(section, name, out Vector3 returnValue, defaultValue);
        return returnValue;
    }
    internal Vector2 GetValue(string section, string name, Vector2 defaultValue = default)
    {
        GetValue(section, name, out Vector2 returnValue, defaultValue);
        return returnValue;
    }
    internal bool GetValue(string section, string name, bool defaultValue = false)
    {
        GetValue(section, name, out bool returnValue, defaultValue);
        return returnValue;
    }
    internal float GetValue(string section, string name, float defaultValue = 0)
    {
        GetValue(section, name, out float returnValue, defaultValue);
        return returnValue;
    }
    internal int GetValue(string section, string name, int defaultValue = 0)
    {
        GetValue(section, name, out int returnValue, defaultValue);
        return returnValue;
    }
    #endregion
    #region Reverse Engineered Stuff
    internal bool TryToLoadBinaryFileV2(string relativePatha)
    {
        relativePatha += "bin";
        if (!File.Exists(relativePatha))
        {
            return false;
        }

        using BinaryReader file = new(File.OpenRead(relativePatha));
        byte[] schnoz = fread(3, 1, file);
        if (schnoz[0] != 2)
        {
            _logger.Warn($"ConfigFile: Old version needs to be recompiled: {m_MainFileName}");
            return false;
        }

        short flags = BitConverter.ToInt16(fread(2, 1, file));
        if ((flags & 1) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Int32List], Int32List);
        }
        if ((flags & 2) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32List], Float32List);
        }
        if ((flags & 4) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatList], FixedPointFloatList);
        }
        if ((flags & 8) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Int16List], Int16List);
        }
        if ((flags & 0x10) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Int8List], Int8List);
        }
        if ((flags & 0x20) != 0)
        {
            ReadListBitSet(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_BitList], BitList);
        }
        if ((flags & 0x40) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatListVec3], FixedPointFloatListVec3);
        }
        if ((flags & 0x80) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32ListVec3], Float32ListVec3);
        }
        if ((flags & 0x100) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatListVec2], FixedPointFloatListVec2);
        }
        if ((flags & 0x200) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32ListVec2], Float32ListVec2);
        }
        if ((flags & 0x400) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatListVec4], FixedPointFloatListVec4);
        }
        if ((flags & 0x800) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32ListVec4], Float32ListVec4);
        }
        if ((flags & 0x1000) != 0)
        {
            ReadList(file, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_StringList], StringOffsetList);
        }

        //int16_t eax_15 = *(uint16_t*)((char*)esp_9 + 0x1d);
        //if (eax_15 >= 0)
        //UNKNOWN: CHECK
        short remainingBytes = (short)(file.BaseStream.Length - file.BaseStream.Position);
        if (remainingBytes >= 0)
        {
            entryData = fread(1, (uint)remainingBytes, file).Select(x => (char)x).ToList();
        }
        else
        {
            entryData = null;
            _logger.Warn($"Corrupt Binary header! {relativePatha}");
        }

        return true;
        //ConfigFileR.ReadList()
    }

    internal void GetValue(out string returnValue, string section, string name, uint hashedSectionAndName, string defaultValue)
    {
        //TODO: Finish reverse engineering this
        if (!GetValueFromBinary(hashedSectionAndName, out returnValue))
        {
            returnValue = defaultValue;
        }
    }
    internal void GetValue(out Vector4 returnValue, string section, string name, uint hashedSectionAndName, Vector4 defaultValue)
    {
        //TODO: Finish reverse engineering this
        //if (!GetValueFromBinary(hashedSectionAndName, out returnValue))
        {
            returnValue = defaultValue;
        }
    }
    internal void GetValue(out Vector3 returnValue, string section, string name, uint hashedSectionAndName, Vector3 defaultValue)
    {
        //TODO: Finish reverse engineering this
        if (!GetValueFromBinary(hashedSectionAndName, out returnValue))
        {
            returnValue = defaultValue;
        }
    }
    internal void GetValue(out Vector2 returnValue, string section, string name, uint hashedSectionAndName, Vector2 defaultValue)
    {
        //TODO: Finish reverse engineering this
        if (!GetValueFromBinary(hashedSectionAndName, out returnValue))
        {
            returnValue = defaultValue;
        }
    }
    internal void GetValue(out bool returnValue, string section, string name, uint hashedSectionAndName, bool defaultValue)
    {
        //TODO: Finish reverse engineering this
        if (!GetValueFromBinary(hashedSectionAndName, out returnValue))
        {
            returnValue = defaultValue;
        }
    }
    internal void GetValue(out float returnValue, string section, string name, uint hashedSectionAndName, float defaultValue)
    {
        //TODO: Finish reverse engineering this
        if (!GetValueFromBinary(hashedSectionAndName, out returnValue))
        {
            returnValue = defaultValue;
        }
    }
    internal void GetValue(out int returnValue, string section, string name, uint hashedSectionAndName, int defaultValue)
    {
        //TODO: Finish reverse engineering this
        if (!GetValueFromBinary(hashedSectionAndName, out returnValue))
        {
            returnValue = defaultValue;
        }
    }
    internal void GetValue(string section, string name, out string returnValue, string defaultValue)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        GetValue(out returnValue, section, name, hash, defaultValue);
    }
    internal void GetValue(string section, string name, out Vector4 returnValue, Vector4 defaultValue)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        GetValue(out returnValue, section, name, hash, defaultValue);
    }
    internal void GetValue(string section, string name, out Vector3 returnValue, Vector3 defaultValue)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        GetValue(out returnValue, section, name, hash, defaultValue);
    }
    internal void GetValue(string section, string name, out Vector2 returnValue, Vector2 defaultValue)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        GetValue(out returnValue, section, name, hash, defaultValue);
    }
    internal void GetValue(string section, string name, out bool returnValue, bool defaultValue)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        GetValue(out returnValue, section, name, hash, defaultValue);
    }
    internal void GetValue(string section, string name, out float returnValue, float defaultValue)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        GetValue(out returnValue, section, name, hash, defaultValue);
    }
    internal void GetValue(string section, string name, out int returnValue, int defaultValue)
    {
        uint hash = HashFunctions.HashStringSdbm(section, name);
        GetValue(out returnValue, section, name, hash, defaultValue);
    }
    internal bool GetValueFromBinary(uint hash, out float returnValue)
    {
        if (LookupRawFloat(hash, out returnValue))
        {
            return true;
        }
        if (LookupRawInt(hash, out int intVal))
        {
            returnValue = intVal;
            return true;
        }
        if (LookupRawBool(hash, out bool boolVal))
        {
            returnValue = boolVal ? 1 : 0; //0x3f800000 : 0 ???
            return true;
        }
        if (LookupString(hash, out string strVal))
        {
            if (float.TryParse(strVal, out returnValue))
            {
                return true;
            }
        }
        returnValue = 0;
        return false;
    }
    internal bool GetValueFromBinary(uint hash, out Vector3 returnValue)
    {
        if (LookupRawFloat3(hash, out returnValue))
        {
            return true;
        }
        if (LookupString(hash, out string strVal))
        {
            string[] split = strVal.Split(" ");
            if (split.Length is 3)
            {
                if (float.TryParse(split[0], out float x) && float.TryParse(split[1], out float y) && float.TryParse(split[2], out float z))
                {
                    returnValue = new Vector3(x, y, z);
                    return true;
                }
            }
        }
        returnValue = Vector3.Zero;
        return false;
    }
    internal bool GetValueFromBinary(uint hash, out int returnValue)
    {
        if (LookupRawInt(hash, out returnValue))
        {
            return true;
        }
        if (LookupRawFloat(hash, out float floatVal))
        {
            returnValue = (int)floatVal;
            return true;
        }
        if (LookupRawBool(hash, out bool boolVal))
        {
            returnValue = boolVal ? 1 : 0;
            return true;
        }
        if (LookupString(hash, out string strVal))
        {
            if (float.TryParse(strVal, out float f))
            {
                returnValue = (int)f;
                return true;
            }
        }
        returnValue = 0;
        return false;
    }
    internal bool GetValueFromBinary(uint hash, out bool returnValue)
    {
        if (LookupRawBool(hash, out returnValue))
        {
            return true;
        }
        if (LookupString(hash, out string strVal))
        {
            returnValue = strVal.FirstOrDefault() is '1';
        }
        returnValue = false;
        return false;
    }
    internal bool GetValueFromBinary(uint hash, out Vector2 returnValue)
    {
        if (LookupRawFloat2(hash, out returnValue))
        {
            return true;
        }
        if (LookupString(hash, out string strVal))
        {
            string[] split = strVal.Split(" ");
            if (split.Length is 2)
            {
                if (float.TryParse(split.First(), out float x) && float.TryParse(split.Last(), out float y))
                {
                    returnValue = new Vector2(x, y);
                    return true;
                }
            }
        }
        returnValue = Vector2.Zero;
        return false;
    }
    internal bool GetValueFromBinary(uint hash, out string returnValue)
    {
        if (LookupString(hash, out returnValue))
        {
            return true;
        }
        if (LookupRawInt(hash, out int intVal))
        {
            returnValue = $"{intVal}";
            return true;
        }
        if (LookupRawFloat(hash, out float floatVal))
        {
            returnValue = $"{floatVal}";
            return true;
        }
        if (LookupRawBool(hash, out bool boolVal))
        {
            returnValue = boolVal ? "1" : "0";
            return true;
        }
        if (LookupRawFloat2(hash, out Vector2 vec2Val))
        {
            returnValue = $"{vec2Val.X} {vec2Val.Y}";
            return true;
        }
        if (LookupRawFloat3(hash, out Vector3 vec3Val))
        {
            returnValue = $"{vec3Val.X} {vec3Val.Y} {vec3Val.Z}";
            return true;
        }
        if (LookupRawFloat4(hash, out Vector4 vec4Val))
        {
            returnValue = $"{vec4Val.X} {vec4Val.Y} {vec4Val.Z} {vec4Val.W}";
            return true;
        }
        returnValue = "";
        return false;
    }
    internal bool LookupString(uint hash, out string value)
    {
        List<ulong> hashMap = mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_StringList];
        if (!Lookup(hash, hashMap, StringOffsetList, out ushort offset))
        {
            if (m_SectionAndNameToValue.TryGetValue(hash, out value!)) //TODO: Check this
            {
                return true;
            }
            return false;
        }

        List<char> buffer = entryData[offset..];
        value = new string(buffer[..buffer.IndexOf('\0')].ToArray()); //Check
        return true;
    }
    internal bool LookupRawBool(uint hash, out bool returnValue)
    {
        List<ulong> hashMap = mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_BitList];
        returnValue = false;
        if (!hashMap.Contains(hash))
        {
            return false;
        }

        returnValue = BitList[hashMap.IndexOf(hash)];
        return true;
    }
    internal bool LookupRawInt(uint hash, out int returnValue)
    {
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Int32List], Int32List, out returnValue))
        {
            return true;
        }
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Int16List], Int16List, out short shortVal))
        {
            returnValue = shortVal;
            return true;
        }
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Int8List], Int8List, out byte byteVal))
        {
            returnValue = byteVal;
            return true;
        }
        returnValue = 0;
        return false;
    }
    internal bool LookupRawFloat(uint hash, out float returnValue)
    {
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32List], Float32List, out returnValue))
        {
            return true;
        }
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatList], FixedPointFloatList, out byte b))
        {
            returnValue = b / 10.0f;
            return true;
        }
        returnValue = 0;
        return false;
    }
    internal bool LookupRawFloat2(uint hash, out Vector2 returnValue)
    {
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32ListVec2], Float32ListVec2, out returnValue))
        {
            return true;
        }
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatListVec2], FixedPointFloatListVec2, out ByteListVec2 b))
        {
            returnValue = new Vector2(b.x / 10.0f, b.y / 10.0f);
            return true;
        }
        returnValue = Vector2.Zero;
        return false;
    }
    internal bool LookupRawFloat3(uint hash, out Vector3 returnValue)
    {
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32ListVec3], Float32ListVec3, out returnValue))
        {
            return true;
        }
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatListVec3], FixedPointFloatListVec3, out ByteListVec3 b))
        {
            returnValue = new Vector3(b.x / 10.0f, b.y / 10.0f, b.z / 10.0f);
            return true;
        }
        returnValue = Vector3.Zero;
        return false;
    }
    internal bool LookupRawFloat4(uint hash, out Vector4 returnValue)
    {
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_Float32ListVec4], Float32ListVec4, out returnValue))
        {
            return true;
        }
        if (Lookup(hash, mHashMaps[(int)TypeGroupIndex.TYPE_INDEX_FixedPointFloatListVec4], FixedPointFloatListVec4, out ByteListVec4 b))
        {
            returnValue = new Vector4(b.x / 10.0f, b.y / 10.0f, b.z / 10.0f, b.w / 10.0f);
            return true;
        }
        returnValue = Vector4.Zero;
        return false;
    }
    internal bool Lookup<T>(uint hash, List<ulong> hashMap, List<T> list, out T returnValue)
    {
        returnValue = default!;
        if (!hashMap.Contains(hash))
        {
            return false;
        }

        returnValue = list[hashMap.IndexOf(hash)];
        return true;
    }

    internal struct ByteListVec2
    {
        internal byte x;
        internal byte y;
    }
    internal struct ByteListVec3
    {
        internal byte x;
        internal byte y;
        internal byte z;
    }
    internal struct ByteListVec4
    {
        internal byte x;
        internal byte y;
        internal byte z;
        internal byte w;
    }
    internal struct BinaryINIEntryHeader
    {
        uint hashKey;
        uint offset;
    };
    internal struct BinaryINIHeaderV2
    {
        byte version;
        //__offset(0x1);
        short charEntries;
        //__padding char _3[1];
    };

    internal enum TypeGroupIndex
    {
        TYPE_INDEX_Int32List = 0x0,
        TYPE_INDEX_Float32List = 0x1,
        TYPE_INDEX_FixedPointFloatList = 0x2,
        TYPE_INDEX_Int16List = 0x3,
        TYPE_INDEX_Int8List = 0x4,
        TYPE_INDEX_BitList = 0x5,
        TYPE_INDEX_FixedPointFloatListVec3 = 0x6,
        TYPE_INDEX_Float32ListVec3 = 0x7,
        TYPE_INDEX_FixedPointFloatListVec2 = 0x8,
        TYPE_INDEX_Float32ListVec2 = 0x9,
        TYPE_INDEX_FixedPointFloatListVec4 = 0xa,
        TYPE_INDEX_Float32ListVec4 = 0xb,
        TYPE_INDEX_StringList = 0xc,
        TYPE_INDEX_NumOf = 0xd
    }
    #endregion
    #region Reverse Engineered Stuff that was supposed to be elsewhere
    internal static void ReadList<T>(BinaryReader file, List<ulong> hashMap, List<T> values) where T : struct
    {
        short count = BitConverter.ToInt16(fread(2, 1, file));
        byte[] data;

        for (int i = 0; i < count; i++)
        {
            data = fread(4, 1, file);
            if (data.Length != 4)
            {
                Array.Resize(ref data, 4);
            }
            hashMap.Add(BitConverter.ToUInt32(data));
        }

        int typeSize = Marshal.SizeOf(typeof(T));
        for (int i = 0; i < count; i++)
        {
            data = fread((uint)typeSize, 1, file);
            values.Add(Unsafe.As<byte, T>(ref data[0]));
        }
    }

    internal static void ReadListBitSet(BinaryReader file, List<ulong> hashMap, List<bool> values)
    {
        short count = BitConverter.ToInt16(fread(2, 1, file));
        int totalBytes = (int)MathF.Ceiling(count / 8f);

        for (int i = 0; i < count; i++)
        {
            byte[] data = fread(4, 1, file);
            ulong hash = BitConverter.ToUInt32(data);
            hashMap.Add(hash);
        }

        int totalBitsRead = 0;
        for (int i = 0; i < totalBytes; i++)
        {
            byte b = fread(1, 1, file)[0];
            for (byte bitCount = 0; bitCount < 8 && totalBitsRead < count; bitCount++, totalBitsRead++)
            {
                byte mask = (byte)(1 << bitCount);
                values.Add((b & mask) != 0);
            }
        }
    }

    //uint32_t fread(void* ptr, uint32_t size, uint32_t n, class r3dFileImpl* f)
    internal static byte[] fread(uint size, uint n, BinaryReader reader)
    {
        return reader.ReadBytes((int)(size * n));
    }
    #endregion
}