using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class EditorScene {

        private static readonly Color CubeColor = Color.green;

        private static List<KeyValuePair<Vector3, CubeData>> m_listPointCubeData;

        public static void SetDrawCubeData(List<KeyValuePair<Vector3, CubeData>> list) => m_listPointCubeData = list;

        [DrawGizmo(GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy)]
        public static void OnDrawCube(GameObject gameObject, GizmoType type) {
            if (m_listPointCubeData == null || m_listPointCubeData.Count == 0)
                return;
            Gizmos.color = CubeColor;
            for (int index = 0; index < m_listPointCubeData.Count; index++) {
                CubeData data = m_listPointCubeData[index].Value;
                Vector3 footPoint = m_listPointCubeData[index].Key + data.Offset;
                footPoint.x += data.width / 2;
                footPoint.y += data.height / 2;
                Gizmos.DrawWireCube(footPoint, data.Size);
            }
        }

        public static void RegisterSceneGUI() => SceneView.duringSceneGui += OnSceneGUI;

        public static void UnregisterSceneGUI() => SceneView.duringSceneGui -= OnSceneGUI;

        public static void OnSceneGUI(SceneView sceneView) {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(controlID);
        }
    }
}