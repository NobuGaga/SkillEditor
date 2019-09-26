using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class SkillEditorManager{

	private static GameObject m_model = null;
    private static SkillEditorMono m_modelScript = null;
    private static string m_modelPath = string.Empty;

	private const string ModelPrefabPath = "Assets/Editor/Asset/prefabs";
	private const string ModelPrefabExtension = "prefab";
	private const string EditScenePath = "Assets/Editor/Scene/EditScene.unity";
	private const string ExitScenePath = "Assets/Editor/Scene/EditScene.unity";

    private static double m_lastTime;

    private static bool isEditorMode {
        get { return m_modelPath != string.Empty; }
    }

    public static SkillEditorMono CurrentModel  {
        get { return m_modelScript; }
    }

    public static void OpenPrefab() {
		string prefabPath = EditorUtility.OpenFilePanel("模型 Prefab 路径", ModelPrefabPath, ModelPrefabExtension);
		if (prefabPath == null || prefabPath == string.Empty || prefabPath == m_modelPath)
			return;
        if (!isEditorMode)
            EditorApplication.ExecuteMenuItem("Window/Layouts/SkillEditor");
        EditorSceneManager.OpenScene(EditScenePath);
        m_modelPath = prefabPath;
        Debug.Log(string.Format("Model Path {0}", m_modelPath));
		if (m_model)
			Object.DestroyImmediate(m_model);
		GameObject prefab = PrefabUtility.LoadPrefabContents(m_modelPath);
		m_model = Object.Instantiate(prefab);
        PrefabUtility.UnloadPrefabContents(prefab);
        m_modelScript = m_model.AddComponent<SkillEditorMono>();
        AddAllAnimationClipName();
        Selection.activeGameObject = m_model;
        SkillEditorWindow.Open();
	}

    private static void AddAllAnimationClipName() {
        string clipPath = m_modelPath.Substring(0, m_modelPath.IndexOf("prefabs/", System.StringComparison.Ordinal)) + "models/";
        DirectoryInfo direction = new DirectoryInfo(clipPath);
        string[] fileNames = Directory.GetFiles(clipPath);
        List<AnimationClip> listClipNames = new List<AnimationClip>(8);
        foreach (string fileName in fileNames) {
            if (fileName.Contains(".meta") || !fileName.Contains("@") || !(fileName.Contains(".fbx") || fileName.Contains(".FBX")))
                continue;
            int subIndex = fileName.IndexOf("Assets/", System.StringComparison.Ordinal);
            string path = fileName.Substring(subIndex);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                continue;
            listClipNames.Add(clip);
            Debug.Log(string.Format("clip name {0}", clip.name));
        }
        SkillEditorMono script = CurrentModel;
        script.AddAllAnimationClipName(listClipNames.ToArray());
    }

    public static void Play() {
        m_lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        CurrentModel.Play();
    }

    private static void Update() {
        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - m_lastTime);
        m_lastTime = currentTime;
        m_modelScript.Update(deltaTime);
    }

    public static void Stop() {
        EditorApplication.update -= Update;
    }

    public static void RevertScene() {
        if (!isEditorMode)
            return;
        EditorApplication.update = null;
        m_model = null;
        m_modelPath = string.Empty;
        SkillEditorWindow.CloseWindow();
        EditorSceneManager.OpenScene(ExitScenePath);
        EditorApplication.ExecuteMenuItem("Window/Layouts/Default");
    }
}