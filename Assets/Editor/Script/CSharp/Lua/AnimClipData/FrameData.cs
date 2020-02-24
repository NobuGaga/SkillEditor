using System;
using System.Text;
using System.Collections.Generic;

namespace Lua.AnimClipData {

    public struct FrameData : IFieldValueTable, ILuaMultipleFileStructure<AnimClipData, FileType, FrameData> {

        public ushort index;
        public float time;
        public HitFrameData hitFrameData;
        public PriorityFrameData trackFrameData;
        public EffectFrameData effectFrameData;
        public PriorityFrameData cacheFrameData;
        public PriorityFrameData sectionFrameData;
        public CameraFrameData cameraFrameData;

        #region ITable Function
        
        public string GetTableName() => "FrameData";
        public ushort GetLayer() => 4;
        public ReadType GetReadType() => ReadType.RepeatToFixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)(int)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() {
            bool isNullTable = true;
            Array array = Enum.GetValues(typeof(FrameType));
            for (short index = 0; index < array.Length; index++) {
                FrameType frameType = (FrameType)array.GetValue(index);
                ITable table = (ITable)GetFieldValueTableValue(frameType.ToString());
                if (!table.IsNullTable()) {
                    isNullTable = false;
                    break;
                }
            }
            return isNullTable;
        }
        public void Clear() {
            time = 0;
            hitFrameData.Clear();
            trackFrameData.Clear();
            effectFrameData.Clear();
            cacheFrameData.Clear();
            sectionFrameData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 11));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, GetFileTypeTable());
        #endregion

        #region IFieldKeyTable Function

        private const string Key_Time = "time";
        private static Dictionary<string, FrameType> m_dicKeyToFrameType;
        
        public void SetFieldValueTableValue(string key, object value) {
            if (key == Key_Time) {
                time = (float)value;
                return;
            }
            FrameType frameType = GetFrameTypeFromKey(key);
            switch (frameType) {
                case FrameType.Hit:
                    hitFrameData = (HitFrameData)value;
                    return;
                case FrameType.Track:
                    trackFrameData = (PriorityFrameData)value;
                    trackFrameData.frameType = FrameType.Track;
                    return;
                case FrameType.PlayEffect:
                    effectFrameData = (EffectFrameData)value;
                    return;
                case FrameType.CacheBegin:
                    cacheFrameData = (PriorityFrameData)value;
                    cacheFrameData.frameType = FrameType.CacheBegin;
                    return;
                case FrameType.SectionOver:
                    sectionFrameData = (PriorityFrameData)value;
                    sectionFrameData.frameType = FrameType.SectionOver;
                    return;
                case FrameType.Camera:
                    cameraFrameData = (CameraFrameData)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            if (key == Key_Time)
                return time;
            FrameData data = GetFileTypeTable();
            FrameType frameType = GetFrameTypeFromKey(key);
            switch (frameType) {
                case FrameType.Hit:
                    return data.hitFrameData;
                case FrameType.Track:
                    return data.trackFrameData;
                case FrameType.PlayEffect:
                    return data.effectFrameData;
                case FrameType.CacheBegin:
                    return data.cacheFrameData;
                case FrameType.SectionOver:
                    return data.sectionFrameData;
                case FrameType.Camera:
                    return data.cameraFrameData;
                default:
                    return null;
            }
        }

        private FrameType GetFrameTypeFromKey(string key) {
            if (!Enum.TryParse(key, false, out FrameType frameType) && !m_dicKeyToFrameType.ContainsKey(key)) {
                UnityEngine.Debug.LogError("FrameData::GetFieldValueTableValue not exit key : " + key);
                return default;
            }
            if (m_dicKeyToFrameType.ContainsKey(key))
                frameType = m_dicKeyToFrameType[key];
            return frameType;
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            Array arrayframeType = Enum.GetValues(typeof(FrameType));
            m_dicKeyToFrameType = new Dictionary<string, FrameType>(arrayframeType.Length);
            m_arraykeyValue = new FieldValueTableInfo[arrayframeType.Length + 1];
            m_arraykeyValue[0] = new FieldValueTableInfo(Key_Time, ValueType.Number);
            for (short index = 0; index < arrayframeType.Length; index++) {
                FrameType frameType = (FrameType)arrayframeType.GetValue(index);
                string key = LuaTable.GetArrayKeyString(frameType);
                m_arraykeyValue[index + 1] = new FieldValueTableInfo(key, ValueType.Table);    
                m_dicKeyToFrameType.Add(key, frameType);
            }
            return m_arraykeyValue;
        }
        #endregion
    
        #region ILuaMultipleFileStructure Function

        public AnimClipData GetRootTableType() => default;
        public FileType GetFileType() => GetRootTableType().GetFileType();
        public FrameData GetFileTypeTable() {
            if (GetFileType() == FileType.Client)
                return this;
            var dataCopy = this;
            dataCopy.effectFrameData.Clear();
            dataCopy.cameraFrameData.Clear();
            return dataCopy;
        }
        #endregion
    }

    public enum FrameType {
        Hit = 4,
        Track = 5,
        PlayEffect = 6,
        CacheBegin = 8,
        SectionOver = 9,
        Camera = 12,
    }
}