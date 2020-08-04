using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct PriorityFrameData : IFieldValueTable, ICommonFrameData {

        public FrameType frameType;
        public CommonFrameData commonData;

        #region ICommonFrameData Function

        public void SetPriority(ushort priority) => commonData.priority = priority;
        public void SetLoop(bool isLoop) => commonData.isLoop = isLoop;
        public bool GetLoop() => commonData.isLoop;
        #endregion

        #region ITable Function
        
        public string GetTableName() => "PriorityFrameData";
        public ushort GetLayer() => 5;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) => Enum.TryParse(key as string, false, out frameType);
        public string GetKey() => LuaTable.GetArrayKeyString(frameType);
        public bool IsNullTable() => commonData.IsNull();
        public void Clear() => commonData.Clear();

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 7));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function

        public void SetFieldValueTableValue(string key, object value) => commonData.SetFieldValueTableValue(key, value);

        public object GetFieldValueTableValue(string key) => commonData.GetFieldValueTableValue(key);

        public FieldValueTableInfo[] GetFieldValueTableInfo() => commonData.GetFieldValueTableInfo();
        #endregion
    }
}