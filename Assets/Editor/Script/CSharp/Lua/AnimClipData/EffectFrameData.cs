using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct EffectFrameData : IFieldValueTable {

        public ushort priority;
        public CustomData<EffectData> effectData;

        public EffectFrameData(ushort type, ushort priority, CustomData<EffectData> effectDatas) {
            this.priority = priority;
            this.effectData = effectDatas;
        }

        #region ITable Function
        
        public string GetTableName() => "EffectFrameData";
        public ushort GetLayer() => 6;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => LuaTable.GetArrayKeyString(FrameType.PlayEffect);
        public bool IsNullTable() => priority <= 0 || effectData.IsNullTable();
        public void Clear() {
            priority = 0;
            effectData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        private const string Key_Priority = PriorityFrameData.Key_Priority;
        private const string Key_EffectData = "data";
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
                case Key_EffectData:
                    effectData = (CustomData<EffectData>)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Priority:
                    return priority;
                case Key_EffectData:
                    return effectData;
                default:
                    Debug.LogError("EffectFrameData::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            const ushort length = 2;
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Priority, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_EffectData, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }
}