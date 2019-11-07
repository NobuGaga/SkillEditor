using UnityEngine;
using System;
using System.Text;
using Lua;
using LuaTableKeyValue = Lua.LuaFormat.LuaTableKeyValue;

namespace SkillEditor.LuaStructure {


    internal enum AnimClipLuaLayer {
        EnterTable = 0,
        Model = 1,
        State = 2,
        Clip = 3,
        FrameGroup = 4,
        FrameType = 5,
        FrameData = 6,
        CustomeData = 7,
        Effect = 8,
        Rect = 9,
    }

    internal struct AnimClipData {
        public string modelName;
        public StateData[] stateList;

        public AnimClipData(string modelName, StateData[] stateList) {
            this.modelName = modelName;
            this.stateList = stateList;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.StateListStringLength);
        public override string ToString() {
            if (stateList == null)
                return string.Empty;
            m_staticBuilder.Clear();
            for (int index = 0; index < stateList.Length; index++)
                m_staticBuilder.Append(stateList[index].ToString());
            string stateListString = m_staticBuilder.ToString();
            string tabString = Tool.GetTabString(AnimClipLuaLayer.Model);
            string format = "{0}[\"{1}\"] = {2}\n{3}{0}{4},\n";
            string toString = string.Format(format, tabString,
                                            modelName,
                                            LuaFormat.CurlyBracesPair.start,
                                            stateListString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
        }
    }

    internal enum State {
        None,
        Atk,
        UseSkill,
        Hit,
        Dead
    }

    internal struct StateData : INullTable {
        public State state;
        public ClipData[] clipList;

        public StateData(State state, ClipData[] clipList) {
            this.state = state;
            this.clipList = clipList;
        }

        public void SetState(string originKey) {
            if (!originKey.Contains(StateHeadString + LuaFormat.PointSymbol))
                return;
            string stateString = originKey.Substring(StateHeadString.Length + 1);
            state = (State)Enum.Parse(typeof(State), stateString);
        }

        public void Clear() {
            state = State.None;
            clipList = null;
        }

        public bool IsNullTable() {
            return state == State.None || clipList == null;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.ClipListStringLength);
        public override string ToString() {
            if (IsNullTable())
                return string.Empty;
            m_staticBuilder.Clear();
            for (int index = 0; index < clipList.Length; index++)
                m_staticBuilder.Append(clipList[index].ToString());
            string clipListString = m_staticBuilder.ToString();
            string tabString = Tool.GetTabString(AnimClipLuaLayer.State);
            string format = "{0}[\"{1}{2}{3}\"] = {4}\n{5}{0}{6},\n";
            string toString = string.Format(format, tabString,
                                            StateHeadString, LuaFormat.PointSymbol, state.ToString(),
                                            LuaFormat.CurlyBracesPair.start,
                                            clipListString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
        }

        private const string StateHeadString = "EntityStateDefine";
    }

    internal struct ClipData : ITable, INullTable {
        public string clipName;
        public PoolType poolType;
        public KeyFrameData[] keyFrameList;
        public ProcessFrameData[] processFrameList;

        public ClipData(string clipName, PoolType poolType, KeyFrameData[] keyFrameList, ProcessFrameData[] processFrameList) {
            this.clipName = clipName;
            this.poolType = poolType;
            this.keyFrameList = keyFrameList;
            this.processFrameList = processFrameList;
        }

        public void SetPoolType(string originKey) {
            if (!originKey.Contains(GameConstantHeadString + LuaFormat.PointSymbol))
                return;
            string poolTypeString = originKey.Substring(GameConstantHeadString.Length + 1);
            poolType = (PoolType)Enum.Parse(typeof(PoolType), poolTypeString);
        }

        public void SetPoolType(State state) {
            switch (state) {
                case State.None:
                    poolType = PoolType.None;
                    break;
                case State.Atk:
                case State.UseSkill:
                    poolType = PoolType.POOL_ANIM_ATTACK;
                    break;
                case State.Hit:
                    poolType = PoolType.POOL_ANIM_HIT;
                    break;
                case State.Dead:
                    poolType = PoolType.POOL_ANIM_DEFAULT;
                    break;
            }
        }

        public KeyFrameData[] GetKeyFrameList() {
            return keyFrameList;
        }

        public ProcessFrameData[] GetProcessFrameList() {
            return processFrameList;
        }

