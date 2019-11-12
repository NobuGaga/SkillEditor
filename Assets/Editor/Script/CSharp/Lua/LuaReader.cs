using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using StringComparison = System.StringComparison;
using SkillEditor;
using SkillEditor.LuaStructure;

namespace Lua {

    using FieldKeyTable = LuaFormat.FieldKeyTable;

    public static class LuaReader {

        private static int m_tableLayer;

        public static void Read<T>() {
            string luaFilePath = string.Empty;
            string luaFileHeadStart = string.Empty;
            object list = null;
            string typeName = typeof(T).Name;
            string animClipDataTypeName = typeof(AnimClipData).Name;
            if (typeName == animClipDataTypeName) {
                luaFilePath = Config.AnimDataFilePath;
                luaFileHeadStart = Config.LuaFileHeadStart;
                list = LuaAnimClipModel.AnimClipList;
            }
            if (!File.Exists(luaFilePath)) {
                Debug.LogError("LuaReader::Read path file is not exit. path : " + luaFilePath);
                return;
            }
            AnalyseLuaText(luaFilePath, luaFileHeadStart, list);
        }

        private static StringBuilder m_luaTextHeadStringBuilder = new StringBuilder(Config.LuaFileHeadLength);
        private static void AnalyseLuaText(string luaFilePath, string luaFileHeadStart, object list) {
            string luaText = File.ReadAllText(luaFilePath);
            int index = luaText.IndexOf(luaFileHeadStart, StringComparison.Ordinal);
            if (index == Config.ErrorIndex) {
                Debug.LogError("LuaReader::AnalyseLuaText lua file start text is not found. text " + luaFileHeadStart);
                return;
            }
            index += Config.LuaFileHeadStart.Length;
            m_luaTextHeadStringBuilder.Clear();
            m_luaTextHeadStringBuilder.Append(Config.LuaFileHeadStart);
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar == LuaFormat.CurlyBracesPair.start)
                    break;
                m_luaTextHeadStringBuilder.Append(curChar);
            }
            LuaWriter.AddHeadText(luaFilePath, m_luaTextHeadStringBuilder.ToString());
            m_tableLayer = 0;
            AnalyseAnimClipData(luaText, ref index, list);
        }

        private static object AnalyseAnimClipData(string luaText, ref int index, object data = null) {
            switch ((AnimClipLuaLayer)m_tableLayer) {
                case AnimClipLuaLayer.EnterTable:
                    AnalyseEntry(luaText, ref index, data);
                    break;
                case AnimClipLuaLayer.Model:
                    AnalyseModelData(luaText, ref index, data as List<AnimClipData>);
                    break;
                case AnimClipLuaLayer.State:
                    AnalyseStateData(luaText, ref index, data as List<StateData>);
                    break;
                case AnimClipLuaLayer.Clip:
                    AnalyseClipListData(luaText, ref index, data as List<ClipData>);
                    break;
                case AnimClipLuaLayer.FrameGroup:
                    return AnalyseClipData(luaText, ref index, (ClipData)data);
                case AnimClipLuaLayer.FrameType:
                    return AnalyseKeyFrameListData(luaText, ref index);
                case AnimClipLuaLayer.FrameData:
                    return AnalyseKeyFrameData(luaText, ref index, (ITable)data);
                case AnimClipLuaLayer.CustomeData:
                    FrameType frameType;
                    if (data is KeyFrameData)
                        frameType = ((KeyFrameData)data).frameType;
                    else
                        frameType = ((ProcessFrameData)data).frameType;
                    return AnalyseCustomData(luaText, ref index, frameType);
                case AnimClipLuaLayer.Effect:
                    return AnalyseEffectData(luaText, ref index, (FrameType)data);
                case AnimClipLuaLayer.Rect:
                    return AnalyseCubeData(luaText, ref index);
            }
            return null;
        }

        private static void AnalyseEntry(string luaText, ref int index, object data) {
            EnterLuaTable(luaText, ref index);
            AnalyseAnimClipData(luaText, ref index, data);
            ExitLuaTable(luaText, ref index);
        }

        private static List<StateData> m_listStateCache = new List<StateData>(Config.ModelStateCount);
        private static void AnalyseModelData(string luaText, ref int index, List<AnimClipData> list) {
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                string modelName = ReadLuaTableHashKey(luaText, ref index, endIndex);
                if (modelName == string.Empty)
                    break;
                AnimClipData data = new AnimClipData();
                data.modelName = modelName;
                m_listStateCache.Clear();
                EnterLuaTable(luaText, ref index);
                AnalyseAnimClipData(luaText, ref index, m_listStateCache);
                ExitLuaTable(luaText, ref index);
                data.stateList = m_listStateCache.ToArray();
                list.Add(data);
            }
        }

        private static List<ClipData> m_listClipCache = new List<ClipData>(Config.ModelStateClipCount);
        private static void AnalyseStateData(string luaText, ref int index, List<StateData> list) {
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                string stateName = ReadLuaTableHashKey(luaText, ref index, endIndex);
                if (stateName == string.Empty)
                    break;
                StateData data = new StateData();
                data.SetState(stateName);
                m_listClipCache.Clear();
                EnterLuaTable(luaText, ref index);
                AnalyseAnimClipData(luaText, ref index, m_listClipCache);
                ExitLuaTable(luaText, ref index);
                data.clipList = m_listClipCache.ToArray();
                list.Add(data);
            }
        }

        private static void AnalyseClipListData(string luaText, ref int index, List<ClipData> list) {
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                string clipName = ReadLuaTableHashKey(luaText, ref index, endIndex);
                if (clipName == string.Empty)
                    break;
                ClipData data = new ClipData();
                data.clipName = clipName;
                EnterLuaTable(luaText, ref index);
                data = (ClipData)AnalyseAnimClipData(luaText, ref index, data);
                ExitLuaTable(luaText, ref index);
                list.Add(data);
            }
        }

        private static object AnalyseClipData(string luaText, ref int index, ClipData clipData) {
            return ReadLuaTable(luaText, ref index, clipData);
        }

        private static List<KeyFrameData> m_listKeyFrameDataCache = new List<KeyFrameData>(Config.ModelStateClipFrameCount);
        private static List<ProcessFrameData> m_listProcessFrameDataCache = new List<ProcessFrameData>(Config.ModelStateClipFrameCount);
        private static object AnalyseKeyFrameListData(string luaText, ref int index) {
            int endIndex = FindLuaTableEndIndex(luaText, index);
            int tempIndex = FindLuaTableKeyStartIndex(luaText, index, endIndex);
            if (tempIndex == Config.ErrorIndex)
                return default;
            bool isProcessFrame = luaText[tempIndex] == LuaFormat.QuotationPair.start;
            if (isProcessFrame)
                m_listProcessFrameDataCache.Clear();
            else
                m_listKeyFrameDataCache.Clear();
            for (; index < luaText.Length; index++) {
                if (isProcessFrame) {
                    string frameName = ReadLuaTableHashKey(luaText, ref index, endIndex);
                    if (frameName == string.Empty)
                        break;
                    ProcessFrameData data = new ProcessFrameData();
                    data.SetFrameType(frameName);
                    EnterLuaTable(luaText, ref index);
                    data = (ProcessFrameData)AnalyseAnimClipData(luaText, ref index, data);
                    ExitLuaTable(luaText, ref index);
                    m_listProcessFrameDataCache.Add(data);
                }
                else {
                    int frameIndex = ReadLuaTableArrayKey(luaText, ref index, endIndex);
                    if (frameIndex == Config.ErrorIndex)
                        break;
                    KeyFrameData data = new KeyFrameData();
                    data.index = frameIndex;
                    EnterLuaTable(luaText, ref index);
                    data = (KeyFrameData)AnalyseAnimClipData(luaText, ref index, data);
                    ExitLuaTable(luaText, ref index);
                    m_listKeyFrameDataCache.Add(data);
                }
            }
            if (isProcessFrame)
                return m_listProcessFrameDataCache.ToArray();
            return m_listKeyFrameDataCache.ToArray();
        }

        private static object AnalyseKeyFrameData(string luaText, ref int index, ITable data) {
            return ReadLuaTable(luaText, ref index, data);
        }

        private static List<CustomData> m_listCustomDataCache = new List<CustomData>(Config.ModelClipFrameCustomDataCount);
        private static object AnalyseCustomData(string luaText, ref int index, FrameType frameType) {
            m_listCustomDataCache.Clear();
            int endIndex = FindLuaTableEndIndex(luaText, index);
            for (; index < luaText.Length; index++) {
                int customDataIndex = ReadLuaTableArrayKey(luaText, ref index, endIndex);
                if (customDataIndex == Config.ErrorIndex)
                    break;
                CustomData data = new CustomData();
                data.index = customDataIndex;
                EnterLuaTable(luaText, ref index);
                data.data = AnalyseAnimClipData(luaText, ref index, frameType);
                ExitLuaTable(luaText, ref index);
                m_listCustomDataCache.Add(data);
            }
            return m_listCustomDataCache.ToArray();
        }

        private static List<CubeData> m_listCubeDataCache = new List<CubeData>(Config.ModelClipFrameCubeDataCount);
        private static object AnalyseEffectData(string luaText, ref int index, FrameType frameType) {
            switch (frameType) {
                case FrameType.PlayEffect:
                    return ReadLuaTable(luaText, ref index, new EffectData());
                case FrameType.Hit:
                    m_listCubeDataCache.Clear();
                    int endIndex = FindLuaTableEndIndex(luaText, index);
                    for (; index < luaText.Length; index++) {
                        if (ReadLuaTableArrayKey(luaText, ref index, endIndex) == Config.ErrorIndex)
                            break;
                        EnterLuaTable(luaText, ref index);
                        m_listCubeDataCache.Add((CubeData)AnalyseAnimClipData(luaText, ref index));
                        ExitLuaTable(luaText, ref index);
                    }
                    return m_listCubeDataCache.ToArray();
            }
            return null;
        }

        private static object AnalyseCubeData(string luaText, ref int index) {
            return ReadLuaTable(luaText, ref index, new CubeData());
        }

        private static int FindLuaTableKeyStartIndex(string luaText, int index, int endIndex) {
            FilterNotesLine(luaText, ref index);
            PairStringChar symbol = LuaFormat.SquareBracketPair;
            index = luaText.IndexOf(symbol.start, index, StringComparison.Ordinal);
            if (index >= endIndex || index == Config.ErrorIndex)
                return Config.ErrorIndex;
            index += symbol.start.Length;
            return index;
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

        // table unsupport contain keyword notes
        private static ITable ReadLuaTable(string luaText, ref int index, ITable table) {
            int maxIndex = index;
            int endIndex = FindLuaTableEndIndex(luaText, index);
            FieldKeyTable[] array = table.GetFieldKeyTables();
            foreach (FieldKeyTable tableKeyValue in array) {
                int keyIndex = luaText.IndexOf(tableKeyValue.key, index, StringComparison.Ordinal);
                if (keyIndex == Config.ErrorIndex || keyIndex >= endIndex)
                    continue;
                keyIndex += tableKeyValue.KeyLength;
                FilterSpaceSymbol(luaText, ref keyIndex);
                if (luaText[keyIndex] != LuaFormat.EqualSymbol) {
                    PrintErrorWhithLayer("关键帧配置表关键帧 Lua table 配置错误", keyIndex);
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

        private static void SetLuaTableData(string luaText, ref int keyIndex, FieldKeyTable keyValue, ref ITable table) {
            switch (keyValue.type) {
                case LuaFormat.ValueType.String:
                    table.SetFieldKeyTable(keyValue.key, GetLuaTextString(luaText, ref keyIndex));
                    return;
                case LuaFormat.ValueType.Int:
                    table.SetFieldKeyTable(keyValue.key, GetLuaTextInt(luaText, ref keyIndex));
                    return;
                case LuaFormat.ValueType.Number:
                    table.SetFieldKeyTable(keyValue.key, GetLuaTextNumber(luaText, ref keyIndex));
                    return;
                case LuaFormat.ValueType.Reference:
                    table.SetFieldKeyTable(keyValue.key, GetLuaTextReferenceString(luaText, ref keyIndex));
                    return;
                case LuaFormat.ValueType.Table:
                    EnterLuaTable(luaText, ref keyIndex);
                    int endIndex = FindLuaTableEndIndex(luaText, keyIndex);
                    if (!CheckNullTable(luaText, keyIndex, endIndex))
                        table.SetFieldKeyTable(keyValue.key, AnalyseAnimClipData(luaText, ref keyIndex, table));
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

        private static void PrintErrorWhithLayer(string text, int index) {
            Debug.LogError(string.Format("{0} 当前层为 {1}, 索引值为 {2}", text, (AnimClipLuaLayer)m_tableLayer, index));
        }
    }
}