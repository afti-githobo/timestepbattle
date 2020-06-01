using System.Collections.Generic;
using UnityEngine;

namespace TimeStepBattle
{
    [AddComponentMenu("Battle/Battler")]
    public class Battler : MonoBehaviour
    {
        public BattleController Controller { get; private set; }
        public ActionChooser ActionChooser { get; private set; }
        public AIModule AIModule { get; private set; }

        private void Awake()
        {
            AIModule = GetComponent<AIModule>();
            ActionChooser = GetComponent<ActionChooser>();
            CurrentHP = MaxHP;
        }

        public void SetController(BattleController con) => Controller = con;

        public string Name;
        public BattleSideFlags BattleSideFlags;

        public LinkedList<Battler> Enemies => Controller.GetEnemiesOf(this);

        // Stats
        public long MaxHP => baseMaxHP;
        public long Speed => baseSpeed;

        // Base stats
        [SerializeField]
        private long baseMaxHP;
        [SerializeField]
        private long baseSpeed;

        // Derived stats
        public long CurrentHP;
        public double SpeedFactor => Speed / (double)Controller.BattleSpeed;

        // Battle state parameters
        public long Delay { get { return delay; } set {
                delay = value;
                if (value > delay) AIModule.GainedDelay();
                else if (value < delay) AIModule.LostDelay();
            } }
        [SerializeField]
        private long delay;

        /// <summary>
        /// Deals fixed damage (ie, not going through a damage formula) to the Battler.
        /// </summary>
        public void DealDamageFixed(long damage, ActionExecutor damageSource)
        {
            AIModule.TakingDamage(damageSource);
#if UNITY_EDITOR
            Debug.Log($"{Name} was dealt {damage} fixed damage by {damageSource.Actor.Name}'s {damageSource.Action.Name} [{damageSource.CurrentStage.name}]");
#endif
            CurrentHP -= damage;
            if (CurrentHP < 0) CurrentHP = 0;
            AIModule.TookDamage(damageSource);
        }

        public bool CanStillFight => !IsDead;

        public bool IsDead => CurrentHP == 0;
    }

    /// <summary>
    /// Container for the various extension methods we use to enable our nice syntax for selecting subsets of
    /// Battler collections.
    /// </summary>
    public static class BattlerExtensions
    {
        /// <summary>
        /// Selects a random Battler from the list.
        /// </summary>
        public static Battler Random (this LinkedList<Battler> list)
        {
            var r = UnityEngine.Random.Range(0, list.Count);
            var node = list.First;
            for (int i = 0; i < r; i++) node = node.Next;
            return node.Value;
        }

        /// <summary>
        /// Selects all alive Battlers from the list.
        /// </summary>
        public static LinkedList<Battler> Alive (this LinkedList<Battler> all)
        {
            var alive = new LinkedList<Battler>();
            foreach (var b in all) if (!b.IsDead) alive.AddLast(b);
            return alive;
        }

        /// <summary>
        /// Selects all dead Battlers from the list.
        /// </summary>
        public static LinkedList<Battler> Dead (this LinkedList<Battler> all)
        {
            var dead = new LinkedList<Battler>();
            foreach (var b in all) if (b.IsDead) dead.AddLast(b);
            return dead;
        }
    }
}