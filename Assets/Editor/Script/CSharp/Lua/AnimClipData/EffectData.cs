using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct EffectData : IFieldKeyTable {

        private ushort index;
        public short type;
        public int id;

        public EffectData(short type, int id) {
            index = 0;
            this.type = type;
            this.id = id;
        }

        #region ITable Function
        public string GetTableName() => "EffectData";
        public ushort GetLayer() => 7;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() => type == 0 || id == 0;
        public void Clear() => id = type = 0;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        private const string Key_Type = "type";
        private const string Key_Id = "id";

        public void SetFieldKeyTableValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (short)(int)value;
                    return;
                case Key_Id:
                    id = (int)value;
                    return;
            }
        }

        public object GetFieldKeyTableValue(string key) {
            switch (key) {
                case Key_Type:
                    return type;
                case Key_Id:
                    return id;
                default:
                    Debug.LogError("EffectData::GetFieldKeyTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[2];
            m_arraykeyValue[0] = new FieldKeyTable(Key_Type, ValueType.Int);
            m_arraykeyValue[1] = new FieldKeyTable(Key_Id, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}