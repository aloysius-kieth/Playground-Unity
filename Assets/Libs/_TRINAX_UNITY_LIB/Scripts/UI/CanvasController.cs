using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {

    public CanvasGroup[] pageList;
    private void Awake () {
        if (pageList.Length == 0) return;
        for (int i = 0; i < pageList.Length; ++i) {
            pageList[i].alpha = 0;
            pageList[i].gameObject.SetActive(false);
        }
        //pageList[0].alpha = 1;
        //pageList[0].gameObject.SetActive(true);
    }
    /// <summary>
    /// To be called on first run of app only
    /// </summary>
    /// <param name="state"></param>
    /// <param name="callback"></param>
    public void OnStartup(STATE state, System.Action callback = null)
    {
        int index = (int)state;
        for (int i = 0; i < pageList.Length; i++)
        {
            if (index == i)
            {
                pageList[index].gameObject.SetActive(true);
                pageList[index].DOFade(1.0f, 0.25f).OnComplete(() => { callback?.Invoke(); });
            }
        }
    }

    public int getCanvasIndex() {
        int index = 0;
        for (; index < pageList.Length; ++index) {
            if (pageList[index].alpha > 0.5)
            {
                break;
            }
        }
        return index;
    }

    public void TransitToCanvas(int tobeActive, float duration, System.Action callback = null) {
        int oldCanvas = getCanvasIndex();
        if (oldCanvas == tobeActive)
        {
            Debug.LogWarning("Fading to same Canvas");
            callback?.Invoke();
            return;
        }
        //Button[] canvasButtons = pageList[tobeActive].GetComponentsInChildren<Button>();
        //if (canvasButtons != null)
        //{
        //    foreach (Button obj in canvasButtons)
        //    {
        //        //Debug.Log("button ");
        //        obj.interactable = false;
        //    }
        //}
        //else { Debug.Log("No buttons found in canvas!"); }

        //Button[] oldCanvasButtons = pageList[oldCanvas].GetComponentsInChildren<Button>();
        //if (oldCanvasButtons != null && oldCanvasButtons.Length > 0)
        //{
        //    foreach (Button obj in oldCanvasButtons)
        //    {
        //        //Debug.Log("button ");
        //        obj.interactable = false;
        //    }
        //}
        //else { Debug.Log("No buttons found in oldCanvas!"); }

        pageList[oldCanvas].DOFade(0, duration).OnComplete(() => {
            pageList[oldCanvas].gameObject.SetActive(false);
            /*
            if (oldCanvas == gameStageIndex) {
                isGameMode = false;
                pageList[1].alpha = 0;
                pageList[1].gameObject.SetActive(false);
                //VuforiaBehaviour.Instance.enabled = true;
            }
            */
            pageList[tobeActive].gameObject.SetActive(true);

            //await new WaitForSeconds(.5f);

            pageList[tobeActive].DOFade(1, duration).OnComplete(() => {

                //foreach (Button obj in canvasButtons)
                //{
                //    obj.interactable = true;
                //}
                callback?.Invoke();
            });
        });

    }
}
