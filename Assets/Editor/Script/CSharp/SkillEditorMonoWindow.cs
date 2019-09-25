using UnityEditor;
using UnityEngine;

public class SkillEditorMonoWindow : EditorWindow {

    public static void Open() {
        SkillEditorMonoWindow window = GetWindow<SkillEditorMonoWindow>();
        window.Show();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("This is a test window!");
        EditorGUILayout.EndHorizontal();
    }
}