using UnityEditor;

public static class SkillEditorMenu {

    [MenuItem("技能编辑器/选择模型预设")]
    private static void SelectPrefab() {
        SkillEditorManager.SelectPrefab();
	}

    [MenuItem("技能编辑器/打开技能编辑窗口")]
    private static void OpenWindow() {
        SkillEditorWindow.Open();
	}

    [MenuItem("技能编辑器/播放")]
    private static void Play() {
        SkillEditorManager.Play();
    }

    [MenuItem("技能编辑器/停止")]
    private static void Stop() {
        SkillEditorManager.Stop();
    }

    [MenuItem("技能编辑器/退出编辑器模式")]
    private static void ExitSkillEditor() {
        SkillEditorManager.Exit();
    }
}