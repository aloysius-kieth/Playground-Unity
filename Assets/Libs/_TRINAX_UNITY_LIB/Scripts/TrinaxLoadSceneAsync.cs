using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

namespace TRINAX {
    public class TrinaxLoadSceneAsync : MonoBehaviour {

        private AsyncOperation async;
        public GameObject loadingScreen;
        public Image imageBG;
        public Image imageFillBG;
        public Image imageFillOverlay;
        public TextMeshProUGUI[] loadingNum;

        public bool useNum = true;

        private void Start() {
            imageFillBG.DOFade(0, 0);
            imageFillOverlay.DOFade(0, 0);

            foreach (TextMeshProUGUI child in loadingNum) {
                child.DOFade(0, 0);
            }

            //loadingScreen.SetActive(false);
            gameObject.SetActive(false);
        }

        public void LoadLevel(int sceneIndex, TweenCallback callback) {
            Debug.Log("Loading scene...");

            loadingNum[0].text = "0";
            imageFillOverlay.fillAmount = 0.0f;
            //gameObject.SetActive(true);
            if (useNum) {
                foreach (TextMeshProUGUI child in loadingNum) {
                    child.DOFade(1, 1);
                }
            }
            imageFillOverlay.DOFade(1, 1);
            imageBG.DOFade(0.75f, 1);
            imageFillBG.DOFade(1, 1).OnComplete(() => {
                StartCoroutine(LoadLevelAsync(sceneIndex, callback));
            });
            //StartCoroutine(LoadLevelAsync(sceneIndex, callback));
        }

        IEnumerator LoadLevelAsync(int sceneIndex, TweenCallback callback) {
            //Debug.Log("Start load scene");
            async = SceneManager.LoadSceneAsync(sceneIndex);
            float progress = 0;

            while (!async.isDone) {
                //Debug.Log(progress);
                progress = Mathf.Clamp01(async.progress / 0.9f);
                imageFillOverlay.fillAmount = progress;
                if (useNum) {
                    loadingNum[0].text = Mathf.RoundToInt(progress * 100).ToString();
                }
                yield return null;
            }
            if (async.isDone) {
                imageFillOverlay.fillAmount = progress;
                if (useNum) {
                    loadingNum[0].text = Mathf.RoundToInt(progress * 100).ToString();
                }
                imageFillBG.DOFade(0, 0.5f);
                imageBG.DOFade(0, 0.5f);
                imageFillOverlay.DOFade(0, 0.5f).OnComplete(callback);
                if (useNum) {
                    foreach (TextMeshProUGUI child in loadingNum) {
                        child.DOFade(0, 1);

                    }

                    loadingScreen.SetActive(false);
                    async = null;
                }
                //loadingNum[0].text = "0";
            }
        }
    }
}