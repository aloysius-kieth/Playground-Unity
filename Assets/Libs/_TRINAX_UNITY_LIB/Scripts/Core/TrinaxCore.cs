using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using TRINAX.SceneManagement;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading.Tasks;

namespace TRINAX {
    public class TrinaxCore : MonoBehaviour {
        #region Singleton
        private static TrinaxCore instance;
        public static TrinaxCore Instance {
            get {
                if (instance == null) {
                    GameObject prefab = Resources.Load("Prefabs/_TRI") as GameObject;
                    GameObject core = Instantiate(prefab);
                    instance = core.GetComponent<TrinaxCore>();
                    //GameObject trinaxCore = new GameObject("_TriCore");
                    //instance = trinaxCore.AddComponent<TrinaxCore>();
                    return instance;
                }
                else {
                    return instance;
                }
            }
            set {
                instance = value;
            }
        }
        #endregion

        private TrinaxLoadSceneAsync _loadAsync;
        private SceneManager _sceneManager;

        public SceneManager SceneManager { get { return _sceneManager; } }

        private void Awake() {
            gameObject.name = "_TRI";
            //if (Instance != null && Instance != this)
            //    Destroy(gameObject);
            //else {
            //    Instance = this;
            //    DontDestroyOnLoad(this);
            //}
        }
        private void Start() {
            if (instance != null && instance != this) {
                Destroy(gameObject);
            }
            else {
                instance = this;
                DontDestroyOnLoad(this);
            }
            _loadAsync = GetComponentInChildren<TrinaxLoadSceneAsync>(true);
            _sceneManager = GetComponentInChildren<SceneManager>();
        }
        #region Load Images from URL / bytes
        public async Task LoadImageFromUrlAsync(Image target, string url, bool useImgSizeFromUrl, System.Action<byte[]> callback = null, Image placeHolder = null) {
            if (placeHolder != null) {
                Debug.Log("Displaying placeholder first...");
                target.sprite = placeHolder.sprite;
                target.rectTransform.sizeDelta = new Vector2(placeHolder.mainTexture.width, placeHolder.mainTexture.height);
            }

            Debug.Log("Loading image from: " + url);
            var request = await new WWW(url);
            target.sprite = Sprite.Create(request.texture, new Rect(0, 0, request.texture.width, request.texture.height), new Vector2(0, 0));

            if (useImgSizeFromUrl) {
                target.rectTransform.sizeDelta = new Vector2(request.texture.width, request.texture.height);
            }

            callback(request.bytes);
        }

        public async Task LoadRawImageFromUrlAsync(RawImage target, string url, bool useImgSizeFromUrl, System.Action<byte[]> callback = null, RawImage placeHolder = null) {
            if (placeHolder != null) {
                Debug.Log("Displaying placeholder first...");
                target.texture = placeHolder.texture;
                target.rectTransform.sizeDelta = new Vector2(placeHolder.texture.width, placeHolder.texture.height);
            }

            Debug.Log("Loading image from: " + url);
            var request = await new WWW(url);

            Texture2D texture = new Texture2D(request.texture.width, request.texture.height, TextureFormat.ARGB32, false);
            texture.SetPixels(request.texture.GetPixels(0, 0, request.texture.width, request.texture.height));
            texture.Apply();

            if (useImgSizeFromUrl) {
                target.rectTransform.sizeDelta = new Vector2(request.texture.width, request.texture.height);
            }
            target.texture = texture;

            callback(request.bytes);
        }

        public void LoadImageFromBytes(Image target, byte[] imgData, bool useLoadedImgSize) {
            Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            texture.LoadImage(imgData);
            texture.Apply();

            target.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            if (useLoadedImgSize)
                target.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
        }

        public void LoadRawImageFromBytes(RawImage target, byte[] imgData, bool useLoadedImgSize) {
            Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            texture.LoadImage(imgData);
            texture.Apply();

            target.texture = texture;
            if (useLoadedImgSize)
                target.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
        }
        #endregion

        #region Send Email
        public void SendEmailInBackground(string sender, string senderPassword, string receiver, string title, string body, string attachment = "", string smtpServerName = "smtp.gmail.com") {
            Debug.Log("Attempting to send email...");
            MailMessage mail = new MailMessage {
                From = new MailAddress(sender),
                Subject = title,
                Body = body,
                IsBodyHtml = true,
            };
            mail.To.Add(receiver);

            if (!string.IsNullOrEmpty(attachment)) {
                mail.Attachments.Add(new Attachment(attachment));
            }

            Debug.Log("Connecting to SMTP server");
            SmtpClient smtpServer = new SmtpClient(smtpServerName) {
                Port = 587,
                Credentials = new NetworkCredential(sender, senderPassword) as ICredentialsByHost,
                EnableSsl = true
            };
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                return true;
            };
            Debug.Log("Sending message");

