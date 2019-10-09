using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using System.IO;

namespace SkillEditor {

    internal static class Manager {

        private static bool isEditorMode {
            get { return Config.PrefabPath != string.Empty; }
        }

        private static bool NotInEditorModeHanle() {
            if (isEditorMode)
                return false;
            Debug.LogError("不在技能编辑器模式");
            return true;
        }

        public static void SelectPrefab() {
		    string prefabPath = EditorUtility.OpenFilePanel(Config.FilePanelTitle,
                                                            Config.ModelPrefabPath, Config.ModelPrefabExtension);
		    if (prefabPath == null || prefabPath == string.Empty || prefabPath == Config.PrefabPath)
			    return;
            if (!isEditorMode && !EditorApplication.ExecuteMenuItem(Config.MenuPath)) {
                Tool.ClearConsole();
                File.Copy(Config.LocalEditorLayoutFilePath, Config.EditorLayoutFilePath);
                InternalEditorUtility.ReloadWindowLayoutMenu();
                EditorApplication.ExecuteMenuItem(Config.MenuPath);
            }
            EditorSceneManager.OpenScene(Config.EditScenePath);
            EditorApplication.wantsToQuit += Exit;
            Config.PrefabPath = prefabPath;
            Controller.Start(prefabPath);
            LuaManager.Start();
        }

        public static void Play() {
            if (NotInEditorModeHanle())
                return;
            Controller.Play();
        }

        public static void Stop() {
            if (NotInEditorModeHanle())
                return;
            Controller.Stop();
        }

        public static bool Exit() {
            if (!isEditorMode)
                return true;
            Controller.Exit();
            Config.Reset();
            EditorApplication.wantsToQuit -= Exit;
            EditorSceneManager.OpenScene(Config.ExitScenePath);
            EditorApplication.ExecuteMenuItem(Config.ExitLayoutMenuPath);
            return true;
        }
    }
}