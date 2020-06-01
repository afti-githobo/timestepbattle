using System.Collections.Generic;
using UnityEngine;

namespace TimeStepBattle
{
    public abstract class AIModule : MonoBehaviour
    {
        protected Battler Battler;
        protected LinkedList<Battler> Enemies => Battler.Controller.GetEnemiesOf(Battler);

        private void Awake()
        {
            Battler = GetComponent<Battler>();
        }

        public abstract ActionExecutor GetAction(BattleController context);

        /// <summary>
        /// AI module virtual method fired when a Battler's delay is added to.
        /// </summary>
        public virtual void GainedDelay() { }

        /// <summary>
        /// AI module virtual method fired when a Battler's delay is subtracted from.
        /// </summary>
        public virtual void LostDelay() { }

        /// <summary>
        /// AI module virtual method fired before a Battler has damage healed off.
        /// </summary>
        /// <param name="damageSource"></param>
        public virtual void HealingDamage(ActionExecutor damageSource) { }

        /// <summary>
        /// Ai module virtual method fired when a Battler has damage healed off.
        /// </summary>
        public virtual void HealedDamage(ActionExecutor damageSource) { }

        /// <summary>
        /// AI module virtual method fired before a Battler takes damage.
        /// </summary>
        public virtual void TakingDamage(ActionExecutor damageSource) { }

        /// <summary>
        /// AI module virtual method fired when a Battler takes damage.
        /// </summary>
        public virtual void TookDamage(ActionExecutor damageSource) { }
    }
}
