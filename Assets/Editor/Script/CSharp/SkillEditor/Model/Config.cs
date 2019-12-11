using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace SkillEditor {

    public static class Config {

        // Common
        private static readonly int ProjectPathSubIndex = Application.dataPath.IndexOf("Assets");
        public static readonly string ProjectPath = Application.dataPath.Substring(0, ProjectPathSubIndex);
        public const string AnimatorControllerFolder = "animatorcontroller";
        public const string AnimationClipFolder = "models";
        public const string ModelPrefabFolder = "prefabs";
        public const string MetaExtension = "meta";
        public const string PrefabExtension = "prefab";
        public const string AnimationClipExtension = "FBX";
        public const string AnimationClipSymbol = "@";
        public const ushort FrameCount = 30;
        public const float FramesPerSecond = 1f / FrameCount;

        // Prefab Group Structure
        public const string ModelPrefabPath = "Assets/character/hero";
        public const string ModelPrefabExtension = "prefab";
        public const string FilePanelTitle = "模型预设路径";
        public const string ModelPrefabPrefix = "hero_";

        // Weapon
        public static readonly string WeaponPath = Tool.CombinePath(Application.dataPath, "character/weapon");
        public const string WeaponFilePrefix = "wp_";
        public static readonly string WeaponAnimatorControllerPath = Tool.CombinePath(WeaponPath, AnimatorControllerFolder);
        public const short ModelWeaponCount = 2;
        public const string WeaponParentNode = "R_Weapon_Point";

        // Effect
        public static string[] ModelSkillEffectPath = new string[] { "Assets/fx/skill" };
        public const float RuntimeEffectDelay = FramesPerSecond * 2;

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
        public static readonly string AnimDataFilePath = Tool.CombinePath(ProjectPath, "../Resources/lua/data/config/AnimClipData.lua");
        public static readonly string EffectConfFilePath = Tool.CombinePath(ProjectPath, "../Resources/lua/data/config/EffectConf.lua");
        public const short ErrorIndex = -1;
        public const short LuaFileHeadLength = 64;
        public const string AnimClipDataFileHeadStart = "AnimClipData = AnimClipData or {}";
        public const string EffectConfFileHeadStart = "EffectConf = Class(\"EffectConf\", ConfigBase)";
        public const short ModelCount = 2;

        // Lua
        public const short ReadFileCount = 4;
        public const ushort FrameListStringLength = 2048;
        public const ushort CustomDataListStringLength = 512;
        public const ushort RectDataListStringLength = 512;

        public enum EffectFilter {
            GrabObj,
        }

        public static KeyValuePair<string, string>[] UseAutoSeedEffectList = new KeyValuePair<string, string>[] {
            new KeyValuePair<string, string>("p_nw_skill03a", "array"),
        };
    }
}