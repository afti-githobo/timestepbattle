using System;

namespace TimeStepBattle
{
    /// <summary>
    /// Flags representing the affiliation of a Battler.
    /// </summary>
    [Flags]
    public enum BattleSideFlags
    {
        NONE = 0,
        PLAYER = 1,
        MISC_ENEMY = 1 << 1,
        MISC_ALLY = 1 << 2,
    }

    public static class BSFExtensions
    {
        public static bool IsEnemy(this BattleSideFlags flags) => flags.HasFlag(BattleSideFlags.MISC_ENEMY);
        public static bool IsFriendly(this BattleSideFlags flags) => flags.HasFlag(BattleSideFlags.PLAYER) | flags.HasFlag(BattleSideFlags.MISC_ALLY);
        public static bool IsPlayer(this BattleSideFlags flags) => flags.HasFlag(BattleSideFlags.PLAYER);
    }
}