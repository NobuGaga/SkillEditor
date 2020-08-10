using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct TrackChangeData : IFieldValueTable {
        
        public uint id;
        public uint replaceID;

        #region ITable Function
        
        public string GetTableName() => "TrackChangeData";
        public ushort GetLayer() => 6;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => CommonFrameData.Key_Data;
        public bool IsNullTable() => id == 0 || replaceID == 0;
        public void Clear() {
            id = 0;
            replaceID = 0;
        }

        private static StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion
    
        #region IFieldKeyTable Function

        private const string Key_ReplaceID = "replace_id";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case CommonFrameData.Key_ID:
                    id = (uint)(int)value;
                    return;
                case Key_ReplaceID:
                    replaceID = (uint)(int)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case CommonFrameData.Key_ID:
                    return id;
                case Key_ReplaceID:
                    return replaceID;
                default:
                    Debug.LogError("TrackChangeData::GetFieldValueTableValue key is not exit. key " + key);
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
            m_arraykeyValue[count++] = new FieldValueTableInfo(CommonFrameData.Key_ID, ValueType.Int);
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_ReplaceID, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}