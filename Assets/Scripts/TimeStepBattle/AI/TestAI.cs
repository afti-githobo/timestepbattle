using UnityEngine;

namespace TimeStepBattle.AI
{
    [AddComponentMenu("Battle/AI/Test")]
    public class TestAI : AIModule
    {
        public BattleAction[] Actions;

        public override ActionExecutor GetAction(BattleController context)
        {
            var a = Actions[Random.Range(0, Actions.Length)];
            var targetSets = new Battler[a.Stages.Length][];
            if (a.IsTargeting)
            {
                for (var i = 0; i < a.Stages.Length; i++) targetSets[i] = new Battler[] { Enemies.Random() };
            }
            return new ActionExecutor(context, Battler, a, targetSets);
        }
    }
}
