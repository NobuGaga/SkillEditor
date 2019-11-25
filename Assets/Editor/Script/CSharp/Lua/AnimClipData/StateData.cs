using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct StateData : IRepeatKeyTable<ClipGroupData> {
        
        public State state;
        public ClipGroupData[] clipList;

        public StateData(State state, ClipGroupData[] clipList) {
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

        public bool IsNullTable() {
            if (state == State.None || clipList == null || clipList.Length == 0)
                return true;
            for (int index = 0; index < clipList.Length; index++)
                if (!clipList[index].IsNullTable())
                    return false;
            return true;
        }

        public void Clear() {
            state = State.None;
            clipList = null;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 15));
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, this);
        #endregion

        #region IRepeatKeyTable Function

        public Type GetTableListType() => typeof(ClipGroupData);
        private static List<ClipGroupData> m_listCache = new List<ClipGroupData>((ushort)Math.Pow(2, 4));
        public List<ClipGroupData> GetStaticCacheList() => m_listCache;
        public object SetTableList() {
            clipList = m_listCache.ToArray();
            return this;
        }
        public void SetTableListData(ushort index, ClipGroupData data) => clipList[index] = data;
        public ClipGroupData[] GetTableList() => clipList;
        #endregion
    }

    public enum State {
        None,
        Atk,
        UseSkill,
    }
}