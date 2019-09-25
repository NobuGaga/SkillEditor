using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SkillEditorManager{

	private static GameObject m_model = null;
	private static string m_modelPath = string.Empty;

	private const string ModelPrefabPath = "Assets/Editor/Asset/Prefab";
	private const string ModelPrefabExtension = "prefab";
	private const string EditScenePath = "Assets/Editor/Scene/EditScene.unity";
	private const string ExitScenePath = "Assets/Editor/Scene/EditScene.unity";

    public static void OpenPrefab() {
		string prefabPath = EditorUtility.OpenFilePanel("模型 Prefab 路径", ModelPrefabPath, ModelPrefabExtension);
		if (prefabPath == null || prefabPath == string.Empty || prefabPath == m_modelPath)
			return;
		m_modelPath = prefabPath;
		if (m_model)
			Object.DestroyImmediate(m_model);
		EditorSceneManager.OpenScene(EditScenePath);
		GameObject prefab = PrefabUtility.LoadPrefabContents(m_modelPath);
		m_model = Object.Instantiate(prefab);
        m_model.AddComponent<SkillEditorMono>();
        SkillEditorWindow.Open();
        Selection.activeGameObject = m_model;
        PrefabUtility.UnloadPrefabContents(prefab);
	}

    public static void RevertScene() {
        m_modelPath = string.Empty;
        EditorSceneManager.OpenScene(ExitScenePath);
	}
}