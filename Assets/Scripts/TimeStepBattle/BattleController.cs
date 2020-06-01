using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeStepBattle
{
    /// <summary>
    /// MonoBehaviour that manages the battle loop and handles turn sequencing.
    /// </summary>
    [AddComponentMenu("Battle/BattleController")]
    public class BattleController : MonoBehaviour
    {
        // Events

        public event EventHandler OnBattleEnd;
        public event EventHandler OnBattleLoss;
        public event EventHandler OnBattleWin;
        public event EventHandler OnFrameUpdate;
        public event EventHandler OnTurnEnd;
        public event EventHandler OnTurnStart;

        // Properties
        
        public ActionExecutor CurrentAction { get; private set; }

        /// <summary>
        /// All Battlers in the battle.
        /// </summary>
        public LinkedList<Battler> Battlers { get; private set; } = new LinkedList<Battler>();
        /// <summary>
        /// All Battlers of side BattleSideFlags.PLAYER in the battle.
        /// </summary>
        public LinkedList<Battler> Players { get; private set; } = new LinkedList<Battler>();
        /// <summary>
        /// All Battlers that are enemies of Players in the battle.
        /// </summary>
        public LinkedList<Battler> Enemies { get; private set; } = new LinkedList<Battler>();

        private bool BattleLogicIsAllowedToProceed => CurrentAction?.HaltingBattleLogic != true;
        private bool BattleLost
        {
            get
            {
                var lost = true;
                foreach (var b in Battlers) if (b.BattleSideFlags.IsPlayer() && b.CanStillFight) lost = false;
                return lost;
            }
        }
        private bool BattleWon
        {
            get
            {
                var won = true;
                foreach (var b in Battlers) if (b.BattleSideFlags.IsEnemy() && !b.IsDead) won = false;
                return won;
            }
        }

        public BattleState State { get; private set; }
        public long BattleSpeed { get; private set; }
        public long CurrentTurn { get; private set; }

        // Unity engine methods

        void Awake()
        {
            foreach (var b in GetComponentsInChildren<Battler>())
            {
                Battlers.AddLast(b);
                if (b.BattleSideFlags.IsPlayer()) Players.AddLast(b);
                if (b.BattleSideFlags.IsEnemy()) Enemies.AddLast(b);
                b.SetController(this);
            }
        }

        void Update()
        {
            switch (State)
            {
                case BattleState.UNRESOLVED:
                    OnFrameUpdate?.Invoke(this, EventArgs.Empty);
                    // If allowed: run next step of battle logic
                    if (BattleLogicIsAllowedToProceed) Step();
                    break;
            }
        }

        // Public methods

        /// <summary>
        /// Get all Battlers in the current battle that are enemies of the provided
        /// Battler.
        /// </summary>
        public LinkedList<Battler> GetEnemiesOf(Battler b)
        {
            if (b.BattleSideFlags.HasFlag(BattleSideFlags.PLAYER)) return Enemies;
            else return Players;
        }

        /// <summary>
        /// Fetches the Battler with the lowest current delay.
        /// If multiple Battlers have the same delay, uses Speed stat
        /// as a tiebreaker. If multiple Battlers have the same Speed stat,
        /// picks one at random.
        /// 
        /// delayReduction is an out parameter containing the lowest dalay of any Battler.
        /// After getting the next actor, we should subtract this value from the delay of all Battlers
        /// in the battle.
        /// </summary>
        public Battler GetNextActor(out long delayReduction)
        {
            // Get all battlers that are ready to act
            var readyBattlers = new LinkedList<Battler>();
            var lowestDelay = long.MaxValue;
            foreach (Battler b in Battlers)
            {
                if (b.Delay < lowestDelay)
                {
                    lowestDelay = b.Delay;
                    readyBattlers.Clear();
                    readyBattlers.AddLast(b);
                }
                else if (b.Delay == lowestDelay) readyBattlers.AddLast(b);
            }
            delayReduction = lowestDelay;
            if (readyBattlers.Count == 1) return readyBattlers.First.Value;
            else return BreakDelayTie(readyBattlers);
        }

        /// <summary>
        /// Removes delayReduction delay from all Battlers.
        /// </summary>
        public void ReduceDelay(long delayReduction)
        {
            foreach (Battler b in Battlers) b.Delay -= delayReduction;
        }

        // Private methods

        /// <summary>
        /// Uses Speed stat as a tiebreaker to return a single Battler
        /// from the provided list.
        /// </summary>
        private Battler BreakDelayTie (LinkedList<Battler> battlers)
        {
            var speedTiedBattlers = new LinkedList<Battler>();
            var highestSpeed = long.MinValue;
            foreach (Battler b in battlers)
            {
                if (b.Speed > highestSpeed)
                {
                    highestSpeed = b.Speed;
                    speedTiedBattlers.Clear();
                    speedTiedBattlers.AddLast(b);
                }
                else if (b.Speed == highestSpeed) speedTiedBattlers.AddLast(b);
            }
            return speedTiedBattlers.Random();
        }

        /// <summary>
        /// Calculates the average speed stat of all Battlers.
        /// </summary>
        private long CalcBattleSpeed()
        {
            long bs = 0;
            var battlers = Battlers.Alive();
            foreach (var b in battlers) bs += b.Speed;
            bs /= Battlers.Count;
            return bs;
        }

        /// <summary>
        /// Ends the current battle w/ the provided state.
        /// Fires off the appropriate events for the final outcome.
        /// </summary>
        private void EndBattle(BattleState newState)
        {
            State = newState;
            switch (State)
            {
                case BattleState.WON:
#if UNITY_EDITOR
                    Debug.Log(" === BATTLE WON ===");
#endif
                    State = BattleState.WON;
                    OnBattleWin?.Invoke(this, EventArgs.Empty);
                    break;
                case BattleState.LOST:
#if UNITY_EDITOR
                    Debug.Log(" === BATTLE LOST ===");
#endif
                    State = BattleState.LOST;
                    OnBattleLoss?.Invoke(this, EventArgs.Empty);
                    break;
            }
            OnBattleEnd?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Ends the current turn, handles end-of-turn housekeeping
        /// </summary>
        private void EndCurrentTurn()
        {
            OnTurnEnd?.Invoke(this, EventArgs.Empty);
#if UNITY_EDITOR
            Debug.Log($"=== TURN END - {CurrentTurn} ===");
#endif
            CurrentAction = null;
            CurrentTurn++;
        }

        /// <summary>
        /// Start the next turn of the ongoing battle.
        /// </summary>
        private void NextTurn ()
        {
            // Make sure the battle isn't over
            if (BattleLost) EndBattle(BattleState.LOST);
            else if (BattleWon) EndBattle(BattleState.WON);
            // It's not over, so start the turn
            else StartTurn();
        }

        private void StartTurn()
        {
            // Get an actor
            var nextActor = GetNextActor(out var delayReduction);
#if UNITY_EDITOR
            Debug.Log($" === TURN START - {CurrentTurn} - Actor: {nextActor.Name} === ");
#endif
            // Do pre-turn-start housekeeping
            ReduceDelay(delayReduction);
            // Request an action and set CurrentAction
            CurrentAction = nextActor.AIModule.GetAction(this);
            CurrentAction.onFinished += (s, e) => EndCurrentTurn();
            OnTurnStart?.Invoke(this, EventArgs.Empty);
            CurrentAction.Step();
        }

        /// <summary>
        /// Runs the next permutation of the battle-logic loop.
        /// </summary>
        private void Step()
        {
            // Recalculate BattleSpeed
            BattleSpeed = CalcBattleSpeed();
            // If CurrentAction is ready to finish, finish it
            if (CurrentAction?.ReadyToFinish == true) CurrentAction.Finish();
            // If we already have an actionexecutor let the action continue running
            if (CurrentAction != null) CurrentAction.Step();
            else NextTurn();
        }
    }
}