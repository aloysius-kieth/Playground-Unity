using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

using System.Threading.Tasks;

namespace TRINAX.Networking {
    /// <summary>
    /// Server Manager
    /// </summary>
    public class TrinaxServerManager : MonoBehaviour, IManager {
        int executionPriority = 200;
        public int ExecutionPriority { get { return executionPriority; } set { value = executionPriority; } }

        public bool IsReady { get; set; }

        private void Start() {

        }

        public void Init() {
            Debug.Log("<color=yellow>Loading TrinaxServerManager...</color>");
            IsReady = false;

            //LOG_DIR = Application.dataPath + LOG_DIR;

            //if (!Directory.Exists(LOG_DIR))
            //    Directory.CreateDirectory(LOG_DIR);

            if (loadingCircle != null)
                loadingCircle.SetActive(false);

            IsReady = true;
            Debug.Log("<color=lime>TrinaxServerManager is loaded!</color>");
        }

        public void PopulateValues(GlobalSettings setting) {
            ip = setting.IP;
            useServer = setting.useServer;
            useMocky = setting.useMocky;
        }

        public static Action OnConnectionLost;
        public static Action OnMaxRetriesReached;

        public string ip = "127.0.0.1";
        public bool useServer;
        public bool useMocky;
        public GameObject loadingCircle;

        public string userID;

        const string interactionServletUrl = "SPFARPhotoBooth/servlet/interactionApiServlet/";
        const string playerServletUrl = "SPFARPhotoBooth/servlet/Api/";
        const string port = ":8080/";

        public const string ERROR_CODE_200 = "200";
        public const string ERROR_CODE_201 = "201";

        bool isLoading = false;
        bool isVerifying = false;
        bool isDelayedScanCoroutineRunning = false;

        string LOG_DIR = "/log/";
        string LOG_FILE = "Async_server_logs.log";

        #region API Methods
        public async Task StartInteraction(StartInteractionSendJsonData json, Action<bool, StartInteractionReceiveJsonData> callback) {
            StartInteractionReceiveJsonData data;
            if (!useServer) {
                Debug.LogWarning("Use Server toggle OFF!");
                return;
            }

            string sendJson = JsonUtility.ToJson(json);
            string url;

            url = "http://" + ip + port + interactionServletUrl + "startInteraction";

            var r = await LoadPostUrl(url, sendJson, (request) => {
                if (string.IsNullOrEmpty(request.error)) {
                    string result = request.text.Trim();
                    data = JsonUtility.FromJson<StartInteractionReceiveJsonData>(result);
                    //Debug.Log("myresult: " + result);

                    if (data.error.error_code == ERROR_CODE_200) {
                        callback(true, data);
                    }
                    else {
                        callback(false, data);
                    }
                }
                else {
                    //WriteError(request, "StartInteraction");
                    data = new StartInteractionReceiveJsonData();
                    callback(false, data);
                }
            });

            //Debug.Log(r);
        }

        public async Task AddInteractionDetails(InteractionDetailsSendJsonData json, Action<bool, InteractionDetailsReceiveJsonData> callback) {
            InteractionDetailsReceiveJsonData data;
            if (!useServer) {
                Debug.LogWarning("Use Server toggle OFF!");
                return;
            }

            string sendJson = JsonUtility.ToJson(json);
            string url;

            url = "http://" + ip + port + interactionServletUrl + "addInteraction";

            var r = await LoadPostUrl(url, sendJson, (request) => {
                if (string.IsNullOrEmpty(request.error)) {
                    string result = request.text.Trim();
                    data = JsonUtility.FromJson<InteractionDetailsReceiveJsonData>(result);

                    if (data.error.error_code == ERROR_CODE_200) {
                        callback(true, data);
                    }
                    else {
                        callback(false, data);
                    }
                }
                else {
                    //WriteError(request, "InteractionDetails");
                    data = new InteractionDetailsReceiveJsonData();
                    callback(false, data);
                }
            });

            //Debug.Log(r);
        }

