using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct UngrabData : IFieldValueTable {

        public float grabState;
        public float horizontalSpeed;

        #region ITable Function
        
        public string GetTableName() => "UngrabData";
        public ushort GetLayer() => 6;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => CommonFrameData.Key_Data;
        public bool IsNullTable() => grabState == 0 && horizontalSpeed == 0;
        public void Clear() {
            grabState = 0;
            horizontalSpeed = 0;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        private const string Key_GravityAccelerate = "iAccelV";
        private const string Key_HorizontalSpeed = "iSpdH";
        
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_GravityAccelerate:
                    grabState = (float)value;
                    return;
                case Key_HorizontalSpeed:
                    horizontalSpeed = (float)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_GravityAccelerate:
                    return grabState;
                case Key_HorizontalSpeed:
                    return horizontalSpeed;
                default:
                    Debug.LogError("UngrabData::GetFieldValueTableValue key is not exit. key " + key);
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
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_GravityAccelerate, ValueType.Number);
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_HorizontalSpeed, ValueType.Number);
            return m_arraykeyValue;
        }
        #endregion
    }
}