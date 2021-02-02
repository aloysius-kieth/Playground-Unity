using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using com.rfilkov.kinect;
using com.rfilkov.components;
using TMPro;

namespace TRINAX {
    public class KinectController : MonoBehaviour, GestureListenerInterface {
        public bool IsReady { get; set; }
        public RawImage displayColorImage;

        public TextMeshProUGUI usersCount;
        public TextMeshProUGUI userDist;
        private Vector2 velocity;
        private float smoothTime = 0.05f;
        public RectTransform cursor;

        //[Header("Filters")]
        //[Space(5)]
        //public List<Filter> FiltersList;
        //public Filter filter;

        [Header("Kinect Users")]
        [Space(5)]

        public List<ulong> KinectIDS;

        public float timeToWaitForDetection = 2f;
        private bool sendActivation = false;
        private bool detectIdle = false;
        private bool debugMode = false;
        public bool isRaisedHand = false;

        private int maxTrackedUsers = 0;
        private int totalTrackedUsers = 0;

        [HideInInspector]
        public KinectManager kinectManager;
        [SerializeField]
        //private KinectSettings _kinectSettings;
        private KinectGestureManager gestureManager;

        [Tooltip("Reference to the chest joint-overlayer component.")]
        public JointOverlayer chestOverlayer;
        [Tooltip("Array of sprite transforms that will be used for chest overlays on each step.")]
        public Transform[] chestMasks;

        private int maskCount = 0;
        private int currentIndex = -1;
        private int prevIndex = -1;

        #region UNITY
        private void Awake() {
            kinectManager = GetComponent<KinectManager>();
            //SaveSettings.Init();
            //UpdateKinectVariables(SaveSettings.kinectSettings);
            ShowDisplayImage(false);
        }

        private void Start() {
            IsReady = false;

            maskCount = 0;
            if (chestMasks != null && chestMasks.Length > maskCount)
                maskCount = chestMasks.Length;
        }

        public void Init() {
            usersCount.text = "Users: 0";
            maxTrackedUsers = kinectManager.maxTrackedUsers;
            KinectIDS = new List<ulong>();
            //TrackingFiltersList = new List<TrackingFilter>();

            gestureManager = kinectManager.gestureManager;

            if (!kinectManager.IsInitialized() && displayColorImage.gameObject.activeSelf) {
                Debug.Log("Hiding displayimage");
                displayColorImage.gameObject.SetActive(false);
                AppManager.Instance.debugMode = true;
            }

            Debug.Log("<color=green> KinectController is ready! </color>");
            IsReady = true;

            ShowDebugText(debugMode);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F11)) {
                debugMode = !debugMode;
                ShowDebugText(debugMode);
            }

            if (!kinectManager && !kinectManager.IsInitialized()) return;

            UpdateColorImage();
            //UpdateCursorPosition();

            // Debug user dist
            if (KinectIDS.Count > 0) {
                userDist.text = "Dist: " + "\n" + "X: " + kinectManager.GetUserPosition(KinectIDS[0]).x + "\n" + "Z: " + kinectManager.GetUserPosition(KinectIDS[0]).z;
            }
            else {
                userDist.text = "";
            }

