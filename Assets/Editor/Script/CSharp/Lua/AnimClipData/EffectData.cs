using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct EffectData : IFieldValueTable {

        public ushort index;
        public EffectType type;
        public int id;
        public EffectRotationData rotation;

        public EffectData(EffectType type, int id, EffectRotationData rotation) {
            index = 0;
            this.type = type;
            this.id = id;
            this.rotation = rotation;
        }

        #region ITable Function
        
        public string GetTableName() => "EffectData";
        public ushort GetLayer() => 8;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => index = (ushort)(int)key;
        public string GetKey() => index.ToString();
        public bool IsNullTable() => id == 0;
        public void Clear() {
            id = 0;
            rotation.Clear();
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_Type = "type";
        private const string Key_Id = "id";
        private const string Key_Rotation = "rotation";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    if (!Enum.TryParse(value.ToString(), false, out EffectType effectType))
                        Debug.LogError("EffectData::SetFieldValueTableValue value type is not a EffectType");
                    type = effectType;
                    return;
                case Key_Id:
                    id = (int)value;
                    return;
                case Key_Rotation:
                    rotation = (EffectRotationData)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Type:
                    return (ushort)type;
                case Key_Id:
                    return id;
                case Key_Rotation:
                    return rotation;
                default:
                    Debug.LogError("EffectData::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new FieldValueTableInfo[3];
            m_arraykeyValue[0] = new FieldValueTableInfo(Key_Type, ValueType.Int);
            m_arraykeyValue[1] = new FieldValueTableInfo(Key_Id, ValueType.Int);
            m_arraykeyValue[2] = new FieldValueTableInfo(Key_Rotation, ValueType.Table);
            return m_arraykeyValue;
        }
        #endregion
    }

    public enum EffectType {
        Body = 1,
        Weapon = 2,
        Hit = 3,
    }
}