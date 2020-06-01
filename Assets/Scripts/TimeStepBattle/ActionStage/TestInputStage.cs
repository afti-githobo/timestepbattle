using UnityEngine;

namespace TimeStepBattle.ActionStages
{
    [CreateAssetMenu(fileName = "NewInputStage", menuName = "Battle/ActionStages/[TEST] Input")]
    public class TestInputStage : BattleActionStage
    {
        public string Line;

        private string ReplaceWildcards(ActionExecutor context) => Line.Replace("$0", context.Actor.Name).Replace("$1", context.Action.Name);

        public override void Perform (Battler[] targets, ActionExecutor executor)
        {
            executor.HaltBattleLogic(this);
            Debug.Log(ReplaceWildcards(executor));
            executor.Context.OnFrameUpdate += (s, e) => DoInputCheck(executor);
        }

        private void DoInputCheck (ActionExecutor executor)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Got input!");
                executor.UnhaltBattleLogic(this);
                executor.Context.OnFrameUpdate -= (s, e) => DoInputCheck(executor);
            }
        }
    }
}