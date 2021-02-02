using System.Collections.Generic;
using UnityEngine;
using TRINAX.Networking;

namespace TRINAX {
    // Use this for storing user's data
    [System.Serializable]
    public class UserData {
        public string interactionID = "";

        public void Clear() {
            interactionID = "";
        }
    }

    /// <summary>
    /// Global Manager
    /// </summary>
    public class Trinax : MonoBehaviour {
        #region SINGLETON
        public static Trinax Instance { get; set; }
        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }
        #endregion

        public bool IsAppPaused { get; set; }
        public bool isReady = false;
        public bool loadNow = false;
        public bool isChangingLevels = false;
        public bool loadedComponentReferences = false;

        public STATE state;
        //public SCENE scene;

        public UserData userData = new UserData();
        public CarSettings carSettings;
        public LED ledSettings;

        TrinaxAdminPanel aP;
        public TrinaxSaveManager saveManager;
        public TrinaxAudioManager audioManager;
        public TrinaxServerManager serverManager;
        public TrinaxCanvas canvas;
        public TrinaxKeyboardManager keyboardManager;

        public List<KeyValuePair<IManager, int>> IManagers = new List<KeyValuePair<IManager, int>>();

        [Header("Settings")]
        public GlobalSettings globalSettings;
        //public GameSettings gameSettings;
        public AudioConfig audioSettings;
        //public KinectSettings kinectSettings;

        private void Start() {
#if !UNITY_EDITOR
    Cursor.visible = false;
#endif
            //scene = SCENE.MAIN;

            // Assign component references
            SetComponentReferences();
            // Collate all managers that derive from IManager
            AddIManagers();
            // Load our managers in order
            LoadManagers();
            // Assign settings and finally, we are ready to load our game logic
            Init();
        }

        #region Init Managers
        private async void Init() {
            await new WaitUntil(() => !loadNow);

            // Indicate that everything is ready
            isReady = true;
            Debug.Log("<color=lime>*** All managers loaded! :) ***</color>");

            // *** Here all managers should be fully loaded. Do whatever you want now! *** //

            if (string.IsNullOrEmpty(globalSettings.IP)) {
                Debug.Log("<color=orange>Mandatory fields in admin panel not filled!</color>" + "\n" + "Opening admin panel...");
                //aP.gameObject.SetActive(true);
            }
            else {
                aP.gameObject.SetActive(false);
            }

            RefreshSettings();
        }

        private void AddIManagers() {
            IManagers.Add(new KeyValuePair<IManager, int>(saveManager, saveManager.ExecutionPriority));
            IManagers.Add(new KeyValuePair<IManager, int>(audioManager, audioManager.ExecutionPriority));
            IManagers.Add(new KeyValuePair<IManager, int>(serverManager, serverManager.ExecutionPriority));
            IManagers.Add(new KeyValuePair<IManager, int>(canvas.adminPanel, canvas.adminPanel.ExecutionPriority));
            //IManagers.Add(new KeyValuePair<IManager, int>(trinaxKeyboardManager, trinaxKeyboardManager.ExecutionPriority));

            // Sort by execution order
            IManagers.Sort((value1, value2) => value1.Value.CompareTo(value2.Value));

            //foreach (KeyValuePair<IManager, int> kvp in IManagers)
            //{
            //    Debug.Log(string.Format("Class: {0} | Order: {1}", kvp.Key, kvp.Value));
            //}
        }

        // Use this to await on managers to be loaded before able to call methods from it
        private  async void LoadManagers() {
            Debug.Log("<color=yellow>Waiting for managers to be loaded...</color>");
            loadNow = true;

            for (int i = 0; i < IManagers.Count; i++) {
                IManagers[i].Key.Init();
                bool ready = IManagers[i].Key.IsReady;
                await new WaitUntil(() => ready);
            }
            loadNow = false;
        }

        #endregion

        private void Update() {
            if (Application.isPlaying) {
                if (Time.frameCount % 30 == 0)
                    System.GC.Collect(1, System.GCCollectionMode.Optimized, false, false);
            }
        }

        // Set necessary component references
        private void SetComponentReferences() {
            loadedComponentReferences = false;
            saveManager = GetComponentInChildren<TrinaxSaveManager>();
            audioManager = GetComponentInChildren<TrinaxAudioManager>();
            serverManager = GetComponentInChildren<TrinaxServerManager>();
            canvas = GetComponentInChildren<TrinaxCanvas>();
            keyboardManager = GetComponentInChildren<TrinaxKeyboardManager>();

            aP = canvas.adminPanel;

            loadedComponentReferences = true;
        }

        public void RefreshSettings() {
            Debug.Log("Refreshed settings!");
            if (serverManager != null) serverManager.PopulateValues(globalSettings);
            else
                Debug.LogWarning("<ServerManager> values not populated!");
            if (audioManager != null)
                audioManager.PopulateValues();
            else
                Debug.LogWarning("<AudioManager> values not populated!");
            //if (TrinaxArduino.Instance != null)
            //    TrinaxArduino.Instance.Populate(globalSettings);
            //else
            //    Debug.LogWarning("<TrinaxArduino> values not populated!");
        }

    }
}