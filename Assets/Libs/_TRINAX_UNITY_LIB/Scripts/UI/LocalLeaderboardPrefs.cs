using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System.Threading.Tasks;

namespace TRINAX {
    [System.Serializable]
    public class LocalPlayerInfo {
        public string name;
        public int score;

        public LocalPlayerInfo(string _name, int _score) {
            name = _name;
            score = _score;
        }
    }

    public class LocalLeaderboardPrefs : MonoBehaviour {
        #region SINGLETON
        public static LocalLeaderboardPrefs Instance { get; set; }
        private void Awake() {
            if (Instance != null & Instance != this) Destroy(gameObject);
            else Instance = this;
        }
        #endregion

        public bool isLoaded = false;
        public InputField nameIF;
        public InputField scoreIF;
        public Text display;

        List<LocalPlayerInfo> infoList;

        private async void Start() {
            await new WaitUntil(() => Trinax.Instance.loadNow);
            Debug.Log("Loading Leaderboard...");
            isLoaded = false;
            Init();
            isLoaded = true;
            Debug.Log("Leaderboard is loaded!");
        }

        void Init() {
            display.text = "";
            infoList = new List<LocalPlayerInfo>();
            LoadLeaderboard();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                AddPlayerInfo(nameIF.text.Trim(), int.Parse(scoreIF.text.Trim()));
            }
            if (Input.GetKeyDown(KeyCode.K)) {
                Clear();
            }
        }

        void AddPlayerInfo(string _name, int _score) {
            // add player to leaderboard
            LocalPlayerInfo stats = new LocalPlayerInfo(_name, _score);
            infoList.Add(stats);

            nameIF.text = "";
            scoreIF.text = "";

            Sort();
        }

        void Sort() {
            for (int i = infoList.Count - 1; i > 0; i--) {
                if (infoList[i].score > infoList[i - 1].score) {
                    LocalPlayerInfo temp = infoList[i - 1];

                    infoList[i - 1] = infoList[i];
                    infoList[i] = temp;
                }
            }

            UpdatePlayerPrefs();
        }

        const string LB_PREFSTRING = "Leaderboard";
        void UpdatePlayerPrefs() {
            string info = "";
            for (int i = 0; i < infoList.Count; i++) {
                info += infoList[i].name + ",";
                info += infoList[i].score + ",";
            }

            TrinaxPlayerPrefs.SetString(LB_PREFSTRING, info, true);

            UpdateHUD();
        }

        void UpdateHUD() {
            display.text = "";

            for (int i = 0; i <= infoList.Count - 1; i++) {
                display.text += infoList[i].name + " : " + infoList[i].score + "\n";
            }
        }

        public void LoadLeaderboard() {
            string infos = TrinaxPlayerPrefs.GetString(LB_PREFSTRING, "");

            string[] tempInfos = infos.Split(',');

            for (int i = 0; i < tempInfos.Length - 2; i += 2) {
                LocalPlayerInfo loadedInfo = new LocalPlayerInfo(tempInfos[i], int.Parse(tempInfos[i + 1]));

                infoList.Add(loadedInfo);
                UpdateHUD();
            }
        }

        public void Clear() {
            TrinaxPlayerPrefs.DeleteKey(LB_PREFSTRING);
            infoList.Clear();
            display.text = "";
        }

    }
}