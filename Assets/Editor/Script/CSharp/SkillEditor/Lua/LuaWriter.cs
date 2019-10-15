using UnityEngine;
using System.IO;
using System.Text;
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

        private static StringBuilder m_stringBuilder = new StringBuilder(Config.KeyFrameFileLength);

        public static void Write() {
            string path = string.Empty;
            string headText = string.Empty;
            foreach (var keyValue in m_dicPathFileHead) {
                path = keyValue.Key;
                headText = keyValue.Value;
            }
            if (path == string.Empty || headText == string.Empty)
                return;
            m_stringBuilder.Clear();
            m_stringBuilder.Append(headText);
            string fileString = KeyFrameModel.GetWriteFileString(m_stringBuilder);
            FileStream file = new FileStream(path, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(file);
            fileWriter.Write(fileString);
            fileWriter.Close();
            fileWriter.Dispose();
        }
    }
}