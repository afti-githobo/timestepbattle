using UnityEngine;

namespace TimeStepBattle.ActionStages
{
    [CreateAssetMenu(fileName = "NewTestPrintlnStage", menuName = "Battle/ActionStages/[TEST] Println")]
    public class TestPrintlnStage : BattleActionStage
    {
        public string Line;

        private string ReplaceWildcards(ActionExecutor executor) => Line.Replace("$0", executor.Actor.Name).Replace("$1", executor.Action.Name);

        public override void Perform(Battler[] targets, ActionExecutor executor) => Debug.Log(ReplaceWildcards(executor));
    }
}