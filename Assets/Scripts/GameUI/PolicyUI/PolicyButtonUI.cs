using UnityEngine;

namespace GameUI.PolicyUI
{
    public class PolicyButtonUI : MonoBehaviour
    {
        public PolicyManagerUI policyManagerUI;

        public void OnClick()
        {
            policyManagerUI.panelUI.gameObject.SetActive(true);
        }
    }
}
