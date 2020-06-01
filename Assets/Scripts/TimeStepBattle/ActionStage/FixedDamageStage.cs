using UnityEngine;

namespace TimeStepBattle.ActionStages
{
    [CreateAssetMenu(fileName = "NewInputStage", menuName = "Battle/ActionStages/FixedDamageStage")]
    public class FixedDamageStage : BattleActionStage
    {
        public long Damage;

        public override void Perform(Battler[] targets, ActionExecutor executor)
        {
            foreach (var b in targets) b.DealDamageFixed(Damage, executor);
        }
    }
}