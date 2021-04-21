using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct GrabData : IFieldValueTable, ICubeData {

        public ushort index;
        public CubeData cubeData;
        public string alignBonePoint;

        #region ICubeData Function

        public Vector3 GetOffset() => cubeData.Offset;
        public Vector3 GetSize() => cubeData.Size;
        public float GetWidth() => cubeData.width;
        public float GetHeight() => cubeData.height;
        #endregion

        #region ITable Function

        public string GetTableName() => "GrabData";
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

        private const string Key_AlignBonePoint = "alignBonePoint";

        public void SetFieldValueTableValue(string key, object value) {
            switch(key) {
                case Key_AlignBonePoint:
                    alignBonePoint = (string)value;
                    return;
            }
            cubeData.SetFieldValueTableValue(key, value);
        }

        public object GetFieldValueTableValue(string key) {
            switch(key) {
                case Key_AlignBonePoint:
                    return alignBonePoint;
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
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_AlignBonePoint, ValueType.String);
            return m_arraykeyValue;
        }
        #endregion
    }
}