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

            LastAccessedFileName = str;

            if (DataCache.TryGetValue(str, out ContentFile data))
            {
                LastAccessedFile = data;
                return data;
            }

            ContentFile cf = new(str);
            if (cf.binaryCached || cf.m_TextFileExists)
            {
                LastAccessedFile = cf;
                DataCache[str] = cf;
                return cf;
            }

            //Hack for spells whose data files are kinda all over the place
            string? path = DataFiles.Find(x => Path.GetFileNameWithoutExtension(x) == Path.GetFileNameWithoutExtension(x));
            if (!string.IsNullOrEmpty(path))
            {
                cf = new(path);
                LastAccessedFile = cf;
                DataCache[str] = cf;
                return cf;
            }

            _logger.Warn($"No data file '{str}' found!");
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
            return new ($"Levels/Map{map.Id}/AIPath.aimesh_ngrid");
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
            ContentFile? file = GetContentFile($"Data/Characters/{characterName}/{characterName}.ini");
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