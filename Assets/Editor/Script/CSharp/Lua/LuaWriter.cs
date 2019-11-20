using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua {

    public static class LuaWriter {

        private static Dictionary<string, string> m_dicPathFileHead = new Dictionary<string, string>(Config.ReadFileCount);

        public static void AddHeadText(string path, string headText) {
            if (!m_dicPathFileHead.ContainsKey(path))
                m_dicPathFileHead.Add(path, headText);
            else
                m_dicPathFileHead[path] = headText;
        }

        private static StringBuilder m_stringBuilder = new StringBuilder((UInt16)Math.Pow(2, 16));

        public static void Write<T>() where T : ITable, ILuaFile<T> {
            T luaFile = default;
            string luaFilePath = luaFile.GetLuaFilePath();
            if (!m_dicPathFileHead.ContainsKey(luaFilePath)) {
                Debug.LogError("LuaWriter::Write lua file head text path is not exit. lua file path " + luaFilePath);
                return;
            }
            m_stringBuilder.Clear();
            string headText = m_dicPathFileHead[luaFilePath];
            m_stringBuilder.Append(headText);
            string fileString = null;
            if (luaFile is AnimClipData.AnimClipData)
                fileString = LuaAnimClipModel.GetWriteFileString(m_stringBuilder);
            if (fileString == null)
                return;
            FileStream file = new FileStream(luaFilePath, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(file);
            fileWriter.Write(fileString);
            fileWriter.Close();
            fileWriter.Dispose();
        }
    }
}