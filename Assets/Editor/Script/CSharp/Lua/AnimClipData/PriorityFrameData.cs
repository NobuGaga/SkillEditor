using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct PriorityFrameData : IFieldValueTable {

        public FrameType frameType;
        public ushort priority;

        public PriorityFrameData(FrameType frameType, ushort priority) {
            this.frameType = frameType;
            this.priority = priority;
        }

        #region ITable Function
        public string GetTableName() => "PriorityFrameData";
        public ushort GetLayer() => 5;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) => Enum.TryParse(key as string, false, out frameType);
        public string GetKey() => frameType.ToString();
        public bool IsNullTable() => priority <= 0;
        public void Clear() => priority = 0;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 7));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        public const string Key_Priority = "priority";
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Priority:
                    return priority;
                default:
                    Debug.LogError("PriorityFrameData::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldValueTableInfo[1];
            m_arraykeyValue[0] = new FieldValueTableInfo(Key_Priority, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}