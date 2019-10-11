using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using StringComparison = System.StringComparison;
using SkillEditor.Structure;

namespace SkillEditor {

    using LuaTableKeyValue = LuaFormat.LuaTableKeyValue;

    internal static class LuaReader {

        private static string m_lastPath;

        private static string m_headText;
        public static string HeadText => m_headText;

        // table layer
        private static int m_tableCount;

        public static List<KeyFrameData> Read(string path) {
            if (m_lastPath == path)
                return null;
            if (!File.Exists(path)) {
                Debug.LogError(path + " is not exit");
                return null;
            }
            m_lastPath = path;
            Reset();
            string luaText = File.ReadAllText(path);
            return AnalyseLuaText(luaText);
        }

        private static List<KeyFrameData> AnalyseLuaText(string luaText) {
            StringBuilder headBuilder = new StringBuilder(Config.LuaFileHeadLength);
            bool isHeadPart = true;
            int index = luaText.IndexOf(Config.LuaFileHeadStart, StringComparison.Ordinal);
            if (index == Config.ErrorIndex)
                return null;
            List<KeyFrameData> list = new List<KeyFrameData>(Config.ModelCount);
            for (; index < luaText.Length; index++) {
                FilterNotesLine(luaText, ref index);
                char curChar = luaText[index];
                if (curChar == LuaFormat.CurlyBracesPair.start) {
                    m_tableCount++;
                    AnalyseKeyFrameData(luaText, ref index, list);
                    isHeadPart = false;
                }
                else if (curChar == LuaFormat.CurlyBracesPair.end) {
                    m_tableCount--;
                }
                else if (isHeadPart)
                    headBuilder.Append(curChar);

                if (m_tableCount <= 0)
                    break;
            }
            m_headText = headBuilder.ToString();
            return list;
        }

        private static object AnalyseKeyFrameData(string luaText, ref int index, object data) {
            switch((KeyFrameLuaLayer)m_tableCount) {
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
                    AnalyseCustomData(luaText, ref index, data as List<CustomData>);
                    break;
                case KeyFrameLuaLayer.Effect:
                    EFrameType frameType = (EFrameType)data;
                    switch (frameType) {
                        case EFrameType.Grab:
                        case EFrameType.Ungrab:
                            return AnalyseEffectData(luaText, ref index, frameType);
                        case EFrameType.Collision:
                            break;
                        case EFrameType.Harm:
                            return AnalyseRectData(luaText, ref index);
                    }
                    break;
                case KeyFrameLuaLayer.Rect:

                    break;
            }
            return null;
        }

        private static List<SkillClipData> m_listClipCache = new List<SkillClipData>(Config.ModelClipCount);
        private static void AnalyseModelData(string luaText, ref int index, List<KeyFrameData> list) {
            for (; index < luaText.Length; index++) {
                KeyFrameData data = new KeyFrameData();
                data.modelName = ReadLuaTableHashKey(luaText, ref index);
                m_listClipCache.Clear();
                AnalyseKeyFrameData(luaText, ref index, m_listClipCache);
                data.clipDatas = m_listClipCache.ToArray();
                list.Add(data);
            }
        }

        private static List<FrameData> m_listFrameCache = new List<FrameData>(Config.ModelClipKeyFrameCount);
        private static void AnalyseClipData(string luaText, ref int index, List<SkillClipData> list) {
            for (; index < luaText.Length; index++) {
                SkillClipData data = new SkillClipData();
                data.clipName = ReadLuaTableHashKey(luaText, ref index);
                m_listFrameCache.Clear();
                AnalyseKeyFrameData(luaText, ref index, m_listFrameCache);
                data.frameDatas = m_listFrameCache.ToArray();
                list.Add(data);
            }
        }

        private static void AnalyseFrameIndexData(string luaText, ref int index, List<FrameData> list) {
            for (; index < luaText.Length; index++) {
                int frameIndex = ReadLuaTableArray(luaText, ref index);
                if (frameIndex == Config.ErrorIndex)
                    continue;
                FrameData data = new FrameData();
                data.frameIndex = frameIndex;
                data = (FrameData)AnalyseKeyFrameData(luaText, ref index, data);
                list.Add(data);
            }
        }

