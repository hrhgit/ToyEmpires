using UnityEngine;
using UnityEngine.UI;
namespace GameUI.CivUI {
    public class CivDetailUI : MonoBehaviour
    {
        public Text civNameUI;
        public Text civDetailUI;
        

        public void Show (CivLineUI lineUI) {
            this.civNameUI.text   = lineUI.civName;
            this.civDetailUI.text = lineUI.civDetail;
        }
    }
}
