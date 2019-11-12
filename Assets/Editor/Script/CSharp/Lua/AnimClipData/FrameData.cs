using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct FrameData : IFieldKeyTable {

        public ushort index;
        public float time;
        public HitFrameData hitFrameData;
        public EffectFrameData effectFrameData;
        public CacheFrameData cacheFrameData;
        public SectionFrameData sectionFrameData;
        public FrameData(float time) {
            this.time = time;
        }

        public ushort GetLayer() => 3;
        public KeyType GetKeyType() => KeyType.Field;
        public bool IsNullTable() => time < 0;

        public void Clear() {
            time = -1;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 11));
        public override string ToString() {
            return string.Empty;
        }

        public void SetOuterField(object outerKey) => index = (ushort)outerKey;

        public KeyType GetOuterField() => KeyType.Array;

        public void SetFieldKeyTable(string key, object value) {
            FrameType frameType = FrameType.None;
            if (!Enum.TryParse(key, false, out frameType)) {
                UnityEngine.Debug.LogError("FrameData::SetFieldKeyTable not exit key : " + key);
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
    }

    public enum FrameType {
        None,
        Hit,
        PlayEffect,
        CacheBegin,
        SectionOver,
    }
}