        private static List<CustomData> m_listCustomDataCache = new List<CustomData>(Config.ModelClipFrameCustomDataCount);
        private static FrameData AnalyseFrameData(string luaText, ref int index, FrameData frameData) {
            return (FrameData)ReadLuaTable(luaText, ref index, frameData, m_listCustomDataCache);
        }

        private static void AnalyseCustomData(string luaText, ref int index, List<CustomData> list) {
            for (; index < luaText.Length; index++) {
                int frameTypeInt = ReadLuaTableArray(luaText, ref index);
                if (frameTypeInt == Config.ErrorIndex)
                    continue;
                EFrameType frameType = (EFrameType)frameTypeInt;
                CustomData data = new CustomData();
                data.frameType = frameType;
                FindLuaTableStartIndex(luaText, ref index);
                data.data = AnalyseKeyFrameData(luaText, ref index, frameType);
                list.Add(data);
            }
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
            if (table == null) {
                Debug.LogError("LuaReader::AnalyseEffectData Frame Type Init Error");
                return null;
            }
            return ReadLuaTable(luaText, ref index, table);
        }


        private static List<HarmData> m_listHarmDataCache = new List<HarmData>(Config.ModelClipFrameRectDataCount);
        private static HarmData[] AnalyseRectData(string luaText, ref int index) {
            m_listHarmDataCache.Clear();
            for (; index < luaText.Length; index++) {
                if (ReadLuaTableArray(luaText, ref index) == Config.ErrorIndex)
                    continue;
                AnalyseKeyFrameData(luaText, ref index, m_listHarmDataCache);
            }
            return m_listHarmDataCache.ToArray();
        }

        private static int ReadLuaTableArray(string luaText, ref int index) {
            FilterNotesLine(luaText, ref index);
            char curChar = luaText[index];
            if (curChar != LuaFormat.SquareBracketPair.start)
                return Config.ErrorIndex;
            index++;
            FilterSpaceSymbol(luaText, ref index);
            int resultIndex = GetLuaTextInt(luaText, ref index);
            FilterSpaceSymbol(luaText, ref index);
            if (luaText[index] != LuaFormat.SquareBracketPair.end)
                Debug.LogError("关键帧配置表索引配置错误");
            index++;
            FindLuaTableStartIndex(luaText, ref index);
            return resultIndex;
        }

        private static string ReadLuaTableHashKey(string luaText, ref int index) {
            FilterNotesLine(luaText, ref index);
            char curChar = luaText[index];
            if (curChar != LuaFormat.SquareBracketPair.start)
                return string.Empty;
            if (!CheckNextIndexSymbol(luaText, index, LuaFormat.QuotationPair.start)) {
                Debug.LogError("关键帧配置表模型名字配置错误");
                return string.Empty;
            }
            index++;
            string key = GetLuaTextString(luaText, ref index);
            FindLuaTableStartIndex(luaText, ref index);
            return key;
        }

        private static ITable ReadLuaTable(string luaText, ref int index, ITable table, object data = null) {
            int maxIndex = index;
            LuaTableKeyValue[] array = table.GetTableKeyValueList();
            foreach (LuaTableKeyValue tableKeyValue in array) {
                int keyIndex = luaText.IndexOf(tableKeyValue.key, index, StringComparison.Ordinal);
                if (keyIndex == Config.ErrorIndex)
                    continue;
                keyIndex += tableKeyValue.KeyLength;
                FilterSpaceSymbol(luaText, ref keyIndex);
                if (luaText[keyIndex] == LuaFormat.EqualSymbol) {
                    Debug.LogError("关键帧配置表关键帧 Lua table 配置错误");
                    break;
                }
                keyIndex++;
                FilterSpaceSymbol(luaText, ref keyIndex);
                SetLuaTableData(luaText, ref keyIndex, tableKeyValue, ref table, data);
                FilterSpaceSymbol(luaText, ref keyIndex);
                if (luaText[index] != LuaFormat.CommaSymbol) {
                    Debug.LogError("关键帧配置表关键帧 lua table 缺少逗号");
                    break;
                }
                if (maxIndex < keyIndex)
                    maxIndex = keyIndex;
            }
            index = maxIndex;
            FindLuaTableEndIndex(luaText, ref index);
            return table; 
        }

