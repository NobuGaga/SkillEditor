using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class EditorScene {

        private static readonly Color CubeColor = Color.green;

        private static List<KeyValuePair<Vector3, HitData>> m_listPointHitData;

        public static void SetDrawHitData(List<KeyValuePair<Vector3, HitData>> list) => m_listPointHitData = list;

        [DrawGizmo(GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy)]
        public static void OnDrawCube(GameObject gameObject, GizmoType type) {
            if (m_listPointHitData == null || m_listPointHitData.Count == 0)
                return;
            Gizmos.color = CubeColor;
            for (int index = 0; index < m_listPointHitData.Count; index++) {
                HitData data = m_listPointHitData[index].Value;
                Vector3 footPoint = m_listPointHitData[index].Key + data.cubeData.Offset;
                footPoint.x += data.cubeData.width / 2;
                footPoint.y += data.cubeData.height / 2;
                Gizmos.DrawWireCube(footPoint, data.cubeData.Size);
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