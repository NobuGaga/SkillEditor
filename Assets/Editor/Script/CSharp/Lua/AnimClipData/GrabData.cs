using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct GrabData : IFieldValueTable {

        public CubeData cubeData;

        #region ITable Function

        public string GetTableName() => "GrabData";
        public ushort GetLayer() => 6;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => "data";
        public bool IsNullTable() => cubeData.IsNull();
        public void Clear() => cubeData.Clear();

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        public void SetFieldValueTableValue(string key, object value) => cubeData.SetFieldValueTableValue(key, value);
        public object GetFieldValueTableValue(string key) => cubeData.GetFieldValueTableValue(key);
        public FieldValueTableInfo[] GetFieldValueTableInfo() => cubeData.GetFieldValueTableInfo();
        #endregion
    }
}