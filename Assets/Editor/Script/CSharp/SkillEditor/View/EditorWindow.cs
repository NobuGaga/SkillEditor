using System;
using System.Reflection;
using Lua;
using Lua.AnimClipData;

namespace SkillEditor {

    internal class EditorWindow : BaseEditorWindow {

        private const string WindowName = "技能编辑器窗口";

        private const string BtnSelectPrefab = "Select Prefab";
        private const string LabelSelectTips = "Please select a model's prefab";
        private const string LabelNoClipTips = "Current prefab has no AnimationClip file";
        private const string LabelModelName = "模型 ";
        private const string LabelWeapon = "武器 ";
        private const string LabelModelClipTips = "动画 ";
        private const string LabelModelClipStateTips = "状态组 ";

        private const string LabelFrameData = "帧数据 ";
        private const string BtnAddFrame = "增加帧数据";
        private const string BtnSave = "保存";
        private const string BtnDelete = "删除";
        private const string BtnAddEffect = "增加特效";
        private const string BtnAddCube = "增加碰撞框";
        private const string BtnAddCache = "增加 Cache Begin";
        private const string BtnAddSection = "增加 Section Over";
        private const string LabelTime = "触发时间点 ";
        private const string LabelPriority = "优先级 ";
        private const string LabelEffect = "特效";
        private const string LabelEffectType = "类型 ";
        private const string LabelEffectID = "ID ";
        private const string LabelCollision = "碰撞框";
        private const string LabelX = "x";
        private const string LabelY = "y";
        private const string LabelZ = "z";
        private const string LabelWidth = "width";
        private const string LabelHeight = "height";
        private const string LabelDepth = "depth";
        private const string BtnPlay = "Play";
        private const string BtnPause = "Pause";
        private const string BtnStop = "Stop";

        private static bool m_isSelectPrefab;
        private static bool m_isNoAnimationClip;

        private static int m_lastWeaponIndex;
        private static int m_lastClipIndex;
        private static bool IsNoSelectClip => m_lastClipIndex == Config.ErrorIndex;
        private static string[] m_animationClipNames;
        private static int[] m_animationClipIndexs;

        public static void Open() {
            Open<EditorWindow>(WindowName);
        }

        public static void CloseWindow() {
            Clear();
            GetWindow<EditorWindow>().Close();
        }

        public static void RefreshRepaint() {
            GetWindow<EditorWindow>().Repaint();
        } 

        public static void Clear() {
            m_isSelectPrefab = false;
            m_isNoAnimationClip = false;

            m_lastWeaponIndex = Config.ErrorIndex;
            m_lastClipIndex = Config.ErrorIndex;
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
            VerticalLayoutUI(EditorWindowUI);
        }

        private void UnselectPrefabUI() {
            Label(LabelSelectTips);
            if (Button(BtnSelectPrefab))
                Manager.SelectPrefab();
        }

        private void NoAnimationClipUI() => Label(LabelNoClipTips);

        private void EditorWindowUI() {
            Space();
            HorizontalLayoutUI(TitleUI);
            if (IsNoSelectClip || LuaAnimClipModel.ClipDataState == State.None)
                return;
            FrameListUI();
            Space();
            HorizontalLayoutUI(AnimationUI);
        }

        private void TitleUI() {
            string modelName = Config.TempModelName;
            SpaceWithLabel(Tool.GetCacheString(LabelModelName + modelName));
            WeaponUI(modelName);
            StateGroupUI();
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
            if (SpaceWithButton(BtnAddFrame))
                Controller.AddFrameData();
            if (SpaceWithButton(BtnSave))
                Controller.WriteAnimClipData();
            Space();
        }

        private void WeaponUI(string modelName) {
            string[] arrayWeaponName = WeaponModel.GetAllWeaponName(modelName);
            if (arrayWeaponName == null)
                return;
            SpaceWithLabel(LabelWeapon);
            int[] arrayIndex = WeaponModel.GetAllWeaponNameIndex(modelName);
            int tempIndex = IntPopup(m_lastWeaponIndex, arrayWeaponName, arrayIndex);
            if (tempIndex != m_lastWeaponIndex && tempIndex != Config.ErrorIndex) {
                m_lastWeaponIndex = tempIndex;
                Controller.SetWeapon(m_lastWeaponIndex);
            }
        }

        private void StateGroupUI() {
            if (IsNoSelectClip)
                return;
            SpaceWithLabel(LabelModelClipStateTips);
            State lastState = LuaAnimClipModel.ClipDataState;
            State selectState = (State)EnumPopup(lastState);
            if (selectState != lastState)
                Controller.SetAnimationStateData(selectState);
        }

        private FrameData GetFrameData(int index) => LuaAnimClipModel.GetFrameData(index);
        private void FrameListUI() {
            ClipData clipData = LuaAnimClipModel.ClipData;
            if (clipData.frameList == null || clipData.frameList.Length == 0)
                return;
            for (int index = 0; index < clipData.frameList.Length; index++) {
                Space();
                if (HorizontalLayoutUI(FrameDataTitleUI, index))
                    break;
                HorizontalLayoutUI(FrameDataUI, index);
                FrameData data = GetFrameData(index);
                if (!data.effectFrameData.IsNullTable()) {
                    HorizontalLayoutUI(EffectFrameDataTitleUI, index);
                    VerticalLayoutUI(EffectFrameDataUI, index);
                }
                if (!data.hitFrameData.IsNullTable()) {
                    HorizontalLayoutUI(HitFrameDataTitleUI, index);
                    VerticalLayoutUI(HitFrameDataUI, index);
                }
                if (!data.cacheFrameData.IsNullTable())
                    HorizontalLayoutUI(CacheFrameDataTitleUI, index);
                if (!data.sectionFrameData.IsNullTable())
                    HorizontalLayoutUI(SectionFrameDataTitleUI, index);
            }
        }

