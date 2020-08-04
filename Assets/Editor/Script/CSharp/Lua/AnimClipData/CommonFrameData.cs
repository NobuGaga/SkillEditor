using UnityEngine;
using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct CommonFrameData {

        public ushort priority;
        public bool isLoop;

        public bool IsNull() => priority <= 0;
        public void Clear() {
            priority = 0;
            isLoop = false;
        }

        public const string Key_Priority = "priority";
        public const string Key_Loop = "loop";
        public const string Key_Data = "data";
        
        private bool SetLoop(object value) => isLoop = ((int)value) == 1;
        private object GetLoop() => isLoop ? 1 : 0;

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_Priority:
                    priority = (ushort)(int)value;
                    return;
                case Key_Loop:
                    SetLoop(value);
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_Priority:
                    return priority;
                case Key_Loop:
                    return GetLoop();
                default:
                    Debug.LogError("CommonFrameData::GetFieldValueTableValue key is not exit. key " + key);
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
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Priority, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Loop, ValueType.Int);
            return m_arraykeyValue;
        }
    }

    public interface ICommonFrameData {
        
        void SetPriority(ushort priority);
        void SetLoop(bool isLoop);
        bool GetLoop();
    }
}