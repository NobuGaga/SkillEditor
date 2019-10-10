using UnityEngine;
using System.IO;
using StringComparison = System.StringComparison;

namespace SkillEditor {

    internal static class LuaReader {

        private static string m_lastPath;

        public static void Read(string path) {
            if (m_lastPath == path)
                return;
            if (!File.Exists(path)) {
                Debug.LogError(path + " is not exit");
                return;
            }
            m_lastPath = path;
            string luaText = File.ReadAllText(path);
            AnalyseLuaText(luaText);
        }

        private static void AnalyseLuaText(string luaText) {

        }
    }
}