            if (/*Trinax.Instance.state == STATE.SELECTION*/true) {
                if (currentIndex != prevIndex) {
                    prevIndex = currentIndex;

                    if (chestOverlayer && chestMasks != null) {
                        if (chestOverlayer.overlayObject) {
                            chestOverlayer.overlayObject.rotation = chestOverlayer.initialRotation;
                            chestOverlayer.overlayObject.gameObject.SetActive(false);
                        }

                        chestOverlayer.overlayObject = currentIndex >= 0 && currentIndex < chestMasks.Length ? chestMasks[currentIndex] : null;
                        chestOverlayer.playerIndex = 0;

                        if (chestOverlayer.overlayObject) {
                            chestOverlayer.overlayObject.gameObject.SetActive(true);
                            chestOverlayer.gameObject.SetActive(true);
                        }

                        chestOverlayer.Start();
                    }
                }
            }
            //else {
            //    chestOverlayer.overlayObject = null;
            //}
        }
        //private void UpdateKinectVariables(KinectSettings _settings)
        //{
        //    _kinectSettings = _settings;
        //    kinectManager.maxTrackedUsers = _settings.maxTrackedUsers;
        //    kinectManager.minUserDistance = _settings.minDetectionDistance;
        //    kinectManager.maxUserDistance = _settings.maxDetectionDistance;
        //    kinectManager.maxLeftRightDistance = _settings.maxSideDetectionDistance;
        //}
        #endregion

        #region KINECT RELATED METHODS
        /// <summary>
        /// Update user count GUI
        /// </summary>
        private void UpdateUserCount() {
            totalTrackedUsers = kinectManager.GetUsersCount();
            usersCount.text = $"Users: {totalTrackedUsers.ToString()}";
        }
        /// <summary>
        /// Update color image from kinect
        /// </summary>
        private void UpdateColorImage() {
            // Update color image display from kinect
            if (displayColorImage != null && displayColorImage.gameObject.activeSelf) {
                displayColorImage.texture = kinectManager.GetColorImageTex(0);
            }
        }
        /// <summary>
        /// Update palm cursor position 
        /// </summary>
        //private void UpdateCursorPosition()
        //{
        //    if (KinectIDS.Count == 0 || AppManager.Instance.debugMode) return;

        //    Vector2 newPos = GetPositionFromKinect(AppManager.Instance.kinectController.KinectIDS[0], KinectInterop.JointType.HandRight, cursor.GetComponentInChildren<Image>().canvas.pixelRect);

        //    Vector2 current = cursor.anchoredPosition;
        //    if (newPos ==  Vector2.zero)
        //    {
        //        newPos = current;
        //    }
        //    cursor.anchoredPosition = Vector2.SmoothDamp(current, newPos, ref velocity, smoothTime);
        //    //Debug.Log(cursor.anchoredPosition);
        //}

        public void ClearUsers() {
            kinectManager.ClearKinectUsers();
            KinectIDS.Clear();
            UpdateUserCount();
        }
        /// <summary>
        /// Detection callback when Kinect detects a user
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_uIndex"></param>
        private async void OnDetected(ulong _id, int _uIndex) {
            detectIdle = false;
            sendActivation = false;

            Debug.Log("User detection accquired");
            // Delay for user accquired
            await new WaitForSeconds(timeToWaitForDetection);
            Debug.Log("Adding user");
            KinectIDS.Add(_id);

            // Detect gesture for specfic user
            //gestureManager.DetectGesture(_id, GestureType.SwipeLeft);
            //gestureManager.DetectGesture(_id, GestureType.SwipeRight);

            UpdateUserCount();

            if (kinectManager.GetUsersCount() > 0 && !sendActivation) {
                sendActivation = true;
                if (Trinax.Instance.state != STATE.SCREENSAVER) {
                    return;
                }
                //AppManager.Instance.uiManager.ToUserDetection();
            }
        }
        /// <summary>
        /// Detection callback when Kinect loses a user
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_uIndex"></param>
        private void OnLost(ulong _id, int _uIndex) {
            KinectIDS.Remove(_id);
            UpdateUserCount();

            if (KinectIDS.Count == 0 && !detectIdle) {
                Debug.Log("Idle for too long...");
                detectIdle = true;
                StartCoroutine(DetectIdle(5));
            }
        }
        /// <summary>
        /// Detection callback when there are no users being tracked by Kinect
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator DetectIdle(float duration) {
            Debug.Log("start idle");
            float timer = duration;
            while (detectIdle) {
                yield return new WaitForSeconds(1);
                timer--;
                if (timer == 0) {
                    ClearUsers();
                    AppManager.Instance.uiManager.ToScreensaver();
                    sendActivation = false;
                    yield break;
                }
            }

            Debug.Log("Idle cancelled");
        }
        /// <summary>
        /// Get joint position from Kinect 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        public Vector2 GetJointPositionFromKinect(ulong uid, KinectInterop.JointType joint) {
            Vector2 position = kinectManager.GetJointPosition(uid, (int)joint);

            return position;
        }
        /// <summary>
        /// Get joint position from Kinect - color overlay
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="joint"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Vector2 GetPositionFromKinect(ulong uid, KinectInterop.JointType joint, Rect rect) {
            Vector2 position = kinectManager.GetJointPosColorOverlay(uid, (int)joint, 0, PortraitBackground.Instance.GetBackgroundRect());
            //Debug.Log(position);
            return position;
        }
        /// <summary>
        /// Get depth position from Kinect
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public float GetDepthFromKinect(ulong uid) {
            Vector3 result = kinectManager.GetUserPosition(uid);
            return result.z;
        }
        #endregion

        #region GESTURE CALLBACKS
        public bool GestureCancelled(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint) {

            return true;
        }
        public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint, Vector3 screenPos) {
            return true;
        }
        public void GestureInProgress(ulong userId, int userIndex, GestureType gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos) {

        }
        public void UserDetected(ulong userId, int userIndex) {
            if (totalTrackedUsers >= maxTrackedUsers) return;
            //Debug.Log($"Detected: ID:{userId}");
            OnDetected(userId, userIndex);
            currentIndex = 0;
        }

        public void UserLost(ulong userId, int userIndex) {
            //Debug.Log($"Lost: ID:{userId}");
            OnLost(userId, userIndex);
            currentIndex = -1;
        }
        #endregion

        #region HELPER METHODS
        public void ShowDisplayImage(bool _show) {
            if (_show) {
                displayColorImage.GetComponent<CanvasGroup>().alpha = 1;
            }
            else {
                displayColorImage.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        private void ShowDebugText(bool show = false) {
            if (show) {
                usersCount.alpha = 1;
                userDist.alpha = 1;
            }
            else {
                usersCount.alpha = 0;
                userDist.alpha = 0;
            }
        }
        #endregion
    }
}