        private bool FrameDataTitleUI(int index) {
            FrameData data = GetFrameData(index);
            SpaceWithLabel(LabelFrameData + (index + 1));
            if (SpaceWithButton(BtnAddEffect))
                Controller.AddNewCustomData(index, FrameType.PlayEffect);
            if (SpaceWithButton(BtnAddCube))
                Controller.AddNewCustomData(index, FrameType.Hit);
            if (data.cacheFrameData.IsNullTable() && SpaceWithButton(BtnAddCache))
                Controller.AddPriorityFrameData(index, FrameType.CacheBegin);
            if (data.sectionFrameData.IsNullTable() && SpaceWithButton(BtnAddSection))
                Controller.AddPriorityFrameData(index, FrameType.SectionOver);
            if (SpaceWithButton(BtnDelete)) {
                Controller.DeleteFrameData(index);
                return true;
            }
            return false;
        }

        private void FrameDataUI(int index) {
            FrameData data = GetFrameData(index);
            SpaceWithLabel(LabelTime); 
            float time = TextField(data.time);
            if (time != data.time)
                Controller.SetFrameDataTime(index, time);
        }

        private void EffectFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.PlayEffect);
        private void EffectFrameDataUI(int frameIndex) => FrameDataListUI(frameIndex, FrameType.PlayEffect);
        private bool EffectDataUI(int frameIndex, object @object) {
            EffectData data = (EffectData)@object;
            SpaceWithLabel(LabelEffectType);
            data.type = TextField(data.type);
            SpaceWithLabel(LabelEffectID);
            data.id = TextField(data.id);
            EffectRotationData rotationData = data.rotation;
            SpaceWithLabel(LabelX);
            rotationData.x = TextField(rotationData.x);
            SpaceWithLabel(LabelY);
            rotationData.y = TextField(rotationData.y);
            SpaceWithLabel(LabelZ);
            rotationData.z = TextField(rotationData.z);
            data.rotation = rotationData;
            Controller.SetCustomeSubData(frameIndex, data, FrameType.PlayEffect);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCustomData(frameIndex, (int)data.index - 1, FrameType.PlayEffect);
            Space();
            return isDelete;
        }

        private void HitFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Hit);
        private void HitFrameDataUI(int frameIndex) => FrameDataListUI(frameIndex, FrameType.Hit);
        private bool CubeDataUI(int frameIndex, object  @object) {
            CubeData data = (CubeData)@object;
            SpaceWithLabel(LabelX);
            data.x = TextField(data.x);
            SpaceWithLabel(LabelY);
            data.y = TextField(data.y);
            SpaceWithLabel(LabelZ);
            data.z = TextField(data.z);
            SpaceWithLabel(LabelWidth);
            data.width = TextField(data.width);
            SpaceWithLabel(LabelHeight);
            data.height = TextField(data.height);
            SpaceWithLabel(LabelDepth);
            data.depth = TextField(data.depth);
            Controller.SetCustomeSubData(frameIndex, data, FrameType.Hit);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCustomData(frameIndex, (int)data.index - 1, FrameType.Hit);
            Space();
            return isDelete;
        }

        private void CacheFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.CacheBegin);
        private void SectionFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.SectionOver);
        private void PriorityFrameDataTitleUI(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            ushort originPriority = (ushort)table.GetFieldValueTableValue(PriorityFrameData.Key_Priority);
            switch (frameType) {
                case FrameType.PlayEffect:
                    SpaceWithLabel(LabelEffect);
                    break;
                case FrameType.Hit:
                    SpaceWithLabel(LabelCollision);
                    break;
                default:
                    SpaceWithLabel(table.GetKey());
                    break;
            }
            SpaceWithLabel(LabelPriority);
            ushort newPriority = (ushort)TextField(originPriority);
            if (newPriority != originPriority)
                Controller.SetFramePriorityData(index, frameType, newPriority);
        }

        private void FrameDataListUI(int frameIndex, FrameType frameType) {
            FrameData frameData = GetFrameData(frameIndex);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            CustomData<EffectData> defaultCustomData = default;
            object customData = table.GetFieldValueTableValue(defaultCustomData.GetKey());
            MethodInfo getTableListMethod = customData.GetType().GetMethod("GetTableList");
            Array dataList = getTableListMethod.Invoke(customData, null) as Array;
            Func<int, object, bool> uiFunction = default;
            switch (frameType) {
                case FrameType.PlayEffect:
                    uiFunction = EffectDataUI;
                    break;
                case FrameType.Hit:
                    uiFunction = CubeDataUI;
                    break;
            }
            for (int index = 0; index < dataList.Length; index++)
                if (HorizontalLayoutUI(uiFunction, frameData.index - 1, dataList.GetValue(index)))
                    break;
        }

        private void AnimationUI() {
            if (SpaceWithButton(BtnPlay))
                Controller.Play();
            if (SpaceWithButton(BtnPause))
                Controller.Pause();
            if (SpaceWithButton(BtnStop))
                Controller.Stop();
            Space();
            float playTime = Controller.PlayTime;
            float clipTime = AnimationModel.SelectAnimationClipTime;
            if (playTime > clipTime)
                playTime = clipTime;
            float time = Slider(playTime, clipTime);
            Controller.SetAnimationPlayTime(time);
        }
    }
}