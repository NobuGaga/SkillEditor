using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct IDFrameData : IFieldValueTable, ICommonFrameData {

        public FrameType frameType;
        public CommonFrameData commonData;
        public IDData idData;

        #region ICommonFrameData Function

        public void SetPriority(ushort priority) => commonData.priority = priority;
        public void SetLoop(bool isLoop) => commonData.isLoop = isLoop;
        public bool GetLoop() => commonData.isLoop;
        #endregion

        #region ITable Function
        
        public string GetTableName() => "IDFrameData";
        public ushort GetLayer() => 5;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) => Enum.TryParse(key as string, false, out frameType);
        public string GetKey() => LuaTable.GetArrayKeyString(frameType);
        public bool IsNullTable() => commonData.IsNull() || idData.IsNullTable();
        public void Clear() {
            commonData.Clear();
            idData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case CommonFrameData.Key_Data:
                    idData = (IDData)value;
                    return;
            }
            commonData.SetFieldValueTableValue(key, value);
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case CommonFrameData.Key_Data:
                    return idData;
            }
            return commonData.GetFieldValueTableValue(key);
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            FieldValueTableInfo[] commonFrameValues = commonData.GetFieldValueTableInfo();
            ushort length = (ushort)(1 + commonFrameValues.Length);
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            for (ushort index = 0; index < commonFrameValues.Length; index++)
                m_arraykeyValue[count++] = commonFrameValues[index];
            m_arraykeyValue[count] = new FieldValueTableInfo(CommonFrameData.Key_Data, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }
}