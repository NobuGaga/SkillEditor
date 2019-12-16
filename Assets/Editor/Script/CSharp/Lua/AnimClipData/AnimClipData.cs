using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct AnimClipData : IRepeatKeyTable<StateData>, ILuaFile<AnimClipData>, ILuaMultipleFile<FileType> {
        
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
        public bool IsNullTable() {
            if (modelName == null || modelName == string.Empty || stateList == null || stateList.Length == 0)
                return true;
            for (int index = 0; index < stateList.Length; index++)
                if (!stateList[index].IsNullTable())
                    return false;
            return true;
        }
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
        public object SetTableList() {
            stateList = m_listCache.ToArray();
            return this;
        }
        public void SetTableListData(ushort index, StateData data) => stateList[index] = data;
        public StateData[] GetTableList() => stateList;
        #endregion
    
        #region ILuaFile Function

        public string GetLuaFilePath() => Tool.CombinePath(Config.ProjectPath, "../Resources/lua/data/config/AnimClipData.lua");
        public string GetLuaFileHeadStart() => "AnimClipData = AnimClipData or {}";
        public List<AnimClipData> GetModel() => LuaAnimClipModel.AnimClipList;
        public string GetWriteFileString() => LuaAnimClipModel.GetWriteFileString();
        #endregion

        #region ILuaMultipleFile Function

        private static FileType m_fileType = (FileType)LuaTable.DefaultFileType;
        public void SetFileType(short type) {
            if (Enum.IsDefined(typeof(FileType), type))
                m_fileType = (FileType)type;
            else
                m_fileType = (FileType)LuaTable.DefaultFileType;
        }
        public FileType GetFileType() => m_fileType;

        private static string[] m_multipleFilePath = new string[] { 
            Tool.CombinePath(Config.ProjectPath, "../Resources/lua/data/config/AnimClipServerData.lua") 
        };
        public string[] GetMultipleLuaFilePath() => m_multipleFilePath;

        private static string[] m_multipleFileHeadStart = new string[] { "AnimClipServerData = AnimClipServerData or {}" };
        public string[] GetMultipleLuaFileHeadStart() => m_multipleFileHeadStart;

        public string[] GetWriteMultipleFileString() => LuaAnimClipModel.GetWriteMultipleFileString();
        #endregion
    }
    public enum FileType {
        Client = LuaTable.DefaultFileType,
        Server = Client + 1,
    }
}