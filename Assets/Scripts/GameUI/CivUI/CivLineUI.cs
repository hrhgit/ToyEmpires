
using System;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI.CivUI {
    public class CivLineUI : MonoBehaviour {
        public int       civId;
        public Text      civNameUI;
        public string    civName;
        public CivListUI listUI;
        [TextArea]
        public string civDetail;

        private void Start () {
            this.civNameUI.text = civName;
        }

        public void OnClick () {
            listUI.Select(this);
        }
        
    }
}
