using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Linq;
using System.IO;
using System.Text;

using TRINAX.Utils;

namespace TRINAX {
    [System.Serializable]
    public class LocalPlayerInfoJson {
        public string name;
        public int score;
    }

    public class LocalLeaderboardJson : MonoBehaviour {
        #region SINGLETON
        public static LocalLeaderboardJson Instance { get; set; }
        private void Awake() {
            if (Instance != null & Instance != this) Destroy(gameObject);
            else Instance = this;
        }
        #endregion

        public bool isLoaded = false;

        const string SAVEFILENAME = "leaderboard.json";

        public List<LocalPlayerInfoJson> data = new List<LocalPlayerInfoJson>();
        public List<GameObject> placesList = new List<GameObject>();
        public int NUMBER_OF_PLACES = 10;

        public GameObject infoDisplayPrefab;
        public Transform slotParent;

        int lastScore;
        string lastName;

        async void Start() {
            await new WaitUntil(() => Trinax.Instance.loadNow);
            Debug.Log("Loading Leaderboard...");
            isLoaded = false;
            Init();
            isLoaded = true;
            Debug.Log("Leaderboard is loaded!");
        }

        void Init() {
            //for (int i = 0; i < NUMBER_OF_PLACES; i++)
            //{
            //    AddPlayerInfo(InitBlankUsers());
            //}

            //CreateSlots();
            LoadLeaderboard();
        }

        private void Update() {
            if (!isLoaded) return;

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    AddPlayerInfo(RandomAddUser());
            //}

            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    LoadLeaderboard();
            //}

            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    Clear();
            //}
        }

        // Testing func to randomly add users
        LocalPlayerInfoJson RandomAddUser() {
            LocalPlayerInfoJson temp = new LocalPlayerInfoJson();
            string[] names = { "abc", "cba", "asd", "ghj", "you" };
            int randName = Random.Range(0, names.Length);
            int randScore = Random.Range(50, 1000);

            temp.name = names[randName];
            temp.score = randScore;

            return temp;
        }

        //  Use this to add info to leaderboard
        public LocalPlayerInfoJson CreatePlayerInfo(string _name, int _score) {
            LocalPlayerInfoJson info = new LocalPlayerInfoJson {
                name = _name,
                score = _score,
            };

            return info;
        }

        //LocalPlayerInfoJson InitBlankUsers()
        //{
        //    LocalPlayerInfoJson info = new LocalPlayerInfoJson
        //    {
        //        name = "-",
        //        score = 0,
        //    };

        //    return info;
        //}

        LocalPlayerInfoJson newlyAdded;
        public void AddPlayerInfo(LocalPlayerInfoJson pInfo) {
            newlyAdded = pInfo;
            data.Add(pInfo);

            string saveJsonStr = JsonArrayUtility.ToJson(data, true);
            JsonFileUtility.WriteJsonToFile(SAVEFILENAME, saveJsonStr, JSONSTATE.PERSISTENT_DATA_PATH);
            Debug.Log("Saving as JSON " + saveJsonStr);

            Sort();
        }

        public void LoadLeaderboard() {
            string loadJsonStr = JsonFileUtility.LoadJsonFromFile(SAVEFILENAME, JSONSTATE.PERSISTENT_DATA_PATH);
            // empty
            if (string.IsNullOrEmpty(loadJsonStr)) {
                for (int i = 0; i < placesList.Count; i++) {
                    int j = i + 1;
                    placesList[i].GetComponent<LeaderboardDisplay>().PopulateDefault();
                    //placesList[i].GetComponent<LeaderboardDisplay>().rankText.text = j.ToString();
                }
                return;
            }

            // existing
            data = JsonArrayUtility.FromJsonWrapped<LocalPlayerInfoJson>(loadJsonStr);
            if (data != null && data.Count > 0) {
                for (int i = 0; i < placesList.Count; i++) {
                    int j = i + 1;
                    //placesList[i].GetComponent<LeaderboardDisplay>().rankText.text = j.ToString();
                }

                //Debug.Log("BEFORE SORT");
                //for (int i = 0; i < data.Count; i++)
                //{
                //    // TODO: attach this to HUD
                //    Debug.Log(data[i].name + " : " + data[i].score + "\n");
                //}
                Sort();
            }
            // smth went wrong
            else {
                Debug.Log("Nothing to load for leaderboard!");
            }

        }

