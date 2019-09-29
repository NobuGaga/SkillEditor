using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR_WIN
using UnityEditorInternal;
#endif

public static class SkillEditorManager{

	private static GameObject m_model = null;
    private static string m_modelPath = string.Empty;

	private const string ModelPrefabPath = "Assets/Editor/Asset/prefabs";
	private const string ModelPrefabExtension = "prefab";
	private const string EditScenePath = "Assets/Editor/Scene/EditScene.unity";
	private const string ExitScenePath = "Assets/Editor/Scene/EditScene.unity";
    private const string SkillEditorLayout = "SkillEditor";

    private static double m_lastTime;

    private static List<AnimationClip> m_animationClips = new List<AnimationClip>(SkillEditorConst.DefaultAnimationClipLength);

    private static bool isEditorMode {
        get { return m_modelPath != string.Empty; }
    }

    public static void OpenPrefab() {
		string prefabPath = EditorUtility.OpenFilePanel("模型 Prefab 路径", ModelPrefabPath, ModelPrefabExtension);
		if (prefabPath == null || prefabPath == string.Empty || prefabPath == m_modelPath)
			return;
        if (!isEditorMode) {
            string menuPath = string.Format("Window/Layouts/{0}", SkillEditorLayout);
            if (!EditorApplication.ExecuteMenuItem(menuPath)) {
#if UNITY_EDITOR_WIN
                CopyLayoutFile();
                EditorApplication.ExecuteMenuItem(menuPath);
#endif
            }
        }
        EditorSceneManager.OpenScene(EditScenePath);
        m_modelPath = prefabPath;
		if (m_model)
			Object.DestroyImmediate(m_model);
		GameObject prefab = PrefabUtility.LoadPrefabContents(m_modelPath);
		m_model = Object.Instantiate(prefab);
        PrefabUtility.UnloadPrefabContents(prefab);
        AddAllAnimationClipName();
        Selection.activeGameObject = m_model;
        SkillEditorScene.RegisterSceneGUI();
        SkillEditorWindow.Open();
    }

    private static void AddAllAnimationClipName() {
        string clipPath = m_modelPath.Substring(0, m_modelPath.IndexOf("prefabs/", System.StringComparison.Ordinal)) + "models/";
        DirectoryInfo direction = new DirectoryInfo(clipPath);
        string[] fileNames = Directory.GetFiles(clipPath);
        m_animationClips.Clear();
        foreach (string fileName in fileNames) {
            if (fileName.Contains(".meta") || !fileName.Contains("@") || !(fileName.Contains(".fbx") || fileName.Contains(".FBX")))
                continue;
            int subIndex = fileName.IndexOf("Assets/", System.StringComparison.Ordinal);
            string path = fileName.Substring(subIndex);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                continue;
            m_animationClips.Add(clip); 
        }
        SkillEditorData.AnimationClips = m_animationClips.ToArray();
        SkillEditorWindow.SetDisplayData(SkillEditorData.AnimationClipNames, SkillEditorData.AnimationClipIndexs);
    }

    public static void SetAnimationClipData(int index) {
        SkillEditorData.SetCurrentAnimationClip(index);
    }

#if UNITY_EDITOR_WIN
    private static void CopyLayoutFile() {
        string targetPath = Application.persistentDataPath;
        string keyWord = "AppData/";
        int subIndex = targetPath.IndexOf(keyWord, System.StringComparison.Ordinal);
        subIndex += keyWord.Length;
        targetPath = targetPath.Substring(0, subIndex);
        targetPath = Path.Combine(targetPath, string.Format("Roaming/Unity/Editor-5.x/Preferences/Layouts/{0}.wlt", SkillEditorLayout));
        subIndex = Application.dataPath.IndexOf("Assets", System.StringComparison.Ordinal);
        string filePath = Application.dataPath.Substring(0, subIndex);
        filePath = Path.Combine(filePath, string.Format("Layout/{0}.wlt", SkillEditorLayout));
        File.Copy(filePath, targetPath);
        InternalEditorUtility.ReloadWindowLayoutMenu();
    }
#endif

    public static void Play() {
        m_lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        SkillEditorClip.Play(m_model, SkillEditorData.SelectAnimationClip);
    }

    private static void Update() {
        if (SkillEditorClip.IsPlayOver)
            Stop();
        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - m_lastTime);
        m_lastTime = currentTime;
        SkillEditorClip.Update(deltaTime);
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
        SkillEditorScene.UnregisterSceneGUI();
        EditorSceneManager.OpenScene(ExitScenePath);
        EditorApplication.ExecuteMenuItem("Window/Layouts/Default");
    }
}