using UnityEditor;

namespace SkillEditor {

    internal static class EditorMenu {

        [MenuItem("技能编辑器/选择模型预设")]
        private static void SelectPrefab() => Manager.SelectPrefab();

        [MenuItem("技能编辑器/打开技能编辑窗口")]
        private static void OpenWindow() => EditorWindow.Open();

        [MenuItem("技能编辑器/保存编辑到 Lua 文件 #S")]
        private static void WriteAnimClipData() {
            Lua.LuaReader.Read<Lua.AnimClipData.AnimClipData>();
            Lua.LuaWriter.Write<Lua.AnimClipData.AnimClipData>();
        }

        [MenuItem("技能编辑器/复制本地布局文件到项目")]
        private static void CopyLocalLayoutFileToProject() => Manager.CopyLocalLayoutFileToProject();

        [MenuItem("技能编辑器/退出编辑器模式")]
        private static void Exit() => Manager.Exit();
    }
}