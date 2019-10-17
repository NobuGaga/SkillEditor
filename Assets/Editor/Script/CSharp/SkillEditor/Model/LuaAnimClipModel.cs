using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor.LuaStructure;

namespace SkillEditor {

    internal static class LuaAnimClipModel {

        private static List<AnimClipData> m_listAnimClip = new List<AnimClipData>(Config.ModelCount);
        public static List<AnimClipData> AnimClipList => m_listAnimClip;
        private static int m_curModelIndex;

        public static void SetCurrentEditModelName(string modelName) {
            ModelName = modelName;
            if (m_curModelIndex == Config.ErrorIndex)
                AddNewAnimClipData(modelName);
        }

        public static string ModelName {
            private set {
                m_curModelIndex = Config.ErrorIndex;
                for (int index = 0; index < m_listAnimClip.Count; index++)
                    if (m_listAnimClip[index].modelName == value)
                        m_curModelIndex = index;
            }
            get {
                return GetAnimClipData().modelName;
            }
        }

        private static void AddNewAnimClipData(string modelName) {
            AnimClipData data = new AnimClipData();
            data.modelName = modelName;
            m_listAnimClip.Add(data);
            m_curModelIndex = m_listAnimClip.Count - 1;
        }

        private static AnimClipData GetAnimClipData() {
            if (m_curModelIndex == Config.ErrorIndex)
                Debug.LogError("AnimClipModel current index is error index");
            return m_listAnimClip[m_curModelIndex];
        }

        private static int m_curStateIndex;
        private static State m_curClipDataState;
        public static State ClipDataState => m_curClipDataState;

        private static int m_curClipIndex;
        private static ClipData m_curClipData;

        public static void SetCurrentEditClipName(string clipName) {
            if (clipName != m_curClipData.clipName)
                SetCurrentClipData();
            ResetClip();
            StateData[] stateList = GetAnimClipData().stateList;
            if (stateList == null) {
                AddNewClipData();
                return;
            }
            bool isExitClipData = false;
            for (int stateIndex = 0; stateIndex < stateList.Length; stateIndex++) {
                StateData stateData = stateList[stateIndex];
                if (stateData.clipList == null && stateData.clipList.Length == 0)
                    continue;
                for (int clipIndex = 0; clipIndex < stateData.clipList.Length; clipIndex++) {
                    ClipData clipData = stateData.clipList[clipIndex];
                    if (clipData.clipName != clipName)
                        continue;
                    m_curStateIndex = stateIndex;
                    m_curClipDataState = stateData.state;
                    m_curClipIndex = clipIndex;
                    m_curClipData = clipData;
                    isExitClipData = true;
                    break;
                }
                if (isExitClipData)
                    break;
            }
        }

        private static void AddNewClipData() {

        }

        private static void SetCurrentClipData() {
            if (m_curClipData.IsNullTable())
                return;
        }

        public static string GetWriteFileString(StringBuilder builder) {
            builder.Append(LuaFormat.CurlyBracesPair.start);
            if (m_listAnimClip != null && m_listAnimClip.Count != 0) {
                builder.Append(LuaFormat.LineSymbol);
                for (int index = 0; index < m_listAnimClip.Count; index++)
                    builder.Append(m_listAnimClip[index].ToString());
            }
            builder.Append(LuaFormat.CurlyBracesPair.end);
            return builder.ToString();
        }

        public static void Reset() {
            ResetModel();
            ResetClip();
        }

        private static void ResetModel() {
            m_curModelIndex = Config.ErrorIndex;
            m_listAnimClip.Clear();
        }

        private static void ResetClip(){
            m_curStateIndex = Config.ErrorIndex;
            m_curClipIndex = Config.ErrorIndex;
            m_curClipData.Clear();
        }
    }
}