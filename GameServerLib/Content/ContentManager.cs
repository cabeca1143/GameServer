using GameServerLib.Content;
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

        private Dictionary<string, ContentFile> DataCache = [];
        private ContentFile LastAccessedFile = null!;
        private string LastAccessedFileName = "";
        private List<string> DataFiles;

        internal ContentManager(Game game)
        {
            _game = game;

            //Hack
            DataFiles = Directory.GetFiles("Data", "*.inibin", SearchOption.AllDirectories).ToList();
            Mesh test = new(
                "C:\\Users\\rbeli\\Desktop\\League of Legends_UNPACKED\\League-of-Legends-4-20\\RADS\\solutions\\lol_game_client_sln\\releases\\0.0.1.68\\deploy\\LEVELS\\Map1\\Scene\\__Spawn_T1.SCB");
        }
        
        internal ContentFile? GetContentFile(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (str == LastAccessedFileName)
            {
                return LastAccessedFile;
            }

            if (DataCache.TryGetValue(str, out ContentFile data))
            {
                return data;
            }

            ContentFile cf = new(str);
            if (cf.binaryCached || cf.m_TextFileExists)
            {
                return cf;
            }

            //Hack for spells whose data files are kinda all over the place
            string? path = DataFiles.Find(x => Path.GetFileNameWithoutExtension(x) == Path.GetFileNameWithoutExtension(x));
            if (!string.IsNullOrEmpty(path))
            {
                return new(path);
            }

            return null;
        }

        public MapData GetMapData(int mapId)
        {
            return new(mapId);
        }

        public Dictionary<string, JArray> GetMapSpawns(int mapId)
        {
            return [];
        }

        public NavigationGrid GetNavigationGrid(MapScriptHandler map)
        {
            return new ($"Levels/Map{_game.Map.Id}/AIPath.aimesh_ngrid");
        }

        public SpellData GetSpellData(string spellName)
        {
            ContentFile? file = GetContentFile(spellName);

            if (file is not null)
            {
                SpellData sd = new();
                sd.Load(file);
                return sd;
            }

            return new();
        }

        public CharData GetCharData(string characterName)
        {
            ContentFile? file = GetContentFile($"Data/Characters/{characterName}");
            if (file is not null)
            {
                CharData cd = new();
                cd.Load(file);
                return cd;
            }

            return new();
        }
    }
}