using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct BuffFrameData : IFieldValueTable {

        public ushort priority;
        public CustomData<BuffData> buffData;

        #region ITable Function
        
        public string GetTableName() => "BuffFrameData";
        public ushort GetLayer() => 5;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => LuaTable.GetArrayKeyString(FrameType.Buff);
        public bool IsNullTable() => priority <= 0 || buffData.IsNullTable();
        public void Clear() {
            priority = 0;
            buffData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        private const string Key_Priority = PriorityFrameData.Key_Priority;
        private const string Key_BuffData = "data";
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
                case Key_BuffData:
                    buffData = (CustomData<BuffData>)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Priority:
                    return priority;
                case Key_BuffData:
                    return buffData;
                default:
                    Debug.LogError("BuffFrameData::GetFieldValueTableValue key is not exit. key " + key);
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
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_BuffData, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }
}