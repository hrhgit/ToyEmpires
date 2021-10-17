using System;
using Gameplay.GameUnit;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.UnitUI
{
    public class UnitStatusBarUI : MonoBehaviour
    {
        public  GameUnitBase unit;
        public  Image        hpBarUI;

        private Camera      _camera;
        private Canvas      _canvas;
        private IDefenable _unitDefenable;

        private void Awake()
        {
            _unitDefenable     = (IDefenable)unit;
            _canvas             = this.GetComponent<Canvas>();
            _camera             = Camera.main;
            _canvas.worldCamera = _camera;
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
