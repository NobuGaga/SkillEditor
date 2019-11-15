using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct AnimClipData : IRepeatKeyTable<StateData>, ILuaFile<AnimClipData> {
        
        public string modelName;
        public StateData[] stateList;

        public AnimClipData(string modelName, StateData[] stateList) {
            this.modelName = modelName;
            this.stateList = stateList;
        }

        #region ITable Function
        public string GetTableName() => "AnimClipData";
        public ushort GetLayer() => 1;
        public ReadType GetReadType() => ReadType.Repeat;
        public KeyType GetKeyType() => KeyType.String;
        public void SetKey(object key) => modelName = key as string;
        public string GetKey() => modelName;
        public bool IsNullTable() => modelName == null || modelName == string.Empty ||
                                            stateList == null || stateList.Length == 0;
        public void Clear() {
            modelName = null;
            stateList = null;
        }

        private static StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 16));
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, this);
        #endregion
    
        #region IRepeatKeyTable Function
        public Type GetTableListType() => typeof(StateData);
        private static List<StateData> m_listCache = new List<StateData>((ushort)Math.Pow(2, 1));
        public List<StateData> GetStaticCacheList() => m_listCache;
        public void SetTableList() => stateList = m_listCache.ToArray();
        public StateData[] GetTableList() => stateList;
        #endregion
    
        #region ILuaFile Function
        public string GetLuaFilePath() => Config.AnimDataFilePath;
        public string GetLuaFileHeadStart() => Config.LuaFileHeadStart;
        public List<AnimClipData> GetModel() => LuaAnimClipModel.AnimClipList;
        #endregion
    }
}