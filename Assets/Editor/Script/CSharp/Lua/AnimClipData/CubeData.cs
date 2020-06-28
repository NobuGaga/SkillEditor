using UnityEngine;
using System;

namespace Lua.AnimClipData {

    public struct CubeData {

        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public float depth;

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

        public bool IsNull() => Size == Vector3.zero;
        public void Clear() => x = y = z = width = height = depth = 0;

        private const string Key_X = "x";
        private const string Key_Y = "y";
        private const string Key_Z = "z";
        private const string Key_Width = "width";
        private const string Key_Height = "height";
        private const string Key_Depth = "depth";

        public bool SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (float)value;
                    return true;
                case Key_Y:
                    y = (float)value;
                    return true;
                case Key_Z:
                    z = (float)value;
                    return true;
                case Key_Width:
                    width = (float)value;
                    return true;
                case Key_Height:
                    height = (float)value;
                    return true;
                case Key_Depth:
                    depth = (float)value;
                    return true;
            }
            return false;
        }

        public object GetFieldValueTableValue(string key) {
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
                    Debug.LogError("CubeData::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            const ushort length = 6;
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_X, ValueType.Number);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Y, ValueType.Number);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Z, ValueType.Number);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Width, ValueType.Number);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Height, ValueType.Number);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Depth, ValueType.Number);
            return m_arraykeyValue;
        }
    }
}