using System;
using System.Text;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct StateData : IRepeatKeyTable {
        
        public State state;
        public ClipData[] clipList;

        public StateData(State state, ClipData[] clipList) {
            this.state = state;
            this.clipList = clipList;
        }

        public void SetState(string originKey) {
            if (!originKey.Contains(StateHeadString + LuaFormat.PointSymbol))
                return;
            string stateString = originKey.Substring(StateHeadString.Length + 1);
            state = (State)Enum.Parse(typeof(State), stateString);
        }

        public ushort GetLayer() => 2;
        public KeyType GetKeyType() => KeyType.String;
        public string GetKey() => Tool.GetCacheString("EntityStateDefine" + LuaFormat.PointSymbol + state.ToString());

        public bool IsNullTable() => state == State.None || clipList == null || clipList.Length == 0;
        public void Clear() {
            state = State.None;
            clipList = null;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 15));
        public override string ToString() {
            string key = Tool.GetCacheString(StateHeadString + LuaFormat.PointSymbol + state.ToString());
            return LuaTable.GetNotFieldKeyTableText(m_staticBuilder, this);
        }
    }

    public enum State {
        None,
        Atk,
        UseSkill,
    }
}