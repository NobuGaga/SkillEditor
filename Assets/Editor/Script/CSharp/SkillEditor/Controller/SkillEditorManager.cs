using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SkillEditorManager{

	private static GameObject m_model = null;

    private static string m_modelPath = string.Empty;
    private static bool isEditorMode {
        get { return m_modelPath != string.Empty; }
    }

    private static double m_lastTime;

    private static List<AnimationClip> m_animationClips = new List<AnimationClip>(SkillEditorConst.DefaultAnimationClipLength);

    public static void SelectPrefab() {
		string prefabPath = EditorUtility.OpenFilePanel(SkillEditorConst.FilePanelTitle, SkillEditorConst.ModelPrefabPath, SkillEditorConst.ModelPrefabExtension);
		if (prefabPath == null || prefabPath == string.Empty || prefabPath == m_modelPath)
			return;
        if (!isEditorMode && !EditorApplication.ExecuteMenuItem(SkillEditorConst.SkillEditorMenuPath)) {
            CopyLayoutFile();
            EditorApplication.ExecuteMenuItem(SkillEditorConst.SkillEditorMenuPath);
        }
        EditorSceneManager.OpenScene(SkillEditorConst.EditScenePath);
        m_modelPath = prefabPath;
		if (m_model)
			Object.DestroyImmediate(m_model);
		GameObject prefab = PrefabUtility.LoadPrefabContents(m_modelPath);
		m_model = Object.Instantiate(prefab);
        PrefabUtility.UnloadPrefabContents(prefab);
        AddAllAnimationClipName();
        if (SkillEditorData.IsGeneric)
            SkillEditorAnimator.Animator = m_model.GetComponent<Animator>();
        Selection.activeGameObject = m_model;
        SkillEditorScene.RegisterSceneGUI();
        SkillEditorWindow.Open();
    }

    private static void CopyLayoutFile() {
        File.Copy(SkillEditorConst.LocalSkillEditorLayoutFilePath, SkillEditorConst.SkillEditorLayoutFilePath);
        InternalEditorUtility.ReloadWindowLayoutMenu();
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

    public static void Play() {
        m_lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        if (SkillEditorData.IsGeneric)
            SkillEditorAnimator.Play(SkillEditorData.SelectAnimationClip);
        else
            SkillEditorClip.Play(m_model, SkillEditorData.SelectAnimationClip);
    }

    private static void Update() {
        bool isGeneric = SkillEditorData.IsGeneric;
        if ((isGeneric && SkillEditorAnimator.IsPlayOver) || (!isGeneric && SkillEditorClip.IsPlayOver))
            Stop();
        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - m_lastTime);
        m_lastTime = currentTime;
        if (isGeneric)
            SkillEditorAnimator.Update(deltaTime);
        else
            SkillEditorClip.Update(deltaTime);
    }

    public static void Stop() {
        Debug.Log(InternalEditorUtility.unityPreferencesFolder);
        //EditorApplication.update -= Update;
    }

    public static void Exit() {
        if (!isEditorMode)
            return;
        EditorApplication.update = null;
        m_model = null;
        m_modelPath = string.Empty;
        SkillEditorWindow.CloseWindow();
        SkillEditorScene.UnregisterSceneGUI();
        EditorSceneManager.OpenScene(SkillEditorConst.ExitScenePath);
        EditorApplication.ExecuteMenuItem("Window/Layouts/Default");
    }
}