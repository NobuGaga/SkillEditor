using UnityEngine;
using System;
using System.Text;
using SkillEditor;

namespace Lua.EffectConf {

    public struct EffectConfTransform : IFieldValueTable {

        public TransformType type;
        public float x;
        public float y;
        public float z;

        private static Vector3 m_vector3;

        public Vector3 Vector {
            set {
                x = (float)value.x;
                y = (float)value.y;
                z = (float)value.z;
            }
            get {
                m_vector3.x = x;
                m_vector3.y = y;
                m_vector3.z = z;
                return m_vector3;
            }
        }

        public string VectorString => Tool.GetCacheString(string.Format("{0} x:{1} y:{2} z:{3}", type, x, y, z));

        #region ITable Function

        public string GetTableName() => "EffectConfTransform";
        public ushort GetLayer() => 2;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => 't' + type.ToString();
        public bool IsNullTable() => Vector == Vector3.zero;
        public void Clear() => Vector = Vector3.zero;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 5));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_X = "[1]";
        private const string Key_Y = "[2]";
        private const string Key_Z = "[3]";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (float)value;
                    return;
                case Key_Y:
                    y = (float)value;
                    return;
                case Key_Z:
                    z = (float)value;
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
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_X, ValueType.Number);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Y, ValueType.Number);
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_Z, ValueType.Number);
            return m_arraykeyValue;
        }
        #endregion
    }

    public enum TransformType {

        Offset,
        Scale,
        Rotation,
    }
}