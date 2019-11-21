using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Lua;
using Lua.AnimClipData;

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
                    if (clipData.clipName != clipName)
                        continue;
                    m_curStateIndex = stateIndex;
                    m_curStateData = stateData;
                    m_curClipIndex = clipIndex;
                    m_curClipData = clipData;
                    SetCollisionList();
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

        public static void AddFrameData() {
            FrameData[] array = ClipData.frameList;
            FrameData data = new FrameData();
            if (array == null) {
                data.index = 1;
                array = new FrameData[] { data };
                m_curClipData.frameList = array;
                return;
            }
            data.index = (ushort)array.Length;
            List<FrameData> list = new List<FrameData>(array);
            list.Add(new FrameData());
            m_curClipData.frameList = list.ToArray();
        }

        public static void DeleteFrameData(int index) {
            FrameData[] array = ClipData.frameList;
            if (array.Length <= 1)
                m_curClipData.frameList = null;
            else {
                List<FrameData> list = new List<FrameData>(array);
                list.RemoveAt(index);
                for (index = 0; index < list.Count; index++) {
                    FrameData data = list[index];
                    data.index = (ushort)(index + 1);
                    list[index] = data;
                }
                m_curClipData.frameList = list.ToArray();
            }
        }

        private static void SetFrameData(int index, FrameData data, bool isRefresHitFrame) {
            FrameData[] array = ClipData.frameList;
            array[index] = data;
            m_curClipData.frameList = array;
            if (isRefresHitFrame)
                SetCollisionList();
        }

        public static FrameData GetFrameData(int index) {
            FrameData[] list = ClipData.frameList;
            if (list == null && index >= 0 && index < list.Length)
                return default;
            return list[index];
        }

        public static void SetFrameDataTime(int index, float time) {
            FrameData data = GetFrameData(index);
            data.time = time;
            SetFrameData(index, data, false);
        }

        public static void AddPriorityFrameData(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            PriorityFrameData priorityFrameData = (PriorityFrameData)frameData.GetFieldValueTableValue(frameType.ToString());
            priorityFrameData.priority = 1;
            frameData.SetFieldValueTableValue(frameType.ToString(), priorityFrameData);
            SetFrameData(index, frameData, false);
        }

        public static void SetFramePriorityData(int index, FrameType frameType, ushort priority) {
            FrameData frameData = GetFrameData(index);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            table.SetFieldValueTableValue(PriorityFrameData.Key_Priority, priority);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(index, frameData, false);
        }

        #region Reflection Method And Data

        private static MethodInfo CustomDataSetTableListMethod;
        private static MethodInfo CustomDataClearMethod;
        private static MethodInfo CustomDataStaticCacheListAddMethod;
        private static MethodInfo CustomDataSetTableListDataMethod;
        private static object[] m_staticCacheListAddArg = new object[1];
        private static void SetStaticCacheListAddArg(object value) => m_staticCacheListAddArg[0] = value;
        private static object[] GetStaticCacheListAddArg() => m_staticCacheListAddArg;
        private static object[] m_setTableListDataArg = new object[2];
        private static void SetSetTableListDataArg(params object[] args) {
            for (ushort index = 0; index < m_setTableListDataArg.Length; index++)
                if (args == null || index >= args.Length)
                     m_setTableListDataArg[index] = null;
                else
                     m_setTableListDataArg[index] = args[index];
        }
        private static object[] GetSetTableListDataArg() => m_setTableListDataArg;
        private static object[] m_customDataCache = new object[3];
        private const ushort CustomDataIndex = 0;
        private const ushort CustomDataStaticListIndex = 1;
        private const ushort CustomDatalistIndex = 2;
        private static object GetCustomData() => m_customDataCache[CustomDataIndex];
        private static object GetCustomDataStaticList() => m_customDataCache[CustomDataStaticListIndex];
        private static object GetCustomDataList() => m_customDataCache[CustomDatalistIndex];
        private static Type ListType;
        private static string Key_Data = null;
        
        private static void SetCustomDataMethodAndData(IFieldValueTable table) {
            if (Key_Data == null) {
                CustomData<EffectData> defaultCustomData = default;
                Key_Data = defaultCustomData.GetKey();
            }
            m_customDataCache[CustomDataIndex] = table.GetFieldValueTableValue(Key_Data);
            object customData = m_customDataCache[CustomDataIndex];
            Type customDataType = customData.GetType();
            CustomDataClearMethod = customDataType.GetMethod("Clear");
            CustomDataSetTableListMethod = customDataType.GetMethod("SetTableList");
            CustomDataSetTableListDataMethod = customDataType.GetMethod("SetTableListData");

            m_customDataCache[CustomDataStaticListIndex] = customDataType.GetMethod("GetStaticCacheList").Invoke(customData, null);
            object staticList = m_customDataCache[CustomDataStaticListIndex];
            Type staticListType = staticList.GetType();
            staticListType.GetMethod("Clear").Invoke(staticList, null);
            CustomDataStaticCacheListAddMethod = staticListType.GetMethod("Add");

            m_customDataCache[CustomDatalistIndex] = customDataType.GetMethod("GetTableList").Invoke(customData, null);
            ListType = customDataType.GetMethod("GetTableListType").Invoke(customData, null) as Type;
        }
        #endregion

        public static void AddNewCustomSubData(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            SetCustomDataMethodAndData(table);
            object customData = GetCustomData();
            object staticList = GetCustomDataStaticList();
            object dataList = GetCustomDataList();
            ITable data = (ITable)Activator.CreateInstance(ListType);
            if (dataList == null) {
                data.SetKey(1);
                SetFramePriorityData(index, frameType, 1);
            }
            else {
                Array array = dataList as Array;
                for (int arrayIndex = 0; arrayIndex < array.Length; arrayIndex++) {
                    SetStaticCacheListAddArg(array.GetValue(arrayIndex));
                    CustomDataStaticCacheListAddMethod.Invoke(staticList, GetStaticCacheListAddArg());
                }
                data.SetKey(array.Length);
            }
            SetStaticCacheListAddArg(data);
            CustomDataStaticCacheListAddMethod.Invoke(staticList, GetStaticCacheListAddArg());
            customData = CustomDataSetTableListMethod.Invoke(customData, null);
            table.SetFieldValueTableValue(Key_Data, customData);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(index, frameData, false);
        }

        public static void DeleteCustomSubData(int frameIndex, int deleteIndex, FrameType frameType) {
            FrameData frameData = GetFrameData(frameIndex);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            SetCustomDataMethodAndData(table);
            object customData = GetCustomData();
            object staticList = GetCustomDataStaticList();
            Array dataList = GetCustomDataList() as Array;
            if (dataList.Length <= 1)
                customData = CustomDataClearMethod.Invoke(customData, null);
            else {
                for (int index = 0; index < dataList.Length; index++) {
                    object key;
                    if (index < deleteIndex)
                        key = index + 1;
                    else if (index == deleteIndex)
                        continue;
                    else
                        key = index;
                    ITable subTable = (ITable)dataList.GetValue(index);
                    subTable.SetKey(key);
                    SetStaticCacheListAddArg(subTable);
                    CustomDataStaticCacheListAddMethod.Invoke(staticList, GetStaticCacheListAddArg());
                }
                customData = CustomDataSetTableListMethod.Invoke(customData, null);
            }
            table.SetFieldValueTableValue(Key_Data, customData);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(frameIndex, frameData, frameType == FrameType.Hit);
        }

        public static void SetCustomeSubData(int frameIndex, ITable data, FrameType frameType) {
            FrameData frameData = GetFrameData(frameIndex);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            SetCustomDataMethodAndData(table);
            string key = data.GetKey();
            if (!ushort.TryParse(key, out ushort dataIndex)) {
                Debug.LogError("LuaAnimClipModel::SetCustomeSubData try parse key error");
                return;
            }
            dataIndex--;
            object customData = GetCustomData();
            SetSetTableListDataArg(dataIndex, data);
            CustomDataSetTableListDataMethod.Invoke(customData, GetSetTableListDataArg());
            table.SetFieldValueTableValue(Key_Data, customData);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(frameIndex, frameData, false);
        }

        private static List<KeyValuePair<float, CubeData[]>> m_listCollision = new List<KeyValuePair<float, CubeData[]>>();
        public static List<KeyValuePair<float, CubeData[]>> ListCollision => m_listCollision;

        private static void SetCollisionList() {
            m_listCollision.Clear();
            if (m_curClipData.frameList == null || m_curClipData.frameList.Length == 0)
                return;
            FrameData[] frameList = m_curClipData.frameList;
            for (int index = 0; index < frameList.Length; index++) {
                FrameData frameData = frameList[index];
                CubeData[] dataList = frameData.hitFrameData.cubeData.dataList;
                if (dataList == null || dataList.Length == 0)
                    continue;
                float time = frameData.time;
                KeyValuePair<float, CubeData[]> timeCubeData = new KeyValuePair<float, CubeData[]>(time, dataList);
                m_listCollision.Add(timeCubeData);
            }
            m_listCollision.Sort(SortCollisionList);
        }

        private static int SortCollisionList(KeyValuePair<float, CubeData[]> left, KeyValuePair<float, CubeData[]> right) {
            return left.Key.CompareTo(right.Key);
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