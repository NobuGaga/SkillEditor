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
            string mainFileHeadText = luaFile.GetLuaFileHeadStart();
            ILuaSplitFile<T> luaSplitFile = (ILuaSplitFile<T>)luaFile;
            string folderPath = luaSplitFile.GetFolderPath();
            string mainFileName = luaSplitFile.GetMainFileName();
            string requirePath = luaSplitFile.GetChildFileRequirePath();
            string fileNameFormat = luaSplitFile.GetChildFileNameFormat();
            WriteSplitFile<T>(folderPath, mainFileName, mainFileHeadText, requirePath, fileNameFormat);
            if (Tool.IsImplementInterface(typeof(T), typeof(ILuaMultipleSplitFile<,>)))
                WriteMultipleSplitFile<T>();
        }

        private static void WriteMultipleSplitFile<T>() where T : ITable, ILuaFile<T> {
            T luaFile = default;
            MethodInfo getMultipleLuaMainFileNameMethod = luaFile.GetType().GetMethod("GetMultipleLuaMainFileName");
            MethodInfo getMultipleFolderPathMethod = luaFile.GetType().GetMethod("GetMultipleFolderPath");
            MethodInfo getMultipleLuaMainFileHeadStartMethod = luaFile.GetType().GetMethod("GetMultipleLuaMainFileHeadStart");
            MethodInfo getMultipleChildFileRequirePathMethod = luaFile.GetType().GetMethod("GetMultipleChildFileRequirePath");
            MethodInfo getMultipleLuaChildFileNameFormatMethod = luaFile.GetType().GetMethod("GetMultipleLuaChildFileNameFormat");
            string[] mainFileNames = getMultipleLuaMainFileNameMethod.Invoke(luaFile, null) as string[];
            string[] folderPaths = getMultipleFolderPathMethod.Invoke(luaFile, null) as string[];
            string[] mainFileHeadStarts = getMultipleLuaMainFileHeadStartMethod.Invoke(luaFile, null) as string[];
            string[] childRequirePath = getMultipleChildFileRequirePathMethod.Invoke(luaFile, null) as string[];
            string[] childFileNameFormats = getMultipleLuaChildFileNameFormatMethod.Invoke(luaFile, null) as string[];
            if (mainFileNames == null || mainFileNames.Length == 0 || folderPaths == null || folderPaths.Length == 0 ||
                mainFileHeadStarts == null || mainFileHeadStarts.Length == 0 || childRequirePath == null ||
                childRequirePath.Length == 0 || childFileNameFormats == null || childFileNameFormats.Length == 0) {
                Debug.Log("Multiple Split Lua File Configure Error. Type " + typeof(T).Name);
                return;
            }
            MethodInfo setFileTypeMethod = luaFile.GetType().GetMethod("SetFileType");
            object[] args = new object[1];
            for (ushort index = 0; index < mainFileNames.Length; index++) {
                args[0] = index;
                setFileTypeMethod.Invoke(luaFile, args);
                WriteSplitFile<T>(folderPaths[index], mainFileNames[index], mainFileHeadStarts[index], childRequirePath[index], childFileNameFormats[index]);
            }
            args[0] = LuaTable.DefaultFileType;
            setFileTypeMethod.Invoke(luaFile, args);
        }

        private static StringBuilder m_mainFileTextBuilder = new StringBuilder((UInt16)Math.Pow(2, 9));
        private static void WriteSplitFile<T>(string folderPath, string mainFileName, string mainFileHeadText, string requirePath, string fileNameFormat) where T : ITable, ILuaFile<T> {
            T luaFile = default;
            ILuaSplitFile<T> luaSplitFile = (ILuaSplitFile<T>)luaFile;
            string requireFunction = luaSplitFile.GetChildFileRequireFunction();
            List<T> list = luaFile.GetModel();
            string fileExtension = luaSplitFile.GetFileExtension();
            m_mainFileTextBuilder.Clear();
            for (ushort index = 0; index < list.Count; index++) {
                T childFileData = list[index];
                string fileName = string.Format(fileNameFormat, childFileData.GetKey());
                string path = requirePath + fileName;
                string requireText = string.Format(requireFunction, path);
                m_mainFileTextBuilder.Append(requireText);
                string filePath = Tool.CombineFilePath(folderPath, fileName, fileExtension);
                Write(filePath, childFileData.GetWriteFileString());
            }
            string mainFilePath = Tool.CombineFilePath(folderPath, mainFileName, fileExtension);
            string mainFileText = m_mainFileTextBuilder.ToString();
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
            MethodInfo getMultipleLuaFileHeadStartMethod = luaFile.GetType().GetMethod("GetMultipleLuaFileHeadStart");
            string[] luaFilePaths = getMultipleLuaFilePathMethod.Invoke(luaFile, null) as string[];
            string[] luaFileHeadStarts = getMultipleLuaFileHeadStartMethod.Invoke(luaFile, null) as string[];
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