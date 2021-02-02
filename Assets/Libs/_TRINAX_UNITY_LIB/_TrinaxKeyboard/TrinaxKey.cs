using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace TRINAX {
    public class TrinaxKey : MonoBehaviour {
        [Header("Key properties")]
        public KeyCode keycode;
        public string custom = "";

        [HideInInspector]
        public TrinaxOnScreenKB screenKB;

        public TextMeshProUGUI text;

        Button button;

        public void Init() {
            button = GetComponent<Button>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            button.onClick.AddListener(OnKeyDown);
        }

        void OnKeyDown() {
            PlaySFX();
            transform.DOScale(Vector3.one * 0.9f, 0.000001f).OnComplete(() => {
                transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            });

            screenKB.OnKeyDown(keycode, custom);
        }

        void PlaySFX() {
            int rnd = Random.Range((int)TrinaxAudioManager.AUDIOS.TYPING, (int)TrinaxAudioManager.AUDIOS.TYPING);
            Trinax.Instance.audioManager.PlayUISFX((TrinaxAudioManager.AUDIOS)rnd, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
        }
    }
}