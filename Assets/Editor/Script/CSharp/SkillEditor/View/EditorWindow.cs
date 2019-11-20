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
            if (!IsNoSelectClip) {
                SpaceWithLabel(LabelModelClipStateTips);
                State lastState = LuaAnimClipModel.ClipDataState;
                State selectState = (State)EnumPopup(lastState);
                if (selectState != lastState)
                    Controller.SetAnimationStateData(selectState);
            }
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
                OnAddFrameDataButton();
            if (SpaceWithButton(BtnSave))
                OnSaveButton();
            Space();
        }
        private void OnAddFrameDataButton() => Controller.AddFrameData();
        private void OnSaveButton() => Controller.WriteAnimClipData();

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

        private FrameData GetFrameData(int index) => LuaAnimClipModel.GetFrameData(index);
        private void FrameListUI() {
            ClipData clipData = LuaAnimClipModel.ClipData;
            if (clipData.frameList == null || clipData.frameList.Length == 0)
                return;
            for (int index = 0; index < clipData.frameList.Length; index++) {
                Space();
                HorizontalLayoutUI(FrameDataTitleUI, index);
                HorizontalLayoutUI(FrameDataUI, index);
                FrameData data = GetFrameData(index);
                if (!data.effectFrameData.IsNullTable()) {
                    HorizontalLayoutUI(EffectFrameDataTitleUI, index);
                    VerticalLayoutUI(EffectFrameDataUI, index);
                }
                if (!data.hitFrameData.IsNullTable()) {
                    HorizontalLayoutUI(HitFrameDataTitleUI, data.hitFrameData);
                    VerticalLayoutUI(HitFrameDataUI, data);
                }
                if (!data.cacheFrameData.IsNullTable())
                    HorizontalLayoutUI(PriorityFrameDataUI, data.cacheFrameData);
                if (!data.sectionFrameData.IsNullTable())
                    HorizontalLayoutUI(PriorityFrameDataUI, data.sectionFrameData);
            }
        }

        private void FrameDataTitleUI(int index) {
            FrameData data = GetFrameData(index);
            SpaceWithLabel(LabelFrameData + (index + 1));
            if (SpaceWithButton(BtnAddEffect))
                OnAddEffectDataButton(index);
            if (SpaceWithButton(BtnAddCube))
                OnAddCubeDataButton(index);
            if (data.cacheFrameData.IsNullTable() && SpaceWithButton(BtnAddCache))
                OnAddCacheDataButton(index);
            if (data.sectionFrameData.IsNullTable() && SpaceWithButton(BtnAddSection))
                OnAddSectionDataButton(index);
            if (SpaceWithButton(BtnDelete))
                OnDeleteFrameDataButton(index);
        }
        private void OnAddEffectDataButton(int index) => Controller.AddNewEffectData(index);
        private void OnAddCubeDataButton(int index) => Controller.AddNewCubeData(index);
        private void OnAddCacheDataButton(int index) => Controller.AddNewCacheData(index);
        private void OnAddSectionDataButton(int index) => Controller.AddNewSectionData(index);
        private void OnDeleteFrameDataButton(int index) => Controller.DeleteFrameData(index);

        private void FrameDataUI(int index) {
            FrameData data = GetFrameData(index);
            SpaceWithLabel(LabelTime); 
            float time = TextField(data.time);
            if (time != data.time)
                Controller.SetFrameDataTime(index, time);
        }

        private void EffectFrameDataTitleUI(int index) {
            FrameData frameData = GetFrameData(index);
            EffectFrameData effectFrameData = frameData.effectFrameData;
            SpaceWithLabel(LabelEffect);
            SpaceWithLabel(LabelPriority);
            ushort priority = (ushort)TextField(effectFrameData.priority);
            if (priority != effectFrameData.priority)
                Controller.SetEffectFramePriorityData(index, priority);
        }

        private void EffectFrameDataUI(object data) {
            FrameData frameData = (FrameData)data;
            EffectFrameData effectFrameData = frameData.effectFrameData;
            EffectData[] dataList = effectFrameData.effectData.dataList;
            for (int index = 0; index < dataList.Length; index++)
                dataList[index] = (EffectData)HorizontalLayoutUI(EffectDataUI, dataList[index], frameData.index - 1);
        }

        private object EffectDataUI(object @object, int framIndex) {
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
            if (SpaceWithButton(BtnDelete))
                OnDeleteEffectDataButton(framIndex, (int)data.index - 1);
            Space();
            return data;
        }
        private void OnDeleteEffectDataButton(int framIndex, int effectIndex) => Controller.DeleteEffectData(framIndex, effectIndex);

        private object HitFrameDataTitleUI(object data) {
            HitFrameData hitFrameData = (HitFrameData)data;
            SpaceWithLabel(LabelCollision);
            SpaceWithLabel(LabelPriority);
            hitFrameData.priority = (ushort)TextField(hitFrameData.priority);
            return hitFrameData;
        }

        private void HitFrameDataUI(object data) {
            FrameData frameData = (FrameData)data;
            HitFrameData hitFrameData = frameData.hitFrameData;
            CubeData[] dataList = hitFrameData.cubeData.dataList;
            for (int index = 0; index < dataList.Length; index++)
                dataList[index] = (CubeData)HorizontalLayoutUI(CubeDataUI, dataList[index], frameData.index - 1);
            hitFrameData.cubeData.dataList = dataList;
        }

        private object CubeDataUI(object @object, int frameIndex) {
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
            if (SpaceWithButton(BtnDelete))
                OnDeleteCubetDataButton(frameIndex, (int)data.index - 1);
            Space();
            return data;
        }
        private void OnDeleteCubetDataButton(int frameIndex, int effectIndex) => Controller.DeleteEffectData(frameIndex, effectIndex);

        private object PriorityFrameDataUI(object data) {
            PriorityFrameData priorityFrameData = (PriorityFrameData)data;
            SpaceWithLabel(priorityFrameData.GetKey());
            SpaceWithLabel(LabelPriority);
            priorityFrameData.priority = (ushort)TextField(priorityFrameData.priority);
            return priorityFrameData;
        }

        private void AnimationUI() {
            if (SpaceWithButton(BtnPlay))
                OnPlayButton();
            if (SpaceWithButton(BtnPause))
                OnPauseButton();
            if (SpaceWithButton(BtnStop))
                OnStopButton();
            Space();
            float playTime = Controller.PlayTime;
            float clipTime = AnimationModel.SelectAnimationClipTime;
            if (playTime > clipTime)
                playTime = clipTime;
            float time = Slider(playTime, clipTime);
            Controller.SetAnimationPlayTime(time);
        }
        private void OnPlayButton() => Controller.Play();
        private void OnPauseButton() => Controller.Pause();
        private void OnStopButton() => Controller.Stop();
    }
}