using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using StringComparison = System.StringComparison;
using SkillEditor.Structure;

namespace SkillEditor {

    internal static class LuaReader {

        private static string m_lastPath;

        private static string m_headText;
        public static string HeadText => m_headText;

        // table layer
        private static int m_tableCount = 0;

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
                char curChar = luaText[index];
                if (curChar == LuaFormat.NotesSymbolStart)
                    AnalyseNotesLine(luaText, ref index);
                else if (curChar == LuaFormat.CurlyBracesPair.start) {
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

        private static void AnalyseNotesLine(string luaText, ref int index) {
            if (!CheckNextIndexSymbol(luaText, index, LuaFormat.NotesSymbolStart))
                return;
            for (; index < luaText.Length; index++)
                if (luaText[index] == LuaFormat.NotesLinePair.end)
                    break;
        }

        private static void AnalyseKeyFrameData(string luaText, ref int index, object data) {
            switch((KeyFrameLuaLayer)m_tableCount) {
                case KeyFrameLuaLayer.Model:
                    AnalyseModelData(luaText, ref index, data as List<KeyFrameData>);
                    return;
                case KeyFrameLuaLayer.Clip:
                    AnalyseClipData(luaText, ref index, data as List<SkillClipData>);
                    return;
                case KeyFrameLuaLayer.Frame:
                    AnalyseFrameData(luaText, ref index, data as List<FrameData>);
                    return;
                case KeyFrameLuaLayer.Shell:

                    return;
                case KeyFrameLuaLayer.Effect:

                    return;
                case KeyFrameLuaLayer.Rect:

                    return;
            }
        }

        private static List<SkillClipData> m_listClipCache = new List<SkillClipData>(Config.ModelClipCount);
        private static void AnalyseModelData(string luaText, ref int index, List<KeyFrameData> list) {
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar != LuaFormat.SquareBracketPair.start)
                    continue;
                if (!CheckNextIndexSymbol(luaText, index, LuaFormat.QuotationPair.start)) {
                    Debug.LogError("关键帧配置表模型名字配置错误");
                    return;
                }
                index++;
                KeyFrameData data = new KeyFrameData();
                data.modelName = GetLuaTextString(luaText, ref index);
                FindLuaTableStartIndex(luaText, ref index);
                m_listClipCache.Clear();
                AnalyseKeyFrameData(luaText, ref index, m_listClipCache);
                data.clipDatas = m_listClipCache.ToArray();
                list.Add(data);
            }
        }

        private static List<FrameData> m_listFrameCache = new List<FrameData>(Config.ModelClipKeyFrameCount);
        private static void AnalyseClipData(string luaText, ref int index, List<SkillClipData> list) {
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar != LuaFormat.SquareBracketPair.start)
                    continue;
                if (!CheckNextIndexSymbol(luaText, index, LuaFormat.QuotationPair.start)) {
                    Debug.LogError("关键帧配置表模型动画名字配置错误");
                    return;
                }
                index++;
                SkillClipData data = new SkillClipData();
                data.clipName = GetLuaTextString(luaText, ref index);
                FindLuaTableStartIndex(luaText, ref index);
                m_listFrameCache.Clear();
                AnalyseKeyFrameData(luaText, ref index, m_listFrameCache);
                data.frameDatas = m_listFrameCache.ToArray();
                list.Add(data);
            }
        }

        private static List<CustomData> m_listCustomDataCache = new List<CustomData>(Config.ModelClipFrameCustomDataCount);
        private static void AnalyseFrameData(string luaText, ref int index, List<FrameData> list) {
            for (; index < luaText.Length; index++) {
                char curChar = luaText[index];
                if (curChar != LuaFormat.SquareBracketPair.start)
                    continue;
                index++;
                int frameIndex = GetLuaTextInt(luaText, ref index);
                index++;
                FrameData data = new FrameData();
                data.frameIndex = frameIndex;
                FindLuaTableStartIndex(luaText, ref index);
                m_listCustomDataCache.Clear();
                AnalyseKeyFrameData(luaText, ref index, m_listCustomDataCache);
                data.customData = m_listCustomDataCache.ToArray();
                list.Add(data);
            }
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
            int frameIndex;
            if (!int.TryParse(intString, out frameIndex))
                Debug.LogError("关键帧配置表模型动画关键帧索引配置错误");
            return frameIndex;
        }

        private static void FindLuaTableStartIndex(string luaText, ref int index) {
            for (; index < luaText.Length; index++)
                if (luaText[index] == LuaFormat.CurlyBracesPair.start) {
                    m_tableCount++;
                    index++;
                    break;
                }

            if (index == luaText.Length)
                switch((KeyFrameLuaLayer)m_tableCount) {
                    case KeyFrameLuaLayer.Model:
                        Debug.LogError("没有模型层数据");
                        return;
                    case KeyFrameLuaLayer.Clip:
                        Debug.LogError("没有模型动画层数据");
                        return;
                    case KeyFrameLuaLayer.Frame:

                        return;
                    case KeyFrameLuaLayer.Shell:

                        return;
                    case KeyFrameLuaLayer.Effect:

                        return;
                    case KeyFrameLuaLayer.Rect:

                        return;
                }
        }

        private static void Reset() {
            m_headText = string.Empty;
        }
    }
}