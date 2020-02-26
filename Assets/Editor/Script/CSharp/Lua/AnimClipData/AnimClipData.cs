using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua.AnimClipData {

    public struct AnimClipData : IRepeatKeyTable<StateData>, ILuaMultipleSplitFile<AnimClipData, FileType> {
        
        public string modelName;
        public StateData[] stateList;

        #region ITable Function

        public string GetTableName() => "AnimClipData";
        public ushort GetLayer() => 0;
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

        public string GetLuaFilePath() => Tool.CombineFilePath(GetFolderPath(), GetMainFileName(), GetFileExtension());
        public string GetLuaFileHeadStart() => "AnimClipData = AnimClipData or {}\nAnimClipData.data = {}\n";
        public List<AnimClipData> GetModel() => LuaAnimClipModel.AnimClipList;
        public string GetWriteFileString() {
            string headText = GetChildFileHeadStart();
            string contentText = ToString();
            m_staticBuilder.Clear();
            m_staticBuilder.Append(headText);
            m_staticBuilder.Append(contentText);
            int length = LuaFormat.TableEndString.Length;
            m_staticBuilder.Remove(m_staticBuilder.Length - length, length);
            return m_staticBuilder.ToString();
        }
        #endregion

        #region ILuaSplitFile Function

        public string GetFileExtension() => "lua";
        public string GetFolderPath() => Tool.CombinePath(Application.dataPath, "Editor/luaconfig/.animclipconfig/client");
        public string GetMainFileName() => "AnimClipBase";
        public string GetChildFileRequirePath() => "data/config/animclipconfig/client/";
        public string GetChildFileNameFormat() => "{0}_clip";
        public string GetChildFileHeadStart() {
            if (m_fileType == FileType.Client)
                return "AnimClipData.data";
            return m_multipleChildFileHeadStart[(ushort)m_fileType];
        }
        #endregion

        #region ILuaMultipleSplitFile Function

        private static FileType m_fileType = FileType.Client;
        public void SetFileType(int type) {
            if (Enum.IsDefined(typeof(FileType), type))
                m_fileType = (FileType)type;
            else
                m_fileType = FileType.Client;
        }
        public FileType GetFileType() => m_fileType;

        private static string[] m_multipleMainFileName = new string[] {
            "AnimClipBaseServer"
        };
        public string[] GetMultipleLuaMainFileName() => m_multipleMainFileName;
        
        private static string[] m_multipleFolderPath = new string[] {
            Tool.CombinePath(Application.dataPath, "Editor/luaconfig/.animclipconfig/server")
        };
        public string[] GetMultipleFolderPath() => m_multipleFolderPath;

        private static string[] m_multipleMainFileHeadStart = new string[] {
            "AnimClipServerData = AnimClipServerData or {}\nAnimClipServerData.data = {}\n"
        };
        public string[] GetMultipleLuaMainFileHeadStart() => m_multipleMainFileHeadStart;

        private static string[] m_multipleChildFileRequirePath = new string[] {
            "data/config/animclipconfig/server/"
        };
        public string[] GetMultipleChildFileRequirePath() => m_multipleChildFileRequirePath;

        private static string[] m_multipleChildFileNameFormat = new string[] {
            "{0}_clip_server"
        };
        public string[] GetMultipleLuaChildFileNameFormat() => m_multipleChildFileNameFormat;

        private static string[] m_multipleChildFileHeadStart = new string[] {
            "AnimClipServerData.data"
        };
        public string[] GetMultipleLuaChildFileHeadStart() => m_multipleChildFileHeadStart;
        #endregion
    }
    
    public enum FileType {

        Client = LuaTable.DefaultFileType,
        Server = Client + 1,
    }
}