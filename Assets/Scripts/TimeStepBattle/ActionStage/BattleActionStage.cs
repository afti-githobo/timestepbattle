using UnityEngine;

namespace TimeStepBattle.ActionStages
{
    /// <summary>
    /// Parent type for all ActionStages.
    /// Provides a common interface for calling functions
    /// as parts of running Actions.
    /// </summary>
    public abstract class BattleActionStage : ScriptableObject
    {
        public abstract void Perform(Battler[] targets, ActionExecutor context);
    }
}