        public async Task EndInteraction(InteractionEndSendJsonData json, Action<bool, InteractionEndReceiveJsonData> callback) {
            InteractionEndReceiveJsonData data;
            if (!useServer) {
                Debug.LogWarning("Use Server toggle OFF!");
                return;
            }

            string sendJson = JsonUtility.ToJson(json);
            string url;

            url = "http://" + ip + port + interactionServletUrl + "endInteraction/" + Trinax.Instance.userData.interactionID;

            var r = await LoadPostUrl(url, sendJson, (request) => {
                if (string.IsNullOrEmpty(request.error)) {
                    string result = request.text.Trim();
                    data = JsonUtility.FromJson<InteractionEndReceiveJsonData>(result);

                    if (data.error.error_code == ERROR_CODE_200) {
                        callback(true, data);
                    }
                    else {
                        callback(false, data);
                    }
                }
                else {
                    //WriteError(request, "EndInteraction");
                    data = new InteractionEndReceiveJsonData();
                    callback(false, data);
                }
            });

            //Debug.Log(r);
        }

        public async Task SendLEDLight(int LED_index, Action<bool, string> callback) {
            if (!useServer) {
                Debug.LogWarning("Use Server toggle OFF!");
                return;
            }

            string url = "http://" + ip + port + "toyota/toyota.do?formType=sendTrigger&trigger=" + LED_index;

            await LoadUrl(url, (request) => {
                if (request.isNetworkError || request.isHttpError) {
                    Debug.Log(request.error);
                    callback(false, "");
                }
                else {
                    callback(true, request.downloadHandler.text);
                }
            });
        }

        /*public async Task AddImage(string base64Str, Action<bool, string> callback) {
            //AddImageReceiveData data;
            if (!useServer) {
                //data = new InteractionEndReceiveJsonData();
                //callback(true, data);
                return;
            }

            //string sendJson = JsonUtility.ToJson(json);
            string url;

            if (!useMocky) {
                //Debug.Log("Using actual server url...");
                url = "http://" + ip + port + playerServletUrl + "AddImage";
            }
            else {
                //Debug.Log("Using mocky url...");
                return;
            }
            string result = "";

            var r = await LoadPostUrl(url, base64Str, (request) => {
                if (string.IsNullOrEmpty(request.error)) {
                    result = request.text.Trim();
                    if (!string.IsNullOrEmpty(result)) {
                        callback(true, result);
                        //Debug.Log(result);
                    }
                    else {
                        callback(false, result);
                    }
                }
                else {
                    //WriteError(request, "EndInteraction");
                    callback(false, result);
                }
            });

            //Debug.Log(r);
        }*/

        /*public async Task VerifyQrCodeAsync(VerifyQRSendJsonData json, Action<bool, VerifyQRReceiveJsonData> callback) {
            VerifyQRReceiveJsonData data;
            if (!useServer) {
                data = new VerifyQRReceiveJsonData();
                callback(true, data);
                return;
            }

            // TODO: Check if main manager is at a certain point in the game for this API to be called

            if (isVerifying) {
                if (isDelayedScanCoroutineRunning) {
                    Debug.Log("Already scanning!");
                }
                else {
                    Debug.Log("Running delay qr scanner task");
                    DelayQrScanner();
                }
                return;
            }
            isVerifying = true;

            string sendJson = JsonUtility.ToJson(json);
            string url;

            if (!useMocky) {
                Debug.Log("Using actual server url...");
                url = "http://www.mocky.io/v2/5b7e53fe3000007a0084c0d4";
                //url = "http://" + ip + port + frontUrl + "formType=addInteraction&qrID=" + qrCode;
            }
            else {
                Debug.Log("Using mocky url...");
                // returns ID
                url = "http://www.mocky.io/v2/5b7e53fe3000007a0084c0d4";
            }

            var r = await LoadPostUrl(url, sendJson, (request) => {
                if (string.IsNullOrEmpty(request.error)) {
                    string result = request.text.Trim();
                    data = JsonUtility.FromJson<VerifyQRReceiveJsonData>(result);
                    //Debug.Log("myresult: " + result);

                    if (data.error.error_code == ERROR_CODE_200) {
                        // Get userID from server

                        callback(true, data);
                    }
                    else {
                        callback(false, data);
                    }
                }
                else {
                    WriteError(request, "VerifyQR");
                    data = new VerifyQRReceiveJsonData();
                    callback(false, data);
                }
            });

            DelayQrScanner();
            //Debug.Log(r);
        }*/

