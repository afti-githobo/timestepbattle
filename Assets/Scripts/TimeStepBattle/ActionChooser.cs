using System;
using TimeStepBattle.ActionStages;
using UnityEngine;

namespace TimeStepBattle
{
    /// <summary>
    /// MonoBehaviour used to interface w/ UI elements and get selected actions
    /// from players.
    /// </summary>
    [AddComponentMenu("Battle/ActionChooser")]
    public class ActionChooser : MonoBehaviour
    {
        public class EventArgs : System.EventArgs
        {
            public BattleAction SelectedAction { get; private set; }
            public Battler[][] TargetSets { get; private set; }

            public EventArgs(BattleAction selectedAction, Battler[][] targetSets)
            {
                SelectedAction = selectedAction;
                TargetSets = targetSets;
            }
        }

        /// <summary>
        /// The ActionExecutor that this ActionChooser is currently choosing an
        /// action for.
        /// </summary>
        public ActionExecutor CurrentActionExecutor { get; private set; }

        /// <summary>
        /// The ActionInputStage that this ActionChooser is currently choosing an
        /// action for.
        /// </summary>
        public ActionInputStage CurrentActionInputStage { get; private set; }

        public bool IsHandlingActionSelection => CurrentActionInputStage != null;

        /// <summary>
        /// Event fired from the UI when an action has been selected.
        /// </summary>
        public event EventHandler<EventArgs> OnActionSelected;

        /// <summary>
        /// Event fired when the ActionChooser needs to Get
        /// </summary>
        public event EventHandler OnGetActionSelection;

        /// <summary>
        /// Sets CurrentActionInputStage to the given ActionInputStage and fires
        /// OnGetActionSelection event to notify UI that a selection needs to be made.
        /// </summary>
        public void GetActionSelection (ActionExecutor actionExecutor, ActionInputStage actionInputStage)
        {
            if (CurrentActionInputStage != null) throw new Exception("Can't choose another action while already handling an ActionInputStage!");
            CurrentActionExecutor = actionExecutor;
            CurrentActionInputStage = actionInputStage;
            OnGetActionSelection?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Passes the given action and target sets through to CurrentActionInputStage.
        /// </summary>
        public void SelectAction (BattleAction action, Battler[][] targetSets)
        {
            CurrentActionInputStage.Select(CurrentActionExecutor, action, targetSets);
            OnActionSelected?.Invoke(this, new EventArgs(action, targetSets));
            Clear();
        }

        private void Clear ()
        {
            CurrentActionExecutor = null;
            CurrentActionInputStage = null;
        }
    }
}