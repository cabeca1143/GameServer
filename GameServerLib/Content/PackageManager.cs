using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerLib.Content
{

    struct BaseCharacterPackageData
    {
        string Character;
        Dictionary<string, Dictionary<string, string>> Files;
    }


    interface PackageInterface
    {
        //PackageManagerInterface* mPackageManager;
    }

    class BasePackage : PackageInterface
    {
        Dictionary<string, Dictionary<string, string>> Files;
        PackageInterface FallbackPackage;
    }

    class CharacterPackage : BasePackage
    {
        BaseCharacterPackageData BaseSkinFiles;
        string Character;
        uint SkinID;
        uint Hash;
    }

    struct LevelPackage : PackageInterface
    {
        Dictionary<string, string> Files;
        PackageInterface mFallbackPackage;
        uint Hash;
        uint MapID;
    }

    internal class PackageManager
    {
        Dictionary<string, BaseCharacterPackageData> BaseCharacterPackageDataMap;
        Dictionary<CharacterPackageKey, CharacterPackage> CharacterMap;
        Dictionary<string, LevelPackage> LevelMap;
        //Riot::PackageManager::HashPackageMap mHashPackageMap;
        //Riot::ConfigurationPackage mConfigurationPackage;
        //Riot::UserInterfacePackage mUserInterfacePackage;
        //Riot::ItemPackage mItemPackage;
        //Riot::MasteryPackage mMasteryPackage;
        //Riot::FallbackPackage mFallbackPackage;
        //Riot::ForwardingPackage mForwardingPackage;
        //Riot::PackageManager::VisitorList mCharacterVisitors;
        bool mInitialized;

        struct CharacterPackageKey
        {
            string Name;
            uint Key;
        }


    }
}
