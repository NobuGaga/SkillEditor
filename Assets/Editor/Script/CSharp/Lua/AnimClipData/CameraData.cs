using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct CameraData : IFieldValueTable {
        
        public uint id;
        public CameraTriggerType triggerType;
        public CameraFocusType focusType;

        #region ITable Function
        
        public string GetTableName() => "CameraData";
        public ushort GetLayer() => 7;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => "data";
        public bool IsNullTable() => id == 0;
        public void Clear() => id = 0;

        private static StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion
    
        #region IFieldKeyTable Function
        
        private const string Key_Id = "id";
        private const string Key_Type = "type";
        private const string Key_Focus = "focus";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Id:
                    id = (uint)(int)value;
                    return;
                case Key_Type:
                    if (!Enum.TryParse(value.ToString(), false, out CameraTriggerType triggerType))
                        Debug.LogError("CameraData::SetFieldValueTableValue value type is not a CameraTriggerType");
                    this.triggerType = triggerType;
                    return;
                case Key_Focus:
                    if (!Enum.TryParse(value.ToString(), false, out CameraFocusType focusType))
                        Debug.LogError("CameraData::SetFieldValueTableValue value type is not a CameraFocusType");
                    this.focusType = focusType;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Id:
                    return id;
                case Key_Type:
                    return (ushort)triggerType;
                case Key_Focus:
                    return (ushort)focusType;
                default:
                    Debug.LogError("CameraData::GetFieldValueTableValue key is not exit. key " + key);
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
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Id, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Type, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Focus, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion

    }
    public enum CameraTriggerType {
        Time = 1,
        Hit = 2,
    }

    public enum CameraFocusType {
        Attacker = 1,
        Hurter = 2,
    }
}