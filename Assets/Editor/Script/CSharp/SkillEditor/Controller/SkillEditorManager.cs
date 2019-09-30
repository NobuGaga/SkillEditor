using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using System.IO;

public static class SkillEditorManager{

    private static bool isEditorMode {
        get { return SkillEditorConfig.PrefabPath != string.Empty; }
    }

    public static void SelectPrefab() {
		string prefabPath = EditorUtility.OpenFilePanel(SkillEditorConfig.FilePanelTitle,
                                                        SkillEditorConfig.ModelPrefabPath, SkillEditorConfig.ModelPrefabExtension);
		if (prefabPath == null || prefabPath == string.Empty || prefabPath == SkillEditorConfig.PrefabPath)
			return;
        if (!isEditorMode && !EditorApplication.ExecuteMenuItem(SkillEditorConfig.SkillEditorMenuPath)) {
            SkillEditorTool.ClearConsole();
            File.Copy(SkillEditorConfig.LocalSkillEditorLayoutFilePath, SkillEditorConfig.SkillEditorLayoutFilePath);
            InternalEditorUtility.ReloadWindowLayoutMenu();
            EditorApplication.ExecuteMenuItem(SkillEditorConfig.SkillEditorMenuPath);
        }
        EditorSceneManager.OpenScene(SkillEditorConfig.EditScenePath);
        SkillEditorConfig.PrefabPath = prefabPath;
        SkillEditorController.Start(prefabPath);
    }

    public static void Play() {
        SkillEditorController.Play();
    }

    public static void Stop() {
        SkillEditorController.Stop();
    }

    public static void Exit() {
        if (!isEditorMode)
            return;
        SkillEditorController.Exit();
        SkillEditorConfig.Reset();
        EditorSceneManager.OpenScene(SkillEditorConfig.ExitScenePath);
        EditorApplication.ExecuteMenuItem(SkillEditorConfig.ExitLayoutMenuPath);
    }
}