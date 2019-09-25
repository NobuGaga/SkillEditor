using UnityEditor;
//using UnityEngine;

[CustomEditor(typeof(SkillEditorMono))]
public class SkillEditorMonoInspector : Editor {

    private SkillEditorMono m_target;

    private void OnEnable() {
        m_target = target as SkillEditorMono;
    }

    public override void OnInspectorGUI() {
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("This is a SkillEditorMono!");
        //EditorGUILayout.EndHorizontal();
    }

    public void OnSceneGUI() {
        Handles.Label(m_target.transform.position, "This is a test!");
    }
}