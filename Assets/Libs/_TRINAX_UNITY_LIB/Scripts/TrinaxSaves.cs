using System.Collections.Generic;

namespace TRINAX {
    [System.Serializable]
    public class TrinaxSaves {
        public GlobalSettings globalSettings;
        //public GameSettings gameSettings;
        public AudioConfig audioSettings;
        //public KinectSettings kinectSettings;
    }

    [System.Serializable]
    public class CarSettings {
        public List<CarData> CarDataList;
    }

    [System.Serializable]
    public class CarData {
        public string name;
        public bool category_active;
        public CarInfo CarInfo;

        public CarData(string _name, bool _category_active, CarInfo _carInfo) {
            name = _name;
            category_active = _category_active;
            CarInfo = _carInfo;
        }
    }

    [System.Serializable]
    public class CarInfo {
        //public List<string> colors;
        //public List<int> LED_index;
        public List<string> categories;
    }

    [System.Serializable]
    public class LED {
        public List<LED_Index> led_indexes;
    }

    [System.Serializable]
    public class LED_Index {
        public string name;
        public LED_Color color;
    }
    [System.Serializable]
    public class LED_Color {
        public List<KeyValuePair<string, int>> leds;
    }

    [System.Serializable]
    public struct GlobalSettings {
        public string IP;
        public string COMPORT1;
        public string COMPORT2;

        public float idleInterval;

        public bool useServer;
        public bool useMocky;
        public bool useKeyboard;
        public bool muteAudioListener;
        public bool useLocalLeaderboard;
    }

    [System.Serializable]
    public struct AudioConfig {
        public float masterVolume;
        public float musicVolume;
        public float SFXVolume;
        public float SFX2Volume;
        public float SFX3Volume;
        public float SFX4Volume;
        public float UI_SFXVolume;
        public float UI_SFX2Volume;
    }

    //[System.Serializable]
    //public struct GameSettings
    //{

    //}

    //[System.Serializable]
    //public struct KinectSettings
    //{
    //    public bool isTrackingBody;
    //    public bool isTrackingHead;
    //}

    #region INTERACTION
    [System.Serializable]
    public struct StartInteractionSendJsonData {
        public string deviceID;
        public string projectID;
    }

    [System.Serializable]
    public struct StartInteractionReceiveJsonData {
        public requestClass request;
        public errorClass error;
        public string data;
    }

    //[System.Serializable]
    //public class StartInteractionData
    //{
    //    //public string interactionID;
    //    //public string deviceID;
    //    public string projectID;
    //    //public string createdDateTime;
    //    //public string endDateTime;
    //    //public string countryID;
    //    //public string state;
    //    //public string postalCode;
    //    //public string engagement;
    //    //public int status;
    //}

    [System.Serializable]
    public struct InteractionDetailsSendJsonData {
        public string interactionID;
        public string location;
        public string isPrint;
        public string isInfoCollect;
    }

    [System.Serializable]
    public struct InteractionDetailsReceiveJsonData {
        public requestClass request;
        public errorClass error;
        public bool data;
    }

    [System.Serializable]
    public struct InteractionEndSendJsonData {

    }

    [System.Serializable]
    public struct InteractionEndReceiveJsonData {
        public requestClass request;
        public errorClass error;
        public bool data;
    }

    #endregion

    #region LEADERBOARD
    [System.Serializable]
    public struct LeaderboardReceiveJsonData {
        public requestClass request;
        public errorClass error;
        public List<LeaderboardData> data;
    }

    [System.Serializable]
    public struct LeaderboardData {
        public string playerid;
        public string name;
        public string mobile;
        public int score;
    }
    #endregion

    #region API DATA
    [System.Serializable]
    public struct requestClass {
        public string api;
        public string result;
    }

    [System.Serializable]
    public struct errorClass {
        public string error_code;
        public string error_message;
    }
    #endregion

    /// <summary>
    /// For using playerPrefs with.
    /// </summary>
    public interface ITrinaxPersistantVars {
        string ip { get; set; }
        string photoPath { get; set; }
        bool useServer { get; set; }
    }
}