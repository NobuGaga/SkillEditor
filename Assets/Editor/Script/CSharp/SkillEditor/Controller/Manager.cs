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
        }

        public static void WriteAnimClipData() {
            if (!isEditorMode) {
                Debug.LogError("不在技能编辑器模式下不能保存编辑");
                return;
            }
            Controller.WriteAnimClipData();
        }

        public static void CopyLocalLayoutFileToProject() {
            if (!File.Exists(Config.EditorLayoutFilePath)) {
                Debug.LogError("本地不存在技能编辑器布局文件");
                return;
            }
            File.Copy(Config.EditorLayoutFilePath, Config.LocalEditorLayoutFilePath, true);
        }

        private const ushort Save = 0;
        private const ushort Cancel = 1;
        private const ushort NotSave = 2;
        private static string[] Button = new string[] { "保存", "取消", "不保存" };

        public static bool Exit() {
            if (!isEditorMode)
                return true;
            ushort result = (ushort)EditorUtility.DisplayDialogComplex("退出技能编辑器", "是否保存当前动作的更改",
                                                                        Button[Save], Button[Cancel], Button[NotSave]);
            if (result == Cancel)
                return false;
            if (result == Save)
                Controller.WriteAnimClipData();
            AnimatorControllerManager.RevertAnimatorControllerFile();
            Controller.Exit();
            Config.Reset();
            EditorApplication.wantsToQuit -= Exit;
            EditorSceneManager.OpenScene(Config.ExitScenePath);
            EditorApplication.ExecuteMenuItem(Config.ExitLayoutMenuPath);
            return true;
        }
    }
}