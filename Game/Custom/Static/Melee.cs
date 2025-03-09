using Game.Custom.Components;

namespace Game.Custom.Static;

public enum MeleeId
{
    Slash,
}

public static class Melee
{
    public static void Activate(MeleeAttack melee)
    {
        switch (melee.MeleeId)
        {
            case MeleeId.Slash:
                break;
            default:
                throw new System.Exception("Melee Attack '" + melee.MeleeId + "' not implemented.");
        }
    }
}
