using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRINAX {
    public enum CARS {
        COROLLA_ALTIS,
        CAMRY,
        PRIUS,
        YARIS_CROSS,
        ALPHARD,
        VELLFIRE,
        SIENTA,
        FORTUNER,
        HARRIER,
        RAV4,
    }

    public class MainManager : MonoBehaviour {
        public bool IsReady { get; set; }

        void Start() {
            IsReady = false;
        }

        // Initialize method 
        public async void Init() {
            Debug.Log("<color=lime> MainManager is ready! </color>");

            IsReady = true;
        }

        public void PopulateSettings(/*GameSettings settings*/) {

        }
    }
}