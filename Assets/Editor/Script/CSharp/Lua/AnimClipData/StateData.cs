using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct StateData : IRepeatKeyTable<ClipData> {
        
        public State state;
        public ClipData[] clipList;

        public StateData(State state, ClipData[] clipList) {
            this.state = state;
            this.clipList = clipList;
        }

        private const string StateHeadString = "EntityStateDefine";
        public void SetState(string originKey) {
            if (!originKey.Contains(StateHeadString + LuaFormat.PointSymbol)) {
                state = State.None;
                return;
            }
            string stateString = originKey.Substring(StateHeadString.Length + 1);
            state = (State)Enum.Parse(typeof(State), stateString);
        }

        #region ITable Function
        public string GetTableName() => "StateData";
        public ushort GetLayer() => 2;
        public ReadType GetReadType() => ReadType.Repeat;
        public KeyType GetKeyType() => KeyType.String;
        public void SetKey(object key) => SetState(key as string);
        public string GetKey() => Tool.GetCacheString(StateHeadString + LuaFormat.PointSymbol + state.ToString());

        public bool IsNullTable() => state == State.None || clipList == null || clipList.Length == 0;
        public void Clear() {
            state = State.None;
            clipList = null;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 15));
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, this);
        #endregion

        #region IRepeatKeyTable Function
        public Type GetTableListType() => typeof(ClipData);
        private static List<ClipData> m_listCache = new List<ClipData>((ushort)Math.Pow(2, 4));
        public List<ClipData> GetStaticCacheList() => m_listCache;
        public void SetTableList() => clipList = m_listCache.ToArray();
        public ClipData[] GetTableList() => clipList;
        #endregion
    }

    public enum State {
        None,
        Atk,
        UseSkill,
    }
}