using System;
using System.Text;
using System.Collections.Generic;

namespace Lua.AnimClipData {

    public struct ClipData : IRepeatKeyTable<FrameData> {

        public string clipName;
        public FrameData[] frameList;

        public ClipData(string clipName, FrameData[] frameList) {
            this.clipName = clipName;
            this.frameList = frameList;
        }

        #region ITable Function
        public string GetTableName() => "ClipData";
        public ushort GetLayer() => 3;
        public ReadType GetReadType() => ReadType.Repeat;
        public KeyType GetKeyType() => KeyType.String;
        public void SetKey(object key) => clipName = key as string;
        public string GetKey() => clipName;
        public bool IsNullTable() {
            if (clipName == null || clipName == string.Empty || frameList == null || frameList.Length == 0)
                return true;
            for (int index = 0; index < frameList.Length; index++)
                if (!frameList[index].IsNullTable())
                    return false;
            return true;
        }

        public void Clear() {
            clipName = null;
            frameList = null;
        }

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