using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace TRINAX {
    public class CountdownTimer : MonoBehaviour {
        public TextMeshProUGUI countText;
        public Sprite[] images;

        public Image display;

        public bool useImages = true;

        private const string PRESTART = "GET READY!";
        private const string END = "GO!";

        public int duration = 3;
        private int count = 0;

        CanvasGroup canvasGrp;
        Animator animator;

        public void Init() {
            canvasGrp = GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            canvasGrp.alpha = 0;
            animator.enabled = false;
            gameObject.SetActive(false);
        }

        public async void StartCountdown(bool useAnimator = false) {
            countText.fontSize = 100;
            Trinax.Instance.audioManager.AdjustMusicVolume(0.5f);
            count = duration + 1;

            await new WaitForSeconds(7);

            if (useAnimator) {
                animator.enabled = true;
                DoCountdownByAnimator();
            }
            else
                DoCountdown();

        }

        // count down by await (text)
        private async void DoCountdown() {
            //SetCountText(PRESTART);

            await new WaitForSeconds(1);
            while (count > 0) {
                countText.text = count.ToString();
                count--;
                await new WaitForSeconds(1);
            }

            SetCountText(END);
            Debug.Log("countdown finished!");

            await new WaitForSeconds(1);

            OnInActive();
        }

        private void DoCountdownByAnimator() {
            if (count == 4) {
                countText.fontSize = 100;
            }
            else {
                countText.fontSize = 250;
            }
            if (useImages) {
                if (count == 3)
                    display.sprite = images[0];
                else if (count == 2)
                    display.sprite = images[1];
                else if (count == 1)
                    display.sprite = images[2];
            }
            //Debug.Log(count);
            if (count > 0) {
                SetCountText(count.ToString());
            }
            else {
                SetCountText("");
            }
            count--;


            if (animator == null) { Debug.LogWarning("No Animator in countdown!"); return; }

            if (count < duration) {
                Trinax.Instance.audioManager.PlaySFX(TrinaxAudioManager.AUDIOS.TICK, TrinaxAudioManager.AUDIOPLAYER.SFX);
            }

            animator.SetTrigger(count == -1 ? "Exit" : "Tick");

        }

        private void SetCountText(string text) {
            if (countText == null) return;
            countText.text = text;
        }

        public void OnActive() {
            SetCountText(PRESTART);
            gameObject.SetActive(true);

            canvasGrp.DOFade(1.0f, 0.25f).OnComplete(() => { StartCountdown(false); });
        }

        public void OnActiveWithAnimator() {
            SetCountText(PRESTART);
            gameObject.SetActive(true);

            StartCountdown(true);
        }

        public void OnInActive() {
            canvasGrp.DOFade(0f, 0.25f).OnComplete(() => {
                animator.enabled = false;
                DeactivateGameobject();
            });
        }

        private void DeactivateGameobject() {
            Trinax.Instance.audioManager.RestoreIdleBGM();
            animator.enabled = false;
            gameObject.SetActive(false);
            EventsCallback.OnCountdownEndEvent?.Invoke();
        }

        private void TakePhoto() {
            Trinax.Instance.audioManager.PlaySFX(TrinaxAudioManager.AUDIOS.SHUTTER, TrinaxAudioManager.AUDIOPLAYER.SFX);
            // TODO: call take photo method here
        }

    }
}