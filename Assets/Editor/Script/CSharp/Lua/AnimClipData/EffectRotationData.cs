using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct EffectRotationData : IFieldValueTable {

        public short x;
        public short y;
        public short z;

        public EffectRotationData(short x, short y, short z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        #region ITable Function

        public string GetTableName() => "EffectRotationData";
        public ushort GetLayer() => 9;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => "rotation";
        public bool IsNullTable() => x == 0 && y == 0 && z == 0;
        public void Clear() => x = y = z = 0;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_X = "x";
        private const string Key_Y = "y";
        private const string Key_Z = "z";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (short)(int)value;
                    return;
                case Key_Y:
                    y = (short)(int)value;
                    return;
                case Key_Z:
                    z = (short)(int)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_X:
                    return x;
                case Key_Y:
                    return y;
                case Key_Z:
                    return z;
                default:
                    Debug.LogError("EffectRotationData::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldValueTableInfo[3];
            m_arraykeyValue[0] = new FieldValueTableInfo(Key_X, ValueType.Int);
            m_arraykeyValue[1] = new FieldValueTableInfo(Key_Y, ValueType.Int);
            m_arraykeyValue[2] = new FieldValueTableInfo(Key_Z, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}