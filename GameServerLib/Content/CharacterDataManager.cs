using GameServerLib.Content.GameVariables;

namespace GameServerLib.Content;

internal static class CharacterDataManager
{
    internal static List<CharacterData> CharacterDataArray = [];
    private static List<LoadingCharacterData> LoadingCharacterArray = [];
    internal static GlobalCharacterData GlobalCharacterData;

    internal static async Task<CharacterData?> LoadCharacterData(string? characterName, int skinID, bool suppressErrorForPreload = false)
    {
        if (string.IsNullOrEmpty(characterName))
        {
            return null;
        }

        foreach (CharacterData characterData in CharacterDataArray)
        {
            if (characterData.CharacterName == characterName && characterData.SkinID == skinID)
            {
                return characterData;
            }
        }

        foreach (LoadingCharacterData characterData in LoadingCharacterArray)
        {
            if (characterData.Name == characterName && characterData.SkinId == skinID)
            {
                return characterData.DataPtr;
            }
        }

        CharacterData data = new();

        LoadingCharacterData loadingEntry = new()
        {
            Name = characterName,
            SkinId = skinID,
            DataPtr = data
        };

        lock (LoadingCharacterArray)
        {
            LoadingCharacterArray.Add(loadingEntry);
        }

        await Task.Run(() =>
        {
            data.Load(characterName, skinID);
        });

        lock (CharacterDataArray) 
        {
            CharacterDataArray.Add(data);
        }

        lock (LoadingCharacterArray) 
        {
            LoadingCharacterArray.Remove(loadingEntry);
        }

        return data;
    }

    struct LoadingCharacterData
    {
        internal string Name;
        internal int SkinId;
        internal CharacterData DataPtr;
    }
}
