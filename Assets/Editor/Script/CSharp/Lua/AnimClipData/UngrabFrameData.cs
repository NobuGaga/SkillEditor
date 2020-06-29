using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct UngrabFrameData : IFieldValueTable {

        public ushort priority;
        public UngrabData ungrabData;

        #region ITable Function
        
        public string GetTableName() => "UngrabFrameData";
        public ushort GetLayer() => 5;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => LuaTable.GetArrayKeyString(FrameType.Ungrab);
        public bool IsNullTable() => priority <= 0 || ungrabData.IsNullTable();
        public void Clear() {
            priority = 0;
            ungrabData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        private const string Key_Priority = PriorityFrameData.Key_Priority;
        private const string Key_UngrabData = "data";
        
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
                case Key_UngrabData:
                    ungrabData = (UngrabData)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Priority:
                    return priority;
                case Key_UngrabData:
                    return ungrabData;
                default:
                    Debug.LogError("UngrabFrameData::GetFieldValueTableValue key is not exit. key " + key);
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
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_UngrabData, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }
}