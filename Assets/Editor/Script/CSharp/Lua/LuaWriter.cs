using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using SkillEditor;

namespace Lua {

    public static class LuaWriter {

        private static Dictionary<string, string> m_dicPathFileHead = new Dictionary<string, string>();

        public static void AddHeadText(string path, string headText) {
            if (!m_dicPathFileHead.ContainsKey(path))
                m_dicPathFileHead.Add(path, headText);
            else
                m_dicPathFileHead[path] = headText;
        }

        private static StringBuilder m_stringBuilder = new StringBuilder((UInt16)Math.Pow(2, 16));
        public static StringBuilder BuilderCache => m_stringBuilder;

        public static void Write<T>() where T : ITable, ILuaFile<T> {
            T luaFile = default;
            string luaFilePath = luaFile.GetLuaFilePath();
            if (!m_dicPathFileHead.ContainsKey(luaFilePath)) {
                Debug.LogError("LuaWriter::Write lua file head text path is not exit. lua file path " + luaFilePath);
                return;
            }
            Write(luaFilePath, luaFile.GetWriteFileString());
            if (!Tool.IsImplementInterface(typeof(T), typeof(ILuaMultipleFile<,>)))
                return;
            MethodInfo getMultipleLuaFilePathMethod = luaFile.GetType().GetMethod("GetMultipleLuaFilePath");
            MethodInfo getTableListTypeMethodMethod = luaFile.GetType().GetMethod("GetMultipleLuaFileHeadStart");
            MethodInfo getWriteMultipleFileStringMethod = luaFile.GetType().GetMethod("GetWriteMultipleFileString");
            string[] luaFilePaths = getMultipleLuaFilePathMethod.Invoke(luaFile, null) as string[];
            string[] luaFileHeadStarts = getTableListTypeMethodMethod.Invoke(luaFile, null) as string[];
            string[] luaMultipleFileStrings = getWriteMultipleFileStringMethod.Invoke(luaFile, null) as string[];
            if (luaFilePaths == null || luaFilePaths.Length == 0 || luaFileHeadStarts == null ||
                luaFileHeadStarts.Length == 0 || luaMultipleFileStrings == null || luaMultipleFileStrings.Length == 0 ||
                luaFilePaths.Length != luaFileHeadStarts.Length || luaFilePaths.Length != luaMultipleFileStrings.Length) {
                Debug.Log("Multiple Lua File Configure Error. Type " + typeof(T).Name);
                return;
            }
            MethodInfo setFileTypeMethod = luaFile.GetType().GetMethod("SetFileType");
            object[] args = new object[1];
            for (ushort index = 0; index < luaFilePaths.Length; index++) {
                args[0] = index;
                setFileTypeMethod.Invoke(luaFile, args);
                Write(luaFilePaths[index], luaFileHeadStarts[index], luaMultipleFileStrings[index]);
            }
        }

        private static void Write(string luaFilePath, string fileString) => 
            Write(luaFilePath, m_dicPathFileHead[luaFilePath], fileString);

        private static void Write(string luaFilePath, string headText, string fileString) {
            m_stringBuilder.Clear();
            m_stringBuilder.Append(headText);
            FileStream file = new FileStream(luaFilePath, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(file);
            fileWriter.Write(fileString);
            fileWriter.Close();
            fileWriter.Dispose();
        }

        public static string GetWriteFileString<T>(List<T> list) {
            m_stringBuilder.Append(LuaFormat.CurlyBracesPair.start);
            if (list != null && list.Count != 0) {
                m_stringBuilder.Append(LuaFormat.LineSymbol);
                for (int index = 0; index < list.Count; index++)
                    m_stringBuilder.Append(list[index].ToString());
            }
            m_stringBuilder.Append(LuaFormat.CurlyBracesPair.end);
            return m_stringBuilder.ToString();
        }
    }
}