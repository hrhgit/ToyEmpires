using Global;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ScenesFlowManager.Init {
    public class Initer : MonoBehaviour
    {
        private void Start () {
            GlobalGameManager.GlobalGameManagerInstance.Init();
            GlobalGameManager.GlobalGameManagerInstance.Load("Main");
            
        }
    }
}
