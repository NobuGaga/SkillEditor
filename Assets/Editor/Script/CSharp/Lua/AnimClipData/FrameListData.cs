using System;
using System.Text;
using System.Collections.Generic;

namespace Lua.AnimClipData {

    public struct FrameListData : IRepeatKeyTable<FrameData>, ILuaMultipleFileStructure<AnimClipData, FileType, FrameListData> {

        public FrameData[] frameList;

        #region ITable Function

        public string GetTableName() => "FrameListData";
        public ushort GetLayer() => 3;
        public ReadType GetReadType() => ReadType.FixedToRepeat;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) {}
        public string GetKey() => "frameList";
        public bool IsNullTable() {
            if (frameList == null || frameList.Length == 0)
                return true;
            for (int index = 0; index < frameList.Length; index++)
                if (!frameList[index].IsNullTable())
                    return false;
            return true;
        }

        public void Clear() => frameList = null;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 14));
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, GetFileTypeTable());
        #endregion

        #region IRepeatKeyTable Function
        
        public Type GetTableListType() => typeof(FrameData);
        private static List<FrameData> m_listCache = new List<FrameData>((ushort)Math.Pow(2, 4));
        public List<FrameData> GetStaticCacheList() => m_listCache;
        public object SetTableList() {
            frameList = m_listCache.ToArray();
            return this;
        }
        public void SetTableListData(ushort index, FrameData data) => frameList[index] = data;
        public FrameData[] GetTableList() => frameList;
        #endregion

        #region ILuaMultipleFileStructure Function

        public AnimClipData GetRootTableType() => default;
        public FileType GetFileType() => GetRootTableType().GetFileType();
        private static List<FrameData> m_listFrameCache = new List<FrameData>();
        public FrameListData GetFileTypeTable() {
            if (IsNullTable() || GetFileType() == FileType.Client)
                return this;
            m_listFrameCache.Clear();
            for (ushort index = 0; index < frameList.Length; index++) {
                FrameData frameData = frameList[index];
                if (frameData.IsNullTable())
                    continue;
                frameData.index = (ushort)(m_listFrameCache.Count + 1);
                m_listFrameCache.Add(frameData);
            }
            FrameListData newData = default;
            newData.frameList = m_listFrameCache.ToArray();
            return newData;
        }
        #endregion
    }
}