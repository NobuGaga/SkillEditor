using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct FrameData : IFieldValueTable {

        public ushort index;
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

        #region ITable Function
        
        public string GetTableName() => "FrameData";
        public ushort GetLayer() => 4;
        public ReadType GetReadType() => ReadType.RepeatToFixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)(int)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() {
            bool isNullTable = true;
            Array array = Enum.GetValues(typeof(FrameType));
            for (short index = 0; index < array.Length; index++) {
                FrameType frameType = (FrameType)array.GetValue(index);
                ITable table = (ITable)GetFieldValueTableValue(frameType.ToString());
                if (!table.IsNullTable()) {
                    isNullTable = false;
                    break;
                }
            }
            return isNullTable;
        }
        public void Clear() {
            time = 0;
            hitFrameData.Clear();
            effectFrameData.Clear();
            cacheFrameData.Clear();
            sectionFrameData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 11));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        private const string Key_Time = "time";
        
        public void SetFieldValueTableValue(string key, object value) {
            if (key == Key_Time) {
                time = (float)value;
                return;
            }
            if (!Enum.TryParse(key, false, out FrameType frameType)) {
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
                    cacheFrameData.frameType = FrameType.CacheBegin;
                    return;
                case FrameType.SectionOver:
                    sectionFrameData = (PriorityFrameData)value;
                    sectionFrameData.frameType = FrameType.SectionOver;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            if (key == Key_Time)
                return time;
            if (!Enum.TryParse(key, false, out FrameType frameType)) {
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
            m_arraykeyValue = new FieldValueTableInfo[arrayframeType.Length + 1];
            m_arraykeyValue[0] = new FieldValueTableInfo(Key_Time, ValueType.Number);
            for (short index = 0; index < arrayframeType.Length; index++) {
                FrameType frameType = (FrameType)arrayframeType.GetValue(index);
                m_arraykeyValue[index + 1] = new FieldValueTableInfo(frameType.ToString(), ValueType.Table);    
            }
            return m_arraykeyValue;
        }
        #endregion
    }

    public enum FrameType {
        Hit,
        PlayEffect,
        CacheBegin,
        SectionOver,
    }
}