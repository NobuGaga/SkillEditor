﻿using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using Lua;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class LuaAnimClipModel {

        private static int m_curModelIndex;
        private static List<AnimClipData> m_listAnimClip = new List<AnimClipData>();
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
                    return default;
                return m_listAnimClip[m_curModelIndex];
            }
        }

        private static void ResetModel() {
            m_curModelIndex = Config.ErrorIndex;
            m_listAnimClip.Clear();
        }

        private static List<StateData> m_listStateDataCache = new List<StateData>(2);
        private static void SetStateDataListCache(StateData[] array) {
            m_listStateDataCache.Clear();
            if (array == null || array.Length == 0)
                return;
            for (int index = 0; index < array.Length; index++)
                m_listStateDataCache.Add(array[index]);
        }

        private static int m_curStateDataIndex;
        private static StateData CurrentStateData {
            set {
                AnimClipData data = CurrentAnimClipData;
                data.stateList[m_curStateDataIndex] = value;
                Array.Sort(data.stateList, SortStateData);
                CurrentAnimClipData = data;
            }
            get {
                StateData[] stateList = CurrentAnimClipData.stateList;
                if (stateList == null || stateList.Length == 0 || m_curStateDataIndex == Config.ErrorIndex)
                    return default;
                return stateList[m_curStateDataIndex];
            }
        }

        private static int SortStateData(StateData leftData, StateData rightData) =>
            leftData.state.CompareTo(rightData.state);

        public static State CurrentState => CurrentStateData.state;
        public static string[] GetClipGropuIDList(out int[] indexList) => CurrentStateData.GetClipGropuIDList(out indexList, m_lastClipName);

        public static void SetCurrentState(State state) {
            int stateDataIndex = FindStateDataIndex(state);
            bool isNotExitState = stateDataIndex == Config.ErrorIndex;
            if (m_curStateDataIndex == Config.ErrorIndex && isNotExitState) {
                AddNewStateData(state);
                return;
            }
            if (state == CurrentState)
                return;
            StateData lastStateData = CurrentStateData;
            if (!lastStateData.IsNullTable()) {
                SetClipGroupDataListCache(lastStateData.clipList);
                lastStateData.clipList = m_listClipGroupDataCache.ToArray();
                CurrentStateData = lastStateData;
            }
            if (isNotExitState) {
                AddNewStateData(state);
                return;
            }
            m_curStateDataIndex = stateDataIndex;
            StateData curStateData = CurrentStateData;
            m_curClipGroupDataIndex = FindClipGroupDataIndex();
        }

        private static void AddNewStateData(State state) {
            AnimClipData animClipData = CurrentAnimClipData;
            StateData[] dataList = animClipData.stateList;
            StateData data = default;
            data.state = state;
            if (dataList == null || dataList. Length == 0) {
                dataList = new StateData[] { data };
                m_curStateDataIndex = 0;
            }
            else {
                SetStateDataListCache(dataList);
                m_listStateDataCache.Add(data);
                m_listStateDataCache.Sort(SortStateData);
                dataList = m_listStateDataCache.ToArray();
                m_curStateDataIndex = dataList.Length - 1;
            }
            m_curClipGroupDataIndex = Config.ErrorIndex;
            animClipData.stateList = dataList;
            CurrentAnimClipData = animClipData;
        }

        private static int FindStateDataIndex(State state) {
            StateData[] dataList = CurrentAnimClipData.stateList;
            if (dataList == null)
                return Config.ErrorIndex;
            for (int index = 0; index < dataList.Length; index++)
                if (dataList[index].state == state)
                    return index;
            return Config.ErrorIndex;
        }

        private static List<StateClipGroupIndex> m_listStateClipIndexPair = new List<StateClipGroupIndex>();
        private static StateClipGroupIndex m_stateClipGroupIndexCache;

        private static List<ClipGroupData> m_listClipGroupDataCache = new List<ClipGroupData>(16);
        private static void SetClipGroupDataListCache(ClipGroupData[] array) {
            m_listClipGroupDataCache.Clear();
            if (array == null || array.Length == 0)
                return;
            for (int index = 0; index < array.Length; index++)
                m_listClipGroupDataCache.Add(array[index]);
        }
        private static void SortClipGroupDataListCache() => m_listClipGroupDataCache.Sort(SortClipGroupData);
        private static int SortClipGroupData(ClipGroupData left, ClipGroupData right) =>
            left.id.CompareTo(right.id);

        private static int m_curClipGroupDataIndex;
        private static ClipGroupData CurrentClipGroupData {
            set {
                StateData data = CurrentStateData;
                data.clipList[m_curClipGroupDataIndex] = value;
                CurrentStateData = data;
            }
            get {
                if (m_curClipGroupDataIndex == Config.ErrorIndex)
                    return default;
                return CurrentStateData.clipList[m_curClipGroupDataIndex];
            }
        }
        public static uint CurrentClipID => CurrentClipGroupData.id;
        private static string m_lastClipName;

        public static void AddNewClipGroupData(uint id) {
            StateData stateData = CurrentStateData;
            SetClipGroupDataListCache(stateData.clipList);
            ClipGroupData clipGroupData = default;
            clipGroupData.id = id;
            clipGroupData.clipName = m_lastClipName;
            m_listClipGroupDataCache.Add(clipGroupData);
            SortClipGroupDataListCache();
            stateData.clipList = m_listClipGroupDataCache.ToArray();
            CurrentStateData = stateData;
            m_curClipGroupDataIndex = m_listClipGroupDataCache.IndexOf(clipGroupData);
            m_stateClipGroupIndexCache.stateIndex = (ushort)m_curStateDataIndex;
            m_stateClipGroupIndexCache.clipGroupIndex = (ushort)m_curClipGroupDataIndex;
            m_listStateClipIndexPair.Add(m_stateClipGroupIndexCache);
            ResetFrameCubeAndEffectData();
        }

        private static int FindClipGroupDataIndex() {
            StateData stateData = CurrentStateData;
            if (stateData.IsNullTable())
                return Config.ErrorIndex;
            for (int index = 0; index < stateData.clipList.Length; index++)
                if (stateData.clipList[index].clipName == m_lastClipName)
                    return index;
            return Config.ErrorIndex;
        }

        public static void DeleteClipGroupData() {
            StateData stateData = CurrentStateData;
            if (stateData.clipList.Length <= 1) {
                stateData.clipList = null;
                CurrentStateData = stateData;
                m_listStateClipIndexPair.Clear();
                m_curClipGroupDataIndex = Config.ErrorIndex;
                ResetFrameCubeAndEffectData();
                return;
            }
            SetClipGroupDataListCache(stateData.clipList);
            m_listClipGroupDataCache.RemoveAt(m_curClipGroupDataIndex);
            stateData.clipList = m_listClipGroupDataCache.ToArray();
            CurrentStateData = stateData;
            m_listStateClipIndexPair.Remove(m_stateClipGroupIndexCache);
            if (m_listStateClipIndexPair.Count == 0) {
                m_curClipGroupDataIndex = Config.ErrorIndex;
                ResetFrameCubeAndEffectData();
                return;
            }
            SetCurrentClipName(m_lastClipName);
        }

        public static void SetClipGroupID(uint id) {
            for (ushort index = 0; index < m_listStateClipIndexPair.Count; index++) {
                int stateIndex = m_listStateClipIndexPair[index].stateIndex;
                if (stateIndex != m_curStateDataIndex)
                    continue;
                int clipGroupIndex = m_listStateClipIndexPair[index].clipGroupIndex;
                ClipGroupData data = CurrentStateData.clipList[clipGroupIndex];
                if (data.clipName != m_lastClipName || data.id != id)
                    continue;
                m_curClipGroupDataIndex = clipGroupIndex;
                ResetFrameCubeAndEffectData();
                break;
            }
        }

        public static FrameData[] FrameList {
            set {
                ClipGroupData data = CurrentClipGroupData;
                data.frameList.frameList = value;
                CurrentClipGroupData = data;
            }
            get => CurrentClipGroupData.frameList.frameList;
        }

        public static void SetCurrentClipName(string clipName) {
            if (clipName == m_lastClipName)
                return;
            ResetClip();
            m_lastClipName = clipName;
            StateData[] stateDataList = CurrentAnimClipData.stateList;
            if (stateDataList == null || stateDataList.Length == 0)
                return;
            for (int stateIndex = 0; stateIndex < stateDataList.Length; stateIndex++) {
                ClipGroupData[] clipList = stateDataList[stateIndex].clipList;
                if (clipList == null || clipList.Length == 0)
                    continue;
                for (int clipGroupIndex = 0; clipGroupIndex < clipList.Length; clipGroupIndex++) {
                    ClipGroupData data = clipList[clipGroupIndex];
                    if (data.clipName != clipName)
                        continue;
                    m_stateClipGroupIndexCache.stateIndex = (ushort)stateIndex;
                    m_stateClipGroupIndexCache.clipGroupIndex = (ushort)clipGroupIndex;
                    if (m_listStateClipIndexPair.Contains(m_stateClipGroupIndexCache))
                        Debug.LogError("关键帧表 clip name " + data.clipName + " 配置相同 ID " + data.id);
                    else
                        m_listStateClipIndexPair.Add(m_stateClipGroupIndexCache);
                }
            }
            if (m_listStateClipIndexPair.Count == 0)
                return;
            m_stateClipGroupIndexCache = m_listStateClipIndexPair[0];
            m_curStateDataIndex = m_stateClipGroupIndexCache.stateIndex;
            m_curClipGroupDataIndex = m_stateClipGroupIndexCache.clipGroupIndex;
            ResetFrameCubeAndEffectData();
        }

        private static void ResetClip(){
            m_lastClipName = null;

            m_curStateDataIndex = Config.ErrorIndex;
            m_curClipGroupDataIndex = Config.ErrorIndex;

            m_listStateDataCache.Clear();
            m_listClipGroupDataCache.Clear();
            m_listFrameDataCache.Clear();

            m_listStateClipIndexPair.Clear();
            
            m_listEffect.Clear();
            m_listCollision.Clear();
            m_listGrabCollision.Clear();
            m_listBlockCollision.Clear();
        }

        public static void Reset() {
            ResetModel();
            ResetClip();
        }

        private static List<FrameData> m_listFrameDataCache = new List<FrameData>(16);
        private static void SetFrameDataListCache(FrameData[] array) {
            m_listFrameDataCache.Clear();
            if (array == null || array.Length == 0)
                return;
            for (int index = 0; index < array.Length; index++)
                m_listFrameDataCache.Add(array[index]);
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
            data.index = (ushort)(array.Length + 1);
            SetFrameDataListCache(array);
            m_listFrameDataCache.Add(data);
            FrameList = m_listFrameDataCache.ToArray();
        }

        public static void AddCopyFrameData(FrameData data) {
            FrameData[] array = FrameList;
            data.index = (ushort)(array.Length + 1);
            SetFrameDataListCache(array);
            m_listFrameDataCache.Add(data);
            FrameList = m_listFrameDataCache.ToArray();
        }

        public static void DeleteFrameData(int index) {
            FrameData[] array = FrameList;
            if (array.Length <= 1)
                FrameList = null;
            else {
                SetFrameDataListCache(array);
                m_listFrameDataCache.RemoveAt(index);
                for (index = 0; index < m_listFrameDataCache.Count; index++) {
                    FrameData data = m_listFrameDataCache[index];
                    data.index = (ushort)(index + 1);
                    m_listFrameDataCache[index] = data;
                }
                FrameList = m_listFrameDataCache.ToArray();
            }
        }

        private static Action m_effectChangeCall;
        public static void SetEffectChangeCallback(Action call) => m_effectChangeCall = call;
        private static void SetFrameData(int index, FrameData data, bool isRefreshEffectFrame = false, bool isRefresHitFrame = false, 
                                            bool isRefreshGrabFrame = false, bool isRefreshBlockFrame = false) {
            FrameData[] array = FrameList;
            array[index] = data;
            FrameList = array;
            if (isRefreshEffectFrame) {
                SetFrameList<EffectData>();
                m_effectChangeCall?.Invoke();
            }
            if (isRefresHitFrame)
                SetFrameList<HitData>();
            if (isRefreshGrabFrame)
                SetFrameList<GrabData>();
            if (isRefreshBlockFrame)
                SetFrameList<BlockData>();
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
            SetFrameData(index, data);
        }

        public static void SetFrameDataEndTime(int index, float endTime) {
            FrameData data = GetFrameData(index);
            data.endTime = endTime;
            SetFrameData(index, data);
        }

        private const ushort DefaultPriority = 1;
        public static void AddPriorityFrameData(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            PriorityFrameData priorityFrameData = (PriorityFrameData)frameData.GetFieldValueTableValue(frameType.ToString());
            priorityFrameData.SetPriority(DefaultPriority);
            frameData.SetFieldValueTableValue(frameType.ToString(), priorityFrameData);
            SetFrameData(index, frameData);
        }

        public static void DeletePriorityFrameData(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            ITable table = (ITable)frameData.GetFieldValueTableValue(frameType.ToString());
            table.Clear();
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(index, frameData, frameType == FrameType.PlayEffect, frameType == FrameType.Hit, frameType == FrameType.Grab, frameType == FrameType.Block);
        }

        public static void SetFramePriorityData(int index, FrameType frameType, ushort priority) {
            FrameData frameData = GetFrameData(index);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            table.SetFieldValueTableValue(CommonFrameData.Key_Priority, (int)priority);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(index, frameData);
        }

        public static void SetFrameLoopData(int index, FrameType frameType, bool isLoop) {
            FrameData frameData = GetFrameData(index);
            ICommonFrameData table = (ICommonFrameData)frameData.GetFieldValueTableValue(frameType.ToString());
            table.SetLoop(isLoop);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(index, frameData);
        }

        public static void AddUngrabFrameData(int index) {
            FrameData frameData = GetFrameData(index);
            UngrabFrameData ungrabFrameData = default;
            ungrabFrameData.SetPriority(DefaultPriority);
            UngrabData ungrabData = default;
            ungrabData.grabState = 1;
            ungrabFrameData.ungrabData = ungrabData;
            frameData.ungrabFrameData = ungrabFrameData;
            SetFrameData(index, frameData);
        }

        public static void SetUngrabFrameData(int index, UngrabFrameData data) {
            FrameData frameData = GetFrameData(index);
            frameData.ungrabFrameData = data;
            SetFrameData(index, frameData);
        }

        private const int DefaultCameraID = 1;
        public static void AddCameraFrameData(int index) {
            FrameData frameData = GetFrameData(index);
            CameraFrameData cameraFrameData = default;
            cameraFrameData.SetPriority(DefaultPriority);
            CameraData cameraData = default;
            cameraData.id = DefaultCameraID;
            cameraData.triggerType = CameraTriggerType.Time;
            cameraData.focusType = CameraFocusType.Attacker;
            cameraFrameData.cameraData = cameraData;
            frameData.cameraFrameData = cameraFrameData;
            SetFrameData(index, frameData);
        }

        public static void DeleteCameraFrameData(int index) {
            FrameData frameData = GetFrameData(index);
            frameData.cameraFrameData.Clear();
            SetFrameData(index, frameData);
        }

        public static void SetCameraFrameData(int index, CameraFrameData data) {
            FrameData frameData = GetFrameData(index);
            frameData.cameraFrameData = data;
            SetFrameData(index, frameData);
        }

        private const int DefaultTrackID = 1;
        public static void AddTrackChangeFrameData(int index) {
            FrameData frameData = GetFrameData(index);
            TrackChangeFrameData trackChangeFrameData = default;
            trackChangeFrameData.SetPriority(DefaultPriority);
            TrackChangeData trackChangeData = default;
            trackChangeData.id = DefaultCameraID;
            trackChangeData.replaceID = DefaultTrackID;
            trackChangeFrameData.trackChangeData = trackChangeData;
            frameData.trackChangeFrameData = trackChangeFrameData;
            SetFrameData(index, frameData);
        }

        public static void DeleteTrackChangeFrameData(int index) {
            FrameData frameData = GetFrameData(index);
            frameData.trackChangeFrameData.Clear();
            SetFrameData(index, frameData);
        }

        public static void SetTrackChangeFrameData(int index, TrackChangeFrameData data) {
            FrameData frameData = GetFrameData(index);
            frameData.trackChangeFrameData = data;
            SetFrameData(index, frameData);
        }

        private const int DefaultID = 1;
        public static void AddIDFrameData(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            IDFrameData idFrameData = default;
            idFrameData.SetPriority(DefaultPriority);
            IDData idData = default;
            idData.id = DefaultID;
            idFrameData.idData = idData;
            switch (frameType) {
                case FrameType.CommandAttack:
                    frameData.commandAttackFrameData = idFrameData;
                    break;
                case FrameType.CommandSkill:
                    frameData.commandSkillFrameData = idFrameData;
                    break;
            }
            SetFrameData(index, frameData);
        }

        public static void DeleteIDFrameData(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            switch (frameType) {
                case FrameType.CommandAttack:
                    frameData.commandAttackFrameData.Clear();
                    break;
                case FrameType.CommandSkill:
                    frameData.commandSkillFrameData.Clear();
                    break;
            }
            SetFrameData(index, frameData);
        }

        public static void SetIDFrameData(int index, FrameType frameType, IDFrameData data) {
            FrameData frameData = GetFrameData(index);
            switch (frameType) {
                case FrameType.CommandAttack:
                    frameData.commandAttackFrameData = data;
                    break;
                case FrameType.CommandSkill:
                    frameData.commandSkillFrameData = data;
                    break;
            }
            SetFrameData(index, frameData);
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
        
        private static void SetCustomDataMethodAndData(IFieldValueTable table) {
            m_customDataCache[CustomDataIndex] = table.GetFieldValueTableValue(CommonFrameData.Key_Data);
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
            if (table.IsNullTable())
                table.SetFieldValueTableValue(CommonFrameData.Key_Priority, (int)DefaultPriority);
            SetCustomDataMethodAndData(table);
            object customData = GetCustomData();
            object staticList = GetCustomDataStaticList();
            object dataList = GetCustomDataList();
            IFieldValueTable data = (IFieldValueTable)Activator.CreateInstance(ListType);
            if (dataList == null || (dataList as Array).Length == 0) {
                data.SetKey(1);
                SetFramePriorityData(index, frameType, DefaultPriority);
            }
            else {
                Array array = dataList as Array;
                for (int arrayIndex = 0; arrayIndex < array.Length; arrayIndex++) {
                    SetStaticCacheListAddArg(array.GetValue(arrayIndex));
                    CustomDataStaticCacheListAddMethod.Invoke(staticList, GetStaticCacheListAddArg());
                }
                data.SetKey(array.Length + 1);
            }
            SetStaticCacheListAddArg(data);
            CustomDataStaticCacheListAddMethod.Invoke(staticList, GetStaticCacheListAddArg());
            customData = CustomDataSetTableListMethod.Invoke(customData, null);
            table.SetFieldValueTableValue(CommonFrameData.Key_Data, customData);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(index, frameData, frameType == FrameType.PlayEffect, frameType == FrameType.Hit, frameType == FrameType.Grab, frameType == FrameType.Block);
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
            table.SetFieldValueTableValue(CommonFrameData.Key_Data, customData);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(frameIndex, frameData, frameType == FrameType.PlayEffect, frameType == FrameType.Hit, frameType == FrameType.Grab, frameType == FrameType.Block);
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
            table.SetFieldValueTableValue(CommonFrameData.Key_Data, customData);
            frameData.SetFieldValueTableValue(frameType.ToString(), table);
            SetFrameData(frameIndex, frameData, frameType == FrameType.PlayEffect, frameType == FrameType.Hit, frameType == FrameType.Grab, frameType == FrameType.Block);
        }

        private static List<KeyValuePair<float, EffectData[]>> m_listEffect = new List<KeyValuePair<float, EffectData[]>>();
        public static List<KeyValuePair<float, EffectData[]>> ListEffect => m_listEffect;

        private static List<KeyValuePair<float, HitData[]>> m_listCollision = new List<KeyValuePair<float, HitData[]>>();
        public static List<KeyValuePair<float, HitData[]>> ListCollision => m_listCollision;

        private static List<KeyValuePair<float, GrabData[]>> m_listGrabCollision = new List<KeyValuePair<float, GrabData[]>>();
        public static List<KeyValuePair<float, GrabData[]>> ListGrabCollision => m_listGrabCollision;

        private static List<KeyValuePair<float, BlockData[]>> m_listBlockCollision = new List<KeyValuePair<float, BlockData[]>>();
        public static List<KeyValuePair<float, BlockData[]>> ListBlockCollision => m_listBlockCollision;

        private static void ResetFrameCubeAndEffectData() {
            SetFrameList<EffectData>();
            SetFrameList<HitData>();
            SetFrameList<GrabData>();
            SetFrameList<BlockData>();
        }

        private static void SetFrameList<T>() where T : IFieldValueTable {
            T temp = default;
            List<KeyValuePair<float, T[]>> list;
            if (temp is EffectData)
                list = m_listEffect as List<KeyValuePair<float, T[]>>;
            else if (temp is HitData)
                list = m_listCollision as List<KeyValuePair<float, T[]>>;
            else if (temp is GrabData)
                list = m_listGrabCollision as List<KeyValuePair<float, T[]>>;
            else
                list = m_listBlockCollision as List<KeyValuePair<float, T[]>>;
            list.Clear();
            if (FrameList == null || FrameList.Length == 0)
                return;
            for (int index = 0; index < FrameList.Length; index++) {
                FrameData frameData = FrameList[index];
                T[] dataList;
                if (temp is EffectData)
                    dataList = frameData.effectFrameData.effectData.dataList as T[];
                else if (temp is HitData)
                    dataList = frameData.hitFrameData.hitData.dataList as T[];
                else if (temp is GrabData)
                    dataList = frameData.grabFrameData.grabData.dataList as T[];
                else
                    dataList = frameData.blockFrameData.blockData.dataList as T[];
                if (dataList == null)
                    continue;
                KeyValuePair<float, T[]> timeHitData = new KeyValuePair<float, T[]>(frameData.time, dataList);
                list.Add(timeHitData);
            }
            list.Sort(SortFrameListByTime);
        }

        private static int SortFrameListByTime<T>(KeyValuePair<float, T[]> left, KeyValuePair<float, T[]> right) =>
            left.Key.CompareTo(right.Key);

        public static string GetWriteFileString() => LuaWriter.GetWriteFileString(m_listAnimClip);

        private struct StateClipGroupIndex {

            public ushort stateIndex;
            public ushort clipGroupIndex;

            public static bool operator ==(StateClipGroupIndex left, StateClipGroupIndex right) =>
                left.stateIndex == right.stateIndex && left.clipGroupIndex == right.clipGroupIndex;

            public static bool operator !=(StateClipGroupIndex left, StateClipGroupIndex right) => !(left == right);

            public override bool Equals(object obj) => this == (StateClipGroupIndex)obj;

            public override int GetHashCode() => (int)(stateIndex + clipGroupIndex);
        }
    }
}