using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct ClipGroupData : IFieldValueTable {

        public uint id;
        public string clipName;
        public FrameListData frameList;

        #region ITable Function
        
        public string GetTableName() => "ClipGroupData";
        public ushort GetLayer() => 2;
        public ReadType GetReadType() => ReadType.RepeatToFixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => id = (uint)(int)key;
        public string GetKey() => id.ToString();
        public bool IsNullTable() => id == 0 || string.IsNullOrEmpty(clipName);
        public void Clear() {
            id = 0;
            clipName = null;
            frameList.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 14));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        private const string Key_Name = "name";
        private const string Key_FrameList = "frameList";
        
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Name:
                    clipName = value as string;
                    break;
                case Key_FrameList:
                    frameList = (FrameListData)value;
                    break;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Name:
                    return clipName;
                case Key_FrameList:
                    return frameList;
                default:
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
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Name, ValueType.String);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_FrameList, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }
}