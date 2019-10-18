using UnityEngine;
using System.Collections.Generic;

namespace SkillEditor {

    using SkillEditor.LuaStructure;

    internal class EditorWindow : BaseEditorWindow {

        private const string WindowName = "技能编辑器窗口";

        private const string BtnSelectPrefab = "Select Prefab";
        private const string LabelSelectTips = "Please select a model's prefab";
        private const string LabelNoClipTips = "Current prefab has no AnimationClip file";
        private const string LabelModelName = "模型名 ";
        private const string LabelModelClipTips = "动画名 ";
        private const string LabelModelClipStateTips = "状态机组 ";

        private const string LabelFrameGroupType = "帧类型组 ";
        private const string BtnAdd = "增加";
        private const string LabelFrameType = "帧类型 ";

        private const string BtnPlay = "Play";
        private const string BtnPause = "Pause";
        private const string BtnStop = "Stop";

        private static bool m_isSelectPrefab;
        private static bool m_isNoAnimationClip;

        private static int m_lastClipIndex;
        private static bool IsNoSelectClip => m_lastClipIndex == Config.ErrorIndex;
        private static string[] m_animationClipNames;
        private static int[] m_animationClipIndexs;

        private static int m_lastFrameIndex;
        private static bool IsNoSelectFrameGroup => m_lastFrameIndex == Config.ErrorIndex;
        private static readonly string[] FrameKeyArray = { ClipData.Key_KeyFrame, ClipData.Key_ProcessFrame };
        private static readonly string[] FrameKeyNameArray = { "关键帧", "动画帧" };
        private static readonly int[] FrameKeyNameIndexArray = { 0, 1 };
        private static string FrameGroupKey {
            get {
                if (IsNoSelectFrameGroup)
                    return default;
                return FrameKeyArray[m_lastFrameIndex];
            }
        }

        private static ClipData m_curClipData;

        public static void Open() {
            Open<EditorWindow>(WindowName);
        }

        public static void CloseWindow() {
            Clear();
            GetWindow<EditorWindow>().Close();
        }

        public static void Clear() {
            m_isSelectPrefab = false;
            m_isNoAnimationClip = false;

            m_lastClipIndex = Config.ErrorIndex;
            m_animationClipNames = null;
            m_animationClipIndexs = null;

            m_lastFrameIndex = Config.ErrorIndex;
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
            VerticalLayoutUI(EditorWindowUI);
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

        private void EditorWindowUI() {
            Space();
            HorizontalLayoutUI(TitleUI);
            if (IsNoSelectClip)
                return;
            Space();
            HorizontalLayoutUI(FrameKeyNameUI);
            FrameListUI();
            Space();
            HorizontalLayoutUI(AnimationUI);
        }

        private void TitleUI() {
            SpaceWithLabel(Tool.GetCacheString(LabelModelName + LuaAnimClipModel.ModelName));
            SpaceWithLabel(LabelModelClipTips);
            int selectIndex = m_lastClipIndex;
            if (m_animationClipNames != null && m_animationClipIndexs != null)
                selectIndex = IntPopup(m_lastClipIndex, m_animationClipNames, m_animationClipIndexs);
            if (selectIndex != m_lastClipIndex && selectIndex != Config.ErrorIndex) {
                m_lastClipIndex = selectIndex;
                Controller.SetAnimationClipData(m_lastClipIndex);
            }
            if (IsNoSelectClip)
                return;
            Space();
            SpaceWithLabel(LabelModelClipStateTips);
            State lastState = LuaAnimClipModel.ClipDataState;
            State selectState = (State)EnumPopup(lastState);
            if (selectState != lastState)
                Controller.SetAnimClipData(selectState);
        }

        private void AnimationUI() {
            if (SpaceWithButton(BtnPlay))
                OnPlayButton();
            if (SpaceWithButton(BtnPause))
                OnPauseButton();
            if (SpaceWithButton(BtnStop))
                OnStopButton();
        }

        private void OnPlayButton() => Controller.Play();

        private void OnPauseButton() => Controller.Pause();

        private void OnStopButton() => Controller.Stop();

        private void FrameKeyNameUI() {
            SpaceWithLabel(LabelFrameGroupType);
            m_lastFrameIndex = IntPopup(m_lastFrameIndex, FrameKeyNameArray, FrameKeyNameIndexArray);
            if (IsNoSelectFrameGroup)
                return;
            if (SpaceWithButton(BtnAdd))
                Controller.AddNewKeyFrameData(FrameGroupKey);
        }

        private void FrameListUI() {
            if (IsNoSelectFrameGroup)
                return;
            m_curClipData = LuaAnimClipModel.ClipData;
            KeyFrameData[] array = m_curClipData.GetKeyFrameList(FrameGroupKey);
            if (array == null || array.Length == 0)
                return;
            Space();
            for (int index = 0; index < array.Length; index++) {
                HorizontalLayoutUI(FrameUI, index);
                Space();
            }
        }

        private void FrameUI(int index) {
            SpaceWithLabel(LabelFrameType);
            KeyFrameData data = LuaAnimClipModel.GetKeyFrameData(FrameGroupKey, index);
            data.frameType = (FrameType)EnumPopup(data.frameType);
            data.time = SpaceWithTextField(data.time);
            data.priority = (short)SpaceWithTextField(data.priority);
            Controller.SetAnimClipData(FrameGroupKey, index, data);
        }
    }
}