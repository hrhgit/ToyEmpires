using System;
using Gameplay;
using Gameplay.GameUnit;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.UnitUI
{
    public class UnitStatusBarUI : MonoBehaviour
    {
        public static Color        blueHpColor = new Color(0.18f, 0.36f, 1f);
        public static Color        redHpColor  = new Color(1f,    0.31f, 0.42f);
        public        GameUnitBase unit;
        public        Image        hpBarUI;

        private Camera      _camera;
        private Canvas      _canvas;
        private IDefenable _unitDefenable;

        private void Awake()
        {
            _unitDefenable     = (IDefenable)unit;
            _canvas             = this.GetComponent<Canvas>();
            _camera             = Camera.main;
            
        }

        private void Start()
        {
            _canvas.worldCamera = BattleGameManager.BattleGameManagerInstance.uiCamera;
            hpBarUI.color = unit.GetComponent<GameUnitBase>().UnitTeam switch
                            {
                                Team.Blue => blueHpColor,
                                Team.Red  => redHpColor,
                                _         => throw new ArgumentOutOfRangeException()
                            };
        }

        private void Update()
        {
            this.transform.forward = -_camera.transform.forward;
        }

        private void FixedUpdate()
        {
            
            hpBarUI.fillAmount         = (float)_unitDefenable.CurHp / _unitDefenable.MaxHp;
        }
    }
}
