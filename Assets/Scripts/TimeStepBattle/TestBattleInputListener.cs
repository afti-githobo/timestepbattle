using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TimeStepBattle
{
    public class TestBattleInputListener : MonoBehaviour
    {
        public Battler Battler;
        private StringBuilder sb = new StringBuilder();
        private static readonly KeyCode[] inputKeyCodes = 
            new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
                            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

        void Start()
        {
            Battler.ActionChooser.OnGetActionSelection += SelectAction;
        }

        void Update()
        {
            if (Battler.ActionChooser.IsHandlingActionSelection)
            {
                var len = Battler.ActionChooser.CurrentActionInputStage.Actions.Length;
                for (int i = 0; i < inputKeyCodes.Length; i++)
                {
                    if (!(len > i)) break;
                    else if (Input.GetKeyDown(inputKeyCodes[i])) SendSelectedAction(Battler.ActionChooser.CurrentActionInputStage.Actions[i]);
                }
            }
        }

        private void SendSelectedAction (BattleAction action)
        {
            var targetSets = new Battler[action.Stages.Length][];
            if (action.IsTargeting)
            {
                for (int i = 0; i < targetSets.Length; i++) targetSets[i] = new Battler[] { Battler.Enemies.Random() };
            }
            Battler.ActionChooser.SelectAction(action, targetSets);
        }

        private void SelectAction(object sender, System.EventArgs e)
        {
            sb.Clear();
            var len = Battler.ActionChooser.CurrentActionInputStage.Actions.Length;
            for (int i = 0; i < len; i++)
            {
                var action = Battler.ActionChooser.CurrentActionInputStage.Actions[i];
                sb.Append($"{i + 1}: {action.Name}");
                if (i < len - 1) sb.Append(", ");
            }
            Debug.Log($"Select an action: {sb.ToString()}");
        }
    }

}