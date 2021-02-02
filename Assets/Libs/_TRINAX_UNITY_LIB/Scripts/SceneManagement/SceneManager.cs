using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TRINAX.UI.Transition;

namespace TRINAX.SceneManagement {
    public class SceneManager : MonoBehaviour, TransitionListenerInterface {

        private Scene scene;
        public SCENE sceneToTransit;

        private void Awake() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            if (scene.buildIndex == 0) {
                return;
            }

            if (TransitionController.Instance != null) {
                Debug.Log("loading from " + scene.name);
                TransitionController.Instance.Init(true);
            }
        }

        void TransitionListenerInterface.TransitInComplete() {
            //Debug.Log("transit in completed");
        }

        void TransitionListenerInterface.TransitOutComplete() {
            switch (sceneToTransit) {
                case SCENE.BOOT:
                    break;
                case SCENE.MAIN:
                    TrinaxCore.Instance.ChangeLevel(sceneToTransit);
                    break;
                //case SCENE.GAME:
                //    TrinaxCore.Instance.ChangeLevel(sceneToTransit);
                //    break;
            }

            sceneToTransit = SCENE.NONE;
            //Debug.Log("transit out completed");
        }
    }

}