using UnityEngine;
using UnityEditor;

namespace SkillEditor {

    internal class EditorWindow : BaseEditorWindow {

        private const string WindowName = "技能编辑器窗口";
        private const string BtnSelectPrefab = "Select Prefab";
        private const string LabelSelectTips = "Please select a model's prefab";
        private const string LabelNoClipTips = "Current prefab has no AnimationClip file";
        private const string LabelModelNameTitle = "模型名 ";

        private static bool m_isSelectPrefab;
        private static bool m_isNoAnimationClip;

        private static int m_lastClipIndex;
        private static int m_curClipIndex;
        private static string[] m_animationClipNames;
        private static int[] m_animationClipIndexs;

        public static void Open() {
            Open<EditorWindow>(WindowName);
            Init();
        }

        public static void CloseWindow() {
            Clear();
            CloseWindow<EditorWindow>();
        }

        private static void Init() {
            AddButtonData(BtnSelectPrefab, Style.PreButton);
        }

        public static void Clear() {
            m_isSelectPrefab = false;
            m_isNoAnimationClip = false;

            m_lastClipIndex = -1;
            m_curClipIndex = 0;
            m_animationClipNames = null;
            m_animationClipIndexs = null;
        }

        public static void InitData(string[] animationClipNames, int[] animationClipIndexs) {
            Clear();
            m_isSelectPrefab = true;
            m_animationClipNames = animationClipNames;
            m_animationClipIndexs = animationClipIndexs;
            m_isNoAnimationClip = animationClipNames == null || animationClipNames.Length == 0;
        }

        private void OnGUI() {
            if (!m_isSelectPrefab) {
                CenterLayoutUI(UnselectPrefabUI);
                return;
            }
            if (m_isNoAnimationClip) {
                CenterLayoutUI(NoAnimationClipUI);
                return;
            }
            Space();
            HorizontalLayoutUI(TitleUI);
        }

        private void UnselectPrefabUI() {
            Label(LabelSelectTips);
            if (Button(BtnSelectPrefab))
                OnSelectPrefabButton();
        }

        private void NoAnimationClipUI() {
            Label(LabelNoClipTips);
        }

        private void OnSelectPrefabButton() {
            Manager.SelectPrefab();
        }

        private void TitleUI() {
            Space();
            Label(Tool.GetCacheString(LabelModelNameTitle + LuaAnimClipModel.ModelName));
            Space();
            if (m_animationClipNames != null && m_animationClipIndexs != null)
                m_curClipIndex = EditorGUILayout.IntPopup(m_curClipIndex, m_animationClipNames, m_animationClipIndexs, GetGUIStyle(Style.PreDropDown));
            if (m_lastClipIndex != m_curClipIndex) {
                m_lastClipIndex = m_curClipIndex;
                Controller.SetAnimationClipData(m_curClipIndex);
            }
        }
    }
}