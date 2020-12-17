using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct IDData : IFieldValueTable {

        public uint id;

        #region ITable Function

        public string GetTableName() => "IDData";
        public ushort GetLayer() => 6;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => CommonFrameData.Key_Data;
        public bool IsNullTable() => id == 0;
        public void Clear() => id = 0;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case CommonFrameData.Key_ID:
                    id = (uint)(int)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case CommonFrameData.Key_ID:
                    return id;
                default:
                    Debug.LogError("IDData::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            const ushort length = 1;
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count] = new FieldValueTableInfo(CommonFrameData.Key_ID, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}