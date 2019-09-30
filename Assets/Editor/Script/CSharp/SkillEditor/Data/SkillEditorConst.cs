using UnityEditorInternal;
using UnityEngine;
using System.IO;


public static class SkillEditorConst {

    public static readonly int ProjectPathSubIndex = Application.dataPath.IndexOf("Assets", System.StringComparison.Ordinal);
    public static readonly string ProjectPath = Application.dataPath.Substring(0, ProjectPathSubIndex);

    public const string ModelPrefabPath = "Assets/Editor/Asset/prefabs";
    public const string ModelPrefabExtension = "prefab";
    public const string FilePanelTitle = "模型预设路径";

    public const string ScenePath = "Assets/Editor/Scene";
    public const string EditSceneName = "EditScene";
    public const string ExitSceneName = "EditScene";
    public const string SceneExtension = "unity";
    public static readonly string EditScenePath = Path.Combine(ScenePath, FileWithExtension(EditSceneName, SceneExtension));
    public static readonly string ExitScenePath = Path.Combine(ScenePath, FileWithExtension(ExitSceneName, SceneExtension));

    public const string LayoutMenuPath = "Window/Layouts";
    public const string SkillEditorLayoutName = "SkillEditor";
    public const string LayoutExtension = "wlt";
    public static readonly string SkillEditorLayoutFullName = FileWithExtension(SkillEditorLayoutName, LayoutExtension);
    public static readonly string SkillEditorMenuPath = Path.Combine(LayoutMenuPath, SkillEditorLayoutName);
    public static readonly string LayoutFileGroupPath =
#if UNITY_EDITOR_WIN
    Path.Combine(InternalEditorUtility.unityPreferencesFolder, "Layouts");
#elif UNITY_EDITOR_OSX
    Path.Combine(InternalEditorUtility.unityPreferencesFolder, "Layouts/default");
#else
    string.Empty;
#endif
    public static readonly string SkillEditorLayoutFilePath = Path.Combine(LayoutFileGroupPath, SkillEditorLayoutFullName);
    public static readonly string LocalLayoutFileGroupPath = Path.Combine(ProjectPath, "Layout");
    public static readonly string LocalSkillEditorLayoutFilePath = Path.Combine(LocalLayoutFileGroupPath, SkillEditorLayoutFullName);

    public const short DefaultClipFrame = 30;
    public const short DefaultAnimationClipLength = 8;

    public const short DefaultGUIStyleCount = 8;
    public const short DefaultWindowButtonCount = 8;

    private static string FileWithExtension(string fileName, string extension) {
        return string.Format("{0}.{1}", fileName, extension);
    }
}