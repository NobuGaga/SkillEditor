using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using Lua;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class LuaAnimClipModel {

        private static int m_curModelIndex;
        private static List<AnimClipData> m_listAnimClip = new List<AnimClipData>(Config.ModelCount);
        public static List<AnimClipData> AnimClipList => m_listAnimClip;

        public static void SetCurrentModelName(string modelName) {
            m_curModelIndex = Config.ErrorIndex;
            for (int index = 0; index < m_listAnimClip.Count; index++)
                if (m_listAnimClip[index].modelName == modelName) {
                    m_curModelIndex = index;
                    break;
                }
            if (m_curModelIndex == Config.ErrorIndex)
                AddNewAnimClipData(modelName);
        }

        private static void AddNewAnimClipData(string modelName) {
            AnimClipData data = new AnimClipData();
            data.modelName = modelName;
            m_listAnimClip.Add(data);
            m_curModelIndex = m_listAnimClip.Count - 1;
        }

        private static AnimClipData CurrentAnimClipData {
            set => m_listAnimClip[m_curModelIndex] = value;
            get {
                if (m_curModelIndex == Config.ErrorIndex)
                    Debug.LogError("AnimClipModel current index is error index");
                return m_listAnimClip[m_curModelIndex];
            }
        }

        private static void ResetModel() {
            m_curModelIndex = Config.ErrorIndex;
            m_listAnimClip.Clear();
        }

        private static List<StateData> m_listStateDataCache = new List<StateData>(2);
        private static void RefreshClipGroupData(StateData[] array) {
            m_listStateDataCache.Clear();
            if (array == null || array.Length == 0)
                return;
            for (int index = 0; index < array.Length; index++)
                m_listStateDataCache.Add(array[index]);
        }

        private static int m_curStateIndex;
        private static StateData CurrentStateData {
            set {
                AnimClipData animClipData = CurrentAnimClipData;
                animClipData.stateList[m_curStateIndex] = value;
                Array.Sort(animClipData.stateList, SortStateList);
                CurrentAnimClipData = animClipData;
            }
            get {
                StateData[] stateList = CurrentAnimClipData.stateList;
                if (stateList == null || stateList.Length == 0 || m_curStateIndex == Config.ErrorIndex)
                    return default;
                return stateList[m_curStateIndex];
            }
        }

        private static int SortStateList(StateData leftData, StateData rightData) =>
            leftData.state.CompareTo(rightData.state);

        public static State CurrentState {
            get {
                StateData[] stateList = CurrentAnimClipData.stateList;
                if (stateList == null || stateList.Length == 0)
                    return State.None;
                return stateList[m_curStateIndex].state;
            }
        }

        public static void SetCurrentStateData(State state) {
            AnimClipData animClipData = CurrentAnimClipData;
            StateData[] dataList = animClipData.stateList;
            if (dataList == null || dataList. Length == 0) {
                StateData newData = default;
                newData.state = state;
                dataList = new StateData[] { newData };
                m_curStateIndex = 0;
                animClipData.stateList = dataList;
                CurrentAnimClipData = animClipData;
                return;
            }
            State lastState = CurrentState;
            if (state == lastState)
                return;
            StateData stateData = CurrentStateData;
            if (stateData.clipList == null || stateData.clipList.Length == 0) {
                stateData.state = state;
                CurrentStateData = stateData;
                return;    
            }
            RefreshClipGroupData(stateData.clipList);
            m_listClipGroupCache.RemoveAt(m_curClipGroupIndex);
            stateData.clipList = m_listClipGroupCache.ToArray();
            CurrentStateData = stateData;
            stateData.state = state;
            m_curStateIndex = Config.ErrorIndex;
            for (int index = 0; index < dataList.Length; index++) {
                if (dataList[index].state == state) {
                    m_curStateIndex = index;
                    break;
                }
            }
            if (m_curStateIndex == Config.ErrorIndex)
            {}
        }

        private static List<ClipGroupData> m_listClipGroupCache = new List<ClipGroupData>(16);
        private static void RefreshClipGroupData(ClipGroupData[] array) {
            m_listClipGroupCache.Clear();
            if (array == null || array.Length == 0)
                return;
            for (int index = 0; index < array.Length; index++)
                m_listClipGroupCache.Add(array[index]);
        }

        private static int m_curClipGroupIndex;
        private static ClipGroupData m_curClipGroupData;
        
        public static FrameData[] FrameList {
            set => m_curClipGroupData.frameList.frameList = value;
            get => m_curClipGroupData.frameList.frameList;
        }

        public static void SetCurrentClipName(string clipName) {
            if (clipName == m_curClipGroupData.clipName)
                return;
            SaveCurrentClipData();
            ResetClip();
            StateData[] stateDataList = CurrentAnimClipData.stateList;
            if (stateDataList == null || stateDataList.Length == 0)
                return;
            for (int stateIndex = 0; stateIndex < stateDataList.Length; stateIndex++) {
                StateData stateData = stateDataList[stateIndex];
                ClipGroupData[] clipList = stateData.clipList;
                if (clipList == null || clipList.Length == 0)
                    continue;
                for (int clipGroupIndex = 0; clipGroupIndex < clipList.Length; clipGroupIndex++) {
                    ClipGroupData data = clipList[clipGroupIndex];
                    if (data.clipName == clipName) {
                        m_curStateIndex = stateIndex;
                        m_curClipGroupIndex = clipGroupIndex;
                        m_curClipGroupData = data;
                        SetFrameList<CubeData>(FrameType.Hit);
                    }
                }
            }
        }

        private static void SaveCurrentClipData() {
            // if (CurrentState == State.None || m_curClipGroupData.IsNullTable())
            //     return;
            // bool isNewData = m_curStateIndex == Config.ErrorIndex && m_curClipIndex == Config.ErrorIndex;
            // if (!isNewData) {
            //     m_curStateData.clipList[m_curClipIndex] = m_curClipData;
            //     m_listAnimClip[m_curModelIndex].stateList[m_curStateIndex] = m_curStateData;
            //     return;
            // }
            // m_listClip.Add(m_curClipData);
            // m_curStateData.clipList = m_listClip.ToArray();
            // int stateIndex = Config.ErrorIndex;
            // for (int index = 0; index < m_listState.Count; index++) {
            //     if (m_listState[index].state == m_curStateData.state){
            //         stateIndex = index;
            //         break;
            //     }
            // }
            // AnimClipData animClipData = CurrentAnimClipData;
            // animClipData.stateList = m_listState.ToArray();
            // m_listAnimClip[m_curModelIndex] = animClipData;
        }

        private static void ResetClip(){
            m_curStateIndex = Config.ErrorIndex;
            m_curClipGroupIndex = Config.ErrorIndex;
            m_curClipGroupData.Clear();
            m_listEffect.Clear();
            m_listCollision.Clear();
        }

        public static void Reset() {
            ResetModel();
            ResetClip();
        }

        public static void AddFrameData() {
            FrameData[] array = FrameList;
            FrameData data = new FrameData();
            if (array == null) {
                data.index = 1;
                array = new FrameData[] { data };
                FrameList = array;
                return;
            }
            data.index = (ushort)array.Length;
            List<FrameData> list = new List<FrameData>(array);
            list.Add(new FrameData());
            FrameList = list.ToArray();
        }

        public static void DeleteFrameData(int index) {
            FrameData[] array = FrameList;
            if (array.Length <= 1)
                FrameList = null;
            else {
                List<FrameData> list = new List<FrameData>(array);
                list.RemoveAt(index);
                for (index = 0; index < list.Count; index++) {
                    FrameData data = list[index];
                    data.index = (ushort)(index + 1);
                    list[index] = data;
                }
                FrameList = list.ToArray();
            }
        }

        private static void SetFrameData(int index, FrameData data, bool isRefresHitFrame) {
            FrameData[] array = FrameList;
            array[index] = data;
            FrameList = array;
            if (isRefresHitFrame)
                SetFrameList<CubeData>(FrameType.Hit);
        }

        public static FrameData GetFrameData(int index) {
            FrameData[] list = FrameList;
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
                CustomDataClearMethod.Invoke(customData, null);
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

        private static List<KeyValuePair<float, EffectData[]>> m_listEffect = new List<KeyValuePair<float, EffectData[]>>();
        public static List<KeyValuePair<float, EffectData[]>> ListEffect => m_listEffect;

        private static List<KeyValuePair<float, CubeData[]>> m_listCollision = new List<KeyValuePair<float, CubeData[]>>();
        public static List<KeyValuePair<float, CubeData[]>> ListCollision => m_listCollision;

        private static void SetFrameList<T>(FrameType frameType) {
            bool isEffect = frameType == FrameType.PlayEffect;
            if (frameType != FrameType.Hit && !isEffect) {
                Debug.LogError("LuaAnimClipModel::SetFrameList frame type error. type" + frameType);
                return;
            }
            List<KeyValuePair<float,T[]>> list;
            if (isEffect)
                list = m_listEffect as List<KeyValuePair<float,T[]>>;
            else
                list = m_listCollision as List<KeyValuePair<float,T[]>>;
            list.Clear();
            if (FrameList == null || FrameList.Length == 0)
                return;
            for (int index = 0; index < FrameList.Length; index++) {
                FrameData frameData = FrameList[index];
                T[] dataList;
                if (isEffect)
                    dataList = frameData.effectFrameData.effectData.dataList as T[];
                else
                    dataList = frameData.hitFrameData.cubeData.dataList as T[];
                if (dataList == null)
                    continue;
                KeyValuePair<float, T[]> timeCubeData = new KeyValuePair<float, T[]>(frameData.time, dataList);
                list.Add(timeCubeData);
            }
            list.Sort(SortFrameListByTime);
        }

        private static int SortFrameListByTime<T>(KeyValuePair<float, T[]> left, KeyValuePair<float, T[]> right) {
            return left.Key.CompareTo(right.Key);
        }

        public static string GetWriteFileString() {
            SaveCurrentClipData();
            return LuaWriter.GetWriteFileString(m_listAnimClip);
        }
    }
}