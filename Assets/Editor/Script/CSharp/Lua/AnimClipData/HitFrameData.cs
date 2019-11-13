using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct HitFrameData : IFieldKeyTable {

        public ushort type;
        public ushort priority;
        public CustomData<CubeData> cubeDatas;

        public HitFrameData(ushort type, ushort priority, CustomData<CubeData> cubeDatas) {
            this.type = type;
            this.priority = priority;
            this.cubeDatas = cubeDatas;
        }

        #region ITable Function
        public string GetTableName() => "HitFrameData";
        public ushort GetLayer() => 5;
        public KeyType GetKeyType() => KeyType.Reference;
        public void SetKey(object key) { }
        public string GetKey() => FrameType.Hit.ToString();
        public bool IsNullTable() => priority <= 0 || cubeDatas.IsNullTable();
        public void Clear() {
            priority = 0;
            cubeDatas.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        private const string Key_Type = "type";
        private const string Key_Priority = "priority";
        private const string Key_CubeData = "data";
        public void SetFieldKeyTableValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (ushort)(int)value;
                    return;
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
                case Key_CubeData:
                    cubeDatas = (CustomData<CubeData>)value;
                    return;
            }
        }

        public object GetFieldKeyTableValue(string key) {
            switch (key) {
                case Key_Type:
                    return type;
                case Key_Priority:
                    return priority;
                case Key_CubeData:
                    return cubeDatas;
                default:
                    Debug.LogError("HitFrameData::GetFieldKeyTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[3];
            m_arraykeyValue[0] = new FieldKeyTable(Key_Type, ValueType.Int);
            m_arraykeyValue[1] = new FieldKeyTable(Key_Priority, ValueType.Int);
            m_arraykeyValue[2] = new FieldKeyTable(Key_CubeData, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }
}