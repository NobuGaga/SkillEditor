using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct BuffData : IFieldValueTable {

        public ushort index;
        public BuffType type;
        public uint id;

        #region ITable Function
        
        public string GetTableName() => "BuffData";
        public ushort GetLayer() => 7;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)(int)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() => id == 0;
        public void Clear() => id = 0;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_Type = "type";
        private const string Key_Id = "id";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    if (!Enum.TryParse(value.ToString(), false, out BuffType buffType))
                        Debug.LogError("BuffData::SetFieldValueTableValue value type is not a BuffType");
                    type = buffType;
                    return;
                case Key_Id:
                    id = (uint)(int)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Type:
                    return (ushort)type;
                case Key_Id:
                    return id;
                default:
                    Debug.LogError("BuffData::GetFieldValueTableValue key is not exit. key " + key);
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
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Type, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Id, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }

    public enum BuffType {
        Self = 0,
        Attacker = 1,
        Hurter = 2,
    }
}