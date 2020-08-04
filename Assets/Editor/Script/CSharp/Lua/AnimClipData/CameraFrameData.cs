using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct CameraFrameData : IFieldValueTable, ICommonFrameData {

        public CommonFrameData commonData;
        public CameraData cameraData;

        #region ICommonFrameData Function

        public void SetPriority(ushort priority) => commonData.priority = priority;
        public void SetLoop(bool isLoop) => commonData.isLoop = isLoop;
        public bool GetLoop() => commonData.isLoop;
        #endregion

        #region ITable Function
        
        public string GetTableName() => "CameraFrameData";
        public ushort GetLayer() => 5;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => LuaTable.GetArrayKeyString(FrameType.Camera);
        public bool IsNullTable() => commonData.IsNull() || cameraData.IsNullTable();
        public void Clear() {
            commonData.Clear();
            cameraData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case CommonFrameData.Key_Data:
                    cameraData = (CameraData)value;
                    return;
            }
            commonData.SetFieldValueTableValue(key, value);
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case CommonFrameData.Key_Data:
                    return cameraData;
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