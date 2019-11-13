using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct FrameData : IFieldKeyTable {

        private ushort index;
        public float time;
        public HitFrameData hitFrameData;
        public EffectFrameData effectFrameData;
        public CacheFrameData cacheFrameData;
        public SectionFrameData sectionFrameData;
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

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 11));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        public void SetFieldKeyTableValue(string key, object value) {
            FrameType frameType = FrameType.None;
            if (!Enum.TryParse(key, false, out frameType)) {
                UnityEngine.Debug.LogError("FrameData::SetFieldKeyTableValue not exit key : " + key);
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
                    cacheFrameData = (CacheFrameData)value;
                    return;
                case FrameType.SectionOver:
                    sectionFrameData = (SectionFrameData)value;
                    return;
            }
        }

        public object GetFieldKeyTableValue(string key) {
            FrameType frameType = FrameType.None;
            if (!Enum.TryParse(key, false, out frameType)) {
                UnityEngine.Debug.LogError("FrameData::GetFieldKeyTableValue not exit key : " + key);
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

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            Array arrayframeType = Enum.GetValues(typeof(FrameType));
            m_arraykeyValue = new FieldKeyTable[arrayframeType.Length - 1];
            short keyIndex = 0;
            for (short frameTypeIndex = 0; frameTypeIndex < arrayframeType.Length; frameTypeIndex++, keyIndex++) {
                FrameType frameType = (FrameType)arrayframeType.GetValue(frameTypeIndex);
                if (frameType == FrameType.None) {
                    keyIndex--;
                    continue;
                }
                m_arraykeyValue[keyIndex] = new FieldKeyTable(frameType.ToString(), ValueType.Table);    
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