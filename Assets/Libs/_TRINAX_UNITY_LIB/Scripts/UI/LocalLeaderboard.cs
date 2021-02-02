using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

using TRINAX.Utils;

using Random = UnityEngine.Random;

namespace TRINAX {
    [System.Serializable]
    public class PlayerInfo {
        public string name;
        public int score;
        public int rank = 0;
        // extra info
        public string mobile = "";
    }

    public class LocalLeaderboard : MonoBehaviour {
        public bool IsReady { get; set; }
        public bool loadByDate = true;

        // Use to check current date time for file prefix
        DateTime currentDateTime;
        string dateStr;

        string FILENAME = "leaderboard";
        string MASTER_FILENAME = "master_leaderboard";
        string FOLDER_PATH = "";
        const string FOLDER_NAME = "leaderboard_data";
        const string FILE_EXTENSION = ".json";

        // Number of places on leaderboard
        public int number_of_slots = 10;
        // List to keep track of leaderboard
        public List<_PlayerInfoDisplay> slots = new List<_PlayerInfoDisplay>();

        public List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
        public List<PlayerInfo> masterPlayerInfoList = new List<PlayerInfo>();

        PlayerInfo lastEntry = new PlayerInfo();
        PlayerInfo newEntry = null;

        // Prefab to spawn from
        public GameObject displayPrefab;
        public GameObject displayContainer;
        public LeaderboardDisplay leaderboardDisplay;

        void Start() {
        }

        public void Init() {
            IsReady = false;

            CreateDirectory();

            currentDateTime = DateTime.Now;
            dateStr = currentDateTime.ToString("dd-MM-yyyy");
            SetFileNameExtensions();
            CreateSlots();
            Load(/*loadByDate*/);

            IsReady = true;
        }

        void CreateDirectory() {
            string path = Application.persistentDataPath + "/" + FOLDER_NAME;
            DirectoryInfo info = Directory.CreateDirectory(path);
            if (info.Exists) {
                FOLDER_PATH = "\\" + info.Name;
                Debug.Log(info.FullName + " created!");
            }
        }

        private void Update() {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    Save(RandomAddUser());
            //}
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    ClearFile(FILENAME);
            //}
#endif
        }

        void SetFileNameExtensions() {
            FILENAME = FILENAME + "_" + dateStr + FILE_EXTENSION;
            MASTER_FILENAME = MASTER_FILENAME + FILE_EXTENSION;
            Debug.Log(string.Format("Current leaderboard file {0}", FILENAME));
        }

        void CreateSlots() {
            if (displayContainer.GetComponentInChildren<Transform>().GetChild(0) != null)
                Destroy(displayContainer.GetComponentInChildren<Transform>().GetChild(0).gameObject);

            GameObject obj = null;
            for (int i = 0; i < number_of_slots; i++) {
                obj = Instantiate(displayPrefab, transform) as GameObject;
                obj.transform.SetParent(displayContainer.transform, false);
                if (i == 0)
                    obj.GetComponent<PlayerInfoDisplay>().highlight.SetActive(true);
                slots.Add(obj.GetComponent<PlayerInfoDisplay>().playerInfoDisplay);
            }

            leaderboardDisplay.Init(slots);
        }

        public void Save(PlayerInfo info) {
            PlayerInfo pInfo = new PlayerInfo {
                name = info.name,
                score = info.score,
                mobile = info.mobile,
            };

            Debug.Log(pInfo.name + " | " + pInfo.score);


            masterPlayerInfoList.Add(pInfo);
            playerInfoList.Add(pInfo);
            // Sort the list in game

            masterPlayerInfoList = Sort(masterPlayerInfoList);
            playerInfoList = Sort(playerInfoList);

            string jsonStr = JsonArrayUtility.ToJson(playerInfoList, true);
            string masterJsonStr = JsonArrayUtility.ToJson(masterPlayerInfoList, true);
            // Write to file by date
            JsonFileUtility.WriteJsonToFile(FOLDER_PATH + "\\" + FILENAME, jsonStr, JSONSTATE.PERSISTENT_DATA_PATH);
            // Write to master file
            JsonFileUtility.WriteJsonToFile(FOLDER_PATH + "\\" + MASTER_FILENAME, masterJsonStr, JSONSTATE.PERSISTENT_DATA_PATH);
            //Debug.Log("Saving leaderboard " + jsonStr);

            //if(AppManager.Instance.uiManager.useLocalLeaderboard)
            //    leaderboardDisplay.PopulateData(playerInfoList);
        }

