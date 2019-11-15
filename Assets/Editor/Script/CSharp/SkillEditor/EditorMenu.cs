using UnityEditor;

namespace SkillEditor {

    internal static class EditorMenu {

        [MenuItem("技能编辑器/选择模型预设")]
        private static void SelectPrefab() {
            Lua.LuaReader.Read<Lua.AnimClipData.AnimClipData>();
            var list = LuaAnimClipModel.AnimClipList;
            foreach (var data in list)
                UnityEngine.Debug.Log(data);
            // Lua.LuaWriter.Write<Lua.AnimClipData.AnimClipData>();
            // Manager.SelectPrefab();
	    }

        [MenuItem("技能编辑器/打开技能编辑窗口")]
        private static void OpenWindow() {
            EditorWindow.Open();
	    }

        [MenuItem("技能编辑器/复制本地布局文件到项目")]
        private static void CopyLocalLayoutFileToProject() {
            Manager.CopyLocalLayoutFileToProject();
        }

        [MenuItem("技能编辑器/退出编辑器模式")]
        private static void Exit() {
            Manager.Exit();
        }
    }
}