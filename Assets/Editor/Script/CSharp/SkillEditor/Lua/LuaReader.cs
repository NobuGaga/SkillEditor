using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SkillEditor {

    internal static class LuaReader {

        private static string m_curLuaText;

        private static List<KeyFrameData> m_curFrameDataList = new List<KeyFrameData>();

        public static void Read(string path) {
            Reset();
            if (!File.Exists(path)) {
                Debug.LogError(path + " is not exit");
                return;
            }
            m_curLuaText = File.ReadAllText(path);
        }

        private static void Reset() {
            m_curLuaText = string.Empty;
        }
    }
}