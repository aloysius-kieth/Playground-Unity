using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

using TRINAX;
using TRINAX.UI;
using TRINAX.Networking;
using TRINAX.UI.Transition;

namespace TRINAX {
    public class UIManager : MonoBehaviour {
        public bool IsReady { get; set; }

        private float idleTimer;
        private float idleDuration = 30f;

        private float durationToTransit = 0.15f;

        private bool detectIdle = false;
        private bool runningIdle = false;

        [Header("Component References")]
        private PageController pageController;

        private void Start() {
            IsReady = false;
            pageController = GetComponentInChildren<PageController>();
        }

        // Initialize method 
        public void Init() {
            PopulateSettings(Trinax.Instance.globalSettings);
            InitButtonListeners();

            IsReady = true;
            Debug.Log("<color=lime> UIManager is ready! </color>");

            Trinax.Instance.state = STATE.SCREENSAVER;
            // Only called on first run
            pageController.Init(() => {
                ResetAll();
                Trinax.Instance.audioManager.PlayMusic(TrinaxAudioManager.AUDIOS.IDLE_BGM, true);
            });
        }

        private void PopulateSettings(GlobalSettings settings) {
            idleDuration = settings.idleInterval;
        }

        private void InitButtonListeners() {
            
        }

        void Update() {
            if (!IsReady) return;

            if (Input.anyKeyDown) ResetIdleTimer();

            if (Trinax.Instance.state == STATE.HOME) {
                if (detectIdle) return;

                idleTimer += Time.deltaTime;
                if (idleTimer > idleDuration) {
                    Trinax.Instance.state = STATE.NONE;
                    detectIdle = true;
                    StartCoroutine(ReachedIdleDuration());
                }
            }

            if (!detectIdle && runningIdle) {
                runningIdle = false;
                StopCoroutine(ReachedIdleDuration());
            }
        }

        #region UI PAGES
        public void ToScreensaver() {

            Trinax.Instance.userData.Clear();
            pageController.TransitScreen(STATE.SCREENSAVER);
        }

        public void ToHome() {
            pageController.TransitScreen(STATE.HOME);
        }

        #endregion

        #region HELPER METHODS
        /// <summary>
        /// Resets idle timer
        /// </summary>
        public void ResetIdleTimer() {
            detectIdle = false;
            idleTimer = 0;
        }
        public IEnumerator ReachedIdleDuration() {
            ResetIdleTimer();
            runningIdle = true;
            //int duration = 10;
            //for (int i = duration; i > 0; i--) {
            //    if (!runningIdle) yield break;

            //    yield return new WaitForSeconds(1);
            //}

            runningIdle = false;
            ToScreensaver();
            yield return null;
        }
        #endregion

        private void ResetAll() {

        }
        private void OnApplicationQuit() {
        }
    }
}