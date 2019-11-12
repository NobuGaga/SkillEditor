using UnityEngine;
using System;
using System.Text;
using Lua;
using FieldKeyTable = Lua.LuaFormat.FieldKeyTable;

namespace SkillEditor.LuaStructure {

    internal enum AnimClipLuaLayer {
        EnterTable = 0,
        Model = 1,
        State = 2,
        Clip = 3,
        FrameGroup = 4,
        FrameType = 5,
        FrameData = 6,
        CustomeData = 7,
        Effect = 8,
        Rect = 9,
    }

    internal struct CustomData : INullTable {
        public int index;
        public object data;

        public CustomData(int index, object data) {
            this.index = index;
            this.data = data;
        }

        public bool IsNullTable() {
            if (data == null)
                return true;
            if (data is EffectData)
                return ((EffectData)data).IsNullTable();
            if (data is CubeData[]) {
                CubeData[] array = data as CubeData[];
                for (int i = 0; i < array.Length; i++)
                    if (!array[i].IsNullTable())
                        return false;
                return true;
            }
            return true;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.RectDataListStringLength);
        public override string ToString() {
            string dataString = string.Empty;
            string tabString;
            string format;
            if (data is EffectData && !((EffectData)data).IsNullTable())
                dataString = ((EffectData)data).ToString();
            else if (data is CubeData[]) {
                index = CubeDataIndex;
                CubeData[] array = data as CubeData[];
                tabString = Tool.GetTabString(AnimClipLuaLayer.Effect);
                format = "{0}[{1}] = {2}\n{3}{0}{4},\n";
                m_staticBuilder.Clear();
                m_staticBuilder.Append(LuaFormat.LineSymbol);
                for (int i = 0; i < array.Length; i++) {
                    if (array[i].IsNullTable())
                        continue;
                    m_staticBuilder.Append(string.Format(format,
                                                         tabString,
                                                         i + 1,
                                                         LuaFormat.CurlyBracesPair.start,
                                                         array[i].ToString(),
                                                         LuaFormat.CurlyBracesPair.end));
                }
                dataString = m_staticBuilder.ToString();
            }
            tabString = Tool.GetTabString(AnimClipLuaLayer.CustomeData);
            format = "{0}[{1}] = {2}{3}{0}{4},\n";
            string toString = string.Format(format,
                                            tabString,
                                            index,
                                            LuaFormat.CurlyBracesPair.start,
                                            dataString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
        }

        public const short CubeDataIndex = 4;
    }

    internal struct EffectData : ITable, INullTable {
        public short type;
        public int id;

        public EffectData(short type, int id) {
            this.type = type;
            this.id = id;
        }

        public bool IsNullTable() {
            return type == 0 || id == 0;
        }

        public override string ToString() {
            string tabString = Tool.GetTabString(AnimClipLuaLayer.Effect);
            string format = "\n{0}type = {1},\n{0}id = {2},\n";
            string toString = string.Format(format, tabString, type, id);
            return Tool.GetCacheString(toString);
        }

        public void SetFieldKeyTable(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (short)(int)value;
                    return;
                case Key_Id:
                    id = (int)value;
                    return;
            }
        }

        private const string Key_Type = "type";
        private const string Key_Id = "id";

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[2];
            m_arraykeyValue[0] = new FieldKeyTable(Key_Type, LuaFormat.ValueType.Int);
            m_arraykeyValue[1] = new FieldKeyTable(Key_Id, LuaFormat.ValueType.Int);
            return m_arraykeyValue;
        }
    }

    internal struct CubeData : ITable, INullTable {
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public short depth;

        public CubeData(float x, float y, float z, float width, float height, short depth) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public bool IsNullTable() {
            return Size == Vector3.zero;
        }

        public override string ToString() {
            string tabString = Tool.GetTabString(AnimClipLuaLayer.Rect);
            string format = "{0}x = {1},\n{0}y = {2},\n{0}z = {3},\n{0}width = {4},\n{0}height = {5},\n{0}depth = {6},\n";
            string toString = string.Format(format, tabString, x, y, z, width, height, depth);
            return Tool.GetCacheString(toString);
        }

        public void SetFieldKeyTable(string key, object value) {
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

        private const string Key_X = "x";
        private const string Key_Y = "y";
        private const string Key_Z = "z";
        private const string Key_Width = "width";
        private const string Key_Height = "height";
        private const string Key_Depth = "depth";

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetFieldKeyTables() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[6];
            m_arraykeyValue[0] = new FieldKeyTable(Key_X, LuaFormat.ValueType.Number);
            m_arraykeyValue[1] = new FieldKeyTable(Key_Y, LuaFormat.ValueType.Number);
            m_arraykeyValue[2] = new FieldKeyTable(Key_Z, LuaFormat.ValueType.Number);
            m_arraykeyValue[3] = new FieldKeyTable(Key_Width, LuaFormat.ValueType.Number);
            m_arraykeyValue[4] = new FieldKeyTable(Key_Height, LuaFormat.ValueType.Number);
            m_arraykeyValue[5] = new FieldKeyTable(Key_Depth, LuaFormat.ValueType.Int);
            return m_arraykeyValue;
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
    }
}