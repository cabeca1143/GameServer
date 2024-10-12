using GameServerLib.Content;
using LeagueSandbox.GameServer.Content;

namespace GameServerLib;

//This class will be used for functions that don't belong in any specific class
internal static class LS
{
    internal static float ReadCFG_F(string fileName, string group, string name, float defaultValue = 0, bool skipCache = false)
    {
        ContentFile file = Cache.Instance.GetFile(fileName, skipCache);

        if(file is null)
        {
            return defaultValue;
        }

        file.GetValue(group, name, out float val, defaultValue);
        return val;
    }
    internal static int ReadCFG_I(string fileName, string group, string name, int defaultValue = 0, bool skipCache = false)
    {
        ContentFile file = Cache.Instance.GetFile(fileName, skipCache);

        if (file is null)
        {
            return defaultValue;
        }

        file.GetValue(group, name, out int val, defaultValue);
        return val;
    }
}
