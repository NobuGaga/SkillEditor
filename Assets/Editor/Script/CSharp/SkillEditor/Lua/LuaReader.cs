using UnityEngine;
using System.IO;

namespace SkillEditor {

    internal static class LuaReader {

        private static string m_curLuaText;

        public static void Read(string path) {
            Reset();
            if (!File.Exists(path)) {
                Debug.LogError(path + " is not exit");
                return;
            }
            m_curLuaText = File.ReadAllText(path);
        }

        public static KeyFrameData GetModelData(string modelName) {

            return new KeyFrameData();
        }

        private static void Reset() {
            m_curLuaText = string.Empty;
        }
    }
}