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
        private static StateData m_curStateData;
        public static State ClipDataState {
            set {
                m_curStateData.state = value;
                m_curClipData.SetPoolType(value);
            }
            get => m_curStateData.state;
        }
        private static List<StateData> m_listState = new List<StateData>(Config.ModelStateCount);

        private static int m_curClipIndex;
        private static ClipData m_curClipData;
        public static ClipData ClipData {
            set => m_curClipData = value;
            get => m_curClipData;
        }
        public static List<ClipData> m_listClip = new List<ClipData>(Config.ModelStateClipCount);

        public static void SetCurrentEditClipName(string clipName) {
            if (clipName != m_curClipData.clipName)
                SaveCurrentClipData();
            ResetClip();
            StateData[] stateList = GetAnimClipData().stateList;
            if (stateList == null) {
                m_curClipData.clipName = clipName;
                return;
            }
            for (int stateIndex = 0; stateIndex < stateList.Length; stateIndex++) {
                StateData stateData = stateList[stateIndex];
                m_listState.Add(stateData);
                if (stateData.clipList == null && stateData.clipList.Length == 0)
                    continue;
                for (int clipIndex = 0; clipIndex < stateData.clipList.Length; clipIndex++) {
                    ClipData clipData = stateData.clipList[clipIndex];
                    m_listClip.Add(clipData);
                    if (clipData.clipName == clipName) {
                        m_curStateIndex = stateIndex;
                        m_curStateData = stateData;
                        m_curClipIndex = clipIndex;
                        m_curClipData = clipData;
                    }
                }
            }
        }

        private static void SaveCurrentClipData() {
            if (m_curStateData.state == State.None || m_curClipData.IsNullTable())
                return;
            bool isNewData = m_curStateIndex == Config.ErrorIndex && m_curClipIndex == Config.ErrorIndex;
            if (!isNewData) {
                m_curStateData.clipList[m_curClipIndex] = m_curClipData;
                m_listAnimClip[m_curModelIndex].stateList[m_curStateIndex] = m_curStateData;
                return;
            }
            m_listClip.Add(m_curClipData);
            m_curStateData.clipList = m_listClip.ToArray();
            int stateIndex = Config.ErrorIndex;
            for (int index = 0; index < m_listState.Count; index++) {
                if (m_listState[index].state == m_curStateData.state){
                    stateIndex = index;
                    break;
                }
            }
            if (stateIndex != Config.ErrorIndex) {
                m_listAnimClip[m_curModelIndex].stateList[stateIndex] = m_curStateData;
                return;
            }
            m_listState.Add(m_curStateData);
            m_listState.Sort(SortStateList);
            AnimClipData animClipData = GetAnimClipData();
            animClipData.stateList = m_listState.ToArray();
            m_listAnimClip[m_curModelIndex] = animClipData;
        }

        private static int SortStateList(StateData leftData, StateData rightData) {
            return leftData.state.CompareTo(rightData.state);
        }

        public static void AddNewKeyFrameData(string key) {
            KeyFrameData[] array = ClipData.GetKeyFrameList(key);
            if (array == null) {
                array = new KeyFrameData[] { new KeyFrameData() };
                m_curClipData.SetTableKeyValue(key, array);
                return;
            }
            List<KeyFrameData> list = new List<KeyFrameData>(array);
            list.Add(new KeyFrameData());
            m_curClipData.SetTableKeyValue(key, list.ToArray());
        }

        public static void SetKeyFrameData(string key, int index, KeyFrameData data) {
            KeyFrameData[] array = ClipData.GetKeyFrameList(key);
            array[index] = data;
            m_curClipData.SetTableKeyValue(key, array);
        }

        public static KeyFrameData GetKeyFrameData(string key, int index) {
            KeyFrameData[] list = ClipData.GetKeyFrameList(key);
            if (list == null && index >= 0 && index < list.Length)
                return default;
            return list[index];
        }

        public static string GetWriteFileString(StringBuilder builder) {
            SaveCurrentClipData();
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
            m_curStateData.Clear();
            m_listState.Clear();
            m_curClipIndex = Config.ErrorIndex;
            m_curClipData.Clear();
            m_listClip.Clear();
        }
    }
}