            smtpServer.SendCompleted += (s, e) => {
                Debug.Log("Attachment: " + mail.Attachments.Count);
                Debug.Log("Send Completed");
                mail.Dispose();
            };

            try {
                smtpServer.SendAsync(mail, null);
                Debug.Log("Sending...");
            }
            catch (SmtpException e) {
                Debug.Log(e);
            }
        }
        #endregion

        #region Helper Methods
        public static async void FadeText(TextMeshProUGUI text, string msg, float fadeInDuration, float fadeOutDuration, Action callback = null) {
            text.alpha = 0;
            text.text = "";

            text.text = msg;
            text.DOFade(1.0f, fadeInDuration);

            await new WaitForSeconds(2.0f);
            text.DOFade(0.0f, fadeOutDuration).OnComplete(() => {
                callback?.Invoke();
            });
        }
        public static string FloatToTime(float toConvert, string format) {
            switch (format) {
                case "00.0":
                    return string.Format("{0:00}:{1:0}",
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 10) % 10));//miliseconds
                case "#0.0":
                    return string.Format("{0:#0}:{1:0}",
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 10) % 10));//miliseconds
                case "00.00":
                    return string.Format("{0:00}:{1:00}",
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 100) % 100));//miliseconds
                case "00.000":
                    return string.Format("{0:00}:{1:000}",
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                case "#00.000":
                    return string.Format("{0:#00}:{1:000}",
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                case "#0:00":
                    return string.Format("{0:#0}:{1:00}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60);//seconds
                case "#00:00":
                    return string.Format("{0:#00}:{1:00}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60);//seconds
                case "0:00.0":
                    return string.Format("{0:0}:{1:00}.{2:0}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 10) % 10));//miliseconds
                case "#0:00.0":
                    return string.Format("{0:#0}:{1:00}:{2:0}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 10) % 10));//miliseconds
                case "0:00.00":
                    return string.Format("{0:0} : {1:00} : {2:00}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 100) % 100));//miliseconds
                case "#0:00.00":
                    return string.Format("{0:#0}:{1:00}.{2:00}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 100) % 100));//miliseconds
                case "0:00.000":
                    return string.Format("{0:0}:{1:00}.{2:000}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                case "#0:00.000":
                    return string.Format("{0:#0}:{1:00}.{2:000}",
                        Mathf.Floor(toConvert / 60),//minutes
                        Mathf.Floor(toConvert) % 60,//seconds
                        Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
            }
            return "Wrong format";
        }
        /// <summary>
        /// Get values from enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        /// <summary>
        /// Get distance between 2 points
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float GetSqrDistance(Vector3 v1, Vector3 v2) {
            return (v1 - v2).sqrMagnitude;
        }
        /// <summary>
        /// Map values from 0 to 1 based on value and max value
        /// </summary>
        /// <param name="mainValue"></param>
        /// <param name="inValueMin"></param>
        /// <param name="inValueMax"></param>
        /// <param name="outValueMin"></param>
        /// <param name="outValueMax"></param>
        /// <returns></returns>
        public static float MapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax) {
            return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
        }
        /// <summary>
        /// Round decimal values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static float RoundDecimal(float value, int digits) {
            float mult = Mathf.Pow(10.0f, digits);
            return Mathf.Round(value * mult) / mult;
        }
        /// <summary>
        /// Lerp over an array of positions
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Vector3 LerpOverDistance(Vector3[] vectors, float time) {
            time = Mathf.Clamp01(time);
            if (vectors == null || vectors.Length == 0) {
                throw (new Exception("Vectors input must have at least one value"));
            }
            if (vectors.Length == 1) {
                return vectors[0];
            }

            if (time == 0) {
                return vectors[0];
            }

            if (time == 1) {
                return vectors[vectors.Length - 1];
            }

            float[] distances = new float[vectors.Length - 1];
            float total = 0;
            for (int i = 0; i < vectors.Length; i++) {
                distances[i] = (vectors[i] - vectors[i + 1]).sqrMagnitude;
                total += distances[i];
            }

            float current = total * time;
            int p = 0;
            while (current - distances[p] > 0) {
                current -= distances[p++];
            }
            if (distances[p] == 0) return vectors[p];

            return Vector3.Lerp(vectors[p], vectors[p + 1], current / distances[p]);
        }
        #endregion

        public void ChangeLevel(SCENE scene, Action callback = null) {
            //Trinax.Instance.scene = scene;
            //sceneManager.currentScene = scene;
            Trinax.Instance.isChangingLevels = true;
            //Trinax.Instance.doneLoadComponentReferences = false;

            int index = (int)scene;
            _loadAsync.gameObject.SetActive(true);
            _loadAsync.LoadLevel(index, () => {
                callback?.Invoke();
                Trinax.Instance.isChangingLevels = false;
            });
        }

        public void Init() {
            Debug.Log($"{name} self-initialized!");
        }
    }

    public static class Extension {
        private static System.Random rng = new System.Random();
        /// <summary>
        /// Shuffle values in a List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

