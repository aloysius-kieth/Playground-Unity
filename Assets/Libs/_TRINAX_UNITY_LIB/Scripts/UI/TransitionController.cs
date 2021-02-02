using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.Components;
using BeautifulTransitions.Scripts.Transitions.Components.Screen;

namespace TRINAX.UI.Transition {

    public interface TransitionListenerInterface {
        void TransitInComplete();

        void TransitOutComplete();
    }

    public class TransitionController : MonoBehaviour {

        #region Singleton
        public static TransitionController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        public Texture2D overlayTexture;
        public Texture2D wipeTexture;
        public WipeScreen wipeScreen;

        public Color _color = Color.white;
        public float _softness = 0.5f;
        public bool _showTexture = false;

        public float durationOut = 2f;
        public float durationIn = 2f;

        public List<MonoBehaviour> transitionListeners = new List<MonoBehaviour>();

        public async void Init(bool FadeIn) {
            Debug.Log("Transit from scene");

            RefreshListeners();

            //await new WaitUntil(() => Trinax.Instance.isReady);
            durationOut = 0.0001f;
            TransitionOut(/*wipeScreen*/durationOut);

            float transitionTime = TransitionHelper.GetTransitionOutTime(new List<TransitionBase> { wipeScreen });

            if (!FadeIn) {
                return;
            }

            await new WaitForSeconds(transitionTime + 1.5f);

            TransitionIn(durationIn);
        }

        private void SetColor(Color color) {
            _color = color;
            wipeScreen.InConfig.Color = _color;
            wipeScreen.OutConfig.Color = _color;
        }

        private void SetSoftness(float softness) {
            _softness = softness;
            //WipeCamera.InConfig.Softness = _softness;
            //WipeCamera.OutConfig.Softness = _softness;
            wipeScreen.InConfig.Softness = _softness;
            wipeScreen.OutConfig.Softness = _softness;
        }

        private void SetShowTexture(bool showTexture) {
            _showTexture = showTexture;
            var texture = _showTexture ? overlayTexture : null;
            wipeScreen.InConfig.Texture = texture;
            wipeScreen.OutConfig.Texture = texture;
        }

        private void SetWipeTexture() {

            var texture = wipeTexture;
            wipeScreen.InConfig.MaskTexture = texture;
            wipeScreen.OutConfig.MaskTexture = texture;
        }

        public IEnumerator StartTransitionIn(TransitionBase transitionBase, float duration) {
            wipeScreen.TransitionInConfig.Duration = duration;

            SetColor(_color);
            SetShowTexture(_showTexture);
            SetWipeTexture();
            SetSoftness(_softness);

            float transitionTime = TransitionHelper.GetTransitionOutTime(new List<TransitionBase> { transitionBase });

            yield return new WaitForSeconds(transitionTime + 1.5f);
            transitionBase.InitTransitionIn();
            transitionBase.TransitionIn();
        }

        public void TransitionIn(float duration) {
            //Debug.Log("transit in");
            StartCoroutine(StartTransitionIn(wipeScreen, duration));
        }

        public void TransitionOut(/*TransitionBase transitionBase*/ float duration) {
            //Debug.Log("transit out");
            wipeScreen.TransitionOutConfig.Duration = duration;

            SetColor(_color);
            SetShowTexture(_showTexture);
            SetWipeTexture();
            SetSoftness(_softness);

            wipeScreen.InitTransitionOut();
            //transitionBase.CurrentTransitionStep.OnComplete = oncomplete;
            wipeScreen.TransitionOut();
        }

        public void Transit(System.Action callback = null) {
            // make sure these are set in the material.
            SetColor(_color);
            SetShowTexture(_showTexture);

            StartCoroutine(TransitInternal(wipeScreen, callback));
        }

        private IEnumerator TransitInternal(TransitionBase transitionBase, System.Action callback = null) {
            // make sure these are set in the materials.
            SetColor(_color);
            SetShowTexture(_showTexture);
            SetWipeTexture();
            SetSoftness(_softness);

            transitionBase.TransitionInConfig.Duration = 1f;
            transitionBase.TransitionOutConfig.Duration = 1f;

            float transitionTime = TransitionHelper.GetTransitionOutTime(new List<TransitionBase> { transitionBase });
            transitionBase.InitTransitionOut();

            transitionBase.TransitionOut();
            //transitionBase.CurrentTransitionStep.OnComplete = oncomplete;
            callback?.Invoke();
            yield return new WaitForSeconds(transitionTime + 1.5f);

            transitionBase.TransitionIn();
        }

        private void RefreshListeners() {
            transitionListeners.Clear();

            MonoBehaviour[] monoScripts = FindObjectsOfType<MonoBehaviour>() as MonoBehaviour[];
            foreach (MonoBehaviour script in monoScripts) {
                if ((script is TransitionListenerInterface) && script.enabled) {
                    transitionListeners.Add(script);
                }
            }
        }

        public void TransitInCompleteListeners() {
            foreach (TransitionListenerInterface listener in transitionListeners) {
                if (listener != null) {
                    listener.TransitInComplete();
                }
            }
        }

        public void TransitOutCompleteListeners() {
            foreach (TransitionListenerInterface listener in transitionListeners) {
                if (listener != null) {
                    listener.TransitOutComplete();
                }
            }
        }
    }

}
