using GameServerLib.Content;

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
}