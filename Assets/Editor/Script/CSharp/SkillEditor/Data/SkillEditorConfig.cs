using UnityEditorInternal;
using UnityEngine;
using StringComparison = System.StringComparison;

public static class SkillEditorConfig {

    // Common
    private static readonly int ProjectPathSubIndex = Application.dataPath.IndexOf("Assets", StringComparison.Ordinal);
    private static readonly string ProjectPath = Application.dataPath.Substring(0, ProjectPathSubIndex);

    // Prefab Group Structure
    public const string ModelPrefabPath = "Assets/Editor/Asset/prefabs";
    public const string ModelPrefabExtension = "prefab";
    public const string FilePanelTitle = "模型预设路径";
    private static string m_prefabFullPath = string.Empty;
    private static string m_clipFullPath = string.Empty;
    private static string m_controllerPath = string.Empty;
    public static string PrefabPath {
        set {
            m_prefabFullPath = value;
            if (m_prefabFullPath == string.Empty)
                return;
            int subIndex = m_prefabFullPath.IndexOf("prefabs/", StringComparison.Ordinal);
            string modelFileGroupFullPath = m_prefabFullPath.Substring(0, subIndex);
            m_clipFullPath = SkillEditorTool.CombinePath(modelFileGroupFullPath, "models");
            m_controllerPath = SkillEditorTool.CombinePath(modelFileGroupFullPath, "animatorcontroller");
            m_controllerPath = SkillEditorTool.FullPathToProjectPath(m_controllerPath);
        }
        get { return m_prefabFullPath; }
    }
    public static string ClipGroupFullPath => m_clipFullPath;

    // Scene
    private const string ScenePath = "Assets/Editor/Scene";
    private const string EditSceneName = "EditScene";
    private const string ExitSceneName = "EditScene";
    public const string SceneExtension = "unity";
    public static readonly string EditScenePath = SkillEditorTool.CombinePath(ScenePath, SkillEditorTool.FileWithExtension(EditSceneName, SceneExtension));
    public static readonly string ExitScenePath = SkillEditorTool.CombinePath(ScenePath, SkillEditorTool.FileWithExtension(ExitSceneName, SceneExtension));

    // Layout
    private const string LayoutMenuPath = "Window/Layouts";
    private const string SkillEditorLayoutName = "SkillEditor";
    private const string LayoutExtension = "wlt";
    private static readonly string SkillEditorLayoutFullName = SkillEditorTool.FileWithExtension(SkillEditorLayoutName, LayoutExtension);
    public static readonly string SkillEditorMenuPath = SkillEditorTool.CombinePath(LayoutMenuPath, SkillEditorLayoutName);
    private static readonly string LayoutFileGroupPath =
#if UNITY_EDITOR_WIN
    SkillEditorTool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts");
#elif UNITY_EDITOR_OSX
    SkillEditorTool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts/default");
#else
    string.Empty;
#endif
    public static readonly string SkillEditorLayoutFilePath = SkillEditorTool.CombinePath(LayoutFileGroupPath, SkillEditorLayoutFullName);
    private static readonly string LocalLayoutFileGroupPath = SkillEditorTool.CombinePath(ProjectPath, "Layout");
    public static readonly string LocalSkillEditorLayoutFilePath = SkillEditorTool.CombinePath(LocalLayoutFileGroupPath, SkillEditorLayoutFullName);
    private const string ExitSkillEditorLayoutName = "Default";
    public static readonly string ExitLayoutMenuPath = SkillEditorTool.CombinePath(LayoutMenuPath, ExitSkillEditorLayoutName);

    // Animation
    public const string AnimatorControllerExtension = "controller";
    public const short DefaultAnimationClipLength = 8;

    // Window
    public const short DefaultGUIStyleCount = 8;
    public const short DefaultWindowButtonCount = 8;

    public static void Reset() {
        m_prefabFullPath = string.Empty;
        m_clipFullPath = string.Empty;
        m_controllerPath = string.Empty;
    }

    public static string GetAnimatorControllerPath(string fileName) {
        return SkillEditorTool.CombineFilePath(m_controllerPath, fileName, AnimatorControllerExtension);
    }
}