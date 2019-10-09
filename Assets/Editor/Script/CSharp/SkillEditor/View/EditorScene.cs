using UnityEditor;
using UnityEngine;

namespace SkillEditor {

    internal static class EditorScene {

        public static void RegisterSceneGUI() {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void UnregisterSceneGUI() {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public static void OnSceneGUI(SceneView sceneView) {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(controlID);
        }
    }
}