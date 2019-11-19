using UnityEngine;
using System.Text;
using System.Collections.Generic;
using Lua;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class LuaAnimClipModel {

        private static List<AnimClipData> m_listAnimClip = new List<AnimClipData>(Config.ModelCount);
        public static List<AnimClipData> AnimClipList => m_listAnimClip;
        private static int m_curModelIndex;

        private static List<KeyValuePair<float, CubeData>> m_listCollision = new List<KeyValuePair<float, CubeData>>();
        public static List<KeyValuePair<float, CubeData>> ListCollision => m_listCollision;

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
            }
            get => m_curStateData.state;
        }
        private static List<StateData> m_listState = new List<StateData>(2);

        private static int m_curClipIndex;
        private static ClipData m_curClipData;
        public static ClipData ClipData {
            set {
                m_curClipData = value;
                SetCollisionList();
            }
            get => m_curClipData;
        }
        public static List<ClipData> m_listClip = new List<ClipData>(16);

        private static List<CustomData> m_listCustomData = new List<CustomData>(Config.ModelClipFrameCustomDataCount);

        private static List<CubeData> m_listCubeData = new List<CubeData>(2);

        public static void SetCurrentEditClipName(string clipName) {
            if (clipName != m_curClipData.clipName)
                SaveCurrentClipData();
            ResetClip();
            m_curClipData.clipName = clipName;
            StateData[] stateList = GetAnimClipData().stateList;
            if (stateList == null)
                return;
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
                        SetCollisionList();
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

        public static void AddNewKeyFrameData() {
            KeyFrameData[] array = ClipData.GetKeyFrameList();
            if (array == null) {
                array = new KeyFrameData[] { new KeyFrameData() };
                m_curClipData.SetFieldValueTableValue(ClipData.Key_KeyFrame, array);
                return;
            }
            List<KeyFrameData> list = new List<KeyFrameData>(array);
            list.Add(new KeyFrameData());
            m_curClipData.SetFieldValueTableValue(ClipData.Key_KeyFrame, list.ToArray());
        }

        public static void AddNewProcessFrameData() {
            ProcessFrameData[] array = ClipData.GetProcessFrameList();
            if (array == null) {
                array = new ProcessFrameData[] { new ProcessFrameData() };
                m_curClipData.SetFieldValueTableValue(ClipData.Key_ProcessFrame, array);
                return;
            }
            List<ProcessFrameData> list = new List<ProcessFrameData>(array);
            list.Add(new ProcessFrameData());
            m_curClipData.SetFieldValueTableValue(ClipData.Key_ProcessFrame, list.ToArray());
        }

        public static void SetKeyFrameData(int index, KeyFrameData data) {
            KeyFrameData[] array = ClipData.GetKeyFrameList();
            array[index] = data;
            m_curClipData.SetFieldValueTableValue(ClipData.Key_KeyFrame, array);
            SetCollisionList();
        }

        public static void SetProcessFrameData(int index, ProcessFrameData data) {
            ProcessFrameData[] array = ClipData.GetProcessFrameList();
            array[index] = data;
            m_curClipData.SetFieldValueTableValue(ClipData.Key_ProcessFrame, array);
        }

        public static KeyFrameData GetKeyFrameData(int index) {
            KeyFrameData[] list = ClipData.GetKeyFrameList();
            if (list == null && index >= 0 && index < list.Length)
                return default;
            return list[index];
        }

        public static ProcessFrameData GetProcessFrameData(int index) {
            ProcessFrameData[] list = ClipData.GetProcessFrameList();
            if (list == null && index >= 0 && index < list.Length)
                return default;
            return list[index];
        }

        private static void SetCollisionList() {
            m_listCollision.Clear();
            if (m_curClipData.keyFrameList == null || m_curClipData.keyFrameList.Length == 0)
                return;
            KeyFrameData[] keyFrameList = m_curClipData.keyFrameList;
            for (int keyFrameDataIndex = 0; keyFrameDataIndex < keyFrameList.Length; keyFrameDataIndex++) {
                KeyFrameData keyFrameData = keyFrameList[keyFrameDataIndex];
                if (keyFrameData.dataList == null || keyFrameData.dataList.Length == 0)
                    continue;
                for (int customDataIndex = 0; customDataIndex < keyFrameData.dataList.Length; customDataIndex++) {
                    CustomData customData = keyFrameData.dataList[customDataIndex];
                    if (!(customData.data is CubeData[]))
                        break;
                    CubeData[] cubeDataList = customData.data as CubeData[];
                    for (int cubeDataIndex = 0; cubeDataIndex < cubeDataList.Length; cubeDataIndex++) {
                        CubeData cubeData = cubeDataList[cubeDataIndex];
                        float time = keyFrameData.time;
                        KeyValuePair<float, CubeData> timeCubeData = new KeyValuePair<float, CubeData>(time, cubeData);
                        m_listCollision.Add(timeCubeData);
                    }
                }
            }
            m_listCollision.Sort(SortCollisionList);
        }

        private static int SortCollisionList(KeyValuePair<float, CubeData> left, KeyValuePair<float, CubeData> right) {
            return left.Key.CompareTo(right.Key);
        }

        public static void AddNewEffectData(int index) {
            ProcessFrameData[] arrayProcessFrameData = ClipData.GetProcessFrameList();
            ProcessFrameData processFrameData = arrayProcessFrameData[index];
            CustomData customData = new CustomData();
            customData.data = new EffectData();
            if (processFrameData.dataList == null) {
                customData.index = 1;
                processFrameData.dataList = new CustomData[] { customData };
            }
            else {
                m_listCustomData.Clear();
                CustomData[] arrayCustomData = processFrameData.dataList as CustomData[];
                for (int customDataIndex = 0; customDataIndex < arrayCustomData.Length; customDataIndex++)
                    m_listCustomData.Add(arrayCustomData[customDataIndex]);
                customData.index = arrayCustomData.Length + 1;
                m_listCustomData.Add(customData);
                processFrameData.dataList = m_listCustomData.ToArray();
            }
            arrayProcessFrameData[index] = processFrameData;
        }

        public static void AddNewCubeData(int index) {
            KeyFrameData[] arrayKeyFrameData = ClipData.GetKeyFrameList();
            KeyFrameData keyFrameData = arrayKeyFrameData[index];
            if (keyFrameData.dataList == null)
                keyFrameData.dataList = new CustomData[] { new CustomData() };
            CustomData customData = keyFrameData.dataList[0];
            customData.index = CustomData.CubeDataIndex;
            if (customData.data == null)
                customData.data = new CubeData[] { new CubeData() };
            else {
                m_listCubeData.Clear();
                CubeData[] arrayCubeData = customData.data as CubeData[];
                for (int cubeDataIndex = 0; cubeDataIndex < arrayCubeData.Length; cubeDataIndex++)
                    m_listCubeData.Add(arrayCubeData[cubeDataIndex]);
                m_listCubeData.Add(new CubeData());
                customData.data = m_listCubeData.ToArray();
            }
            keyFrameData.dataList[0] = customData;
            arrayKeyFrameData[index] = keyFrameData;
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
            m_listCollision.Clear();
        }
    }
}