using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using DG.Tweening;

namespace TRINAX.Networking {
    public class ConnectionLostPanel : MonoBehaviour {
        public TextMeshProUGUI text;
        public Transform box;

        private void Awake() {
            TrinaxServerManager.OnMaxRetriesReached += Hide;
            TrinaxServerManager.OnConnectionLost += Show;
            gameObject.SetActive(false);
        }

        public void Show() {
            gameObject.SetActive(true);
            text.text = "Connection Lost!" + "\n" + "Please wait...";
            box.transform.DOScale(1f, 0.5f).SetEase(Ease.OutExpo, 5);
        }

        public void Hide() {
            box.transform.DOScale(0f, 0.5f).SetEase(Ease.OutExpo, 5).OnComplete(() => { gameObject.SetActive(false); });
        }

        //void OnMaxRetriesReached()
        //{
        //    text.text = "Returning to Home";
        //}
    }
}