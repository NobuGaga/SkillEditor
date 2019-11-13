using System;
using System.Text;

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
        public KeyType GetKeyType() => KeyType.String;
        public void SetKey(object key) => clipName = key as string;
        public string GetKey() => clipName;
        public bool IsNullTable() => clipName == null || clipName == string.Empty || 
                                        frameList == null || frameList.Length == 0;

        public void Clear() {
            clipName = null;
            frameList = null;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 14));
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, this);
        #endregion

        #region IRepeatKeyTable Function
        public FrameData[] GetTableList() => frameList;
        #endregion
    }
}