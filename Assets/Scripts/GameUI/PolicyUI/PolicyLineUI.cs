using System;
using Gameplay;
using Gameplay.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.PolicyUI
{
    public class PolicyLineUI : MonoBehaviour
    {
        public static Color NormalPolicyColor   = Color.white;
        public static Color EconomyPolicyColor  = new Color(0.46f, 0.93f, 0.56f);
        public static Color MilitaryPolicyColor = new Color(0.83f, 0.24f, 0.3f);
        public static Color SpeciaPolicyColor   = new Color(1f,    0.79f, 0.04f);
        
        public            Text       TitleTextUI;
        public            Text       OccTextUI;
        public            Image      TypeUI;
        public            string     title;
        [TextArea] public string     content;
        public            int        occ;
        public            PolicyType type;
        public            PolicyData policyData;
        public            bool       isSelected;

        public  PolicyManagerUI policyManagerUI;
        private bool            _isTracing = false;

        private void Start()
        {
            Show();
        }

        public void Show()
        {
            this.TitleTextUI.text       = title;
            this.OccTextUI.text         = occ.ToString();
            SetColor();
        }

        private void SetColor()
        {
            Color color = type switch
                          {
                              PolicyType.Normal   => PolicyLineUI.NormalPolicyColor,
                              PolicyType.Economy  => PolicyLineUI.EconomyPolicyColor,
                              PolicyType.Military => PolicyLineUI.MilitaryPolicyColor,
                              PolicyType.Special  => PolicyLineUI.SpeciaPolicyColor,
                              _                   => throw new ArgumentOutOfRangeException()
                          };

            this.TitleTextUI.color = color;
            this.OccTextUI.color   = color;
            this.TypeUI.color      = color;

        }

        public void OnMouseEnter()
        {
            _isTracing = true;
            policyManagerUI.EnableDetailPanel(this);
        }
        

        public void OnMouseExit()
        {
            _isTracing = false;
            policyManagerUI.DisableDetailPanel(this);
        }

        public void OnClick()
        {
            _isTracing = false;
            policyManagerUI.DisableDetailPanel(this);
            if(isSelected)
                policyManagerUI.DeselectPolicy(this);
            else
                policyManagerUI.SelectPolicy(this);
        }

        
        private void Update()
        {
            if (_isTracing)
            {
                policyManagerUI.TraceDetail(this);

            }
        }
    }
}
