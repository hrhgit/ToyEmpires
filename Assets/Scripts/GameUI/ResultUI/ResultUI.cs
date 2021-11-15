using System;
using Gameplay;
using Global;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI.ResultUI {
    public class ResultUI : MonoBehaviour {
        public GameObject panel;
        public Text       resultText;

        private void Awake () {
            BattleGameManager.BattleGameManagerInstance.matchEndEvent.AddListener((ShowResult));
        }
        public void ShowResult (bool isWin) {
            panel.SetActive(true);
            this.resultText.text = isWin ? "你赢了！" : "你输了！";
        }

        public void Back () {
            GlobalGameManager.GlobalGameManagerInstance.Load("Main");
        }
    }
}
