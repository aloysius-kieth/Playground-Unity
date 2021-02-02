using UnityEngine;

namespace TRINAX {

    // Inputs relating to debugging, put them here!
    public class TrinaxHandleInputs : MonoBehaviour {
        private bool isReady = false;

        private async void Start() {
            isReady = false;
            await new WaitUntil(() => Trinax.Instance.isReady);

            isReady = true;
        }

        private void Update() {
            if (!isReady) return;

            //if (aP != null && aP.gameObject.activeSelf)
            //{
            //if (Input.GetKeyDown(KeyCode.F2) && Trinax.Instance.state == PAGES.GAME)
            //{
            //    MainManager.Instance.StopGame();
            //    UIManager.Instance.ToScreensaver();
            //}
            //}

            if (Input.GetKeyDown(KeyCode.F10)) {
                Cursor.visible = !Cursor.visible;
            }

            if (Input.GetKeyDown(KeyCode.F1)) {
                Trinax.Instance.IsAppPaused = !Trinax.Instance.IsAppPaused;
            }
            if (Input.GetKeyDown(KeyCode.F12) && Trinax.Instance.state == STATE.SCREENSAVER) {
                //MainManager.Instance.ClearLocalsaveFile();
                //UIManager.Instance.ToScreensaver();
            }

            if (Trinax.Instance.IsAppPaused) {
                Time.timeScale = 0;
            }
            else Time.timeScale = 1;
        }

    }
}