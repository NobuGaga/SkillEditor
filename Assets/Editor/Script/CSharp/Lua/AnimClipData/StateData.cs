using System;
using System.Text;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct StateData : ITable {
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
        public KeyType GetKeyType() => KeyType.Reference;
        public bool IsNullTable() => state == State.None || clipList == null || clipList.Length == 0;
        public void Clear() {
            state = State.None;
            clipList = null;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.ClipListStringLength);
        public override string ToString() {
            if (IsNullTable())
                return string.Empty;
            m_staticBuilder.Clear();
            for (int index = 0; index < clipList.Length; index++)
                m_staticBuilder.Append(clipList[index].ToString());
            string clipListString = m_staticBuilder.ToString();
            string tabString = LuaTable.GetTabString(GetLayer());
            string format = "{0}[\"{1}{2}{3}\"] = {4}\n{5}{0}{6},\n";
            string toString = string.Format(format, tabString,
                                            StateHeadString, LuaFormat.PointSymbol, state.ToString(),
                                            LuaFormat.CurlyBracesPair.start,
                                            clipListString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
        }

        private const string StateHeadString = "EntityStateDefine";
    }

    public enum State {
        None,
        Atk,
        UseSkill,
    }
}