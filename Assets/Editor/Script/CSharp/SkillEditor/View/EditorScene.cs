using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class EditorScene {

        private static readonly Color CubeColor = new Color(0, 0.7f, 0.7f, 1);

        private static List<CubeData> m_listDrawCubeData;

        public static void SetDrawCubeData(List<CubeData> list) {
            m_listDrawCubeData = list;
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy)]
        public static void OnDrawCube(GameObject gameObject, GizmoType type) {
            if (gameObject.name != Config.DrawCubeNodeName || m_listDrawCubeData == null || 
                m_listDrawCubeData.Count == 0)
                return;
            Gizmos.color = CubeColor;
            for (int index = 0; index < m_listDrawCubeData.Count; index++) {
                CubeData data = m_listDrawCubeData[index];
                Vector3 footPoint = gameObject.transform.position + data.Offset;
                footPoint.x += data.width / 2;
                footPoint.y += data.height / 2;
                Gizmos.DrawWireCube(footPoint, data.Size);
            }
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