using Game.Custom.Components;

namespace Game.Custom.Static;

public enum RangedId
{
    Arrow,
}

public static class Ranged
{
    public static void Activate(RangedAttack ranged)
    {
        switch (ranged.RangedId)
        {
            case RangedId.Arrow:
                break;
            default:
                throw new System.Exception("Ranged Attack '" + ranged.RangedId + "' not implemented.");
        }
    }
}
