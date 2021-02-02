using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TRINAX;
using TRINAX.UI.Transition;

namespace TRINAX {
    public class Boot : MonoBehaviour {
        private async void Start() {
            await new WaitUntil(() => Trinax.Instance.isReady);

            // *** Do your preloading here! ***

            Debug.Log("<color=orange>Preloading...</color>");


            TrinaxCore.Instance.SceneManager.sceneToTransit = SCENE.MAIN;
            TransitionController.Instance.Init(false);
        }
    }
}
