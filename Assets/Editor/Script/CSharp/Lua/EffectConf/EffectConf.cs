using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua.EffectConf {

    public struct EffectData : IFieldValueTable, ILuaFile<EffectData> {
        
        public uint id;
        public string name;
        public ushort pivotType;
        public string pivotNodeName;
        public ushort parentPivotType;
        public string resourceName;
        public bool isLoop;
        public bool isBreak;

        #region ITable Function

        public string GetTableName() => "EffectData";
        public ushort GetLayer() => 1;
        public ReadType GetReadType() => ReadType.RepeatToFixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => id = (uint)(int)key;
        public string GetKey() => id.ToString();
        public bool IsNullTable() =>
            id == 0 || string.IsNullOrEmpty(name) || pivotType == 0 || string.IsNullOrEmpty(pivotNodeName) ||
            string.IsNullOrEmpty(resourceName);
        public void Clear() {
            id = pivotType = parentPivotType = 0;
            name = pivotNodeName = resourceName = null;
            isLoop = isBreak = false;
        }

        private static StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion
    
        #region IFieldKeyTable Function

        private const string Key_ID = "ID";
        private const string Key_Name = "sName";
        private const string Key_PivotType = "iPivotType";
        private const string Key_PivotNodeName = "sPivot";
        private const string Key_ParentPivotType = "iParentPivotType";
        private const string Key_ResourceName = "sResName";
        private const string Key_Loop = "iLoop";
        private const string Key_Break = "iBreak";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_ID:
                    id = (ushort)(int)value;
                    return;
                case Key_Name:
                    name = value as string;
                    return;
                case Key_PivotType:
                    pivotType = (ushort)(int)value;
                    return;
                case Key_PivotNodeName:
                    pivotNodeName = value as string;
                    return;
                case Key_ParentPivotType:
                    parentPivotType = (ushort)(int)value;
                    return;
                case Key_ResourceName:
                    resourceName = value as string;
                    return;
                case Key_Loop:
                    isLoop = (int)value > 0;
                    return;
                case Key_Break:
                    isBreak = (int)value > 0;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_ID:
                    return id;
                case Key_Name:
                    return name;
                case Key_PivotType:
                    return pivotType;
                case Key_PivotNodeName:
                    return pivotNodeName;
                case Key_ParentPivotType:
                    return parentPivotType;
                case Key_ResourceName:
                    return resourceName;
                case Key_Loop:
                    return isLoop;
                case Key_Break:
                    return isBreak;
                default:
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldValueTableInfo[8];
            m_arraykeyValue[0] = new FieldValueTableInfo(Key_ID, ValueType.Int);
            m_arraykeyValue[1] = new FieldValueTableInfo(Key_Name, ValueType.String);
            m_arraykeyValue[2] = new FieldValueTableInfo(Key_PivotType, ValueType.Int);
            m_arraykeyValue[3] = new FieldValueTableInfo(Key_PivotNodeName, ValueType.String);
            m_arraykeyValue[4] = new FieldValueTableInfo(Key_ParentPivotType, ValueType.Int);
            m_arraykeyValue[5] = new FieldValueTableInfo(Key_ResourceName, ValueType.String);
            m_arraykeyValue[6] = new FieldValueTableInfo(Key_Loop, ValueType.Int);
            m_arraykeyValue[7] = new FieldValueTableInfo(Key_Break, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion

        #region ILuaFile Function

        public string GetLuaFilePath() => Config.EffectConfFilePath;
        public string GetLuaFileHeadStart() => Config.EffectConfFileHeadStart;
        public List<EffectData> GetModel() => LuaEffectConfModel.EffectList;
        #endregion
    }
}