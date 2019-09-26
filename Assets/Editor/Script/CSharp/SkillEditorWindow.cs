using UnityEditor;
using UnityEngine;

public class SkillEditorWindow : EditorWindow {

    public static void Open() {
        GetWindow<SkillEditorWindow>().Show();
    }

    public static void CloseWindow() {
        GetWindow<SkillEditorWindow>().Close();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("This is a test window!");
        EditorGUILayout.EndHorizontal();
    }
}