        public void Clear() {
            clipName = string.Empty;
            poolType = PoolType.None;
            keyFrameList = null;
            processFrameList = null;
        }

        public bool IsNullTable() {
            return clipName == string.Empty || poolType == PoolType.None;
        }

        private bool CheckFrameArrayIsNull<T>(T[] array) where T : INullTable {
            if (array == null)
                return true;
            for (int index = 0; index < array.Length; index++)
                if (!array[index].IsNullTable())
                    return false;
            return true;
        }

        private bool IsNoKeyFrameGroupData => CheckFrameArrayIsNull(keyFrameList);

        private bool IsNoProcessFrameGroupData => CheckFrameArrayIsNull(processFrameList);

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.FrameListStringLength);
        public override string ToString() {
            string keyFrameString = string.Empty;
            string format;
            string frameGroupTabString = Tool.GetTabString(AnimClipLuaLayer.FrameGroup);
            if (!IsNoKeyFrameGroupData) {
                format = "{0}keyframe = {1}{3}{2},\n";
                keyFrameString = Tool.GetArrayString(m_staticBuilder, AnimClipLuaLayer.FrameGroup, keyFrameList);
                keyFrameString = string.Format(format, frameGroupTabString,
                                               LuaFormat.CurlyBracesPair.start,
                                               LuaFormat.CurlyBracesPair.end,
                                               keyFrameString);
            }
            string processFrameString = string.Empty;
            if (!IsNoProcessFrameGroupData) {
                format = "{0}processFrame = {1}{3}{2},\n";
                processFrameString = Tool.GetArrayString(m_staticBuilder, AnimClipLuaLayer.FrameGroup, processFrameList);
                processFrameString = string.Format(format, frameGroupTabString,
                                               LuaFormat.CurlyBracesPair.start,
                                               LuaFormat.CurlyBracesPair.end,
                                               processFrameString);
            }
            format = "{0}[\"{4}\"] = {2}\n{1}iPoolType = {5}{6}{7},\n{8}{9}{0}{3},\n";
            string clipTabString = Tool.GetTabString(AnimClipLuaLayer.Clip);
            string toString = string.Format(format, clipTabString,
                                            frameGroupTabString,
                                            LuaFormat.CurlyBracesPair.start,
                                            LuaFormat.CurlyBracesPair.end,
                                            clipName,
                                            GameConstantHeadString, LuaFormat.PointSymbol, poolType.ToString(),
                                            keyFrameString,
                                            processFrameString);
            return Tool.GetCacheString(toString);
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_IPoolType:
                    SetPoolType(value as string);
                    return;
                case Key_KeyFrame:
                    keyFrameList = value as KeyFrameData[];
                    return;
                case Key_ProcessFrame:
                    processFrameList = value as ProcessFrameData[];
                    return;
            }
        }

