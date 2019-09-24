using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SkillEditorMenu {

	private static GameObject m_model = null;
	private static string m_modelPath = string.Empty;

	private const string ModelPrefabPath = "Assets/Editor/Asset/Prefab";
	private const string ModelPrefabExtension = "prefab";
	private const string EditScenePath = "Assets/Scene/EditScene.unity";

	[MenuItem("技能编辑器/选择模型 Prefab")]
    private static void OpenPrefab() {
		string prefabPath = EditorUtility.OpenFilePanel("模型 Prefab 路径", ModelPrefabPath, ModelPrefabExtension);
		if (prefabPath == null || prefabPath == string.Empty || prefabPath == m_modelPath)
			return;
		m_modelPath = prefabPath;
		if (m_model)
			GameObject.DestroyImmediate(m_model);
		EditorSceneManager.OpenScene(EditScenePath);
		GameObject prefab = PrefabUtility.LoadPrefabContents(m_modelPath);
		m_model = GameObject.Instantiate(prefab);
		PrefabUtility.UnloadPrefabContents(prefab);
	}

	[MenuItem("技能编辑器/退出编辑器模式")]
    private static void ExitSkillEditor() {
		if (m_model)
			GameObject.DestroyImmediate(m_model);
        EditorSceneManager.OpenScene(EditScenePath);
	}
}