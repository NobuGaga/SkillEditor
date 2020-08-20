using UnityEngine;
using System;
using System.Text;
using SkillEditor;

namespace Lua.EffectConf {

    public struct EffectConfShow : IFieldValueTable {

        public ushort singlePVE;
        public ushort multiplePVE;
        public ushort singlePVP;
        public ushort multiplePVP;

        #region ITable Function

        public string GetTableName() => "EffectConfShow";
        public ushort GetLayer() => 2;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => Key_ShowType;
        public bool IsNullTable() => false;
        public void Clear() => singlePVE = multiplePVE = singlePVP = multiplePVP;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 5));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_SinglePVE = "[1]";
        private const string Key_MultiplePVE = "[2]";
        private const string Key_SinglePVP = "[3]";
        private const string Key_MultiplePVP = "[4]";
        public const string Key_ShowType = "IsSelfOnly";
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_SinglePVE:
                    singlePVE = (ushort)(int)value;
                    return;
                case Key_MultiplePVE:
                    multiplePVE = (ushort)(int)value;
                    return;
                case Key_SinglePVP:
                    singlePVP = (ushort)(int)value;
                    return;
                case Key_MultiplePVP:
                    multiplePVP = (ushort)(int)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_SinglePVE:
                    return singlePVE;
                case Key_MultiplePVE:
                    return multiplePVE;
                case Key_SinglePVP:
                    return singlePVP;
                case Key_MultiplePVP:
                    return multiplePVP;
                default:
                    Debug.LogError("EffectConfShow::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            const ushort length = 4;
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_SinglePVE, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_MultiplePVE, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_SinglePVP, ValueType.Int);
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_MultiplePVP, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }
}