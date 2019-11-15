﻿using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using SkillEditor;

namespace Lua {

    public static class LuaReader {

        public static void Read<T>() where T : ITable, ILuaFile<T> {
            T luaFile = default;
            string luaFilePath = luaFile.GetLuaFilePath();
            string luaFileHeadStart = luaFile.GetLuaFileHeadStart();
            List<T> list = luaFile.GetModel();
            if (!File.Exists(luaFilePath)) {
                Debug.LogError("LuaReader::Read path file is not exit. path : " + luaFilePath);
                return;
            }
            string luaText = File.ReadAllText(luaFilePath);
            int index = 0;
            ReadLuaFileHeadText(luaText, luaFilePath, luaFileHeadStart, ref index);
            ReadLuaFileTable(luaText, ref index, list);
        }

        private static StringBuilder m_luaTextHeadStringBuilder = new StringBuilder(Config.LuaFileHeadLength);
        private static void ReadLuaFileHeadText(string luaText, string luaFilePath, string luaFileHeadStart, ref int index) {
            index = luaText.IndexOf(luaFileHeadStart);
            if (index == Config.ErrorIndex) {
                Debug.LogError("LuaReader::ReadLuaFileHeadText lua file start text is not found. text " + luaFileHeadStart);
                return;
            }
            index += luaFileHeadStart.Length;
            m_luaTextHeadStringBuilder.Clear();
            m_luaTextHeadStringBuilder.Append(luaFileHeadStart);
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar == LuaFormat.CurlyBracesPair.start)
                    break;
                m_luaTextHeadStringBuilder.Append(curChar);
            }
            LuaWriter.AddHeadText(luaFilePath, m_luaTextHeadStringBuilder.ToString());
        }

        private static ITable ReadLuaFileTable<T>(string luaText, ref int index, List<T> list) where T : ITable {
            EnterLuaTable(luaText, ref index);
            T table = default;
            switch (table.GetReadType()) {
                case ReadType.Repeat:
                    ReadLuaFileRepeatTable(luaText, ref index, list);
                    break;
                case ReadType.RepeatToFixed:
                    ReadLuaFileRepeatToFixedFieldTable(luaText, ref index, list);
                    break;
                case ReadType.Fixed:
                    table = ReadLuaFileFixedFieldTable<T>(luaText, ref index);
                    break;
                case ReadType.FixedToRepeat:
                    table = ReadLuaFileFixedToRepeatTable<T>(luaText, ref index);
                    break;
            }
            ExitLuaTable(luaText, ref index);
            return table;
        }

        private static MethodInfo m_readLuaFileTableMethod = typeof(LuaReader).GetMethod("ReadLuaFileTable",
                                                                        BindingFlags.NonPublic | BindingFlags.Static);
        private static void ReadLuaFileRepeatTable<T>(string luaText, ref int index, List<T> list) where T : ITable {
            T table = default;
            if (!IsImplementIRepeatKeyTable(typeof(T), table.GetTableName()))
                return;
            object staticList = GetStaticListAndSetRepeatTableMethod<T>();
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                table = InitTableAndhKey<T>(luaText, ref index, endIndex, out bool isSuccess);
                if (!isSuccess)
                    break;
                ClearStaticListMethod.Invoke(staticList, null);
                ReadLuaFileTableMethod.Invoke(null, new object[] { luaText, index, staticList });
                SetTableListMethod.Invoke(table, null);
                list.Add(table);
            }
        }

        private static MethodInfo m_readFixedFieldTableValueMethod = typeof(LuaReader).GetMethod("ReadFixedFieldTableValue",
                                                                                BindingFlags.NonPublic | BindingFlags.Static);
        private static void ReadLuaFileRepeatToFixedFieldTable<T>(string luaText, ref int index, List<T> list) where T : ITable {
            T table = default;
            if (!IsImplementIFieldValueTable(typeof(T), table.GetTableName()))
                return;
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                table = InitTableAndhKey<T>(luaText, ref index, endIndex, out bool isSuccess);
                if (!isSuccess)
                    break;
                EnterLuaTable(luaText, ref index);
                m_readFixedFieldTableValueMethod.Invoke(null, new object[] { luaText, index, table });
                ExitLuaTable(luaText, ref index);
                list.Add(table);
            }
        }

        private static T ReadLuaFileFixedFieldTable<T>(string luaText, ref int index) where T : ITable {
            T table = default;
            if (!IsImplementIFieldValueTable(typeof(T), table.GetTableName()))
                return table;
            int endIndex = FindLuaTableEndIndex(luaText, index);
            table = InitTableAndhKey<T>(luaText, ref index, endIndex, out bool isSuccess);
            if (!isSuccess) {
                Debug.LogError("LuaReader::ReadLuaFileFixedFieldTable read key error. table " + table.GetTableName());
                return table;
            }
            m_readFixedFieldTableValueMethod.Invoke(null, new object[] { luaText, index, table });
            return table;
        }

        private static T ReadLuaFileFixedToRepeatTable<T>(string luaText, ref int index) where T : ITable {
            T table = default;
            if (!IsImplementIRepeatKeyTable(typeof(T), table.GetTableName()))
                return table;
            object staticList = GetStaticListAndSetRepeatTableMethod<T>();
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                // TODO endindex judge
                object subTable = ReadLuaFileTableMethod.Invoke(null, new object[] { luaText, index, null });
                AddStaticListMethod.Invoke(staticList, new object[] { subTable });
            }
            SetTableListMethod.Invoke(table, null);
            return table;
        }

        private static bool IsImplementIRepeatKeyTable(Type type, string tableName) {
            bool isImplement = Tool.IsImplementInterface(type, typeof(IRepeatKeyTable<>));
            if (!isImplement)
                Debug.LogError(string.Format("LuaReader table {0} is not implement interface IRepeatKeyTable<>", tableName));
            return isImplement;
        }

        private static bool IsImplementIFieldValueTable(Type type, string tableName) {
            bool isImplement = Tool.IsImplementInterface(type, typeof(IFieldValueTable));
            if (!isImplement)
                Debug.LogError(string.Format("LuaReader table {0} is not implement interface IFieldValueTable", tableName));
            return isImplement;
        }

        private static MethodInfo[] m_repeatKeyTableMethod = new MethodInfo[4];
        private const ushort ClearStaticListMethodIndex = 0;
        private const ushort AddStaticListMethodIndex = 1;
        private const ushort SetTableListMethodIndex = 2;
        private const ushort ReadLuaFileTableIndex = 3;
        private static MethodInfo ClearStaticListMethod => m_repeatKeyTableMethod[ClearStaticListMethodIndex];
        private static MethodInfo AddStaticListMethod => m_repeatKeyTableMethod[AddStaticListMethodIndex];
        private static MethodInfo SetTableListMethod => m_repeatKeyTableMethod[SetTableListMethodIndex];
        private static MethodInfo ReadLuaFileTableMethod => m_repeatKeyTableMethod[ReadLuaFileTableIndex];
        private static object GetStaticListAndSetRepeatTableMethod<T>() where T :ITable {
            T table = default;
            MethodInfo getStaticCacheListMethod = table.GetType().GetMethod("GetStaticCacheList");
            object staticList = getStaticCacheListMethod.Invoke(table, null);
            MethodInfo getTableListTypeMethod = table.GetType().GetMethod("GetTableListType");
            Type tableListType = getTableListTypeMethod.Invoke(table, null) as Type;
            m_repeatKeyTableMethod[ClearStaticListMethodIndex] = staticList.GetType().GetMethod("Clear");
            m_repeatKeyTableMethod[AddStaticListMethodIndex] = staticList.GetType().GetMethod("Add");
            m_repeatKeyTableMethod[SetTableListMethodIndex] = table.GetType().GetMethod("SetTableList");
            m_repeatKeyTableMethod[ReadLuaFileTableIndex] = m_readLuaFileTableMethod.MakeGenericMethod(new Type[] { tableListType });
            return staticList;
        }

        private static T InitTableAndhKey<T>(string luaText, ref int index, int endIndex, out bool isSuccess) where T : ITable {
            T table = default;
            isSuccess = true;
            switch (table.GetKeyType()) {
                case KeyType.Array:
                    int arrayKey = ReadLuaTableArrayKey(luaText, ref index, endIndex);
                    if (arrayKey == Config.ErrorIndex)
                        isSuccess = false;
                    else
                        table.SetKey(arrayKey);
                    break;
                case KeyType.Reference:
                case KeyType.String:
                    string stringKey = ReadLuaTableStringKey(luaText, ref index, endIndex);
                    if (stringKey == string.Empty)
                        isSuccess = false;
                    else
                        table.SetKey(stringKey);
                    break;
                case KeyType.FixedField:
                    break;
            }
            return table;
        }

        private static int ReadLuaTableArrayKey(string luaText, ref int index, int endIndex) {
            int copyIndex = index;
            FilterNotesLine(luaText, ref index);
            LuaFormat.PairStringChar symbol = LuaFormat.SquareBracketPair;
            index = luaText.IndexOf(symbol.start, index);
            if (index >= endIndex || index == Config.ErrorIndex) {
                index = copyIndex;
                return Config.ErrorIndex;
            }
            index += symbol.start.Length;
            int resultIndex = GetLuaTextInt(luaText, ref index);
            index++;
            return resultIndex;
        }

        private static string ReadLuaTableStringKey(string luaText, ref int index, int endIndex) {
            int copyIndex = index;
            FilterNotesLine(luaText, ref index);
            LuaFormat.PairString symbol = LuaFormat.HashKeyPair;
            index = luaText.IndexOf(symbol.start, index);
            if (index >= endIndex || index == Config.ErrorIndex) {
                index = copyIndex;
                return string.Empty;
            }
            index++;
            string key = GetLuaTextString(luaText, ref index);
            index++;
            return key;
        }

        private static void ReadFixedFieldTableValue(string luaText, ref int index, ref IFieldValueTable table) {
            int maxIndex = index;
            int endIndex = FindLuaTableEndIndex(luaText, index);
            FieldValueTableInfo[] array = table.GetFieldValueTableInfo();
            foreach (FieldValueTableInfo keyValue in array) {
                int valueIndex = luaText.IndexOf(keyValue.key, index);
                if (valueIndex == Config.ErrorIndex || valueIndex >= endIndex)
                    continue;
                valueIndex += keyValue.KeyLength;
                FilterSpaceSymbol(luaText, ref valueIndex);
                if (luaText[valueIndex] != LuaFormat.EqualSymbol) {
                    PrintErrorWhithLayer("关键帧配置表关键帧 Lua table 配置错误", valueIndex);
                    break;
                }
                valueIndex++;
                FilterSpaceSymbol(luaText, ref valueIndex);
                SetFixedFieldTableValue(luaText, ref valueIndex, keyValue, ref table);
                if (valueIndex > maxIndex)
                    maxIndex = valueIndex;
            }
            index = maxIndex;
        }

        private static void SetFixedFieldTableValue(string luaText, ref int valueIndex, FieldValueTableInfo keyValue, ref IFieldValueTable table) {
            object value = default;
            switch (keyValue.type) {
                case ValueType.Int:
                    value = GetLuaTextInt(luaText, ref valueIndex);
                    break;
                case ValueType.Number:
                    value = GetLuaTextNumber(luaText, ref valueIndex);
                    break;
                case ValueType.String:
                    value = GetLuaTextString(luaText, ref valueIndex);
                    break;
                case ValueType.Reference:
                    value = GetLuaTextReferenceString(luaText, ref valueIndex);
                    break;;
                case ValueType.Table:
                    Type valueType = LuaTable.GetFieldValueTableValueType(table, keyValue.key);
                    if (valueType == null)
                        return;
                    MethodInfo readLuaFileValueTextMethod = m_readLuaFileTableMethod.MakeGenericMethod(new Type[] { valueType });
                    value = readLuaFileValueTextMethod.Invoke(null, new object[] { luaText, valueIndex, null });
                    break;
            }
            table.SetFieldValueTableValue(keyValue.key, value);
        }

        private static int GetLuaTextInt(string luaText, ref int index) {
            int startIndex = index;
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if ((curChar < LuaFormat.IntMin || curChar > LuaFormat.IntMax) && curChar != LuaFormat.NotesSymbolStart)
                    break;
            }
            string intString = luaText.Substring(startIndex, index - startIndex);
            if (!int.TryParse(intString, out int interge))
                PrintErrorWhithLayer("关键帧配置表读取整型错误", index);
            if (luaText[index] == LuaFormat.CommaSymbol)
                index++;
            if (luaText[index] == LuaFormat.LineSymbol)
                index++;
            return interge;
        }

        private static float GetLuaTextNumber(string luaText, ref int index) {
            int startIndex = index;
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if ((curChar < LuaFormat.IntMin || curChar > LuaFormat.IntMax) &&
                    curChar != LuaFormat.NumberPoint && curChar != LuaFormat.NotesSymbolStart)
                    break;
            }
            string numberString = luaText.Substring(startIndex, index - startIndex);
            if (!float.TryParse(numberString, out float number))
                PrintErrorWhithLayer("关键帧配置表读取浮点型错误", index);
            if (luaText[index] == LuaFormat.CommaSymbol)
                index++;
            if (luaText[index] == LuaFormat.LineSymbol)
                index++;
            return number;
        }

        private static string GetLuaTextString(string luaText, ref int index) {
            if (luaText[index] != LuaFormat.QuotationPair.start) {
                PrintErrorWhithLayer("关键帧配置表读取字符串错误", index);
                return string.Empty;
            }
            index++;
            int startIndex = index;
            for (; index < luaText.Length; index++)
                if (luaText[index] == LuaFormat.QuotationPair.end)
                    break;
            string result = luaText.Substring(startIndex, index - startIndex);
            index++;
            if (luaText[index] == LuaFormat.CommaSymbol)
                index++;
            if (luaText[index] == LuaFormat.LineSymbol)
                index++;
            return result;
        }

        private static string GetLuaTextReferenceString(string luaText, ref int index) {
            int startIndex = index;
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar == LuaFormat.CommaSymbol || curChar == LuaFormat.SpaceSymbol)
                    break;
            }
            string result = luaText.Substring(startIndex, index - startIndex);
            FilterSpaceSymbol(luaText, ref index);
            if (luaText[index] == LuaFormat.CommaSymbol)
                index++;
            if (luaText[index] == LuaFormat.LineSymbol)
                index++;
            return result;
        }

        private static void EnterLuaTable(string luaText, ref int index) {
            for (; index < luaText.Length; index++) {
                if (luaText[index] != LuaFormat.CurlyBracesPair.start)
                    continue;
                index++;
                break;
            }
        }

        private static void ExitLuaTable(string luaText, ref int index) {
            for (; index < luaText.Length; index++) {
                if (luaText[index] != LuaFormat.CurlyBracesPair.end)
                    continue;
                index++;
                if (index >= luaText.Length)
                    break;
                if (luaText[index] == LuaFormat.CommaSymbol)
                    index++;
                if (index >= luaText.Length)
                    break;
                if (luaText[index] == LuaFormat.LineSymbol)
                    index++;
                break;
            }
        }

        private static int FindLuaTableEndIndex(string luaText, int index) {
            int curlyBracesCount = 0;
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar == LuaFormat.CurlyBracesPair.start)
                    curlyBracesCount++;
                else if (curChar == LuaFormat.CurlyBracesPair.end) {
                    curlyBracesCount--;
                    if (curlyBracesCount < 0) {
                        index++;
                        break;
                    }
                }
            }
            return index;
        }

        private static void FilterNotesLine(string luaText, ref int index) {
            while (luaText[index] == LuaFormat.NotesSymbolStart && index < luaText.Length) {
                index++;
                if (index >= luaText.Length || luaText[index] != LuaFormat.NotesSymbolStart)
                    break;
                for (; index < luaText.Length; index++)
                    if (luaText[index] == LuaFormat.NotesLinePair.end)
                        break;
                index++;
            }
        }

        private static void FilterSpaceSymbol(string luaText, ref int index) {
            for (; index < luaText.Length; index++)
                if (luaText[index] != LuaFormat.SpaceSymbol)
                    break;
        }

        private static void PrintErrorWhithLayer(string text, int index) {
            Debug.LogError(string.Format("{0} 当前索引值为 {1}", text, index));
        }
    }
}