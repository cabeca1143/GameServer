using GameServerLib.Content.GameVariables;
using System.Collections.Generic;

namespace GameServerLib.Content;

internal static class CharacterDataManager
{
    internal static List<CharacterData> CharacterDataArray = [];
    private static List<LoadingCharacterData> LoadingCharacterArray = [];
    internal static GlobalCharacterData GlobalCharacterData;

    internal static CharacterData? LoadCharacterData(string? characterName, int skinID, bool suppressErrorForPreload = false)
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

        LoadingCharacterArray.Add(loadingEntry);
        data.Load(characterName, skinID);

        return data;
    }

    struct LoadingCharacterData
    {
        internal string Name;
        internal int SkinId;
        internal CharacterData DataPtr;
    };
}