        void Load(/*bool loadByDate = true*/) {
            string loadDateJsonStr = "";
            string loadMasterJsonStr = "";

            loadDateJsonStr = JsonFileUtility.LoadJsonFromFile(FOLDER_PATH + "\\" + FILENAME, JSONSTATE.PERSISTENT_DATA_PATH);
            loadMasterJsonStr = JsonFileUtility.LoadJsonFromFile(FOLDER_PATH + "\\" + MASTER_FILENAME, JSONSTATE.PERSISTENT_DATA_PATH);

            //if (loadByDate)
            //{
            if (string.IsNullOrEmpty(loadDateJsonStr)) {
                //if (AppManager.Instance.uiManager.useLocalLeaderboard)
                //    leaderboardDisplay.PopulateDefault();
            }
            else {
                playerInfoList = JsonArrayUtility.FromJsonWrapped<PlayerInfo>(loadDateJsonStr);
                // At least 1 entry to be sorted
                if (playerInfoList != null && playerInfoList.Count > 1) {
                    playerInfoList = Sort(playerInfoList);
                    Debug.Log("Current last entry: " + GetLastEntryScore());
                }
                else {
                    Debug.Log("Load leaderboard error!");
                }
            }
            if (string.IsNullOrEmpty(loadMasterJsonStr)) {
                return;
            }
            else {
                masterPlayerInfoList = JsonArrayUtility.FromJsonWrapped<PlayerInfo>(loadMasterJsonStr);
                if (masterPlayerInfoList != null && masterPlayerInfoList.Count > 1) {
                    masterPlayerInfoList = Sort(masterPlayerInfoList);
                }
            }
            // Populate the display in game
            //if (AppManager.Instance.uiManager.useLocalLeaderboard)
            //    leaderboardDisplay.PopulateData(playerInfoList);
        }

        // This method prefixes rank after sorting the score in the list
        void PrefixRank(List<PlayerInfo> list) {
            int index = 1;
            // need to have 1 entry in list
            if (list.Count > 0) {
                for (int i = 0; i < list.Count; i++) {
                    list[i].rank = index;
                    index++;
                }
            }
        }

        public int GetLastEntryScore() {
            int score = 0;
            if (playerInfoList.Count == 0 || playerInfoList.Count < number_of_slots) score = 0;
            else score = playerInfoList[playerInfoList.Count - 1].score;
            return score;
        }

        List<PlayerInfo> Sort(List<PlayerInfo> list) {
            // need > 1 entry
            list = list.OrderByDescending(y => y.score).ToList();
            // Get last entry score
            if (list.Count > number_of_slots) {
                lastEntry.name = list[slots.Count - 1].name;
                lastEntry.score = list[slots.Count - 1].score;
                Debug.Log(string.Format("Last Entry: Name: {0} | Score: {1}", lastEntry.name, lastEntry.score));
            }

            PrefixRank(list);
            return list;
        }

        void ClearFile(string fileName) {
            string path = Application.persistentDataPath + FOLDER_PATH + "\\" + fileName;
            if (File.Exists(path)) {
                Debug.Log(string.Format("Deleted: {0}", path));
                File.Delete(path);
                ClearData();
            }
            else {
                Debug.LogWarning(string.Format("{0} does not exist!", path));
            }
        }

        void ClearData() {
            playerInfoList.Clear();
            lastEntry = null;
            newEntry = null;
            //if (AppManager.Instance.uiManager.useLocalLeaderboard)
            //    leaderboardDisplay.PopulateDefault();
        }

#if UNITY_EDITOR
        PlayerInfo RandomAddUser() {
            PlayerInfo temp = new PlayerInfo();
            string[] names = { "abc", "cba", "asd", "ghj", "you" };
            int randName = Random.Range(0, names.Length);
            int randScore = Random.Range(50, 1000);

            temp.name = names[randName];
            temp.score = randScore;

            return temp;
        }
#endif
    }
}