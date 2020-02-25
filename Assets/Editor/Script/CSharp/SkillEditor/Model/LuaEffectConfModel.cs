using System;
using System.Collections.Generic;
using Lua;
using Lua.EffectConf;

namespace SkillEditor {

    internal static class LuaEffectConfModel {

        private static ushort EffectConfLength = (ushort)Math.Pow(2, 5);
        private static List<EffectData> m_listEffect = new List<EffectData>(EffectConfLength);
        public static List<EffectData> EffectList => m_listEffect;
        public static Dictionary<uint, EffectData> m_dicIDEffectData = new Dictionary<uint, EffectData>(EffectConfLength);

        public static void Init() {
            if (m_listEffect.Count == 0) {
                m_dicIDEffectData.Clear();
                return;
            }
            for (int index = m_listEffect.Count - 1; index >= 0 ; index--) {
                EffectData data = m_listEffect[index];
                if (data.IsNullTable())
                    m_listEffect.RemoveAt(index);
                m_dicIDEffectData.Add(data.id, data);
            }
        }

        public static EffectData GetEffectData(uint id) {
            if (!m_dicIDEffectData.ContainsKey(id))
                return default;
            return m_dicIDEffectData[id];
        }

        public static string GetWriteFileString() => LuaWriter.GetWriteFileString(m_listEffect);

        public static void Reset() {
            m_listEffect.Clear();
            m_dicIDEffectData.Clear();
        }
    }
}