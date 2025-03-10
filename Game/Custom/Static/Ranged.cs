using Game.Custom.Components;

namespace Game.Custom.Static;

public enum RangedType
{
    Arrow,
}

public static class Ranged
{
    public static void Attack(RangedAttack ranged)
    {
        switch (ranged.RangedType)
        {
            case RangedType.Arrow:
                break;
            default:
                throw new System.Exception("Ranged Attack '" + ranged.RangedType + "' not implemented.");
        }
    }
}
