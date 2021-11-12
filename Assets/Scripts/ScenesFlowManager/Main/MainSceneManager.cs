using Global;
using UnityEngine;
namespace ScenesFlowManager.Main {
    public class MainSceneManager : MonoBehaviour
    {
        public void ToGameSetup () {
            GlobalGameManager.GlobalGameManagerInstance.Load("GameSetup");
        }

        public void Exit () {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else    
            Application.Quit();
#endif
        }
    }
}
