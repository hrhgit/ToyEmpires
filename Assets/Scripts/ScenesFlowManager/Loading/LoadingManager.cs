using System.Collections;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ScenesFlowManager.Loading {
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager LoadingManagerInstance { get; private set; }
        
        private float progressValue;
        public string nextSceneName;
        private AsyncOperation async = null;
        
        private void Awake()
        {
            LoadingManagerInstance = this;
        }

        private void Start() {
            nextSceneName = GlobalGameManager.GlobalGameManagerInstance.nextSceneName;
            StartCoroutine(LoadScene());
        }

        IEnumerator LoadScene () {
            async = SceneManager.LoadSceneAsync(nextSceneName);
            async.allowSceneActivation = false;
            while (!async.isDone) {
                if (async.progress < 0.9f)
                    progressValue = async.progress;
                else
                    progressValue = 1.0f;


                if (progressValue >= 0.9) {
                    GlobalGameManager.GlobalGameManagerInstance.audioManager.BgmId = (nextSceneName switch
                                                                                      {
                                                                                          "BattleScene0" => 1,
                                                                                          _              => 0
                                                                                      });

                    async.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
