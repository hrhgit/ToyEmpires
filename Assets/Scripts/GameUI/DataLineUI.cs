using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class DataLineUI : MonoBehaviour
    {
        public string title;
        public string content;

        public Text titleUI;
        public Text contentUI;

        private void Start()
        {
            titleUI.text = title;
        }

        private void FixedUpdate()
        {
            contentUI.text = content;
        }
    }
}
