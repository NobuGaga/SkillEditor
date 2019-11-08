using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct FrameData : ITable, FieldKeyTable {

        public float time;

        public FrameData(float time) {
            this.time = time;
        }

        public ushort GetLayer() => 3;
        public KeyType GetKeyType() => KeyType.Field;
        public bool IsNullTable() => time < 0;

        public void Clear() {
            time = -1;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 11));
        public override string ToString() {
            return string.Empty;
        }
    }

    internal enum FrameType {
        Hit,
        PlayEffect,
        CacheBegin,
        SectionOver,
    }

    internal struct KeyFrameData : ITable, INullTable {
        public int index;
        public FrameType frameType;
        public float time;
        public short priority;
        public CustomData[] dataList;

        public KeyFrameData(int index, FrameType frameType, float time, short priority, CustomData[] dataList) {
            this.index = index;
            this.frameType = frameType;
            this.time = time;
            this.priority = priority;
            this.dataList = dataList;
        }

        private void SetFrameType(string frameTypeString) {
            frameType = (FrameType)Enum.Parse(typeof(FrameType), frameTypeString);
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.CustomDataListStringLength);
        public override string ToString() {
            string dataListString = string.Empty;
            string format;
            string frameDataString = Tool.GetTabString(AnimClipLuaLayer.FrameData);
            if (dataList != null) {
                format = "{0}data = {1}{3}{2},\n";
                dataListString = Tool.GetArrayString(m_staticBuilder, AnimClipLuaLayer.FrameData, dataList);
                dataListString = string.Format(format, frameDataString,
                                               LuaFormat.CurlyBracesPair.start,
                                               LuaFormat.CurlyBracesPair.end,
                                               dataListString);
            }
            format = "{0}[{4}] = {2}\n{1}name = \"{5}\",\n{1}time = {6},\n{1}priority = {7},\n{8}{0}{3},\n";
            string frameTypeTabString = Tool.GetTabString(AnimClipLuaLayer.FrameType);
            string toString = string.Format(format, frameTypeTabString,
                                            frameDataString,
                                            LuaFormat.CurlyBracesPair.start,
                                            LuaFormat.CurlyBracesPair.end,
                                            index,
                                            frameType.ToString(),
                                            time,
                                            priority,
                                            dataListString);
            return Tool.GetCacheString(toString);
        }

        public bool IsNullTable() {
            return index == 0 || frameType == FrameType.None || priority == 0;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Name:
                    SetFrameType(value as string);
                    return;
                case Key_Time:
                    time = (float)value;
                    return;
                case Key_Priority:
                    priority = (short)(int)value;
                    return;
                case Key_Data:
                    dataList = value as CustomData[];
                    return;
            }
        }

        private const string Key_Name = "name";
        private const string Key_Time = "time";
        private const string Key_Priority = "priority";
        private const string Key_Data = "data";

        private static FieldKeyTable[] m_arraykeyValue;
        public FieldKeyTable[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldKeyTable[4];
            m_arraykeyValue[0] = new FieldKeyTable(Key_Name, LuaFormat.ValueType.String);
            m_arraykeyValue[1] = new FieldKeyTable(Key_Time, LuaFormat.ValueType.Number);
            m_arraykeyValue[2] = new FieldKeyTable(Key_Priority, LuaFormat.ValueType.Int);
            m_arraykeyValue[3] = new FieldKeyTable(Key_Data, LuaFormat.ValueType.Table);
            return m_arraykeyValue;
        }
    }
}