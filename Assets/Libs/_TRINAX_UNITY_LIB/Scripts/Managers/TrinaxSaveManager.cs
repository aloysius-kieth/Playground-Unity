using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

using TRINAX.Utils;

namespace TRINAX {
    public class TrinaxSaveManager : MonoBehaviour, IManager {
        int executionPriority = 0;
        public int ExecutionPriority { get { return executionPriority; } set { value = executionPriority; } }
        public bool IsReady { get; set; }

        public TrinaxSaves saveObj;
        private const string ADMINSAVEFILE = "settings.json";

        public void Init() {
            //await new WaitUntil(() => Trinax.Instance.loadNow);
            Debug.Log("<color=yellow>Loading SaveManager...</color>");
            IsReady = false;
            LoadJson();
            IsReady = true;
            Debug.Log("<color=lime>SaveManager is loaded!</color>");
        }

        private TrinaxSaves CreateAdminSave() {
            //GameSettings saveGameSettings = new GameSettings
            //{

            //};

            GlobalSettings saveGlobalSettings = new GlobalSettings {
                IP = Trinax.Instance.globalSettings.IP,
                //COMPORT1 = Trinax.Instance.globalSettings.COMPORT1,
                //COMPORT2 = Trinax.Instance.globalSettings.COMPORT2,
                idleInterval = Trinax.Instance.globalSettings.idleInterval,

                useServer = Trinax.Instance.globalSettings.useServer,
                useMocky = Trinax.Instance.globalSettings.useMocky,
                useKeyboard = Trinax.Instance.globalSettings.useKeyboard,
                muteAudioListener = Trinax.Instance.audioManager._muteAudioListener,
                useLocalLeaderboard = Trinax.Instance.globalSettings.useLocalLeaderboard,
            };

            AudioConfig saveAudioSettings = new AudioConfig {
                masterVolume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MASTER].slider.value,
                musicVolume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MUSIC].slider.value,
                SFXVolume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX].slider.value,
                SFX2Volume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX2].slider.value,
                SFX3Volume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX3].slider.value,
                SFX4Volume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX4].slider.value,
                UI_SFXVolume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX].slider.value,
                UI_SFX2Volume = Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX2].slider.value,
            };

            //KinectSettings saveKinectSettings = new KinectSettings
            //{
            //    isTrackingBody = Trinax.Instance.kinectSettings.isTrackingBody,
            //    isTrackingHead = Trinax.Instance.kinectSettings.isTrackingHead,
            //};

            TrinaxSaves save = new TrinaxSaves {
                //gameSettings = saveGameSettings,
                globalSettings = saveGlobalSettings,
                audioSettings = saveAudioSettings,
                //kinectSettings = saveKinectSettings,
            };

            return save;
        }

        public void SaveJson() {
            saveObj = CreateAdminSave();

            string saveJsonString = JsonUtility.ToJson(saveObj, true);

            JsonFileUtility.WriteJsonToFile(ADMINSAVEFILE, saveJsonString, JSONSTATE.PERSISTENT_DATA_PATH);    
            Debug.Log("Saving as JSON " + saveJsonString);
        }

        private void PopulateGlobalSettings() {
            Trinax.Instance.globalSettings = saveObj.globalSettings;
        }

        private void PopulateGameSettings() { }

        private void PopulateAudioSettings() {
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MASTER].slider.value = saveObj.audioSettings.masterVolume;
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MUSIC].slider.value = saveObj.audioSettings.musicVolume;
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX].slider.value = saveObj.audioSettings.SFXVolume;
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX2].slider.value = saveObj.audioSettings.SFX2Volume;
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX3].slider.value = saveObj.audioSettings.SFX3Volume;
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX4].slider.value = saveObj.audioSettings.SFX4Volume;
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX].slider.value = saveObj.audioSettings.UI_SFXVolume;
            Trinax.Instance.audioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX2].slider.value = saveObj.audioSettings.UI_SFX2Volume;

            Trinax.Instance.audioSettings.musicVolume = saveObj.audioSettings.musicVolume;
        }

        //void PopulateKinectSettings()
        //{
        //    Trinax.Instance.kinectSettings.isTrackingBody = saveObj.kinectSettings.isTrackingBody;
        //    Trinax.Instance.kinectSettings.isTrackingHead = saveObj.kinectSettings.isTrackingHead;
        //}

        public void LoadJson() {
            string loadJsonString = JsonFileUtility.LoadJsonFromFile(ADMINSAVEFILE, JSONSTATE.PERSISTENT_DATA_PATH);
            saveObj = JsonUtility.FromJson<TrinaxSaves>(loadJsonString);

            // Assign our values back!
            if (saveObj != null) {
                PopulateGlobalSettings();
                PopulateGameSettings();
                PopulateAudioSettings();
                //PopulateKinectSettings();
            }
            else {
                Debug.Log("Json file is empty! Creating a new save file...");
                saveObj = CreateAdminSave();
                string saveJsonString = JsonUtility.ToJson(saveObj, true);
                JsonFileUtility.WriteJsonToFile(ADMINSAVEFILE, saveJsonString, JSONSTATE.PERSISTENT_DATA_PATH);
            }
        }
    }
}

