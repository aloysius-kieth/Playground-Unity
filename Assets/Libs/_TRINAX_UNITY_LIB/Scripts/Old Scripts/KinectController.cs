////#define ENABLE_CALIBRATION

//using System;
//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//using TMPro;
//using DG.Tweening;

//public class KinectController : MonoBehaviour/*, KinectGestures.GestureListenerInterface*/
//{
//    #region SINGLETON
//    public static KinectController Instance { get; set; }
//    private void Awake()
//    {
//        if (Instance != null && Instance != this) Destroy(gameObject);
//        else Instance = this;
//    }
//    #endregion

//    public delegate void StartGameAction();
//    public static event StartGameAction GameStarting;

//    public static Action OnDetectedUserAcquired;
//    public static Action OnDetectedAllUsersLost;

//    public CanvasGroup CalibrationCanvas;
//    public Image calibrationImageParent;
//    public Image calibrationImage;
//    public TextMeshProUGUI calibrationText;
//    public TextMeshProUGUI calibrationCompletionRate;

//    Vector3 userPosition;

//    bool startCalibrating;
//    bool calibrated;
//    float recalibrateTimer;
//    float recalibrateDuration = 0.5f;

//    public List<KeyValuePair<int, long>> uIDs = new List<KeyValuePair<int, long>>();
//    int totalUsers = 0;
//    int maxTrackedUsers;
//    bool detectingUserAcquired = false;
//    bool detectingUserLost = false;
//    bool detectingRaiseHand = false;

//    public TextMeshProUGUI userCountText;

//    KinectManager kinectMan;

//    void Start()
//    {
//        kinectMan = GetComponent<KinectManager>();
//        maxTrackedUsers = kinectMan.maxTrackedUsers;

//        userPosition = Vector3.zero;
//        calibrationImage.fillAmount = 0;
//        recalibrateTimer = 0.0f;
//        calibrated = false;
//        startCalibrating = false;

//        CalibrationCanvas.blocksRaycasts = false;
//        //CalibrationCanvas.gameObject.SetActive(false);
//        //CalibrationCanvas.DOFade(0, 0.00001f);
//    }

//    KinectInterop.JointType jType;
//    public KinectInterop.JointType SetTrackedJoint()
//    {
//        //if (Trinax.Instance.kinectSettings.isTrackingBody)
//        //{
//        //    jType = KinectInterop.JointType.SpineBase;
//        //}
//        //else if (Trinax.Instance.kinectSettings.isTrackingHead)
//        //{
//        //    jType = KinectInterop.JointType.Head;
//        //}
//        //else if (!Trinax.Instance.kinectSettings.isTrackingBody && !Trinax.Instance.kinectSettings.isTrackingHead)
//        //{
//           // Debug.Log("No save config for tracking joint!");
//           // Debug.Log("Setting default joint of Spinebase");
//            jType = KinectInterop.JointType.SpineBase;
//        //}

//        return jType;
//    }

//    void Update()
//    {
//        if(userCountText != null)
//            userCountText.text = "User: " + kinectMan.GetUsersCount();

//        if (isKinectTracking() /*&& uIDs.Count > 0*/)
//        {
//            //Debug.Log("KinectIsTracking");

//            Int64 userId = kinectMan.GetUserIdByIndex(0)/*uIDs[0].Key*/;
//            userPosition = kinectMan.GetJointPosition(userId, (int)SetTrackedJoint());
//        }

//        if (startCalibrating)
//        {
//            recalibrateTimer += Time.deltaTime;
//            if (recalibrateTimer > recalibrateDuration)
//            {
//                //ApplyTweenToLine(0, BubblyText);
//                StartCoroutine(WaitForKinect());
//                recalibrateTimer = 0.0f;
//                startCalibrating = false;
//                Debug.Log("starting calibration");
//            }
//        }

