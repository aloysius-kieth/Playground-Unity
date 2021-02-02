//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//using System.Threading.Tasks;

//public class GameTimerOld : MonoBehaviour {
//    public struct timeResult {
//        public string min;
//        public string sec;
//        public string millisec;

//    }
//    public TextMeshProUGUI minutes;
//    public TextMeshProUGUI seconds;
//    public float timer;
//    //public float timerDuration;
//    public bool countUp;
//    public delegate IEnumerator TimeUpCallback(bool winstate);
//    public static event TimeUpCallback timesUp;
//    public GameObject minHand;
//    public GameObject secHand;
//    public bool timerStarted;
//    // Use this for initialization

//    void Awake () {
//        //resetTimer();
//    }
	
//	// Update is called once per frame
//	void Update () {
//		if(timerStarted) {

//            if(countUp) {
//                timer += Time.deltaTime;
//                if(timer > Trinax.Instance.gameSettings.gameDuration) {
//                    timer = Trinax.Instance.gameSettings.gameDuration;
//                    if (timesUp != null) {
//                        //if (GameManager.Instance.isTop10)
//                        //{
//                        //    StartCoroutine(timesUp(true));
//                        //}
//                        //else
//                        //{
//                        //    StartCoroutine(timesUp(false));
//                        //}
//                        //if (ScoreManager.Instance.Score > GameManager.Instance.scoreToBeat)
//                        //{
//                        //    StartCoroutine(timesUp(true));
//                        //}
//                        //else
//                        //{
//                        //    StartCoroutine(timesUp(false));
//                        //}
//                    }

//                }
//            } else {
//                timer -= Time.deltaTime;
//                if (timer <= 0.0f) {
//                    stopTimer();
//                    //timer = Trinax.Instance.gameSettings.gameDuration;
//                    if (timesUp != null) {
//                        //timesUp?.Invoke(false);
//                        //OnTimesUp().WrapErrors();
//                        Debug.Log("time over");
//                    }
//                }
//            }
//            float tempmin = Mathf.FloorToInt(timer / 60);
//            float tempsec = Mathf.FloorToInt(timer % 60);

//            if (minutes != null)
//            {
//                minutes.text = tempmin.ToString() + ":";
//            }
   
//            seconds.text = tempsec.ToString();

//            if (minHand != null && secHand != null) {
//                if (countUp) {
//                    minHand.transform.localRotation = Quaternion.Euler(0, 0, 1 - (tempmin / 24) * 360);
//                    secHand.transform.localRotation = Quaternion.Euler(0, 0, 1 - (tempsec / 60) * 360);
//                } else {
//                    minHand.transform.localRotation = Quaternion.Euler(0, 0, (tempmin / 24) * 360);
//                    secHand.transform.localRotation = Quaternion.Euler(0, 0, (tempsec / 60) * 360);
//                }
//            }
//            //Debug.Log(tempsec / (2 * Mathf.PI));
            
//        }
//	}

//    //async Task OnTimesUp()
//    //{
//    //    await GameManager.Instance.RunAddResult();
//    //    if (GameManager.Instance.isTop10) StartCoroutine(timesUp?.Invoke(true));
//    //    else StartCoroutine(timesUp?.Invoke(false));
//    //}

//    public void startTimer() {
//        timerStarted = true;
//    }

//    public void resetTimer() {
//        if (countUp) {
//            timer = 0.0f;
//        } else {
//            timer = Trinax.Instance.gameSettings.gameDuration;
//        }
//        float tempmin = Mathf.FloorToInt(timer / 60);
//        float tempsec = Mathf.FloorToInt(timer % 60);
//        if (minutes != null)
//        {
//            minutes.text = tempmin.ToString() + ":";
//        }

//        seconds.text = tempsec.ToString();

//        if (minHand != null && secHand != null) {
            
//            if (countUp) {
//                minHand.transform.localRotation = Quaternion.Euler(0, 0, 1 - (tempmin / 24) * 360);
//                secHand.transform.localRotation = Quaternion.Euler(0, 0, 1 - (tempsec / 60) * 360);
//            } else {
//                minHand.transform.localRotation = Quaternion.Euler(0, 0, (tempmin / 24) * 360);
//                secHand.transform.localRotation = Quaternion.Euler(0, 0, (tempsec / 60) * 360);
//            }
//        }
//    }

//    public timeResult getTimeResult() {
//        timeResult tResult = new timeResult();
//        float remainingTime = Trinax.Instance.gameSettings.gameDuration - timer;
//        tResult.min = Mathf.FloorToInt(remainingTime / 60).ToString();
//        tResult.sec = Mathf.FloorToInt(remainingTime % 60).ToString();
//        tResult.millisec = (Mathf.FloorToInt((remainingTime * 100)) % 100).ToString();
//        return tResult;
//    }


//    public static int convertToTotalMilliSeconds(timeResult tResult) {
//        int temp = 0;
//        temp = int.Parse(tResult.min) * 60;
//        temp += int.Parse(tResult.sec);
//        temp *= 100;
//        temp += int.Parse(tResult.millisec);
//        Debug.Log("total milliseconds Elapsed " + temp);
//        return temp;
//    }

//    public static timeResult convertToTimeResult(int time) {
//        timeResult tResult = new timeResult();
//        float min, sec, ms;
//        ms = (time % 100);
//        int temp = time / 100;

//        sec = (temp % 60);
//        min = (temp / 60);

//        if (ms < 10)
//            tResult.millisec = "0" + ms.ToString();
//        else
//            tResult.millisec = ms.ToString();

//        if (sec < 10)
//            tResult.sec = "0" + sec.ToString();
//        else
//            tResult.sec = sec.ToString();

//        if (min < 10)
//            tResult.min = "0" + min.ToString();
//        else
//            tResult.min = min.ToString();

//        Debug.Log("Min :" + tResult.min + " Sec :" + tResult.sec + " Millisec :" + tResult.millisec);
//        return tResult;
//    }
//    public static string convertToString(timeResult tResult) {
        
//        if (int.Parse(tResult.sec) < 10)
//            tResult.sec = "0" + tResult.sec;

//        if (int.Parse(tResult.min) < 10)
//            tResult.min = "0" + tResult.min;

//        Debug.Log("Min :" + tResult.min + " Sec :" + tResult.sec);
//        return tResult.min +":"+ tResult.sec;
//    }

//    public void stopTimer() {
//        timerStarted = false;
//    }
//}
