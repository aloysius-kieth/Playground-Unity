using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRINAX {
    // After everything is initialized in global manager, actual game logic is initialized here
    public class AppManager : MonoBehaviour {
        public bool IsReady = false;
        public bool debugMode = false;

        #region SINGLETON
        public static AppManager Instance { get; set; }
        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else Instance = this;
        }
        #endregion

        // Place all reference to managers here

        [HideInInspector]
        public MainManager mainManager;
        [HideInInspector]
        public UIManager uiManager;

        async void Start() {
            IsReady = false;
            mainManager = GetComponent<MainManager>();
            uiManager = GetComponent<UIManager>();

#if UNITY_EDITOR
            if (TrinaxCore.Instance == null) {
                TrinaxCore.Instance.Init();
            }
#endif
            await new WaitUntil(() => Trinax.Instance.isReady);

            // App start
            Execute();
        }

        async void Execute() {
            Debug.Log("App starting...");
            mainManager.Init();
            await new WaitUntil(() => mainManager.IsReady);
            Debug.Log("Done load MainManager");
            uiManager.Init();
            await new WaitUntil(() => uiManager.IsReady);
            Debug.Log("Done load UIManager");

            IsReady = true;
        }
    }
}

