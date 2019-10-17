using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SkillEditor {

    internal static class LuaWriter {

        private static Dictionary<string, string> m_dicPathFileHead = new Dictionary<string, string>(Config.ReadFileCount);

        public static void AddHeadText(string path, string headText) {
            if (!m_dicPathFileHead.ContainsKey(path))
                m_dicPathFileHead.Add(path, headText);
            else
                m_dicPathFileHead[path] = headText;
        }

        private static StringBuilder m_stringBuilder = new StringBuilder(Config.AnimClipLuaFileLength);

        public static void Write() {
            foreach (var keyValue in m_dicPathFileHead) {
                string path = keyValue.Key;
                if (path != Config.AnimDataFilePath)
                    continue;
                m_stringBuilder.Clear();
                string headText = keyValue.Value;
                m_stringBuilder.Append(headText);
                string fileString = LuaAnimClipModel.GetWriteFileString(m_stringBuilder);
                FileStream file = new FileStream(path, FileMode.Create);
                StreamWriter fileWriter = new StreamWriter(file);
                fileWriter.Write(fileString);
                fileWriter.Close();
                fileWriter.Dispose();
            }
        }
    }
}