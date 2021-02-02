using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TRINAX.Networking {
    public static class APICalls {
        private const string DEVICEID = "001";
        private const string PROJECTID = "19201906031726124714487252743244008143275f-ffa4-4624-8036-d70cb9bb3fa4";

        #region APICALLS
        public static async Task RunStartInteraction() {
            StartInteractionSendJsonData sJson = new StartInteractionSendJsonData {
                deviceID = DEVICEID,
                projectID = PROJECTID,
            };

            await Trinax.Instance.serverManager.StartInteraction(sJson, (bool success, StartInteractionReceiveJsonData rJson) => {
                if (success) {
                    if (rJson.data != null) {
                        Trinax.Instance.userData.interactionID = rJson.data;
                        Debug.Log("Started interaction!");
                    }
                }
                else {
                    Debug.Log("Error in <StartInteraction> API!");
                }
            });
        }

        public static async Task RunAddInteraction(string _location) {
            InteractionDetailsSendJsonData sJson = new InteractionDetailsSendJsonData {
                interactionID = Trinax.Instance.userData.interactionID,
                location = _location,
            };

            await Trinax.Instance.serverManager.AddInteractionDetails(sJson, (bool success, InteractionDetailsReceiveJsonData rJson) => {
                if (success) {
                    Debug.Log("Added interaction!");
                }
                else {
                    Debug.Log("Error in <AddInteraction> API!");
                }
            });
        }

        public static async Task RunEndInteraction() {
            InteractionEndSendJsonData sJson = new InteractionEndSendJsonData();

            await Trinax.Instance.serverManager.EndInteraction(sJson, (bool success, InteractionEndReceiveJsonData rJson) => {
                if (success) {
                    if (rJson.data) {
                        Debug.Log("End interaction!");
                        Trinax.Instance.userData.interactionID = "";
                    }
                }
                else {
                    Debug.Log("Error in <EndInteraction> API!");
                }
            });
        }

        /// <summary>
        /// API call to LED wall
        /// 0 to turn off all lights
        /// 1 - 15 (1st arduino)
        /// 16 - 30 (2nd arduino)
        /// 31 - 45 (3rd arduino)
        /// 46 - 60 (4th arduino)
        /// </summary>
        /// <param name="LED_index"></param>
        /// <returns></returns>
        public static async Task SendColorToLED(int LED_index) {
            Debug.Log("Sending " + LED_index);
            await Trinax.Instance.serverManager.SendLEDLight(LED_index, (success, result) => {
                if (success) {
                    Debug.Log("<color=green>API <LED Trigger> Success!</color>" + "\n" + result);
                }
                else {
                    Debug.Log("<color=red>API <LED Trigger> Failed!</color>");
                }
            });
        }

        public static async Task SendColorToLED(int LED_index, System.Action callback) {
            Debug.Log("Sending " + LED_index);
            await Trinax.Instance.serverManager.SendLEDLight(LED_index, (success, result) => {
                if (success) {
                    Debug.Log("<color=green>API <LED Trigger> Success!</color>" + "\n" + result);
                    callback?.Invoke();
                }
                else {
                    Debug.Log("<color=red>API <LED Trigger> Failed!</color>");
                }
            });
        }

        /*public static async Task SendEmail(System.Action callback) {
            AddPlayerInfoSendJsonData sJson = new AddPlayerInfoSendJsonData {
                email = Trinax.Instance.userData.email,
                imageurl = Trinax.Instance.userData.imageUrl
            };

            await Trinax.Instance.serverManager.AddPlayerInfo(sJson, (bool success, AddPlayerInfoReceiveData rJson) => {
                if (success) {
                    Debug.Log("Send email success!");
                    callback?.Invoke();
                }
                else {
                    Debug.Log("Send email fail!");
                }
            });
        }

        public static async Task AddImage(string base64Str) {
            await Trinax.Instance.serverManager.AddImage(base64Str, (bool success, string result) => {
                if (success) {
                    Debug.Log("Added to dropbox");
                    Trinax.Instance.userData.imageUrl = result;
                }
            });
        }*/

        /* public static async Task RunPopulateLeaderboard(LeaderboardDisplay lb) {
             List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
             await TrinaxAsyncServerManager.Instance.PopulateLeaderboard((bool success, LeaderboardReceiveJsonData rJson) => {
                 if (success) {
                     if (rJson.data != null) {
                         for (int i = 0; i < rJson.data.Count; i++) {
                             PlayerInfo info = new PlayerInfo {
                                 rank = int.Parse(rJson.data[i].playerid),
                                 name = rJson.data[i].name,
                                 mobile = rJson.data[i].mobile,
                                 score = rJson.data[i].score,
                             };
                             playerInfoList.Add(info);
                         }
                         lb.PopulateData(playerInfoList);
                     }
                     else {
                         Debug.Log("no data");
                     }
                 }
                 else {
                     Debug.Log("Error in <getLeaderBoard> API!");
                     lb.PopulateDefault();
                 }
             });
         }*/
        #endregion
    }
}