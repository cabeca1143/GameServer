using LeagueSandbox.GameServer.Scripting.CSharp;

namespace GameServerCore.Domain;

public interface IMapScript
{
    MapScriptMetadata MapScriptMetadata { get; }
    void OnLevelInit() { }
    void OnLevelInitServer() { }
    void OnPostLevelLoad() { }
}