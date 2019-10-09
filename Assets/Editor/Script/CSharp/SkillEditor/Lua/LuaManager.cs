using UnityEngine;

namespace SkillEditor {

    internal static class LuaManager {

        public static void Start() {
            LuaReader.Read(Config.KeyFrameFilePath);
        }
    }
}