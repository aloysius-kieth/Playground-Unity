using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.Components;
using BeautifulTransitions.Scripts.Transitions.Components.Screen;
using BeautifulTransitions.Scripts.Transitions.Components.Camera;

namespace TRINAX.UI {

    public enum TRANSITION_EFFECT {
        FADE,
        BARS_LEFT_TO_RIGHT,
        BARS_TOP_TO_BOTTOM,
        BOX,
        CIRCLE,
        CLOUDS_1,
        CLOUDS_2,
    }

    public class PageController : MonoBehaviour {
        public List<CanvasGroup> pageList;
        public STATE currentPage = STATE.SCREENSAVER;

        private void Awake() {
            for (int i = 0; i < pageList.Count; i++) {
                if (pageList[i] != null) {
                    pageList[i].alpha = 0;
                    pageList[i].gameObject.SetActive(false);
                }
            }
        }

        private void Start() {
            //Init();
        }

        public void Init(System.Action callback) {
            for (int i = 0; i < pageList.Count; i++) {
                if (pageList[i] != null) {
                    if ((int)currentPage == i) {
                        pageList[i].gameObject.SetActive(true);
                        pageList[i].DOFade(1.0f, 0.25f).OnComplete(() => { callback?.Invoke(); });
                    }
                }
            }
        }

        public void ChangePage(STATE state) {
            int previous = GetCanvasIndex();
            int current = (int)state;
            if (previous == current) {
                Debug.LogWarning("Fading to same Canvas");
                return;
            }

            Trinax.Instance.state = state;

            pageList[previous].DOFade(0f, 0.001f).OnComplete(() => {
                pageList[previous].gameObject.SetActive(false);
                pageList[current].gameObject.SetActive(true);
                pageList[current].DOFade(1f, 0.001f);
            });
        }

        public int GetCanvasIndex() {
            int index = 0;
            for (; index < pageList.Count; ++index) {
                if (pageList[index].alpha > 0.5) {
                    break;
                }
            }
            return index;
        }

        //public FadeCamera FadeCamera;
        //public WipeCamera WipeCamera;
        public FadeScreen FadeScreen;
        public WipeScreen WipeScreen;
        public Texture2D OverlayTexture;
        public Texture2D[] WipeTextures;

        public Color _color = Color.white;
        public int _effect;
        public bool _showTexture = true;
        public float _softness;

        public void SetColorWhite() {
            SetColor(Color.white);
        }

        public void SetColorRed() {
            SetColor(Color.red);
        }

        public void SetColorBlue() {
            SetColor(Color.blue);
        }

        public void SetColorGreen() {
            SetColor(Color.green);
        }

        public void SetColorBlack() {
            SetColor(Color.black);
        }

        void SetColor(Color color) {
            _color = color;
            //FadeCamera.InConfig.Color = _color;
            //FadeCamera.OutConfig.Color = _color;
            //WipeCamera.InConfig.Color = _color;
            //WipeCamera.OutConfig.Color = _color;
            FadeScreen.InConfig.Color = _color;
            FadeScreen.OutConfig.Color = _color;
            WipeScreen.InConfig.Color = _color;
            WipeScreen.OutConfig.Color = _color;
        }

        public void SetEffect(int effect) {
            _effect = effect;
        }

        public void SetSoftness(float softness) {
            _softness = softness;
            //WipeCamera.InConfig.Softness = _softness;
            //WipeCamera.OutConfig.Softness = _softness;
            WipeScreen.InConfig.Softness = _softness;
            WipeScreen.OutConfig.Softness = _softness;
        }

        public void SetShowTexture(bool showTexture) {
            _showTexture = showTexture;
            var texture = _showTexture ? OverlayTexture : null;
            //FadeCamera.InConfig.Texture = texture;
            //FadeCamera.OutConfig.Texture = texture;
            //WipeCamera.InConfig.Texture = texture;
            //WipeCamera.OutConfig.Texture = texture;
            FadeScreen.InConfig.Texture = texture;
            FadeScreen.OutConfig.Texture = texture;
            WipeScreen.InConfig.Texture = texture;
            WipeScreen.OutConfig.Texture = texture;
        }

        public void SetWipeTexture() {
            if (_effect < 1 || _effect - 1 > WipeTextures.Length) return;

            var texture = WipeTextures[_effect - 1];
            //WipeCamera.InConfig.MaskTexture = texture;
            //WipeCamera.OutConfig.MaskTexture = texture;
            WipeScreen.InConfig.MaskTexture = texture;
            WipeScreen.OutConfig.MaskTexture = texture;
        }

        public void TransitScreen(int nextPage) {
            currentPage = (STATE)nextPage;
            // make sure these are set in the material.
            SetColor(_color);
            SetShowTexture(_showTexture);

            if (_effect == 0) {
                StartCoroutine(TransitInternal(FadeScreen));
            }
            else {
                StartCoroutine(TransitInternal(WipeScreen));
            }
        }

        public void TransitScreen(STATE nextPage) {
            currentPage = nextPage;
            // make sure these are set in the material.
            SetColor(_color);
            SetShowTexture(_showTexture);

            if (_effect == 0) {
                StartCoroutine(TransitInternal(FadeScreen));
            }
            else {
                StartCoroutine(TransitInternal(WipeScreen));
            }
        }

        public IEnumerator TransitInternal(TransitionBase transitionBase) {
            // make sure these are set in the materials.
            SetColor(_color);
            SetShowTexture(_showTexture);
            SetWipeTexture();
            SetSoftness(_softness);

            float transitionTime = TransitionHelper.GetTransitionOutTime(new List<TransitionBase> { transitionBase });
            transitionBase.InitTransitionOut();
            transitionBase.TransitionOut();
            yield return new WaitForSeconds(transitionTime + 0.5f);
            transitionBase.TransitionIn();
        }

        public void TransitInComplete() {
            //Debug.Log("Page transit in complete");
        }

        public void TransitOutComplete() {
            //Debug.Log("Page transit out complete");
            //currentPage++;
            ChangePage(currentPage);
        }

    }
}