using LeagueSandbox.GameServer.Content.Navigation;
using LeagueSandbox.GameServer.Handlers;
using LeagueSandbox.GameServer.Logging;
using log4net;
using Newtonsoft.Json.Linq;

namespace LeagueSandbox.GameServer.Content
{
    [Obsolete("This entire class will get nuked")]
    public class ContentManager
    {
        private static ILog _logger = LoggerProvider.GetLogger();
        private readonly Game _game;

        public string ContentPath { get; }

        private ContentManager(Game game, string dataPackageName, string contentPath)
        {
            _game = game;

            ContentPath = contentPath;
        }

        public MapData GetMapData(int mapId)
        {
            foreach (var dataPackage in _loadedPackages)
            {
                var toReturnMapData = dataPackage.GetMapData(mapId);

                if (toReturnMapData == null)
                {
                    continue;
                }

                return toReturnMapData;
            }

            throw new ContentNotFoundException($"No map data found for map with id: {mapId}");
        }

        public Dictionary<string, JArray> GetMapSpawns(int mapId)
        {
            foreach (var dataPackage in _loadedPackages)
            {
                var toReturnMapSpawns = dataPackage.GetMapSpawns(mapId);

                if (toReturnMapSpawns == null)
                {
                    continue;
                }

                return toReturnMapSpawns;
            }

            throw new ContentNotFoundException($"No map spawns found for map with id: {mapId}");
        }

        public NavigationGrid GetNavigationGrid(MapScriptHandler map)
        {
            foreach (var dataPackage in _loadedPackages)
            {
                NavigationGrid toReturnNavgrid = dataPackage.GetNavigationGrid(map);

                if (toReturnNavgrid != null)
                {
                    return toReturnNavgrid;
                }
            }

            throw new ContentNotFoundException($"No NavGrid for map with id {map.Id} found in packages, skipping map load...");
        }

        public SpellData GetSpellData(string spellName)
        {
            foreach (var dataPackage in _loadedPackages)
            {
                SpellData toReturnSpellData = dataPackage.GetSpellData(spellName);

                if (toReturnSpellData != null)
                {
                    return toReturnSpellData;
                }
            }

            throw new ContentNotFoundException($"No Spell Data found with name: {spellName}");
        }

        public CharData GetCharData(string characterName)
        {
            foreach (var dataPackage in _loadedPackages)
            {
                CharData toReturnCharData = dataPackage.GetCharData(characterName);

                if (toReturnCharData != null)
                {
                    return toReturnCharData;
                }
            }

            throw new ContentNotFoundException($"No Character found with name: {characterName}");
        }

        private void GetDependenciesRecursively(List<string> resultList, string packageName, string contentPath)
        {
            foreach (var dependency in GetDependenciesFromPackage(packageName, contentPath))
            {
                if (!resultList.Contains(dependency))
                {
                    resultList.Add(dependency);

                    GetDependenciesRecursively(resultList, dependency, contentPath);
                }
            }
        }




    }
}
