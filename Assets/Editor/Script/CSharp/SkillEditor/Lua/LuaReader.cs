using UnityEngine;
using System.IO;
using StringComparison = System.StringComparison;

namespace SkillEditor {

    internal static class LuaReader {

        private static string m_lastPath;
        private static string m_curLuaText;

        public static void Read(string path) {
            if (m_lastPath == path)
                return;
            if (!File.Exists(path)) {
                Debug.LogError(path + " is not exit");
                return;
            }
            m_lastPath = path;
            m_curLuaText = File.ReadAllText(path);
        }

        public static void SetModelData(ref KeyFrameData data) {
            int index = m_curLuaText.IndexOf(data.modelName, StringComparison.Ordinal);
            if (index == Config.ErrorIndex)
                return;

        }
    }
}