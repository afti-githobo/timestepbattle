using System;
using UnityEngine;

namespace TimeStepBattle.ActionStages
{
    /// <summary>
    /// Action stage that goes out to the UI and gets user input to select an action from the set.
    /// The Battler executing an action w/ an ActionInputStage must have an ActionChooser to handle
    /// UI passthrouggh or ActionInputStage will crash.
    /// </summary>
    [CreateAssetMenu(fileName = "NewActionInputStage", menuName = "Battle/ActionStages/ActionInputStage")]
    public class ActionInputStage : BattleActionStage
    {
        /// <summary>
        /// Identifies the type of action selection that's to be made.
        /// Listeners can use this to determine what to do with the ActionInputStage,
        /// allowing for multiple menus, etc.
        /// </summary>
        public ActionInputType ActionInputType;

        /// <summary>
        /// The set of actions that this ActionInputStage should be choosing between.
        /// </summary>
        public BattleAction[] Actions;

        public override void Perform(Battler[] targets, ActionExecutor executor)
        {
            executor.HaltBattleLogic(this);
            if (executor.Actor.ActionChooser == null) throw new Exception("ActionInputStage must be executed by a Battler with an ActionChooser attached!");     
            executor.Actor.ActionChooser.GetActionSelection(executor, this);
        }

        /// <summary>
        /// Selects an action to stage for execution and unhalts battle logic.
        /// </summary>
        public void Select (ActionExecutor executor, BattleAction action, Battler[][] targetSets)
        {
#if UNITY_EDITOR
            Debug.Log($"{executor.Actor.name} selected action {action.Name}!");
#endif
            executor.StageAction(action, targetSets);
            executor.UnhaltBattleLogic(this);
        }
    }
}