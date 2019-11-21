using System;
using System.Collections.Generic;
using Lua.EffectConf;

namespace SkillEditor {

    internal static class LuaEffectConfModel {

        private static List<EffectData> m_listEffect = new List<EffectData>((ushort)Math.Pow(2, 5));
        public static List<EffectData> EffectList => m_listEffect;
    }
}