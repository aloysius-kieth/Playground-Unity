using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRINAX.Utils {

    /// <summary>
    /// Use this for writing/loading json array objects
    /// </summary>
    public static class JsonArrayUtility {
        //const string HEADER = "{ \"leaderboard\": ";

        public static List<T> FromJson<T>(string jsonArray) {
            jsonArray = WrapArray(jsonArray);
            return FromJsonWrapped<T>(jsonArray);
        }

        public static List<T> FromJsonWrapped<T>(string jsonObject) {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonObject);
            return wrapper.leaderboard;
        }

        private static string WrapArray(string jsonArray) {
            return /*HEADER + */jsonArray + "}";
        }

        public static string ToJson<T>(List<T> array) {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.leaderboard = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(List<T> array, bool prettyPrint) {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.leaderboard = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T> {
            public List<T> leaderboard;
        }
    }
}