//#define ENABLE_LOGS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Logger = TRINAX.Utils.Logger;

namespace TRINAX.Networking {
    public class NetworkChecker : MonoBehaviour {
        public System.Action OnNetworkFound;
        public System.Action OnNetworkLost;

        public bool networkConnectionActive = false;
        bool checking = false;

        float nextPingDuration = 2f;
        float pingTimeout = 5f;

        public void StartCheckConnectionToMasterServer() {
            checking = true;
            StartCoroutine("DoCheckConnectionToMasterServer");
        }

        public void StopCheckConnectionToMasterServer() {
            checking = false;
            StopCoroutine("DoCheckConnectionToMasterServer");
        }

        float lastime = 0;
        float elaspedTime = 0;
        IEnumerator DoCheckConnectionToMasterServer() {
            while (checking) {
                Ping pingMasterServer = new Ping(ThreadedClient.Instance.host);
                Logger.Debug("Pinging: " + pingMasterServer.ip);
                float startTime = Time.time;
                elaspedTime = startTime - lastime;
                while (!pingMasterServer.isDone && Time.time < startTime + pingTimeout) {
                    yield return new WaitForSeconds(0.1f);
                }
                lastime = startTime;
                if (pingMasterServer.isDone) {
                    Logger.Debug("Network: " + "<color=green>UP</color>");
                    networkConnectionActive = true;
                    OnNetworkFound?.Invoke();
                    if (Trinax.Instance.canvas != null) {
                        Trinax.Instance.canvas.UpdateNetworkStatus("Network: " + "<color=green> UP");
                    }
                }
                else {
                    if (Time.time > startTime + pingTimeout) {
                        Logger.Debug("Pinged too long: " + elaspedTime);
                    }
                    Logger.Debug("Network: " + "<color=red>DOWN</color>");
                    networkConnectionActive = false;
                    OnNetworkLost?.Invoke();
                    if (Trinax.Instance.canvas != null) {
                        Trinax.Instance.canvas.UpdateNetworkStatus("Network: " + "<color=red> DOWN");
                    }
                }
                Logger.Debug("Ping done: " + elaspedTime);
                yield return new WaitForSeconds(nextPingDuration);
            }

        }
    }
}