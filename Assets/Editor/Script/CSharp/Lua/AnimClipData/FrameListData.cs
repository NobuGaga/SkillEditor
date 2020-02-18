using System;
using System.Text;
using System.Collections.Generic;

namespace Lua.AnimClipData {

    public struct FrameListData : IRepeatKeyTable<FrameData> {

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
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, this);
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
    }
}