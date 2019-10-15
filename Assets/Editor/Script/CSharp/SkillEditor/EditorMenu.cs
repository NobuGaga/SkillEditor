using UnityEditor;

using UnityEngine;
using System.Collections.Generic;
using SkillEditor.Structure;

namespace SkillEditor {

    internal static class EditorMenu {

        [MenuItem("技能编辑器/选择模型预设")]
        private static void SelectPrefab() {
            Manager.SelectPrefab();
	    }

        [MenuItem("技能编辑器/打开技能编辑窗口")]
        private static void OpenWindow() {
            EditorWindow.Open();
	    }

        [MenuItem("技能编辑器/播放")]
        private static void Play() {
            //Manager.Play();
            List<KeyFrameData> list = LuaReader.Read<KeyFrameData>(Config.KeyFrameFilePath);
            foreach (var data in list)
                Debug.Log("Lua Data\n" + data.ToString());
            Debug.Log("Lua Head\n" + LuaWriter.GetHeadText(Config.KeyFrameFilePath));
        }

        [MenuItem("技能编辑器/停止")]
        private static void Stop() {
            Manager.Stop();
        }

        [MenuItem("技能编辑器/退出编辑器模式")]
        private static void Exit() {
            Manager.Exit();
        }
    }
}