        /*public async Task PopulateLeaderboard(Action<bool, LeaderboardReceiveJsonData> callback) {
            LeaderboardReceiveJsonData data;
            if (!useServer) {
                //data = new LeaderboardReceiveJsonData();
                //callback(true, data);
                return;
            }

            string url;
            if (!useMocky) {
                Debug.Log("Using actual server url...");
                url = "http://" + ip + port + playerServletUrl + "getLeaderBoard";
            }
            else {
                Debug.Log("Using mocky url...");
                url = "http://www.mocky.io/v2/5ba30c4e2f000077008d2eee";
            }

            var r = await LoadUrlAsync(url, (request) => {
                if (string.IsNullOrEmpty(request.error)) {
                    string result = request.text.Trim();
                    data = JsonUtility.FromJson<LeaderboardReceiveJsonData>(result);
                    //Debug.Log("myresult: " + result);

                    if (data.error.error_code == ERROR_CODE_200) {
                        callback(true, data);
                    }
                    else {
                        callback(false, data);
                    }
                }
                else {
                    WriteError(request, "PopulateLeaderboard");
                    data = new LeaderboardReceiveJsonData();
                    callback(false, data);
                }
            });
        }*/
        #endregion

        async Task<string> LoadUrlAsync(string url, Action<WWW> callback) {
            isLoading = true;

            DelayLoadingCircle();

            Debug.Log("Loading url: " + url);
            WWW request = await new WWW(url);
            Debug.Log(url + "\n" + request.text);

            //if (Trinax.Instance.state == PAGES.SCREENSAVER)
            //    MainManager.Instance.appStartBtn.interactable = false;

            await ApiCallback(request, url, (WWW r) => { request = r; });
            //await new WaitForSeconds(3.0f); // artifical wait

            callback(request);

            isLoading = false;
            if (loadingCircle != null) {
                loadingCircle.SetActive(false);
            }

            Debug.Log(request.text);
            return request.text;
        }

        async Task<string> LoadPostUrl(string url, string jsonString, Action<WWW> callback) {
            isLoading = true;
            //then set the headers Dictionary headers=form.headers; headers["Content-Type"]="application/json";

            DelayLoadingCircle();

            WWWForm form = new WWWForm();
            byte[] jsonSenddata = null;
            if (!string.IsNullOrEmpty(jsonString)) {
                Debug.Log(jsonString);
                jsonSenddata = System.Text.Encoding.UTF8.GetBytes(jsonString);
            }

            form.headers["Content-Type"] = "application/json";
            form.headers["Accept"] = "application/json";
            Dictionary<string, string> headers = form.headers;
            headers["Content-Type"] = "application/json";

            Debug.Log("Loading url: " + url);
            WWW request = await new WWW(url, jsonSenddata, headers);
            Debug.Log(url + "\n" + request.text);

            await ApiCallback(request, url, jsonSenddata, headers, (WWW r) => { request = r; });

            //await new WaitForSeconds(3f); // artifical wait for 150ms

            callback(request);

            isLoading = false;
            if (loadingCircle != null) {
                loadingCircle.SetActive(false);
            }
            //Debug.Log(request.text);
            return request.text;
        }

        async Task ApiCallback(WWW _request, string _url, byte[] _data, Dictionary<string, string> _headers, Action<WWW> _result) {
            await LoopResultPost(_request, _url, _data, _headers, _result);
        }

        async Task ApiCallback(WWW _request, string _url, Action<WWW> _result) {
            await LoopResultGet(_request, _url, _result);
        }

