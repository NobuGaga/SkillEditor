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
            if (Tool.IsImplementInterface(typeof(T), typeof(ILuaSplitFile<>)))
                WriteSplitFile<T>();
            else
                WriteSimpleFile<T>();
        }

        private static void WriteSplitFile<T>() where T : ITable, ILuaFile<T> {
            T luaFile = default;
            ILuaSplitFile<T> luaSplitFile = (ILuaSplitFile<T>)luaFile;
            List<T> list = luaFile.GetModel();
            string folderPath = luaSplitFile.GetFolderPath();
            string fileExtension = luaSplitFile.GetFileExtension();
            string requirePath = luaSplitFile.GetChildFileRequirePath();
            string fileNameFormat = luaSplitFile.GetChildFileNameFormat();
            StringBuilder mainFileContent = new StringBuilder((UInt16)Math.Pow(2, 9));
            for (ushort index = 0; index < list.Count; index++) {
                T childFileData = list[index];
                string fileName = string.Format(fileNameFormat, childFileData.GetKey());
                string path = requirePath + fileName;
                string requireText = string.Format(LuaFormat.RequireFunction, path);
                mainFileContent.Append(requireText);
                string filePath = Tool.CombineFilePath(folderPath, fileName, fileExtension);
                Write(filePath, childFileData.GetWriteFileString());
            }
            string mainFilePath = luaFile.GetLuaFilePath();
            string mainFileHeadText = luaFile.GetLuaFileHeadStart();
            string mainFileText = mainFileContent.ToString();
            WriteWithHeadText(mainFilePath, mainFileHeadText, mainFileText);
        }

        private static void WriteSimpleFile<T>() where T : ITable, ILuaFile<T> {
            T luaFile = default;
            string luaFilePath = luaFile.GetLuaFilePath();
            if (!m_dicPathFileHead.ContainsKey(luaFilePath)) {
                Debug.LogError("LuaWriter::Write lua file head text path is not exit. lua file path " + luaFilePath);
                return;
            }
            WriteWithHeadText(luaFilePath, luaFile.GetWriteFileString());
            if (!Tool.IsImplementInterface(typeof(T), typeof(ILuaMultipleFile<,>)))
                return;
            MethodInfo getMultipleLuaFilePathMethod = luaFile.GetType().GetMethod("GetMultipleLuaFilePath");
            MethodInfo getTableListTypeMethodMethod = luaFile.GetType().GetMethod("GetMultipleLuaFileHeadStart");
            string[] luaFilePaths = getMultipleLuaFilePathMethod.Invoke(luaFile, null) as string[];
            string[] luaFileHeadStarts = getTableListTypeMethodMethod.Invoke(luaFile, null) as string[];
            if (luaFilePaths == null || luaFilePaths.Length == 0 || luaFileHeadStarts == null ||
                luaFileHeadStarts.Length == 0 || luaFilePaths.Length != luaFileHeadStarts.Length) {
                Debug.Log("Multiple Lua File Configure Error. Type " + typeof(T).Name);
                return;
            }
            MethodInfo setFileTypeMethod = luaFile.GetType().GetMethod("SetFileType");
            object[] args = new object[1];
            for (ushort index = 0; index < luaFilePaths.Length; index++) {
                args[0] = index;
                setFileTypeMethod.Invoke(luaFile, args);
                WriteWithHeadText(luaFilePaths[index], luaFileHeadStarts[index], luaFile.GetWriteFileString());
            }
            args[0] = LuaTable.DefaultFileType;
            setFileTypeMethod.Invoke(luaFile, args);
        }

        private static void WriteWithHeadText(string luaFilePath, string fileString) => 
            WriteWithHeadText(luaFilePath, m_dicPathFileHead[luaFilePath], fileString);

        private static void WriteWithHeadText(string luaFilePath, string headText, string fileString) {
            m_stringBuilder.Clear();
            m_stringBuilder.Append(headText);
            m_stringBuilder.Append(fileString);
            Write(luaFilePath, m_stringBuilder.ToString());
        }

        private static void Write(string filePath, string text) {
            FileStream file = new FileStream(filePath, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(file);
            fileWriter.Write(text);
            fileWriter.Close();
            fileWriter.Dispose();
        }

        public static string GetWriteFileString<T>(List<T> list) {
            m_stringBuilder.Clear();
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