using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct EffectFrameData : IFieldKeyTable {

        public ushort priority;
        public CustomData<EffectData> effectDatas;

        public EffectFrameData(ushort type, ushort priority, CustomData<EffectData> effectDatas) {
            this.priority = priority;
            this.effectDatas = effectDatas;
        }

        #region ITable Function
        public string GetTableName() => "EffectFrameData";
        public ushort GetLayer() => 5;
        public KeyType GetKeyType() => KeyType.Reference;
        public void SetKey(object key) { }
        public string GetKey() => FrameType.PlayEffect.ToString();
        public bool IsNullTable() => priority <= 0 || effectDatas.IsNullTable();
        public void Clear() {
            priority = 0;
            effectDatas.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        private const string Key_Priority = "priority";
        private const string Key_EffectData = "data";
        public void SetFieldKeyTableValue(string key, object value) {
            switch (key) {
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
                case Key_EffectData:
                    effectDatas = (CustomData<EffectData>)value;
                    return;
            }
        }

        public object GetFieldKeyTableValue(string key) {
            switch (key) {
                case Key_Priority:
                    return priority;
                case Key_EffectData:
                    return effectDatas;
                default:
                    Debug.LogError("EffectFrameData::GetFieldKeyTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[2];
            m_arraykeyValue[0] = new FieldKeyTable(Key_Priority, ValueType.Int);
            m_arraykeyValue[1] = new FieldKeyTable(Key_EffectData, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }
}