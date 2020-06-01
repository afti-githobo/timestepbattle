using System;
using System.Collections.Generic;
using TimeStepBattle.ActionStages;
using UnityEngine;

namespace TimeStepBattle
{
    /// <summary>
    /// Object emitted by AIModule to encapsulate a BattleAction, the parameters
    /// it's being run with, and the state of the action execution.
    /// </summary>
    public class ActionExecutor
    {
        public event EventHandler onFinished;

        public BattleAction Action { get; private set; }
        public Battler Actor { get; private set; }
        /// <summary>
        /// Array-of-arrays containing the targets of each of this action's stages.
        /// If a stage is non-targeting, its entry in TargetSets should simply be an
        /// empty array.
        /// </summary>
        public Battler[][] TargetSets { get; private set; }
        private Queue<BattleAction> ActionQueue = new Queue<BattleAction>();
        private Queue<Battler[][]> TargetQueue = new Queue<Battler[][]>();
        public BattleActionStage CurrentStage { get
            { if (actionStageIndex >= Action.Stages.Length) return null; else return Action.Stages[actionStageIndex]; } }
        private int actionStageIndex = 0;
        public BattleController Context { get; private set; }

        /// <summary>
        /// The number of actions this ActionExecutor has processed. Incremented
        /// when we fire DequeueAction.
        /// </summary>
        public int ActionNumber = 1;

        public bool ReadyToFinish => actionStageIndex >= Action.Stages.Length && !HaltingBattleLogic;

        private LinkedList<BattleActionStage> stagesWithLocks = new LinkedList<BattleActionStage>();

        public ActionExecutor(BattleController ctx, Battler actor, BattleAction action, Battler[][] targetSets)
        {
            Context = ctx;
            Actor = actor;
            Action = action;
            TargetSets = targetSets;
        }

        /// <summary>
        /// Adds an action to this executor's action queue.
        /// </summary>
        public void StageAction(BattleAction action, Battler[][] targetSets)
        {
            ActionQueue.Enqueue(action);
            TargetQueue.Enqueue(targetSets);
        }

        /// <summary>
        /// Gets an action out of the action queue and resets action-dependent executor state variables.
        /// </summary>
        private void ReadyNextAction()
        {
            Action = ActionQueue.Dequeue();
            TargetSets = TargetQueue.Dequeue();
            actionStageIndex = 0;
            ActionNumber++;
        }

        public void Step()
        {
            if (actionStageIndex == 0) Start();
#if UNITY_EDITOR
            Debug.Log($" ======= STAGE - {actionStageIndex}  - {CurrentStage.name} ======= ");
#endif
            CurrentStage.Perform(TargetSets[actionStageIndex], this);
            actionStageIndex++;
        }

        private void Start()
        {
#if UNITY_EDITOR
            Debug.Log($" ===== TURN ACTION - {ActionNumber}  - Action: {Action.name} - Actor: {Actor.Name} ===== ");
#endif
            Actor.Delay += (long)Math.Round(Action.Delay / Actor.SpeedFactor);
        }

        public void Finish()
        {
            if (ActionQueue.Count > 0) ReadyNextAction();
            else onFinished?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Prevent battle logic from continuing until this stage calls UnhaltBattleLogic.
        /// </summary>
        public void HaltBattleLogic(BattleActionStage stage) => stagesWithLocks.AddLast(stage);

        public void UnhaltBattleLogic(BattleActionStage stage) => stagesWithLocks.Remove(stage);

        /// <summary>
        /// Check to determine if we should be allowed to move forward with battle logic,
        /// or if the ActionExecutor is showing animations, requesting input, etc. and thus
        /// needs us to hold on.
        /// </summary>
        public bool HaltingBattleLogic => stagesWithLocks.Count > 0;
    }
}