using System.Diagnostics;

// Put #define ENABLE_LOGS at the top of script to enable/disable log messages

namespace TRINAX.Utils {
    public static class Logger {
        [Conditional("ENABLE_LOGS")]
        public static void Debug(object logMsg) {
            UnityEngine.Debug.Log(logMsg);
        }

        [Conditional("ENABLE_LOGS")]
        public static void DebugWarning(object logMsg) {
            UnityEngine.Debug.LogWarning(logMsg);
        }

        [Conditional("ENABLE_LOGS")]
        public static void DebugError(object logMsg) {
            UnityEngine.Debug.LogError(logMsg);
        }
    }
}