        int numOfRetries = 10;
        public GameObject lostConnectionPanel;
        IEnumerator LoopResultPost(WWW _request, string _url, byte[] _data, Dictionary<string, string> _headers, Action<WWW> _result) {
            int tries = 0;
            while (true) {
                if (string.IsNullOrEmpty(_request.text)) {
                    OnConnectionLost?.Invoke();
                    WWW r = new WWW(_url, _data, _headers);
                    yield return r;
                    if (string.IsNullOrEmpty(r.text)) {
                        if (!lostConnectionPanel.activeSelf) {
                            lostConnectionPanel.SetActive(true);
                        }
                        tries++;
                        if (tries >= numOfRetries) {
                            Debug.Log("Tried " + numOfRetries + " !" + " Going back to start...");
                            OnMaxRetriesReached?.Invoke();
                            yield return new WaitForSeconds(5f);
                            // GameManager.Instance.ToScreensaver();
                            lostConnectionPanel.SetActive(false);
                            yield break;
                        }
                        Debug.Log("Request from server is empty! Getting a new request...");

                        yield return new WaitForSeconds(3f);
                    }
                    else {
                        //requestPost = r;
                        _result(r);
                        Debug.Log("Result: " + r.text);
                        lostConnectionPanel.SetActive(false);
                        yield break;
                    }
                }
                else yield break;
            }
        }

        IEnumerator LoopResultGet(WWW _request, string _url, Action<WWW> _result) {
            int tries = 0;
            while (true) {
                if (string.IsNullOrEmpty(_request.text)) {
                    OnConnectionLost?.Invoke();
                    WWW r = new WWW(_url);
                    yield return r;
                    if (string.IsNullOrEmpty(r.text)) {
                        if (!lostConnectionPanel.activeSelf) {
                            lostConnectionPanel.SetActive(true);
                        }
                        tries++;
                        Debug.Log(tries);
                        if (tries >= numOfRetries) {
                            Debug.Log("Tried " + numOfRetries + " !" + " Going back to start...");
                            OnMaxRetriesReached?.Invoke();
                            yield return new WaitForSeconds(5f);
                            //GameManager.Instance.ToScreensaver();
                            lostConnectionPanel.SetActive(false);
                            yield break;
                        }
                        Debug.Log("Request from server is empty! Getting a new request...");

                        yield return new WaitForSeconds(3f);
                    }
                    else {
                        _result(r);
                        //resultGet = r;
                        Debug.Log("Result: " + r.text);
                        lostConnectionPanel.SetActive(false);
                        yield break;
                    }
                }
                else yield break;
            }
        }

        async void DelayLoadingCircle() {
            await new WaitForSeconds(1.5f);

            if (isLoading && loadingCircle != null)
                loadingCircle.SetActive(true);
        }

        async void DelayQrScanner() {
            isDelayedScanCoroutineRunning = true;
            await new WaitForSeconds(3f);

            isVerifying = false;
            isDelayedScanCoroutineRunning = false;
        }

        private async Task<string> LoadUrl(string url, Action<UnityWebRequest> callback) {
            isLoading = true;
            DelayLoadingCircle();

            using(UnityWebRequest uwr = UnityWebRequest.Get(url)) {
                await uwr.SendWebRequest();

                Debug.Log(url + "\n" + uwr.downloadHandler.text);
                callback(uwr);
                isLoading = false;
                if (loadingCircle != null) {
                    loadingCircle.SetActive(false);
                }

                return uwr.downloadHandler.text;
            }
        }

        private void WriteError(WWW request, string api) {
            string error = "<" + api + "> --- Begin Error Message: " + request.error + " >> Url: " + request.url + System.Environment.NewLine;
            File.AppendAllText(LOG_DIR + LOG_FILE, error);
        }

        private void WriteError(string errorStr, string api) {
            string error = "<" + api + "> --- Begin Error Message: " + errorStr + System.Environment.NewLine;
            File.AppendAllText(LOG_DIR + LOG_FILE, error);
        }
    }
}