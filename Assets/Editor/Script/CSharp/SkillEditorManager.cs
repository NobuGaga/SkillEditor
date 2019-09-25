using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SkillEditorManager{

	private static GameObject m_model = null;
    private static SkillEditorMono m_modelScript = null;
    private static string m_modelPath = string.Empty;

	private const string ModelPrefabPath = "Assets/Editor/Asset/Prefab";
	private const string ModelPrefabExtension = "prefab";
	private const string EditScenePath = "Assets/Editor/Scene/EditScene.unity";
	private const string ExitScenePath = "Assets/Editor/Scene/EditScene.unity";

    private static bool isEditorMode {
        get { return m_modelPath != string.Empty; }
    }

    public static void OpenPrefab() {
		string prefabPath = EditorUtility.OpenFilePanel("模型 Prefab 路径", ModelPrefabPath, ModelPrefabExtension);
		if (prefabPath == null || prefabPath == string.Empty || prefabPath == m_modelPath)
			return;
        if (!isEditorMode)
            EditorApplication.ExecuteMenuItem("Window/Layouts/SkillEditor");
        EditorSceneManager.OpenScene(EditScenePath);
        m_modelPath = prefabPath;
		if (m_model)
			Object.DestroyImmediate(m_model);
		GameObject prefab = PrefabUtility.LoadPrefabContents(m_modelPath);
		m_model = Object.Instantiate(prefab);
        PrefabUtility.UnloadPrefabContents(prefab);
        m_modelScript = m_model.AddComponent<SkillEditorMono>();
        Selection.activeGameObject = m_model;
        SkillEditorWindow.Open();
	}

    public static void RevertScene() {
        if (!isEditorMode)
            return;
        m_model = null;
        m_modelPath = string.Empty;
        EditorSceneManager.OpenScene(ExitScenePath);
        EditorApplication.ExecuteMenuItem("Window/Layouts/Default");
    }
}