        private static void SetLuaTableData(string luaText, ref int keyIndex, LuaTableKeyValue keyValue, ref ITable table, object data) {
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
                    if (data == null)
                        break;
                    FindLuaTableStartIndex(luaText, ref keyIndex);
                    AnalyseKeyFrameData(luaText, ref keyIndex, data);
                    table.SetTableKeyValue(keyValue.key, data);
                    return;
            }
        }

        private static void FilterNotesLine(string luaText, ref int index) {
            while (luaText[index] == LuaFormat.NotesSymbolStart && index < luaText.Length)
                if (!FilterOneNotesLine(luaText, ref index))
                    index++;
        }

        private static bool FilterOneNotesLine(string luaText, ref int index) {
            if (!CheckNextIndexSymbol(luaText, index, LuaFormat.NotesSymbolStart))
                return false;
            for (; index < luaText.Length; index++)
                if (luaText[index] == LuaFormat.NotesLinePair.end)
                    break;
            return true;
        }

        private static void FilterSpaceSymbol(string luaText, ref int index) {
            for (; index < luaText.Length; index++)
                if (luaText[index] == LuaFormat.SpaceSymbol)
                    continue;
        }

        private static bool CheckNextIndexSymbol(string luaText, int index, char symbol) {
            int nextIndex = index + 1;
            return nextIndex < luaText.Length && luaText[nextIndex] == symbol;
        }

        private static string GetLuaTextString(string luaText, ref int index) {
            int startIndex = index;
            for (; index < luaText.Length; index++)
                if (CheckNextIndexSymbol(luaText, index, LuaFormat.QuotationPair.end))
                    break;
            return luaText.Substring(startIndex, index - 1);
        }

        private static int GetLuaTextInt(string luaText, ref int index) {
            int startIndex = index;
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar < LuaFormat.IntMin || curChar > LuaFormat.IntMax)
                    break;
            }
            string intString = luaText.Substring(startIndex, index - 1);
            if (!int.TryParse(intString, out int intData))
                Debug.LogError("关键帧配置表整型配置错误");
            return intData;
        }

        private static float GetLuaTextNumber(string luaText, ref int index) {
            int startIndex = index;
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if ((curChar < LuaFormat.IntMin || curChar > LuaFormat.IntMax) && curChar != LuaFormat.NumberPoint)
                    break;
            }
            string numberString = luaText.Substring(startIndex, index - 1);
            if (!float.TryParse(numberString, out float numberData))
                Debug.LogError("关键帧配置表浮点型配置错误");
            return numberData;
        }

        private static void FindLuaTableStartIndex(string luaText, ref int index) {
            for (; index < luaText.Length; index++)
                if (luaText[index] == LuaFormat.CurlyBracesPair.start) {
                    m_tableCount++;
                    index++;
                    break;
                }

            if (index >= luaText.Length)
                switch((KeyFrameLuaLayer)m_tableCount) {
                    case KeyFrameLuaLayer.Model:
                        Debug.LogError("关键帧配置表没有模型层数据");
                        return;
                    case KeyFrameLuaLayer.Clip:
                        Debug.LogError("关键帧配置表没有模型动画层数据");
                        return;
                    case KeyFrameLuaLayer.FrameIndex:
                        Debug.LogError("关键帧配置表没有模型动画层帧索引数据");
                        return;
                    case KeyFrameLuaLayer.FrameData:
                        Debug.LogError("关键帧配置表没有模型动画层帧数据");
                        return;
                }
        }

        private static void FindLuaTableEndIndex(string luaText, ref int index) {
            for (; index < luaText.Length; index++) {
                if (luaText[index] != LuaFormat.CurlyBracesPair.end)
                    continue;
                m_tableCount--;
                FilterSpaceSymbol(luaText, ref index);
                if (luaText[index] == LuaFormat.CommaSymbol)
                    index++;
                break;
            }
            
            if (index >= luaText.Length)
                Debug.LogError(string.Format("关键帧配置表结尾缺少{0}个 table 结束符 }", m_tableCount));
        }

        private static void Reset() {
            m_headText = string.Empty;
        }
    }
}
