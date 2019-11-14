using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct FrameData : IFieldValueTable {

        private ushort index;
        public float time;
        public HitFrameData hitFrameData;
        public EffectFrameData effectFrameData;
        public PriorityFrameData cacheFrameData;
        public PriorityFrameData sectionFrameData;
        public FrameData(float time) {
            index = 0;
            this.time = time;
            hitFrameData = default;
            effectFrameData = default;
            cacheFrameData = default;
            sectionFrameData = default;
        }

        private const short NullTableFlag = -1;

        #region ITable Function
        public string GetTableName() => "FrameData";
        public ushort GetLayer() => 4;
        public ReadType GetReadType() => ReadType.Repeat;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() => time == NullTableFlag;
        public void Clear() {
            time = NullTableFlag;
            hitFrameData.Clear();
            effectFrameData.Clear();
            cacheFrameData.Clear();
            sectionFrameData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 11));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        public void SetFieldValueTableValue(string key, object value) {
            FrameType frameType = FrameType.None;
            if (!Enum.TryParse(key, false, out frameType)) {
                UnityEngine.Debug.LogError("FrameData::SetFieldValueTableValue not exit key : " + key);
                return;
            }
            switch (frameType) {
                case FrameType.Hit:
                    hitFrameData = (HitFrameData)value;
                    return;
                case FrameType.PlayEffect:
                    effectFrameData = (EffectFrameData)value;
                    return;
                case FrameType.CacheBegin:
                    cacheFrameData = (PriorityFrameData)value;
                    return;
                case FrameType.SectionOver:
                    sectionFrameData = (PriorityFrameData)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            FrameType frameType = FrameType.None;
            if (!Enum.TryParse(key, false, out frameType)) {
                UnityEngine.Debug.LogError("FrameData::GetFieldValueTableValue not exit key : " + key);
                return null;
            }
            switch (frameType) {
                case FrameType.Hit:
                    return hitFrameData;
                case FrameType.PlayEffect:
                    return effectFrameData;
                case FrameType.CacheBegin:
                    return cacheFrameData;
                case FrameType.SectionOver:
                    return sectionFrameData;
                default:
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            Array arrayframeType = Enum.GetValues(typeof(FrameType));
            m_arraykeyValue = new FieldValueTableInfo[arrayframeType.Length - 1];
            short keyIndex = 0;
            for (short frameTypeIndex = 0; frameTypeIndex < arrayframeType.Length; frameTypeIndex++, keyIndex++) {
                FrameType frameType = (FrameType)arrayframeType.GetValue(frameTypeIndex);
                if (frameType == FrameType.None) {
                    keyIndex--;
                    continue;
                }
                m_arraykeyValue[keyIndex] = new FieldValueTableInfo(frameType.ToString(), ValueType.Table);    
            }
            return m_arraykeyValue;
        }
        #endregion
    }

    public enum FrameType {
        None,
        Hit,
        PlayEffect,
        CacheBegin,
        SectionOver,
    }
}