using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class EditorScene {

        private static readonly Color HitCubeColor = Color.green;
        private static readonly Color GrabCubeColor = Color.yellow;
        private static readonly Color BlockCubeColor = Color.red;

        private static List<KeyValuePair<Vector3, ICubeData>> m_listPointHitData;
        private static List<KeyValuePair<Vector3, ICubeData>> m_listPointGrabData;
        private static List<KeyValuePair<Vector3, ICubeData>> m_listPointBlockData;

        public static void SetDrawHitData(List<KeyValuePair<Vector3, ICubeData>> list) => m_listPointHitData = list;
        public static void SetDrawGrabData(List<KeyValuePair<Vector3, ICubeData>> list) => m_listPointGrabData = list;
        public static void SetDrawBlockData(List<KeyValuePair<Vector3, ICubeData>> list) => m_listPointBlockData = list;

        [DrawGizmo(GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy)]
        public static void OnDrawCube(GameObject gameObject, GizmoType type) {
            if (m_listPointHitData != null && m_listPointHitData.Count > 0) {
                Gizmos.color = HitCubeColor;
                for (int index = 0; index < m_listPointHitData.Count; index++) {
                    Vector3 footPoint = m_listPointHitData[index].Key;
                    ICubeData data = m_listPointHitData[index].Value;
                    DrawCube(footPoint, data);
                }
            }
            if (m_listPointGrabData != null && m_listPointGrabData.Count > 0) {
                Gizmos.color = GrabCubeColor;
                for (int index = 0; index < m_listPointGrabData.Count; index++) {
                    Vector3 footPoint = m_listPointGrabData[index].Key;
                    ICubeData data = m_listPointGrabData[index].Value;
                    DrawCube(footPoint, data);
                }
             }
            if (m_listPointBlockData != null && m_listPointBlockData.Count > 0) {
                Gizmos.color = BlockCubeColor;
                for (int index = 0; index < m_listPointBlockData.Count; index++) {
                    Vector3 footPoint = m_listPointBlockData[index].Key;
                    ICubeData data = m_listPointBlockData[index].Value;
                    DrawCube(footPoint, data);
                }
            }
        }

        private static void DrawCube(Vector3 originPoint, ICubeData data) {
            Vector3 footPoint = originPoint + data.GetOffset();
            footPoint.x += data.GetWidth() / 2;
            footPoint.y += data.GetHeight() / 2;
            Gizmos.DrawWireCube(footPoint, data.GetSize());
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