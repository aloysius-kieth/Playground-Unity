using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TRINAX {
    public class TrinaxCanvas : MonoBehaviour {
        public TrinaxAdminPanel adminPanel;
        public TextMeshProUGUI fpsText;
        public TextMeshProUGUI networkStatusText;

        [HideInInspector]
        public Reporter reporter;

        const string cheat_code_adminPanel = "Tri@naxPL";
        const string cheat_code_reporter = "report@ER";
        int typeIndex = 0;
        string typedStr = "";

        //#region SINGLETON
        //public static TrinaxCanvas Instance { get; set; }
        //private void Awake() {
        //    if (Instance != null && Instance != this)
        //        Destroy(gameObject);
        //    else {
        //        Instance = this;
        //        DontDestroyOnLoad(gameObject);
        //    }
        //}
        //#endregion

        private void Start() {
            reporter = FindObjectOfType<Reporter>();
        }

        private void Update() {
            ToggleAdminPanel(adminPanel);
            fpsText.text = "FPS: " + reporter.fps.ToString("F1");
        }

        public void UpdateNetworkStatus(string status) {
            networkStatusText.text = status;
        }

        public void HideDebugText(bool _hide) {
            if (_hide) {
                fpsText.color = Color.clear;
                networkStatusText.color = Color.clear;
            }
            else {
                fpsText.color = Color.black;
                networkStatusText.color = Color.black;
            }
        }

        void ToggleAdminPanel(TrinaxAdminPanel _aP) {
            foreach (char c in Input.inputString) {
                if ((c == cheat_code_adminPanel[typeIndex] && !_aP.gameObject.activeSelf) || (c == cheat_code_reporter[typeIndex] && !reporter.show)) {
                    typeIndex++;
                    typedStr += c;
                }
                else {
                    typeIndex = 0;
                    typedStr = "";
                }
            }
            if (typeIndex == cheat_code_adminPanel.Length && !_aP.gameObject.activeSelf && typedStr == cheat_code_adminPanel) {
                typeIndex = 0;
                typedStr = "";
                _aP.ShowResultOverlay(true);
                _aP.gameObject.SetActive(!_aP.gameObject.activeSelf);
                //if (reporter == null) return;
                //else
                //{
                //    if (reporter.show)
                //    {
                //        reporter.show = !reporter.show;
                //    }
                //}
            }

            if (typeIndex == cheat_code_reporter.Length && !reporter.show && typedStr == cheat_code_reporter) {
                typeIndex = 0;
                typedStr = "";
                if (reporter == null) return;
                reporter.show = !reporter.show;
                if (reporter.show) {
                    reporter.doShow();
                }
            }
            if (Input.GetKeyDown(KeyCode.F9) /*&& _aP.gameObject.activeSelf*/) {
                if (reporter == null) return;
                reporter.show = !reporter.show;
                if (reporter.show) {
                    reporter.doShow();
                }
            }

        }

    }
}