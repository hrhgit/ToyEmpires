using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.BuildingUI
{
    public class BuildingPosUI : MonoBehaviour
    {
        public Road          road;
        public int           posIdx;
        public BuildingGrpUI groupUI;
        public Button        mainBtn;
        public GameObject    subMenu;
        public Image         bar;

        private bool _isSet = false;
        private bool _showSubmenu = false;

        public bool IsSet
        {
            get => _isSet;
            set
            {
                if(_isSet == value)
                    return;
                _isSet = value;
                if (_isSet)
                {
                    mainBtn.colors = new ColorBlock()
                                     {
                                         normalColor      = new Color(1f,    0.82f, 0.14f),
                                         highlightedColor = new Color(0.96f, 0.96f, 0.96f),
                                         pressedColor     = new Color(0.78f, 0.78f, 0.78f),
                                         selectedColor    = new Color(0.96f, 0.96f, 0.96f),
                                         disabledColor    = new Color(0.78f, 0.78f, 0.78f, .5f),
                                         colorMultiplier  = 1,
                                         fadeDuration     = .1f
                                     };
                }
                else
                {
                    mainBtn.colors = new ColorBlock()
                                     {
                                         normalColor      = new Color(1f,    0.98f, 1f),
                                         highlightedColor = new Color(0.96f, 0.96f, 0.96f),
                                         pressedColor     = new Color(0.78f, 0.78f, 0.78f),
                                         selectedColor    = new Color(0.96f, 0.96f, 0.96f),
                                         disabledColor    = new Color(0.78f, 0.78f, 0.78f, .5f),
                                         colorMultiplier  = 1,
                                         fadeDuration     = .1f
                                     };


                }

            }
        }

        private void Start()
        {
            if (IsSet)
            {
                mainBtn.colors = new ColorBlock()
                                 {
                                     normalColor      = new Color(1f,    0.82f, 0.14f),
                                     highlightedColor = new Color(0.96f, 0.96f, 0.96f),
                                     pressedColor     = new Color(0.78f, 0.78f, 0.78f),
                                     selectedColor    = new Color(0.96f, 0.96f, 0.96f),
                                     disabledColor    = new Color(0.78f, 0.78f, 0.78f, .5f),
                                     colorMultiplier  = 1,
                                     fadeDuration     = .1f
                                 };
            }
            else
            {
                mainBtn.colors = new ColorBlock()
                                 {
                                     normalColor      = new Color(1f,    0.98f, 1f),
                                     highlightedColor = new Color(0.96f, 0.96f, 0.96f),
                                     pressedColor     = new Color(0.78f, 0.78f, 0.78f),
                                     selectedColor    = new Color(0.96f, 0.96f, 0.96f),
                                     disabledColor    = new Color(0.78f, 0.78f, 0.78f, .5f),
                                     colorMultiplier  = 1,
                                     fadeDuration     = .1f
                                 };


            }
        }

        private void Update()
        {
            if (IsSet)
            {
                bar.fillAmount = groupUI.panelUI.manager.buildingsGroups[groupUI.grpIdx].buildings[posIdx].process;
            }
        }

        private void FixedUpdate()
        {
            this.IsSet = this.groupUI.panelUI.manager.buildingsGroups[groupUI.grpIdx].buildings[posIdx] != null;
        }

        public void OnClick()
        {
            if(IsSet)
            {
                if (_showSubmenu)
                    subMenu.SetActive(false);
                else
                    subMenu.SetActive(true);
            }
            else
            {
                groupUI.panelUI.OpenListPanel(this.road,this.posIdx);
            }
        }
    }
}
