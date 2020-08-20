using UnityEngine;
using System;
using System.Text;
using SkillEditor;

namespace Lua.EffectConf {

    public struct EffectConfSound : IFieldValueTable {

        public ushort frame;
        public string soundName;

        #region ITable Function

        public string GetTableName() => "EffectConfSound";
        public ushort GetLayer() => 2;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => Key_Sound;
        public bool IsNullTable() => frame == 0 || string.IsNullOrEmpty(soundName);
        public void Clear() {
            frame = 0;
            soundName = string.Empty;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 5));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_Frame = "[1]";
        private const string Key_SoundName = "[2]";
        public const string Key_Sound = "tEffectSound";
        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Frame:
                    frame = (ushort)(int)value;
                    return;
                case Key_SoundName:
                    soundName = value as string;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Frame:
                    return frame;
                case Key_SoundName:
                    return soundName;
                default:
                    Debug.LogError("EffectConfSound::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            const ushort length = 2;
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Frame, ValueType.Int);
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_SoundName, ValueType.String);
            return m_arraykeyValue;
        }
        #endregion
    }
}