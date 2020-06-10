using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace Lua.AnimClipData {

    public struct FrameData : IFieldValueTable, ILuaMultipleFileStructure<AnimClipData, FileType, FrameData> {

        public ushort index;
        public float time;
        public float endTime;
        public HitFrameData hitFrameData;
        public PriorityFrameData trackFrameData;
        public EffectFrameData effectFrameData;
        public PriorityFrameData cacheFrameData;
        public PriorityFrameData sectionFrameData;
        public CameraFrameData cameraFrameData;
        public BuffFrameData buffFrameData;
        public PriorityFrameData airBeginFrameData;
        public PriorityFrameData airEndFrameData;
        public PriorityFrameData dodgeFrameData;

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
            cameraFrameData.Clear();
            buffFrameData.Clear();
            airBeginFrameData.Clear();
            airEndFrameData.Clear();
            dodgeFrameData.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 11));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, GetFileTypeTable());
        #endregion

        #region IFieldKeyTable Function

        private const string Key_Time = "time";
        private const string Key_EndTime = "endTime";
        
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Time:
                    time = (float)value;
                    return;
                case Key_EndTime:
                    endTime = (float)value;
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
                case FrameType.Buff:
                    buffFrameData = (BuffFrameData)value;
                    return;
                case FrameType.OverheadStart:
                    airBeginFrameData = (PriorityFrameData)value;
                    airBeginFrameData.frameType = FrameType.OverheadStart;
                    return;
                case FrameType.OverheadBreak:
                    airEndFrameData = (PriorityFrameData)value;
                    airEndFrameData.frameType = FrameType.OverheadBreak;
                    return;
                case FrameType.DodgeBreak:
                    dodgeFrameData = (PriorityFrameData)value;
                    dodgeFrameData.frameType = FrameType.DodgeBreak;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Time:
                    return time;
                case Key_EndTime:
                    if (endTime <= time)
                        return null;
                    return endTime;
            }
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
                case FrameType.Buff:
                    return data.buffFrameData;
                case FrameType.OverheadStart:
                    return data.airBeginFrameData;
                case FrameType.OverheadBreak:
                    return data.airEndFrameData;
                case FrameType.DodgeBreak:
                    return data.dodgeFrameData;
            }
            Debug.LogError("FrameData::GetFieldValueTableValue key is not exit. key " + key);
            return null;
        }

        private static Dictionary<string, FrameType> m_dicKeyToFrameType;
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
            const ushort customFieldCount = 2;
            m_arraykeyValue = new FieldValueTableInfo[arrayframeType.Length + customFieldCount];
            ushort count = 0;
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Time, ValueType.Number);
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_EndTime, ValueType.Number);
            for (short index = 0; index < arrayframeType.Length; index++) {
                FrameType frameType = (FrameType)arrayframeType.GetValue(index);
                string key = LuaTable.GetArrayKeyString(frameType);
                m_arraykeyValue[index + customFieldCount] = new FieldValueTableInfo(key, ValueType.Table);    
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
        Buff = 13,
        OverheadStart = 14,
        OverheadBreak = 15,
        DodgeBreak = 16,
    }
}