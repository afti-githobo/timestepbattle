using UnityEngine;
using TimeStepBattle.ActionStages;

namespace TimeStepBattle
{
    [CreateAssetMenu(fileName = "NewBattleAction", menuName = "Battle/Action")]
    public class BattleAction : ScriptableObject
    {
        public string Name;
        public long Delay;
        public bool IsTargeting;

        public BattleActionStage[] Stages { get { return stages; } }
        [SerializeField]
        private BattleActionStage[] stages;
    }
}