using GameServerLib.Content;
using Newtonsoft.Json.Linq;

namespace GameServerLib;

//This class will be used for functions that don't belong in any specific class
internal static class LS
{
    internal static float ReadCFG_F(string fileName, string group, string name, float defaultValue = 0, bool skipCache = false)
    {
        float val = defaultValue;
        Cache.Instance.GetFile(fileName, skipCache)?.GetValue(group, name, out val, defaultValue);
        return val;
    }
    internal static int ReadCFG_I(string fileName, string group, string name, int defaultValue = 0, bool skipCache = false)
    {
        int val = defaultValue;
        Cache.Instance.GetFile(fileName, skipCache)?.GetValue(group, name, out val, defaultValue);
        return val;
    }
    internal static bool ReadCFG_B(string fileName, string group, string name, bool defaultValue = false, bool skipCache = false)
    {
        bool val = defaultValue;
        Cache.Instance.GetFile(fileName, skipCache)?.GetValue(group, name, out val, defaultValue);
        return val;
    }
    internal static string ReadCFG_S(string fileName, string group, string name, string defaultValue = "", bool skipCache = false)
    {
        string val = defaultValue;
        Cache.Instance.GetFile(fileName, skipCache)?.GetValue(group, name, out val, defaultValue);
        return val;
    }
    internal static bool ReadCFG_6UI(string fileName, string group, string name, ref uint[] values, bool skipCache = false)
    {
        string? val = "";

        values ??= new uint[6];
        if (values.Length != 6)
        {
            Array.Resize(ref values, 6);
        }

        Cache.Instance.GetFile(fileName, skipCache)?.GetValue(group, name, out val, null!);

        if (val is null)
        {
            return false;
        }

        string[] split = val.Split(' ');
        for (int i = 0; i < 6; i++)
        {
            if (i >= split.Length || !uint.TryParse(split[i], out uint num))
            {
                values[i] = 0;
                continue;
            }
            values[i] = num;
        }
        return true;
    }
    internal static bool ReadCFG_4UI(string fileName, string group, string name, ref uint[] values, bool skipCache = false)
    {
        string? val = "";

        values ??= new uint[4];
        if (values.Length != 4)
        {
            Array.Resize(ref values, 4);
        }

        Cache.Instance.GetFile(fileName, skipCache)?.GetValue(group, name, out val, null!);

        if (val is null)
        {
            return false;
        }

        string[] split = val.Split(' ');
        for (int i = 0; i < 4; i++)
        {
            if (i >= split.Length || !uint.TryParse(split[i], out uint num))
            {
                values[i] = 0;
                continue;
            }
            values[i] = num;
        }
        return true;
    }
}