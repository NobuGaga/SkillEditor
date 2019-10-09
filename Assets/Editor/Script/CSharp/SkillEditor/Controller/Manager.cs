using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using System.IO;

namespace SkillEditor {

    internal static class Manager {

        private static bool isEditorMode {
            get { return Config.PrefabPath != string.Empty; }
        }

        public static void SelectPrefab() {
		    string prefabPath = EditorUtility.OpenFilePanel(Config.FilePanelTitle,
                                                            Config.ModelPrefabPath, Config.ModelPrefabExtension);
		    if (prefabPath == null || prefabPath == string.Empty || prefabPath == Config.PrefabPath)
			    return;
            if (!isEditorMode && !EditorApplication.ExecuteMenuItem(Config.SkillEditorMenuPath)) {
                Tool.ClearConsole();
                File.Copy(Config.LocalSkillEditorLayoutFilePath, Config.SkillEditorLayoutFilePath);
                InternalEditorUtility.ReloadWindowLayoutMenu();
                EditorApplication.ExecuteMenuItem(Config.SkillEditorMenuPath);
            }
            EditorSceneManager.OpenScene(Config.EditScenePath);
            Config.PrefabPath = prefabPath;
            Controller.Start(prefabPath);
        }

        public static void Play() {
            Controller.Play();
        }

        public static void Stop() {
            Controller.Stop();
        }

        public static void Exit() {
            if (!isEditorMode)
                return;
            Controller.Exit();
            Config.Reset();
            EditorSceneManager.OpenScene(Config.ExitScenePath);
            EditorApplication.ExecuteMenuItem(Config.ExitLayoutMenuPath);
        }
    }
}