using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct AnimClipData : IRepeatKeyTable {
        
        public string modelName;
        public StateData[] stateList;

        public AnimClipData(string modelName, StateData[] stateList) {
            this.modelName = modelName;
            this.stateList = stateList;
        }

        public ushort GetLayer() => 1;
        public KeyType GetKeyType() => KeyType.String;
        public string GetKey() => modelName;
        public bool IsNullTable() => modelName == null || modelName == string.Empty || 
                                        stateList == null || stateList.Length == 0;
        public void Clear() {
            modelName = null;
            stateList = null;
        }

        private static StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 16));
        public override string ToString() {
            return LuaTable.GetNotFieldKeyTableText(m_staticBuilder, this);
        }

        public StateData[] GetTableList() => stateList;
    }
}