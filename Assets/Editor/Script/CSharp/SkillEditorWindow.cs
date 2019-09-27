using UnityEditor;

public class SkillEditorWindow : BaseEditorWindow {

    private static bool m_isSelectPrefab;
    private const string BtnSelectPrefab = "Select Prefab";

    private static bool m_isNoAnimationClip;

    private static int m_lastClipIndex;
    private static int m_curClipIndex;
    private static string[] m_animationClipNames;
    private static int[] m_animationClipIndexs;

    public static void Open() {
        Open<SkillEditorWindow>();
        Init();
    }

    public static void CloseWindow() {
        CloseWindow<SkillEditorWindow>();
    }

    public static void Init() {
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

    public static void SetDisplayData(string[] animationClipNames, int[] animationClipIndexs) {
        Clear();
        m_isSelectPrefab = true;
        m_animationClipNames = animationClipNames;
        m_animationClipIndexs = animationClipIndexs;
        m_isNoAnimationClip = m_animationClipNames == null || m_animationClipNames.Length == 0;
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
        HorizontalLayoutUI(TitleUI);
    }

    private void UnselectPrefabUI() {
        Label("Please select a model's prefab");
        if (Button(BtnSelectPrefab))
            OnSelectPrefabButton();
    }

    private void NoAnimationClipUI() {
        Label("Current prefab has no AnimationClip file");
    }

    private void OnSelectPrefabButton() {
        SkillEditorManager.OpenPrefab();
    }

    private void TitleUI() {
        if (m_animationClipNames != null && m_animationClipIndexs != null)
            m_curClipIndex = EditorGUILayout.IntPopup(m_curClipIndex, m_animationClipNames, m_animationClipIndexs, GetGUIStyle(Style.PreDropDown));
        if (m_lastClipIndex != m_curClipIndex) {
            m_lastClipIndex = m_curClipIndex;
            SkillEditorManager.SetAnimationClipData(m_curClipIndex);
        }
    }
}