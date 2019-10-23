using UnityEditorInternal;
using UnityEngine;
using StringComparison = System.StringComparison;

namespace SkillEditor {

    internal static class Config {

        // Common
        private static readonly int ProjectPathSubIndex = Application.dataPath.IndexOf("Assets", StringComparison.Ordinal);
        public static readonly string ProjectPath = Application.dataPath.Substring(0, ProjectPathSubIndex);
        public const string AnimatorControllerFolder = "animatorcontroller";
        public const string AnimationClipFolder = "models";
        public const string ModelPrefabFolder = "prefabs";
        public const string MetaExtension = "meta";
        public const string PrefabExtension = "prefab";
        public const string AnimationClipExtension = "FBX";
        public const string AnimationClipSymbol = "@";
        public const float FramesPerSecond = 1f / 30;

        // Prefab Group Structure
        public const string ModelPrefabPath = "Assets/Editor/Asset/prefabs";
        public const string ModelPrefabExtension = "prefab";
        public const string FilePanelTitle = "模型预设路径";
        private static string m_prefabFullPath = string.Empty;
        private static string m_clipFullPath = string.Empty;
        private static string m_controllerPath = string.Empty;
        public static string ControllerPath => m_controllerPath;
        public static string PrefabPath {
            set {
                m_prefabFullPath = value;
                if (m_prefabFullPath == string.Empty)
                    return;
                int subIndex = m_prefabFullPath.IndexOf("prefabs/", StringComparison.Ordinal);
                string modelFileGroupFullPath = m_prefabFullPath.Substring(0, subIndex);
                m_clipFullPath = Tool.CombinePath(modelFileGroupFullPath, "models");
                m_controllerPath = Tool.CombinePath(modelFileGroupFullPath, AnimatorControllerFolder);
                m_controllerPath = Tool.FullPathToProjectPath(m_controllerPath);
            }
            get { return m_prefabFullPath; }
        }
        public static string ClipGroupFullPath => m_clipFullPath;
        public const string TempModelName = "nvwang";

        // Weapon
        public static readonly string WeaponPath = Tool.CombinePath(Application.dataPath, "character/weapon");
        public const string WeaponFilePrefix = "wp_";
        public static readonly string WeaponAnimatorControllerPath = Tool.CombinePath(WeaponPath, AnimatorControllerFolder);
        public const short ModelWeaponCount = 2;

        // Scene
        private const string ScenePath = "Assets/Editor/Scene";
        private const string EditSceneName = "EditScene";
        private const string ExitSceneName = "EditScene";
        public const string SceneExtension = "unity";
        public static readonly string EditScenePath = Tool.CombinePath(ScenePath, Tool.FileWithExtension(EditSceneName, SceneExtension));
        public static readonly string ExitScenePath = Tool.CombinePath(ScenePath, Tool.FileWithExtension(ExitSceneName, SceneExtension));
        public const string DrawCubeNodeName = "FootCenter_Point00";

        // Layout
        private const string LayoutMenuPath = "Window/Layouts";
        private const string EditorLayoutName = "SkillEditor";
        private const string LayoutExtension = "wlt";
        private static readonly string EditorLayoutFullName = Tool.FileWithExtension(EditorLayoutName, LayoutExtension);
        public static readonly string MenuPath = Tool.CombinePath(LayoutMenuPath, EditorLayoutName);
        private static readonly string LayoutFileGroupPath =
    #if UNITY_EDITOR_WIN
        Tool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts");
    #elif UNITY_EDITOR_OSX
        Tool.CombinePath(InternalEditorUtility.unityPreferencesFolder, "Layouts/default");
    #else
        string.Empty;
    #endif
        public static readonly string EditorLayoutFilePath = Tool.CombinePath(LayoutFileGroupPath, EditorLayoutFullName);
        private static readonly string LocalLayoutFileGroupPath = Tool.CombinePath(Application.dataPath, "Editor/.Layout");
        public static readonly string LocalEditorLayoutFilePath = Tool.CombinePath(LocalLayoutFileGroupPath, EditorLayoutFullName);
        private const string ExitEditorLayoutName = "Default";
        public static readonly string ExitLayoutMenuPath = Tool.CombinePath(LayoutMenuPath, ExitEditorLayoutName);

        // Animation
        public const string AnimatorControllerExtension = "controller";

        // Structure
        public static readonly string AnimDataFilePath = Tool.CombinePath(ProjectPath, "Assets/Editor/Script/Lua/AnimClipData.lua");
        public const short ErrorIndex = -1;
        public const short LuaFileHeadLength = 64;
        public const string LuaFileHeadStart = "AnimClipData = AnimClipData or {}";
        public const short ModelCount = 2;
        public const short ModelStateCount = 4;
        public const short ModelStateClipCount = 8;
        public const short ModelStateClipFrameCount = 8;
        public const short ModelClipFrameCustomDataCount = 4;
        public const short ModelClipFrameCubeDataCount = 2;

        // Lua
        public const short ReadFileCount = 4;
        public const ushort AnimClipLuaFileLength = 32768;
        public const ushort StateListStringLength = 16384;
        public const ushort ClipListStringLength = 16384;
        public const ushort FrameListStringLength = 2048;
        public const ushort CustomDataListStringLength = 512;
        public const ushort RectDataListStringLength = 512;

        public static void Reset() {
            m_prefabFullPath = string.Empty;
            m_clipFullPath = string.Empty;
            m_controllerPath = string.Empty;
        }
    }
}