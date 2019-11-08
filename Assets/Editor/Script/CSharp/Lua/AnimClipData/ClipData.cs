using System;
using System.Text;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct ClipData : ITable {
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
            }
        }

        public KeyFrameData[] GetKeyFrameList() {
            return keyFrameList;
        }

        public ProcessFrameData[] GetProcessFrameList() {
            return processFrameList;
        }

        public ushort GetLayer() => 3;
        public KeyType GetKeyType() => KeyType.Field;
        public bool IsNullTable() => clipName == null || clipName == string.Empty || 
                                        poolType == Pool == null || stateList.Length == 0;

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

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[3];
            m_arraykeyValue[0] = new FieldKeyTable(Key_IPoolType, LuaFormat.ValueType.Reference);
            m_arraykeyValue[1] = new FieldKeyTable(Key_KeyFrame, LuaFormat.ValueType.Table);
            m_arraykeyValue[2] = new FieldKeyTable(Key_ProcessFrame, LuaFormat.ValueType.Table);
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
}