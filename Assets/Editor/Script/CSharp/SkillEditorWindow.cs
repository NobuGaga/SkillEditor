using UnityEditor;
using UnityEngine;

public class SkillEditorWindow : EditorWindow {

    public static void Open() {
        SkillEditorWindow window = GetWindow<SkillEditorWindow>();
        window.Show();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("This is a test window!");
        EditorGUILayout.EndHorizontal();
    }
}