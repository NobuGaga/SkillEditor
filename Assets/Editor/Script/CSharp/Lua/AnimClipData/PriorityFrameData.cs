using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct PriorityFrameData : IFieldKeyTable {

        private FrameType frameType;
        public ushort priority;

        public PriorityFrameData(FrameType frameType, ushort priority) {
            this.frameType = frameType;
            this.priority = priority;
        }

        #region ITable Function
        public string GetTableName() => "PriorityFrameData";
        public ushort GetLayer() => 5;
        public KeyType GetKeyType() => KeyType.Reference;
        public void SetKey(object key) => Enum.TryParse(key as string, false, out frameType);
        public string GetKey() => frameType.ToString();
        public bool IsNullTable() => priority <= 0;
        public void Clear() => priority = 0;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 7));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        private const string Key_Priority = "priority";
        public void SetFieldKeyTableValue(string key, object value) {
            switch (key) {
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
            }
        }

        public object GetFieldKeyTableValue(string key) {
            switch (key) {
                case Key_Priority:
                    return priority;
                default:
                    Debug.LogError("PriorityFrameData::GetFieldKeyTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[1];
            m_arraykeyValue[0] = new FieldKeyTable(Key_Priority, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}