        private const string Key_IPoolType = "iPoolType";
        public const string Key_KeyFrame = "keyframe";
        public const string Key_ProcessFrame = "processFrame";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[3];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_IPoolType, LuaFormat.Type.LuaReference);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_KeyFrame, LuaFormat.Type.LuaTable);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_ProcessFrame, LuaFormat.Type.LuaTable);
            return m_arraykeyValue;
        }

        private const string GameConstantHeadString = "GameConstant";

        internal enum PoolType {
            None,
            POOL_ANIM_ATTACK,
            POOL_ANIM_HIT,
            POOL_ANIM_DEFAULT,
        }
    }

    internal enum FrameType {
        None,
        Hit,
        Start,
        PlayEffect,
        CacheBegin,
        SectionOver,
        End,
    }

    internal struct KeyFrameData : ITable, INullTable {
        public int index;
        public FrameType frameType;
        public float time;
        public short priority;
        public CustomData[] dataList;

        public KeyFrameData(int index, FrameType frameType, float time, short priority, CustomData[] dataList) {
            this.index = index;
            this.frameType = frameType;
            this.time = time;
            this.priority = priority;
            this.dataList = dataList;
        }

        private void SetFrameType(string frameTypeString) {
            frameType = (FrameType)Enum.Parse(typeof(FrameType), frameTypeString);
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.CustomDataListStringLength);
        public override string ToString() {
            string dataListString = string.Empty;
            string format;
            string frameDataString = Tool.GetTabString(AnimClipLuaLayer.FrameData);
            if (dataList != null) {
                format = "{0}data = {1}{3}{2},\n";
                dataListString = Tool.GetArrayString(m_staticBuilder, AnimClipLuaLayer.FrameData, dataList);
                dataListString = string.Format(format, frameDataString,
                                               LuaFormat.CurlyBracesPair.start,
                                               LuaFormat.CurlyBracesPair.end,
                                               dataListString);
            }
            format = "{0}[{4}] = {2}\n{1}name = \"{5}\",\n{1}time = {6},\n{1}priority = {7},\n{8}{0}{3},\n";
            string frameTypeTabString = Tool.GetTabString(AnimClipLuaLayer.FrameType);
            string toString = string.Format(format, frameTypeTabString,
                                            frameDataString,
                                            LuaFormat.CurlyBracesPair.start,
                                            LuaFormat.CurlyBracesPair.end,
                                            index,
                                            frameType.ToString(),
                                            time,
                                            priority,
                                            dataListString);
            return Tool.GetCacheString(toString);
        }

        public bool IsNullTable() {
            return index == 0 || frameType == FrameType.None || priority == 0;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Name:
                    SetFrameType(value as string);
                    return;
                case Key_Time:
                    time = (float)value;
                    return;
                case Key_Priority:
                    priority = (short)(int)value;
                    return;
                case Key_Data:
                    dataList = value as CustomData[];
                    return;
            }
        }

        private const string Key_Name = "name";
        private const string Key_Time = "time";
        private const string Key_Priority = "priority";
        private const string Key_Data = "data";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[4];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Name, LuaFormat.Type.LuaString);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Time, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_Priority, LuaFormat.Type.LuaInt);
            m_arraykeyValue[3] = new LuaTableKeyValue(Key_Data, LuaFormat.Type.LuaTable);
            return m_arraykeyValue;
        }
    }

    internal struct ProcessFrameData : ITable, INullTable {
        public FrameType frameType;
        public float time;
        public short priority;
        public CustomData[] dataList;

        public ProcessFrameData(FrameType frameType, float time, short priority, CustomData[] dataList) {
            this.frameType = frameType;
            this.time = time;
            this.priority = priority;
            this.dataList = dataList;
        }

        public void SetFrameType(string frameTypeString) {
            frameType = (FrameType)Enum.Parse(typeof(FrameType), frameTypeString);
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.CustomDataListStringLength);
        public override string ToString() {
            string dataListString = string.Empty;
            string format;
            string frameDataString = Tool.GetTabString(AnimClipLuaLayer.FrameData);
            if (dataList != null) {
                format = "{0}data = {1}{3}{2},\n";
                dataListString = Tool.GetArrayString(m_staticBuilder, AnimClipLuaLayer.FrameData, dataList);
                dataListString = string.Format(format, frameDataString,
                                               LuaFormat.CurlyBracesPair.start,
                                               LuaFormat.CurlyBracesPair.end,
                                               dataListString);
            }
            format = "{0}[\"{4}\"] = {2}\n{1}time = {5},\n{1}priority = {6},\n{7}{0}{3},\n";
            string frameTypeTabString = Tool.GetTabString(AnimClipLuaLayer.FrameType);
            string toString = string.Format(format, frameTypeTabString,
                                            frameDataString,
                                            LuaFormat.CurlyBracesPair.start,
                                            LuaFormat.CurlyBracesPair.end,
                                            frameType.ToString(),
                                            time,
                                            priority,
                                            dataListString);
            return Tool.GetCacheString(toString);
        }

        public bool IsNullTable() {
            return frameType == FrameType.None || priority == 0;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Time:
                    time = (float)value;
                    return;
                case Key_Priority:
                    priority = (short)(int)value;
                    return;
                case Key_Data:
                    dataList = value as CustomData[];
                    return;
            }
        }

        private const string Key_Time = "time";
        private const string Key_Priority = "priority";
        private const string Key_Data = "data";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[3];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Time, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Priority, LuaFormat.Type.LuaInt);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_Data, LuaFormat.Type.LuaTable);
            return m_arraykeyValue;
        }
    }

    internal struct CustomData : INullTable {
        public int index;
        public object data;

        public CustomData(int index, object data) {
            this.index = index;
            this.data = data;
        }

        public bool IsNullTable() {
            if (data == null)
                return true;
            if (data is EffectData)
                return ((EffectData)data).IsNullTable();
            if (data is CubeData[]) {
                CubeData[] array = data as CubeData[];
                for (int i = 0; i < array.Length; i++)
                    if (!array[i].IsNullTable())
                        return false;
                return true;
            }
            return true;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.RectDataListStringLength);
        public override string ToString() {
            string dataString = string.Empty;
            string tabString;
            string format;
            if (data is EffectData && !((EffectData)data).IsNullTable())
                dataString = ((EffectData)data).ToString();
            else if (data is CubeData[]) {
                index = CubeDataIndex;
                CubeData[] array = data as CubeData[];
                tabString = Tool.GetTabString(AnimClipLuaLayer.Effect);
                format = "{0}[{1}] = {2}\n{3}{0}{4},\n";
                m_staticBuilder.Clear();
                m_staticBuilder.Append(LuaFormat.LineSymbol);
                for (int i = 0; i < array.Length; i++) {
                    if (array[i].IsNullTable())
                        continue;
                    m_staticBuilder.Append(string.Format(format,
                                                         tabString,
                                                         i + 1,
                                                         LuaFormat.CurlyBracesPair.start,
                                                         array[i].ToString(),
                                                         LuaFormat.CurlyBracesPair.end));
                }
                dataString = m_staticBuilder.ToString();
            }
            tabString = Tool.GetTabString(AnimClipLuaLayer.CustomeData);
            format = "{0}[{1}] = {2}{3}{0}{4},\n";
            string toString = string.Format(format,
                                            tabString,
                                            index,
                                            LuaFormat.CurlyBracesPair.start,
                                            dataString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
        }

        public const short CubeDataIndex = 4;
    }

    internal struct EffectData : ITable, INullTable {
        public short type;
        public int id;

        public EffectData(short type, int id) {
            this.type = type;
            this.id = id;
        }

        public bool IsNullTable() {
            return type == 0 || id == 0;
        }

        public override string ToString() {
            string tabString = Tool.GetTabString(AnimClipLuaLayer.Effect);
            string format = "\n{0}type = {1},\n{0}id = {2},\n";
            string toString = string.Format(format, tabString, type, id);
            return Tool.GetCacheString(toString);
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (short)(int)value;
                    return;
                case Key_Id:
                    id = (int)value;
                    return;
            }
        }

        private const string Key_Type = "type";
        private const string Key_Id = "id";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[2];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Type, LuaFormat.Type.LuaInt);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Id, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }
    }

    internal struct CubeData : ITable, INullTable {
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public short depth;

        public CubeData(float x, float y, float z, float width, float height, short depth) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public bool IsNullTable() {
            return Size == Vector3.zero;
        }

        public override string ToString() {
            string tabString = Tool.GetTabString(AnimClipLuaLayer.Rect);
            string format = "{0}x = {1},\n{0}y = {2},\n{0}z = {3},\n{0}width = {4},\n{0}height = {5},\n{0}depth = {6},\n";
            string toString = string.Format(format, tabString, x, y, z, width, height, depth);
            return Tool.GetCacheString(toString);
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (float)value;
                    return;
                case Key_Y:
                    y = (float)value;
                    return;
                case Key_Z:
                    z = (float)value;
                    return;
                case Key_Width:
                    width = (float)value;
                    return;
                case Key_Height:
                    height = (float)value;
                    return;
                case Key_Depth:
                    depth = (short)(int)value;
                    return;
            }
        }

        private const string Key_X = "x";
        private const string Key_Y = "y";
        private const string Key_Z = "z";
        private const string Key_Width = "width";
        private const string Key_Height = "height";
        private const string Key_Depth = "depth";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[6];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_X, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Y, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_Z, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[3] = new LuaTableKeyValue(Key_Width, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[4] = new LuaTableKeyValue(Key_Height, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[5] = new LuaTableKeyValue(Key_Depth, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }

        private static Vector3 m_offsetCache;
        private static Vector3 m_sizeCache;

        public Vector3 Offset {
            get {
                m_offsetCache.x = x;
                m_offsetCache.y = y;
                m_offsetCache.z = z;
                return m_offsetCache;
            }
        }

        public Vector3 Size {
            get {
                m_sizeCache.x = width;
                m_sizeCache.y = height;
                m_sizeCache.z = depth;
                return m_sizeCache;
            }
        }
    }

    internal interface INullTable {

        bool IsNullTable();
    }

    public interface ITable {

        void SetTableKeyValue(string key, object value);
        LuaTableKeyValue[] GetTableKeyValueList();
    }
}