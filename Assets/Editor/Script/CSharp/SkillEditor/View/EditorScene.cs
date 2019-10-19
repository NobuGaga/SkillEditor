using UnityEditor;
using UnityEngine;

namespace SkillEditor {

    using LuaStructure;

    internal static class EditorScene {

        private static readonly Color CubeColor = new Color(0, 0.7f, 0.7f, 1);

        [DrawGizmo(GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy)]
        public static void OnDrawCube(GameObject gameObject, GizmoType type, CubeData cubeData) {
            // if (gameObject.name != Config.DrawCubeNodeName)
            //     return;
            if (cubeData.IsNullTable())
                return;
            Gizmos.color = CubeColor;
            Vector3 footPos = gameObject.transform.position + cubeData.Offset;
            Gizmos.DrawWireCube(footPos, cubeData.Szie);
        }

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