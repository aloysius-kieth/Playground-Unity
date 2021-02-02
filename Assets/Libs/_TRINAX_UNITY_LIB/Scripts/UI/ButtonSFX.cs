using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TRINAX.UI {
    public class ButtonSFX : MonoBehaviour {
        public TrinaxAudioManager.AUDIOS audio;

        private Button button;
        private TrinaxAudioManager audioManager;

        private void Start() {
            button = GetComponent<Button>();

            if (Trinax.Instance.audioManager != null) {
                audioManager = Trinax.Instance.audioManager;
            }

            if (button != null) {
                button.onClick.AddListener(() => {
                    if (audioManager == null) {
                        Debug.LogWarning("audioManager null reference!");
                        return;
                    }
                    audioManager.PlayUISFX(audio, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
                });
            }
        }
    }
}

