using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using StringComparison = System.StringComparison;
using SkillEditor.Structure;

namespace SkillEditor {

    using LuaTableKeyValue = LuaFormat.LuaTableKeyValue;

    internal static class LuaReader {

        private static int m_tableLayer;

        public static List<T> Read<T>(string path) {
            if (!File.Exists(path)) {
                Debug.LogError(path + " is not exit");
                return null;
            }
            m_tableLayer = 0;
            return AnalyseLuaText<T>(path, File.ReadAllText(path));
        }

        private static List<T> AnalyseLuaText<T>(string path, string luaText) {
            string luaFileHeadStart = string.Empty;
            string typeName = typeof(T).Name;
            string keyFrameDataTypeName = typeof(KeyFrameData).Name;
            if (typeName == keyFrameDataTypeName)
                luaFileHeadStart = Config.LuaFileHeadStart;
            int index = luaText.IndexOf(luaFileHeadStart, StringComparison.Ordinal);
            if (index == Config.ErrorIndex)
                return null;
            StringBuilder headBuilder = new StringBuilder(Config.LuaFileHeadLength);
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar == LuaFormat.CurlyBracesPair.start)
                    break;
                headBuilder.Append(curChar);
            }
            LuaWriter.AddHeadText(path, headBuilder.ToString());
            List<T> list = null;
            if (typeName == keyFrameDataTypeName)
                AnalyseKeyFrameData(luaText, ref index, list = new List<T>(Config.ModelCount));
            return list;
        }

        private static object AnalyseKeyFrameData(string luaText, ref int index, object data = null) {
            switch((KeyFrameLuaLayer)m_tableLayer) {
                case KeyFrameLuaLayer.EnterTable:
                    AnalyseEntry(luaText, ref index, data);
                    break;
                case KeyFrameLuaLayer.Model:
                    AnalyseModelData(luaText, ref index, data as List<KeyFrameData>);
                    break;
                case KeyFrameLuaLayer.Clip:
                    AnalyseClipData(luaText, ref index, data as List<SkillClipData>);
                    break;
                case KeyFrameLuaLayer.FrameIndex:
                    AnalyseFrameIndexData(luaText, ref index, data as List<FrameData>);
                    break;
                case KeyFrameLuaLayer.FrameData:
                    return AnalyseFrameData(luaText, ref index, (FrameData)data);
                case KeyFrameLuaLayer.CustomData:
                    return AnalyseCustomData(luaText, ref index);
                case KeyFrameLuaLayer.Effect:
                    EFrameType frameType = (EFrameType)data;
                    switch (frameType) {
                        case EFrameType.Grab:
                        case EFrameType.Ungrab:
                            return AnalyseEffectData(luaText, ref index, frameType);
                        case EFrameType.Collision:
                        case EFrameType.Harm:
                            return AnalyseRectData(luaText, ref index);
                    }
                    break;
                case KeyFrameLuaLayer.Rect:
                    return AnalyseHarmData(luaText, ref index);
            }
            return null;
        }

        private static void AnalyseEntry(string luaText, ref int index, object data) {
            EnterLuaTable(luaText, ref index);
            AnalyseKeyFrameData(luaText, ref index, data);
            ExitLuaTable(luaText, ref index);
        }

        private static List<SkillClipData> m_listClipCache = new List<SkillClipData>(Config.ModelClipCount);
        private static void AnalyseModelData(string luaText, ref int index, List<KeyFrameData> list) {
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                string modelName = ReadLuaTableHashKey(luaText, ref index, endIndex);
                if (modelName == string.Empty)
                    break;
                KeyFrameData data = new KeyFrameData();
                data.modelName = modelName;
                m_listClipCache.Clear();
                EnterLuaTable(luaText, ref index);
                AnalyseKeyFrameData(luaText, ref index, m_listClipCache);
                ExitLuaTable(luaText, ref index);
                data.clipDatas = m_listClipCache.ToArray();
                list.Add(data);
            }
        }

        private static List<FrameData> m_listFrameCache = new List<FrameData>(Config.ModelClipKeyFrameCount);
        private static void AnalyseClipData(string luaText, ref int index, List<SkillClipData> list) {
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                string clipName = ReadLuaTableHashKey(luaText, ref index, endIndex);
                if (clipName == string.Empty)
                    break;
                SkillClipData data = new SkillClipData();
                data.clipName = clipName;
                m_listFrameCache.Clear();
                EnterLuaTable(luaText, ref index);
                AnalyseKeyFrameData(luaText, ref index, m_listFrameCache);
                ExitLuaTable(luaText, ref index);
                data.frameDatas = m_listFrameCache.ToArray();
                list.Add(data);
            }
        }

        private static void AnalyseFrameIndexData(string luaText, ref int index, List<FrameData> list) {
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                int frameIndex = ReadLuaTableArrayKey(luaText, ref index, endIndex);
                if (frameIndex == Config.ErrorIndex)
                    break;
                FrameData data = new FrameData();
                data.frameIndex = frameIndex;
                EnterLuaTable(luaText, ref index);
                data = (FrameData)AnalyseKeyFrameData(luaText, ref index, data);
                ExitLuaTable(luaText, ref index);
                list.Add(data);
            }
        }

        private static FrameData AnalyseFrameData(string luaText, ref int index, FrameData frameData) {
            return (FrameData)ReadLuaTable(luaText, ref index, frameData);
        }

        private static List<CustomData> m_listCustomDataCache = new List<CustomData>(Config.ModelClipFrameCustomDataCount);
        private static object AnalyseCustomData(string luaText, ref int index) {
            m_listCustomDataCache.Clear();
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                int frameTypeInt = ReadLuaTableArrayKey(luaText, ref index, endIndex);
                if (frameTypeInt == Config.ErrorIndex)
                    break;
                CustomData data = new CustomData();
                data.frameType = (EFrameType)frameTypeInt;
                EnterLuaTable(luaText, ref index);
                data.data = AnalyseKeyFrameData(luaText, ref index, data.frameType);
                ExitLuaTable(luaText, ref index);
                m_listCustomDataCache.Add(data);
            }
            return m_listClipCache.ToArray();
        }

        private static ITable AnalyseEffectData(string luaText, ref int index, EFrameType frameType) {
            ITable table = null;
            switch (frameType) {
                case EFrameType.Grab:
                    table = new GrabData();
                    break;
                case EFrameType.Ungrab:
                    table = new UngrabData();
                    break;
            }
            return ReadLuaTable(luaText, ref index, table);
        }

        private static List<HarmData> m_listHarmDataCache = new List<HarmData>(Config.ModelClipFrameRectDataCount);
        private static HarmData[] AnalyseRectData(string luaText, ref int index) {
            m_listHarmDataCache.Clear();
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                if (ReadLuaTableArrayKey(luaText, ref index, endIndex) == Config.ErrorIndex)
                    break;
                EnterLuaTable(luaText, ref index);
                m_listHarmDataCache.Add((HarmData)AnalyseKeyFrameData(luaText, ref index));
                ExitLuaTable(luaText, ref index);
            }
            return m_listHarmDataCache.ToArray();
        }

        private static HarmData AnalyseHarmData(string luaText, ref int index) {
            HarmData data = new HarmData();
            return (HarmData)ReadLuaTable(luaText, ref index, data);
        }

        private static int ReadLuaTableArrayKey(string luaText, ref int index, int endIndex) {
            int copyIndex = index;
            FilterNotesLine(luaText, ref index);
            PairStringChar symbol = LuaFormat.SquareBracketPair;
            index = luaText.IndexOf(symbol.start, index, StringComparison.Ordinal);
            if (index >= endIndex || index == Config.ErrorIndex) {
                index = copyIndex;
                return Config.ErrorIndex;
            }
            index += symbol.start.Length;
            int resultIndex = GetLuaTextInt(luaText, ref index);
            index++;
            return resultIndex;
        }

        private static string ReadLuaTableHashKey(string luaText, ref int index, int endIndex) {
            int copyIndex = index;
            FilterNotesLine(luaText, ref index);
            PairString symbol = LuaFormat.HashKeyPair;
            index = luaText.IndexOf(symbol.start, index, StringComparison.Ordinal);
            if (index >= endIndex || index == Config.ErrorIndex) {
                index = copyIndex;
                return string.Empty;
            }
            index++;
            string key = GetLuaTextString(luaText, ref index);
            index++;
            return key;
        }

        private static ITable ReadLuaTable(string luaText, ref int index, ITable table) {
            int maxIndex = index;
            LuaTableKeyValue[] array = table.GetTableKeyValueList();
            foreach (LuaTableKeyValue tableKeyValue in array) {
                int keyIndex = luaText.IndexOf(tableKeyValue.key, index, StringComparison.Ordinal);
                if (keyIndex == Config.ErrorIndex)
                    continue;
                keyIndex += tableKeyValue.KeyLength;
                FilterSpaceSymbol(luaText, ref keyIndex);
                if (luaText[keyIndex] != LuaFormat.EqualSymbol) {
                    PrintErrorWhithLayer("关键帧配置表关键帧 Lua table 配置错误", index);
                    break;
                }
                keyIndex++;
                FilterSpaceSymbol(luaText, ref keyIndex);
                SetLuaTableData(luaText, ref keyIndex, tableKeyValue, ref table);
                if (keyIndex > maxIndex)
                    maxIndex = keyIndex;
            }
            index = maxIndex;
            return table;
        }

        private static void SetLuaTableData(string luaText, ref int keyIndex, LuaTableKeyValue keyValue, ref ITable table) {
            switch (keyValue.type) {
                case LuaFormat.Type.LuaString:
                    table.SetTableKeyValue(keyValue.key, GetLuaTextString(luaText, ref keyIndex));
                    return;
                case LuaFormat.Type.LuaInt:
                    table.SetTableKeyValue(keyValue.key, GetLuaTextInt(luaText, ref keyIndex));
                    return;
                case LuaFormat.Type.LuaNumber:
                    table.SetTableKeyValue(keyValue.key, GetLuaTextNumber(luaText, ref keyIndex));
                    return;
                case LuaFormat.Type.LuaTable:
                    EnterLuaTable(luaText, ref keyIndex);
                    int endIndex = FindLuaTableEndIndex(luaText, keyIndex);
                    if (!CheckNullTable(luaText, keyIndex, endIndex))
                        table.SetTableKeyValue(keyValue.key, AnalyseKeyFrameData(luaText, ref keyIndex));
                    ExitLuaTable(luaText, ref keyIndex);
                    return;
            }
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

        private static bool CheckNullTable(string luaText, int tableStartIndex, int tableEndIndex) {
            if (tableEndIndex - tableStartIndex <= 1)
                return true;
            for (; tableStartIndex < tableEndIndex; tableStartIndex++) {
                char curChar = luaText[tableStartIndex];
                if (curChar != LuaFormat.NotesSymbolStart && curChar != LuaFormat.SpaceSymbol &&
                    curChar != LuaFormat.LineSymbol)
                    return false;
            }
            return true;
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

        private static void EnterLuaTable(string luaText, ref int index) {
            for (; index < luaText.Length; index++) {
                if (luaText[index] != LuaFormat.CurlyBracesPair.start)
                    continue;
                m_tableLayer++;
                index++;
                break;
            }
        }

        private static void ExitLuaTable(string luaText, ref int index) {
            for (; index < luaText.Length; index++) {
                if (luaText[index] != LuaFormat.CurlyBracesPair.end)
                    continue;
                m_tableLayer--;
                index++;
                if (luaText[index] == LuaFormat.CommaSymbol)
                    index++;
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

        private static void PrintErrorWhithLayer(string text, int index) {
            Debug.LogError(string.Format("{0} 当前层为 {1}, 索引值为 {2}", text, (KeyFrameLuaLayer)m_tableLayer, index));
        }
    }
}