//        //if (Input.GetKeyDown(KeyCode.Alpha1))
//        //{
//        //    startCalibration();
//        //}
//        //if (Input.GetKeyDown(KeyCode.Alpha2))
//        //{
//        //    stopKinect();
//        //}
//    }

//    public bool isKinectTracking()
//    {
//        return calibrated && kinectMan.IsUserDetected();
//    }

//    public void startCalibration()
//    {
//        Debug.Log("KinectStarting");
//        CalibrationCanvas.gameObject.SetActive(true);
//        CalibrationCanvas.blocksRaycasts = false;
//        CalibrationCanvas.DOFade(1, 1f);
//        calibrationText.DOFade(1f, 0f);
//        calibrationImage.DOFade(1f, 0f);
//        recalibrateTimer = 0.0f;
//        calibrated = false;
//        startCalibrating = true;
//        calibrationText.text = "WAITING FOR USERS...";
//    }

//    public void stopKinect()
//    {
//        CalibrationCanvas.blocksRaycasts = false;
//        CalibrationCanvas.DOFade(0, 1f);
//        calibrationText.DOFade(0f, 0f);
//        calibrationImage.DOFade(0f, 0f);
//        calibrated = false;
//        startCalibrating = false;
//        calibrationText.text = "";
//        CalibrationCanvas.gameObject.SetActive(false);
//        //calibrated = false;
//        //calibrationText.DOFade(1f, 0f);
//        //calibrationImage.DOFade(1f, 0f);
//        //calibrationImage.fillAmount = 0;
//        //recalibrateTimer = 0.0f;
//        //calibrationText.text = "";
//        //Debug.Log("KinectStopping");
//    }

//    public Vector3 updatedPositionByKinect()
//    {
//        //Debug.Log(userPosition);
//        return userPosition;
//    }

//    IEnumerator WaitForKinect()
//    {
//        Debug.Log("Waiting for Kinect");
//        calibrationImage.fillAmount = 0;
//        calibrationCompletionRate.text = (Mathf.CeilToInt(calibrationImage.fillAmount * 100)).ToString() + "%";
//        kinectMan.ClearKinectUsers();
//        bool calibrateKinect = true;
//        //kinectWaitingScreen.SetActive(true);
//        //kinect.StartAcquiringTargetBody();

//#if ENABLE_CALIBRATION
//        yield return new WaitUntil(() => kinectMan.IsUserDetected());
//#endif
//        while (calibrationImage.fillAmount < 1f)
//        {
//#if ENABLE_CALIBRATION
//            calibrateKinect = kinectMan.IsUserDetected();
//#endif
//            if (!calibrateKinect)
//            {
//                Debug.Log("Break waiting for Kinect coroutine");
//                //set fill to 0;
//                //restart calibration in 0.5s;
//                startCalibration();
//                calibrationImage.DOFillAmount(0.0f, 0.2f);
//                calibrationText.text = "CALIBRATION WAS INTERRUPTED!";
//                yield break;
//            }
//            if (calibrationImage != null)
//            {
//                calibrationImage.fillAmount = calibrationImage.fillAmount + 0.01f;
//                calibrationImage.color = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f), calibrationImage.fillAmount);
//            }
//            calibrationCompletionRate.text = (Mathf.CeilToInt(calibrationImage.fillAmount * 100)).ToString() + "%";
//            if (calibrationText != null)
//                calibrationText.text = calibrationImage.fillAmount < 0.3f ? "IN PROGRESS..." :
//                        calibrationImage.fillAmount < 0.5f ? "HALFWAY THERE!" :
//                        calibrationImage.fillAmount < 0.95f ? "JUST A LITTLE MORE!" : "COMPLETED!";

//            yield return new WaitForSeconds(0.02f);
//        }

//        if (calibrationImageParent != null)
//            calibrationImageParent.transform.DOPunchScale(new Vector3(0.1f, 0.1f), 0.5f, 5, 0.5f);
//        //if (calibrationText != null)
//        //    calibrationText.transform.DOLocalJump(calibrationText.transform.localPosition, 100f, 1, 0.4f);

