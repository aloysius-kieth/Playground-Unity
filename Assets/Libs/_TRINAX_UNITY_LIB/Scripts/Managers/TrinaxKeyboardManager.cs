using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TRINAX {
    [System.Serializable]
    public class Keyboard {
        public List<TrinaxKey> keys;
        public TrinaxOnScreenKB keyboard;

        public void Init() {
            if (keys.Count == 0) return;
            for (int i = 0; i < keys.Count; i++) {
                keys[i].screenKB = keyboard;
                keys[i].Init();
            }
            keyboard.Init();
        }
    }

    public class TrinaxKeyboardManager : MonoBehaviour, IManager {
        int executionPriority = 400;
        public int ExecutionPriority { get { return executionPriority; } set { value = executionPriority; } }

        public bool IsReady { get; set; }
        public List<Keyboard> KeyboardContainer;

        public enum KEYBOARDS {
            ENTER_DETAILS,
        }

        private void Start() {

        }

        public void Init() {
            Debug.Log("<color=yellow>Loading KeyboardManager</color>");
            IsReady = false;

            if (KeyboardContainer.Count == 0) return;
            for (int i = 0; i < KeyboardContainer.Count; i++) {
                Keyboard keyboard = KeyboardContainer[i];
                keyboard.Init();
            }

            IsReady = true;
            Debug.Log("<color=lime>KeyboardManager is loaded!</color>");

        }

        //public TrinaxOnScreenKB GetKeyboard(KEYBOARDS keyboard)
        //{
        //    if (keyboards.Length == 0) return null;
        //    return keyboards[(int)keyboard];
        //}
    }
}