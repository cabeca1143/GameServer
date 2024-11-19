using GameServerCore.Scripting.CSharp;

namespace LeagueSandbox.GameServer.GameObjects;

public class Talent
{
    public string Name { get; }
    public byte Rank { get; }
    public ITalentScript Script { get; }
    public uint ScriptNameHash { get; private set; }
    public IEventSource ParentScript => null;
}

