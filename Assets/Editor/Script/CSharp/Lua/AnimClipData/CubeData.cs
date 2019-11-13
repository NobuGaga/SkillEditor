using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct CubeData : IFieldKeyTable {

        private ushort index;
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public short depth;

        public CubeData(float x, float y, float z, float width, float height, short depth) {
            index = 0;
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        private static Vector3 m_offsetCache;
        private static Vector3 m_sizeCache;

        public Vector3 Offset {
            get {
                m_offsetCache.x = x;
                m_offsetCache.y = y;
                m_offsetCache.z = z;
                return m_offsetCache;
            }
        }

        public Vector3 Size {
            get {
                m_sizeCache.x = width;
                m_sizeCache.y = height;
                m_sizeCache.z = depth;
                return m_sizeCache;
            }
        }

        #region ITable Function
        public string GetTableName() => "CubeData";
        public ushort GetLayer() => 7;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() => Size == Vector3.zero;
        public void Clear() => x = y = z = width = height = depth = 0;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        private const string Key_X = "x";
        private const string Key_Y = "y";
        private const string Key_Z = "z";
        private const string Key_Width = "width";
        private const string Key_Height = "height";
        private const string Key_Depth = "depth";
        
        public void SetFieldKeyTableValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (float)value;
                    return;
                case Key_Y:
                    y = (float)value;
                    return;
                case Key_Z:
                    z = (float)value;
                    return;
                case Key_Width:
                    width = (float)value;
                    return;
                case Key_Height:
                    height = (float)value;
                    return;
                case Key_Depth:
                    depth = (short)(int)value;
                    return;
            }
        }

        public object GetFieldKeyTableValue(string key) {
            switch (key) {
                case Key_X:
                    return x;
                case Key_Y:
                    return y;
                case Key_Z:
                    return z;
                case Key_Width:
                    return width;
                case Key_Height:
                    return height;
                case Key_Depth:
                    return depth;
                default:
                    Debug.LogError("CubeData::GetFieldKeyTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[6];
            m_arraykeyValue[0] = new FieldKeyTable(Key_X, ValueType.Number);
            m_arraykeyValue[1] = new FieldKeyTable(Key_Y, ValueType.Number);
            m_arraykeyValue[2] = new FieldKeyTable(Key_Z, ValueType.Number);
            m_arraykeyValue[3] = new FieldKeyTable(Key_Width, ValueType.Number);
            m_arraykeyValue[4] = new FieldKeyTable(Key_Height, ValueType.Number);
            m_arraykeyValue[5] = new FieldKeyTable(Key_Depth, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}