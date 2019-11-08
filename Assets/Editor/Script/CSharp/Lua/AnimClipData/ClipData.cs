using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct ClipData : ITable {

        public string clipName;
        public FrameData[] frameList;

        public ClipData(string clipName, FrameData[] frameList) {
            this.clipName = clipName;
            this.frameList = frameList;
        }

        public ushort GetLayer() => 3;
        public KeyType GetKeyType() => KeyType.Array;
        public bool IsNullTable() => clipName == null || clipName == string.Empty || 
                                        frameList == null || frameList.Length == 0;

        public void Clear() {
            clipName = null;
            frameList = null;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 14));
        public override string ToString() {
            return LuaTable.GetNotFieldKeyTableText(m_staticBuilder, this, clipName, frameList);
        }
    }
}