        void CreateSlots() {
            GameObject obj = null;
            for (int i = 0; i < NUMBER_OF_PLACES; i++) {
                obj = Instantiate(infoDisplayPrefab, this.transform) as GameObject;
                obj.transform.SetParent(slotParent);
                placesList.Add(obj);
            }
        }

        void Populate(List<LocalPlayerInfoJson> list) {
            for (int i = 0; i < placesList.Count; i++) {
                if (i >= list.Count) {
                    placesList[i].GetComponent<LeaderboardDisplay>().PopulateDefault();
                }
                else {
                    //placesList[i].GetComponent<LeaderboardDisplay>().PopulateData(list[i]);
                }
            }
        }

        void Sort() {
            List<LocalPlayerInfoJson> sortedTemp = data.OrderByDescending(x => x.score).ToList();
            if (sortedTemp != null && sortedTemp.Count > 2) {
                //Debug.Log("AFTER SORT");
                //for (int i = 0; i < sortedTemp.Count; i++)
                //{
                //    Debug.Log(sortedTemp[i].name + " : " + sortedTemp[i].score + "\n");
                ////}
                if (sortedTemp.Count > 10) {
                    lastScore = sortedTemp[placesList.Count - 1].score;
                    lastName = sortedTemp[placesList.Count - 1].name;
                    Debug.Log("Last name: " + lastName + " | " + "Last score: " + lastScore);
                }

                Populate(sortedTemp);

                if (newlyAdded != null) {
                    Debug.Log("Newlyadded: " + newlyAdded.score);
                    for (int i = 0; i < sortedTemp.Count; i++) {
                        if (newlyAdded == sortedTemp[i]) {
                            int index = sortedTemp.FindIndex(x => x.score == newlyAdded.score && x.name == newlyAdded.name);
                            //Debug.Log("index: " + index);
                            HighlightNewlyAdded(index);
                            newlyAdded = null;
                        }
                    }
                }
                else Debug.Log("No newly added place!");
            }
            else {
                Populate(sortedTemp);

                if (newlyAdded != null) {
                    Debug.Log("Newlyadded: " + newlyAdded.score);
                    for (int i = 0; i < sortedTemp.Count; i++) {
                        if (newlyAdded == sortedTemp[i]) {
                            int index = sortedTemp.FindIndex(x => x.score == newlyAdded.score && x.name == newlyAdded.name);
                            //Debug.Log("index: " + index);
                            HighlightNewlyAdded(index);
                            newlyAdded = null;
                        }
                    }
                }
                Debug.Log("Not enough to sort!");
            }
        }

        public void HighlightNewlyAdded(int index) {
            //index = index - 1;
            for (int i = 0; i < placesList.Count; i++) {
                if (index == i) {
                    //Debug.Log(placesList[i].name);
                    //placesList[i].GetComponent<LeaderboardDisplay>().doBlink = true;
                    //DoHighlight();
                }
            }
        }

        public int GetLastScore() {
            return lastScore;
        }

        public void Clear() {
            string emptyStr = "";
            if (JsonFileUtility.FileExists(SAVEFILENAME)) {
                string path = Application.persistentDataPath + "/" + SAVEFILENAME;
                Debug.Log("File found...clearing it now");

                FileStream stream = File.Create(path);
                byte[] contentBytes = new UTF8Encoding(true).GetBytes(emptyStr);
                stream.Write(contentBytes, 0, contentBytes.Length);
                stream.Close();
                Debug.Log(emptyStr + " has been written to " + path);

                if (data.Count > 0) data.Clear();

                if (placesList.Count > 0) {
                    for (int i = 0; i < placesList.Count; i++) {
                        placesList[i].GetComponent<LeaderboardDisplay>().PopulateDefault();
                    }
                }
            }
            else {
                Debug.Log("File not found!");
            }
        }
    }
}