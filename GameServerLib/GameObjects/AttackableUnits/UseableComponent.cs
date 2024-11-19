using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using System;

namespace GameServerLib.GameObjects.AttackableUnits;

internal class UseableComponent
{
    internal bool IsUseable;
    internal bool AllyUseable;
    internal bool EnemyUseable;
    internal bool MinionUseable;
    internal bool GoldRedirectTargetUseableOnly;
    internal string UseHeroSpellName;
    internal string UseSpellName;
    internal int UseCooldownSpellSlot;
    private WeakReference<AttackableUnit> Owner;

    internal void InitFromFile(AttackableUnit unit, int skinId)
    {

    }

    void Use()
    {

    }

    internal bool IsAllowedToUse(AttackableUnit user)
    {
        if (!Owner.TryGetTarget(out AttackableUnit owner))
        {
            return false;
        }

        if (!IsUseable)
        {
            return false;
        }

        if (GoldRedirectTargetUseableOnly)
        {
            if (owner.GoldRedirectTarget.TryGetTarget(out ObjAIBase goldRedirectTarget))
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
