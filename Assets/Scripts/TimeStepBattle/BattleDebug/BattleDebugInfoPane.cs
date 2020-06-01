using UnityEngine;
using TMPro;

namespace TimeStepBattle.BattleDebug
{
    [AddComponentMenu("Battle/Debug Info Pane")]
    public class BattleDebugInfoPane : MonoBehaviour
    {
        private BattleController battleController;
        private TextMeshProUGUI textMesh;

        void Awake()
        {
            battleController = FindObjectOfType<BattleController>();
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Update()
        {
            if (battleController == null || textMesh == null) return;
            textMesh.text = $"BattleSpeed: {battleController.BattleSpeed}\n" +
                $"Turn {battleController.CurrentTurn} Action {(battleController.CurrentAction?.ActionNumber)} Stage {(battleController.CurrentAction?.CurrentStage)}\n" +
                $"Actor: {(battleController.CurrentAction?.Actor.Name)} ({(battleController.CurrentAction?.Actor.name)})\n" +
                $"Players: {battleController.Players.Count} ({battleController.Players.Alive().Count} alive) Enemies: {battleController.Enemies.Count} ({battleController.Enemies.Alive().Count} alive)\n" +
                $"Battle state: {battleController.State}";
        }
    }
}