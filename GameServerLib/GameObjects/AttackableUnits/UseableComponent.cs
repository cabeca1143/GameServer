using GameServerLib.Content;
using LeagueSandbox.GameServer.Content;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Logging;
using log4net;

namespace GameServerLib.GameObjects.AttackableUnits;

internal class UseableComponent
{
    private ILog _logger = LoggerProvider.GetLogger();

    internal bool IsUseable;
    internal bool AllyUseable;
    internal bool EnemyUseable;
    internal bool MinionUseable;
    internal bool GoldRedirectTargetUseableOnly;
    internal string UseHeroSpellName = "";
    internal string UseSpellName = "";
    internal int UseCooldownSpellSlot;
    private WeakReference<AttackableUnit> Owner = null!;

    internal void Initialize(AttackableUnit owner, string skinName)
    {
        Owner = new(owner);
        InitFromFile(owner, skinName);
    }

    internal void InitFromFile(AttackableUnit unit, string skinName)
    {
        ContentFile? file = Cache.Instance.GetDataCharacterINI(skinName);

        if (file is null)
        {
            _logger.Warn($"No Character Data file found for {skinName}"!);
            return;
        }

        file.GetValue("Useable", "IsUseable", out IsUseable, false);
        file.GetValue("Useable", "AllyCanUse", out AllyUseable, false);
        file.GetValue("Useable", "EnemyCanUse", out EnemyUseable, false);
        file.GetValue("Useable", "HeroUseSpell", out UseHeroSpellName, "");
        file.GetValue("Useable", "MinionUseSpell", out UseSpellName, "");
        file.GetValue("Useable", "MinionUseable", out MinionUseable, false);
        //GoldRedirectTargetUseableOnly unused/uninitialized?
    }

    //TODO:
    void Use()
    {

    }

    internal bool IsAllowedToUse(AttackableUnit user)
    {
        if (!Owner.TryGetTarget(out AttackableUnit? owner))
        {
            return false;
        }

        if (!IsUseable)
        {
            return false;
        }

        if (GoldRedirectTargetUseableOnly)
        {
            if (owner.GoldRedirectTarget.TryGetTarget(out ObjAIBase? goldRedirectTarget))
            {
                return user == goldRedirectTarget;
            }
        }

        if ((AllyUseable && owner.Team == user.Team) || EnemyUseable)
        {
            if (MinionUseable && user is Minion)
            {
                return UseSpellName is not null;
            }
            return UseHeroSpellName is not null;
        }

        return false;
    }
}
