using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct EffectRotationData : IFieldValueTable {

        public short x;
        public short y;
        public short z;

        private static Vector3 m_rotation;

        public Vector3 Rotation {
            set {
                x = (short)value.x;
                y = (short)value.y;
                z = (short)value.z;
            }
            get {
                m_rotation.x = x;
                m_rotation.y = y;
                m_rotation.z = z;
                return m_rotation;
            }
        }

        #region ITable Function

        public string GetTableName() => "EffectRotationData";
        public ushort GetLayer() => 9;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => "rotation";
        public bool IsNullTable() => Rotation == Vector3.zero;
        public void Clear() => Rotation = Vector3.zero;

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
            const ushort length = 3;
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_X, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Y, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Z, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}