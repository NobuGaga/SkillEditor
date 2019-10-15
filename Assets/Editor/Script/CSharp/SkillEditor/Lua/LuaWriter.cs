using UnityEngine;
using System.Collections.Generic;

namespace SkillEditor {

    internal static class LuaWriter {

        private static Dictionary<string, string> m_dicPathFileHead = new Dictionary<string, string>(Config.ReadFileCount);

        public static void AddHeadText(string path, string headText) {
            if (m_dicPathFileHead.ContainsKey(path)) {
                Debug.LogError("LuaWriter::AddHeadText file already added");
                return;
            }
            m_dicPathFileHead.Add(path, headText);
        }

        public static string GetHeadText(string path) {
            if (!m_dicPathFileHead.ContainsKey(path)) {
                Debug.LogError(string.Format("LuaWriter:;GetHeadText {0} path is not exit", path));
                return string.Empty;
            }
            return m_dicPathFileHead[path];
        }
    }
}