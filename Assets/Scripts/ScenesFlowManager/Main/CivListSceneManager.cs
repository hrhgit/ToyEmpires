using Global;
using UnityEngine;
namespace ScenesFlowManager.Main {
    public class CivListSceneManager : MonoBehaviour
    { public void Back () {
            GlobalGameManager.GlobalGameManagerInstance.Load("Main");
        }
    }
}
