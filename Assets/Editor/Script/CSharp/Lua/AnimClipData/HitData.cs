using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct HitData : IFieldValueTable {

        public ushort index;
        public CubeData cubeData;
        public int crush;

        #region ITable Function

        public string GetTableName() => "HitData";
        public ushort GetLayer() => 7;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)(int)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() => cubeData.IsNull();
        public void Clear() => cubeData.Clear();

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_Crush = "crush";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Crush:
                    crush = (int)value;
                    return;
            }
            cubeData.SetFieldValueTableValue(key, value);
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Crush:
                    return crush;
            }
            return cubeData.GetFieldValueTableValue(key);
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            FieldValueTableInfo[] cubeValues = cubeData.GetFieldValueTableInfo();
            ushort length = (ushort)(1 + cubeValues.Length);
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            for (ushort index = 0; index < cubeValues.Length; index++)
                m_arraykeyValue[count++] = cubeValues[index];
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_Crush, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}