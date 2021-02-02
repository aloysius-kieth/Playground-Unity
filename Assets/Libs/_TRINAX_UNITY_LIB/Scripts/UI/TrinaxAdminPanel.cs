using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace TRINAX {
    /// <summary>
    /// IDs of all inputFields
    /// </summary>
    /// 
    public enum FIELD_ID {
        // Adjust as needed
        SERVER_IP,
        IDLE_INTERVAL,
    }

    /// <summary>
    /// IDs of all toggles
    /// </summary>
    public enum TOGGLE_ID {
        // Adjust as needed
        USE_SERVER,
        USE_MOCKY,
        USE_KEYBOARD,
        MUTE_SOUND,
        USE_LOCALLEADERBOARD,
    }

    /// <summary>
    /// IDs of all sliders
    /// </summary>
    public enum SLIDER_ID {
        // Adjust as needed
        MASTER,
        MUSIC,
        SFX,
        SFX2,
        SFX3,
        SFX4,
        UI_SFX,
        UI_SFX2,
    }

    /// <summary>
    /// Admin Panel
    /// </summary>
    public class TrinaxAdminPanel : MonoBehaviour, IManager {
        int executionPriority = 300;
        public int ExecutionPriority { get { return executionPriority; } set { value = executionPriority; } }

        public bool IsReady { get; set; }
        [Header("Adminpanel Pages")]
        public CanvasGroup[] pages;

        [Header("Display result feedback")]
        public TextMeshProUGUI result;
        public GameObject resultOverlay;

        [Header("InputFields")]
        public TMP_InputField[] inputFields;

        [Header("Toggles")]
        public Toggle[] toggles;

        [Header("Sliders")]
        public Slider[] sliders;
        public TextMeshProUGUI[] sliderValue;

        [Header("Panel Buttons")]
        public Button closeBtn;
        public Button submitBtn;
        public Button pageBtn;
        //public Button clearLocalsavefileBtn;
        //public Button reporterBtn;
        //public Button clearLB;
        //public Button trainingRoomBtn;
        //public Button mainBtn;

        Color red = Color.red;
        Color green = Color.green;

        int pageSelected = 0;

        private void Start() {
        }

        public void Init() {
            Debug.Log("<color=yellow>Loading AdminPanel...</color>");
            IsReady = false;

            resultOverlay.SetActive(false);
            PopulateCurrentValues();

            InitListeners();

            IsReady = true;
            Debug.Log("<color=lime>AdminPanel is loaded!</color>");

            CycleThroughPages(pageSelected);
            gameObject.SetActive(false);
        }

        private void InitListeners() {
            closeBtn.onClick.AddListener(Close);
            submitBtn.onClick.AddListener(Submit);
            //clearLocalsavefileBtn.onClick.AddListener(MainManager.Instance.ClearLocalsaveFile);
            //mainBtn.onClick.AddListener(ToMain);
            //clearLB.onClick.AddListener(() => { LocalLeaderboardJson.Instance.Clear(); });
            //reporterBtn.onClick.AddListener(TrinaxCanvas.Instance.reporter.doShow);
            pageBtn.onClick.AddListener(() =>
            {
                pageSelected++;
                if (pageSelected >= pages.Length) {
                    pageSelected = 0;
                }

                CycleThroughPages(pageSelected);
            });

            toggles[(int)TOGGLE_ID.MUTE_SOUND].onValueChanged.AddListener(delegate { OnmuteAudioListener(toggles[(int)TOGGLE_ID.MUTE_SOUND]); });
            //toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD].onValueChanged.AddListener(delegate { OnEnableLocalLeaderboard(toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD]); });
        }

        private void CycleThroughPages(int page) {
            int num = page;
            for (int i = 0; i < pages.Length; i++) {
                CanvasGroup cGrp = pages[i];
                if (i == num) {
                    cGrp.interactable = true;
                    cGrp.blocksRaycasts = true;
                    cGrp.DOFade(1.0f, 0.25f);
                }
                else {
                    cGrp.interactable = false;
                    cGrp.blocksRaycasts = false;
                    cGrp.DOFade(0.0f, 0.25f);
                }
            }
        }

        void Update() {
            UpdateSliderValueText();
            HandleInputs();
        }

        /// <summary>
        /// Updates slider text values.
        /// </summary>
        private void UpdateSliderValueText() {
            sliderValue[(int)SLIDER_ID.MASTER].text = sliders[(int)SLIDER_ID.MASTER].value.ToString("0.0");
            sliderValue[(int)SLIDER_ID.MUSIC].text = sliders[(int)SLIDER_ID.MUSIC].value.ToString("0.0");
            sliderValue[(int)SLIDER_ID.SFX].text = sliders[(int)SLIDER_ID.SFX].value.ToString("0.0");
            sliderValue[(int)SLIDER_ID.SFX2].text = sliders[(int)SLIDER_ID.SFX2].value.ToString("0.0");
            sliderValue[(int)SLIDER_ID.SFX3].text = sliders[(int)SLIDER_ID.SFX3].value.ToString("0.0");
            sliderValue[(int)SLIDER_ID.SFX4].text = sliders[(int)SLIDER_ID.SFX4].value.ToString("0.0");
            sliderValue[(int)SLIDER_ID.UI_SFX].text = sliders[(int)SLIDER_ID.UI_SFX].value.ToString("0.0");
            sliderValue[(int)SLIDER_ID.UI_SFX2].text = sliders[(int)SLIDER_ID.UI_SFX2].value.ToString("0.0");
        }

        private void OnmuteAudioListener(Toggle toggle) {
            Trinax.Instance.audioManager.muteAudioListener(toggle.isOn);
        }

        private void OnEnableLocalLeaderboard(Toggle toggle) {
            //AppManager.Instance.uiManager.useLocalLeaderboard = toggle.isOn;
        }

        /// <summary>
        /// Handles all inputs.
        /// </summary>
        private void HandleInputs() {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                Submit();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                resultOverlay.SetActive(false);
                Close();
            }
        }

        private void PopulateGlobalValues() {
            inputFields[(int)FIELD_ID.SERVER_IP].text = Trinax.Instance.globalSettings.IP.ToString();
            inputFields[(int)FIELD_ID.IDLE_INTERVAL].text = Trinax.Instance.globalSettings.idleInterval.ToString();

            toggles[(int)TOGGLE_ID.USE_SERVER].isOn = Trinax.Instance.globalSettings.useServer;
            toggles[(int)TOGGLE_ID.USE_MOCKY].isOn = Trinax.Instance.globalSettings.useMocky;
            toggles[(int)TOGGLE_ID.USE_KEYBOARD].isOn = Trinax.Instance.globalSettings.useKeyboard;
            toggles[(int)TOGGLE_ID.MUTE_SOUND].isOn = Trinax.Instance.globalSettings.muteAudioListener;
            toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD].isOn = Trinax.Instance.globalSettings.useLocalLeaderboard;
        }

        private void PopulateGameValues() {

        }

        /// <summary>
        /// Sets current values to fields.
        /// </summary>
        private void PopulateCurrentValues() {
            PopulateGlobalValues();
            PopulateGameValues();
        }

        private  void UpdateSaveValues() {
            GlobalSettings globalSettings = new GlobalSettings {
                IP = inputFields[(int)FIELD_ID.SERVER_IP].text.Trim(),
                idleInterval = float.Parse(inputFields[(int)FIELD_ID.IDLE_INTERVAL].text.Trim()),

                useServer = toggles[(int)TOGGLE_ID.USE_SERVER].isOn,
                useMocky = toggles[(int)TOGGLE_ID.USE_MOCKY].isOn,
                useKeyboard = toggles[(int)TOGGLE_ID.USE_KEYBOARD].isOn,
                muteAudioListener = toggles[(int)TOGGLE_ID.MUTE_SOUND].isOn,
                useLocalLeaderboard = toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD].isOn,
            };

            Trinax.Instance.globalSettings = globalSettings;
        }

        /// <summary>
        /// Saves the value to respective fields.
        /// </summary>
        private void Submit() {
            string resultText = "Empty";
            if (string.IsNullOrEmpty(inputFields[(int)FIELD_ID.SERVER_IP].text.Trim())) {
                Debug.Log("Mandatory fields in admin panel is empty!");
                ShowResultOverlay(false);
                result.color = red;
                resultText = "Need to fill mandatory fields!";
            }
            else {
                ShowResultOverlay(false);
                result.color = green;
                resultText = "Success!";

                UpdateSaveValues();
                Trinax.Instance.saveManager.SaveJson();

                Trinax.Instance.RefreshSettings();
                //TrinaxArduino.Instance.Restart();
            }

            result.text = resultText;
            result.gameObject.SetActive(true);
        }

        public async void ShowResultOverlay(bool immediate) {
            resultOverlay.SetActive(true);

            if (!immediate)
                await new WaitForSeconds(2f);

            resultOverlay.SetActive(false);
        }

        /// <summary>
        /// Closes admin panel.
        /// </summary>
        private void Close() {
            gameObject.SetActive(false);
        }
    }
}