//        TrinaxAudioManager.Instance.PlayUISFX(TrinaxAudioManager.AUDIOS.CALIBRATION_DONE, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);

//        yield return new WaitForSeconds(1.5f);

//        CalibrationCanvas.DOFade(0, 0.2f).OnComplete(() =>
//        {
//            CalibrationCanvas.blocksRaycasts = false;
//            CalibrationCanvas.gameObject.SetActive(false);
//        });

//        yield return new WaitForSeconds(0.65f);
//        Debug.Log("Calibration Complete");
//        calibrated = true;
//        GameStarting?.Invoke();
//    }

//    public void ResetValues()
//    {
//        calibrationImage.fillAmount = 0;
//    }

//    //IEnumerator DetectingUserAcquired(long userId, int userIndex)
//    //{
//    //    Debug.Log("User: " + userId + " acquired!");
//    //    float duration = 3.0f;

//    //    detectingUserAcquired = true;
//    //    StopCoroutine("DetectingUserLost");
//    //    detectingUserLost = false;

//    //    yield return new WaitForSeconds(duration);

//    //    if (uIDs.Contains(new KeyValuePair<int, long>(userIndex, userId)))
//    //    {
//    //        Debug.Log("Welcome user " + kinectMan.GetUserIndexById(userId));

//    //        // Subscribe to this callback in your script!
//    //        OnDetectedUserAcquired?.Invoke();
//    //    }
//    //    detectingUserAcquired = false;
//    //}

//    //IEnumerator DetectingAllUsersLost()
//    //{
//    //    float duration = 5f;
//    //    Debug.Log("All users gone!");
//    //    detectingUserLost = true;

//    //    yield return new WaitForSeconds(duration);
//    //    if (!detectingUserAcquired && uIDs.Count == 0)
//    //    {
//    //        // Subscribe to this callback in your script!
//    //        OnDetectedAllUsersLost?.Invoke();
//    //    }

//    //    detectingUserLost = false;
//    //}

//    //#region GESTURE LISTENERS
//    //public void UserDetected(long userId, int userIndex)
//    //{
//    //    if (totalUsers == maxTrackedUsers)
//    //    {
//    //        Debug.Log("Reached total tracked users of " + maxTrackedUsers + ", not adding anymore");
//    //        return;
//    //    }

//    //    Debug.Log("Detecting a user...");
//    //    uIDs.Add(new KeyValuePair<int, long>(userIndex, userId));

//    //    foreach (KeyValuePair<int, long> id in uIDs)
//    //    {
//    //        Debug.Log("List: " + " Key: " + id.Key + " Value: " + id.Value);
//    //    }

//    //    totalUsers = kinectMan.GetUsersCount();
//    //    userCountText.text = "USERS: " + totalUsers;

//    //    // Put a condition here if you want to wish to perform below action at certain state of the application
//    //    //if (Trinax.Instance.state == PAGES.PAGE1)
//    //    //{
//    //    StartCoroutine(DetectingUserAcquired(userId, userIndex));
//    //    //}
//    //}

//    //public void UserLost(long userId, int userIndex)
//    //{
//    //    foreach (KeyValuePair<int, long> id in uIDs)
//    //    {
//    //        Debug.Log("Key: " + id.Key + " Value: " + id.Value);
//    //    }
//    //    uIDs.Remove(new KeyValuePair<int, long>(userIndex, userId));

//    //    totalUsers = kinectMan.GetUsersCount();
//    //    userCountText.text = "USERS: " + totalUsers;

//    //    if (uIDs.Count == 0 && !detectingUserLost)
//    //    {
//    //        StartCoroutine("DetectingAllUsersLost");
//    //    }
//    //    Debug.Log("Lost user: " + userId);
//    //}

//    //public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
//    //{
//    //    return;
//    //}

//    //public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
//    //{
//    //    return true;
//    //}

//    //public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
//    //{
//    //    return true;
//    //}
//    //#endregion
//}
