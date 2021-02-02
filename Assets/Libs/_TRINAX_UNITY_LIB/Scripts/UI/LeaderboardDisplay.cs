using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;

namespace TRINAX {
    public class LeaderboardDisplay : MonoBehaviour {
        public List<TextMeshProUGUI> rankText = new List<TextMeshProUGUI>();
        public List<TextMeshProUGUI> nameText = new List<TextMeshProUGUI>();
        public List<TextMeshProUGUI> scoreText = new List<TextMeshProUGUI>();

        public Sprite[] rankImages;

        public void Init(List<_PlayerInfoDisplay> infoDisplayList) {
            for (int i = 0; i < infoDisplayList.Count; i++) {
                rankText.Add(infoDisplayList[i].rankText);
                nameText.Add(infoDisplayList[i].nameText);
                scoreText.Add(infoDisplayList[i].scoreText);
                infoDisplayList[i].rankImage.sprite = rankImages[i];
            }
        }

        public void PopulateData(LeaderboardReceiveJsonData rJson) {
            if (rJson.data != null && rJson.data.Count > 0) {
                for (int i = 0; i < nameText.Count; i++) {
                    if (i >= rJson.data.Count) {
                        nameText[i].text = "-";
                        scoreText[i].text = "-";
                    }
                    else {
                        nameText[i].text = rJson.data[i].name;
                        scoreText[i].text = rJson.data[i].score.ToString().Trim();
                    }
                }
            }
        }

        public void PopulateData(List<PlayerInfo> pinfoList) {
            if (pinfoList != null && pinfoList.Count > 0) {
                for (int i = 0; i < nameText.Count; i++) {
                    if (i >= pinfoList.Count) {
                        nameText[i].text = "-";
                        scoreText[i].text = "-";
                        rankText[i].text = "-";
                    }
                    else {
                        nameText[i].text = pinfoList[i].name;
                        scoreText[i].text = pinfoList[i].score.ToString().Trim();
                        rankText[i].text = pinfoList[i].rank.ToString().Trim();
                    }
                }
            }
        }

        public void PopulateDefault() {
            for (int i = 0; i < nameText.Count; i++) {
                nameText[i].text = "-";
            }
            for (int i = 0; i < scoreText.Count; i++) {
                scoreText[i].text = "-";
            }
            for (int i = 0; i < rankText.Count; i++) {
                rankText[i].text = "-";
            }
        }

        //void DoHighlight()
        //{
        //    StartCoroutine("DoBlink");
        //}

        //public void StopHighlight()
        //{
        //    StopCoroutine("DoBlink");
        //}

        private void Update() {
            //if(doBlink)
            //{
            //    //Debug.Log("Did i come in here?");
            //    DoHighlight();
            //}
        }

        //Color lerpedColor;
        //public bool doBlink;
        //public IEnumerator DoBlink()
        //{
        //    while (true)
        //    {
        //        //Debug.Log("blink blink");
        //        lerpedColor = Color.Lerp(Color.white, new Color(0f, 0.86f, 1f), Mathf.PingPong(Time.time * 1f, 1.0f));
        //        gameObject.GetComponentInChildren<Image>().color = lerpedColor;
        //        yield return new WaitForSeconds(1f); 
        //    }
        //}

        //public void ResetColor()
        //{
        //    gameObject.GetComponentInChildren<Image>().color = Color.white;
        //}
    }
}
