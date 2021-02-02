using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TRINAX {
    public class PhotoboothManager : MonoBehaviour {

        public bool IsReady { get; set; }

        private byte[] png;
        private string encodedImageStr = "";
        private const string encodeToBase64toImage = "data:image/png;base64,";
        // File names and folder paths
        private string dropboxPhotoPath;
        private string localPhotoPath;
        private const string fileName = "\\tmp.png";

        public bool detectingTakePhoto = false;
        public bool IsTakingPhoto { get; set; }

        [Header("Image Data")]
        public RawImage review_cachedDisplay;
        public RawImage email_cachedDisplay;
        public RawImage thankyou_cachedDisplay;
        private Texture2D cachedImage;

        [Header("Component References")]
        public CountdownTimer countdownTimer;

        private void Start() {
            IsReady = false;
        }

        public void Init() {

            EventsCallback.OnCountdownEndEvent += OnCountdownEndEvent;
            countdownTimer.Init();

            cachedImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            review_cachedDisplay.texture = cachedImage;
            email_cachedDisplay.texture = cachedImage;
            thankyou_cachedDisplay.texture = cachedImage;

            IsTakingPhoto = false;

            IsReady = true;
            Debug.Log("<color=green> PhotoboothManager is ready! </color>");
        }

        public void PopulateSettings(GlobalSettings settings) {
            //localPhotoPath = settings.localPhotoPath;
            //dropboxPhotoPath = settings.dropboxPhotoPath;
        }

        private void OnCountdownEndEvent() {
            IsTakingPhoto = false;
            // Save to file
            WriteImageToFile(png);
        }

        public IEnumerator OnTakingPhoto() {
            int durationToDetect = 5;

            //if (Trinax.Instance.state != STATE.SELECTION) yield break;

            for (int i = durationToDetect; i > 0; i--) {
                //instructionsText.text = $"Please hold for {i} seconds.";
                yield return new WaitForSeconds(1.0f);

                if (true/*!cursorPointer.GetTakePhotoImage()!AppManager.Instance.kinectController.isRaisedHand*/) {
                    detectingTakePhoto = false;
                    //Debug.Log("Cursor not on photo taking image!");
                    yield break;
                }
                else {
                    detectingTakePhoto = true;
                    //Debug.Log($"Starting countdown to take a photo in {i} second");
                }
            }

            //cursorPointer.col.enabled = false;
            StartCountdown();
            detectingTakePhoto = false;
        }

        private void StartCountdown() {
            IsTakingPhoto = true;

            countdownTimer.gameObject.SetActive(true);
            countdownTimer.StartCountdown(true);
        }

        public void TakePhoto() {
            //AppManager.Instance.kinectController.usersCount.alpha = 0;

            Texture2D screenShot = new Texture2D(Screen.width, Screen.height);
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            Camera.main.targetTexture = rt;
            Camera.main.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            cachedImage.SetPixels(screenShot.GetPixels());
            cachedImage.Apply();
            png = screenShot.EncodeToPNG();


            // Clean up
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
        }

        private void WriteImageToFile(byte[] png) {
            try {
                // dropbox folder
                //File.WriteAllBytes(dropboxPhotoPath + fileName, jpeg);
                //Debug.Log("Photo saved to: " + dropboxPhotoPath);
                // local folder (in case)
                //File.WriteAllBytes(localPhotoPath + fileName, png);
                //Debug.Log("Photo saved to: " + localPhotoPath);

                //await APICalls.AddImage("data:image/png;base64," + enc, ()=> {
                //    ToReview();
                //});

                // Upload to dropbox folder as base64
                encodedImageStr = Convert.ToBase64String(png);
                //Debug.Log(encodedImageStr);


            }
            catch (Exception ex) {
                Debug.LogWarning(ex.Message);
            }
        }
    }
}
