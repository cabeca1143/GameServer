using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace MapScripts.Map1;

//TODO: Rewrite the entire Map system (Again)
public class LevelScript : IMapScript
{
    public MapScriptMetadata MapScriptMetadata { get; } = new();

    public void OnLevelInit()
    {
    }
    public void OnLevelInitServer()
    {
    }
    public void OnPostLevelLoad()
    {
    }
}
