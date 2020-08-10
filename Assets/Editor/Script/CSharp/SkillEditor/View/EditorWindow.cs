using UnityEngine;
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
        private const string LabelRightWeapon = "右手武器 ";
        private const string LabelLeftWeapon = "左手武器 ";
        private const string LabelModelClipTips = "动画 ";
        private const string LabelModelClipStateTips = "状态组 ";
        private const string LabelModelClipIDTips = "动画 ID";
        private const string BtnAddClipGroupData = "增加动画数据";
        private const string BtnDeleteClipGroupData = "删除动画数据";

        private const string LabelFrameData = "帧数据 ";
        private const string BtnAddFrame = "增加帧数据";
        private const string BtnSave = "保存";
        private const string BtnDelete = "删除";
        private const string BtnReloadEffect = "重载特效表";
        private const string BtnAddEffect = "增加特效";
        private const string BtnAddCube = "增加碰撞框";
        private const string BtnAddTrack = "增加轨迹段";
        private const string BtnAddTrackChange = "增加轨迹改变";
        private const string BtnAdd = "增加 ";
        private const string BtnCopy = "复制帧数据";
        private const string LabelTime = "触发时间点 ";
        private const string LabelEndTime = "结束触发时间点 ";
        private const string LabelPriority = "优先级 ";
        private const string LabelLoop = "循环 ";
        private const string LabelEffect = "特效";
        private const string LabelEffectType = "类型 ";
        private const string LabelEffectID = "ID ";
        private const string LabelGrab = "抓取点";
        private const string LabelUngrab = "投技释放点";
        private const string LabelBlockStart = "轨迹格挡开始";
        private const string LabelBlockEnd = "轨迹格挡结束";
        private const string LabelBlock = "格挡框";
        private const string LabelTrack = "轨迹改变";
        private const string LabelGravityAccelerate = "重力加速度";
        private const string LabelHorizontalSpeed = "水平速度";
        private const string LabelCollision = "碰撞框";
        private const string LabelX = "x";
        private const string LabelY = "y";
        private const string LabelZ = "z";
        private const string LabelWidth = "width";
        private const string LabelHeight = "height";
        private const string LabelDepth = "depth";
        private const string LabelCrush = "碎尸";
        private const string LabelCameraTriggerType = "镜头触发类型";
        private const string LabelCameraFocusType = "镜头聚焦类型";
        private const string LabelReplaceID = "替换轨迹 ID";
        private const string BtnPlay = "Play";
        private const string BtnPause = "Pause";
        private const string BtnStop = "Stop";
        private const string LabelFrameFormat = "第 {0:f0} 帧";

        private static bool m_isSelectPrefab;
        private static bool m_isNoAnimationClip;

        private static int m_lastRightWeaponIndex;
        private static int m_lastLeftWeaponIndex;
        
        private static int m_lastClipIndex;
        private static bool IsNoSelectClip => m_lastClipIndex == Config.ErrorIndex;
        private static int m_lastIDIndex;
        private static string[] m_animationClipNames;
        private static int[] m_animationClipIndexs;

        public static void Open() => Open<EditorWindow>(WindowName);

        public static void CloseWindow() {
            Clear();
            GetWindow<EditorWindow>().Close();
        }

        public static void RefreshRepaint() => GetWindow<EditorWindow>().Repaint();

        public static void Clear() {
            m_isSelectPrefab = false;
            m_isNoAnimationClip = false;

            m_lastRightWeaponIndex = Config.ErrorIndex;
            m_lastLeftWeaponIndex = Config.ErrorIndex;
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
            if (!IsNoSelectClip && LuaAnimClipModel.CurrentState != State.None)
                FrameListUI();
            if (!IsNoSelectClip) {
                Space();
                HorizontalLayoutUI(AnimationUI);
            }
            Space();
        }

        private void TitleUI() {
            string modelName = ModelDataModel.ModelName;
            SpaceWithLabel(Tool.GetCacheString(LabelModelName + modelName));
            WeaponUI(modelName);
            bool isNoneState = StateAndIDUI();
            bool isNoClipGroupData = true;
            if (!isNoneState)
                isNoClipGroupData = ClipIDUI();
            SpaceWithLabel(LabelModelClipTips);
            int selectIndex = m_lastClipIndex;
            if (m_animationClipNames != null && m_animationClipIndexs != null)
                selectIndex = IntPopup(m_lastClipIndex, m_animationClipNames, m_animationClipIndexs);
            if (selectIndex != m_lastClipIndex && selectIndex != Config.ErrorIndex) {
                m_lastClipIndex = selectIndex;
                Controller.SetAnimationClipData(m_lastClipIndex);
            }
            if (IsNoSelectClip || isNoneState || isNoClipGroupData)
                return;
            if (SpaceWithButton(BtnAddFrame))
                Controller.AddFrameData();
            if (SpaceWithButton(BtnSave))
                Controller.WriteAnimClipData();
            if (SpaceWithButton(BtnReloadEffect))
                Controller.ReloadEffectConf();
            Space();
        }

        private void WeaponUI(string modelName) {
            string[] arrayWeaponName = WeaponModel.GetAllWeaponName(modelName);
            if (arrayWeaponName == null)
                return;
            SpaceWithLabel(LabelRightWeapon);
            int[] arrayIndex = WeaponModel.GetAllWeaponNameIndex(modelName);
            int tempIndex = IntPopup(m_lastRightWeaponIndex, arrayWeaponName, arrayIndex);
            if (tempIndex != m_lastRightWeaponIndex && tempIndex != Config.ErrorIndex) {
                m_lastRightWeaponIndex = tempIndex;
                Controller.SetRightWeapon(m_lastRightWeaponIndex);
            }
            SpaceWithLabel(LabelLeftWeapon);
            tempIndex = IntPopup(m_lastLeftWeaponIndex, arrayWeaponName, arrayIndex);
            if (tempIndex != m_lastLeftWeaponIndex && tempIndex != Config.ErrorIndex) {
                m_lastLeftWeaponIndex = tempIndex;
                Controller.SetLeftWeapon(m_lastLeftWeaponIndex);
            }
        }

        private bool StateAndIDUI() {
            if (IsNoSelectClip)
                return true;
            SpaceWithLabel(LabelModelClipStateTips);
            State lastState = LuaAnimClipModel.CurrentState;
            State selectState = (State)EnumPopup(lastState);
            if (selectState != lastState && selectState != State.None)
                Controller.SetAnimationStateData(selectState);
            return selectState == State.None;
        }

        private bool ClipIDUI() {
            uint id = LuaAnimClipModel.CurrentClipID;
            if (id != 0) {
                SpaceWithLabel(LabelModelClipIDTips);
                string[] idList = LuaAnimClipModel.GetClipGropuIDList(out int[] idIndexList);
                int index = IntPopup(m_lastIDIndex, idList, idIndexList);
                if (index != m_lastIDIndex && uint.TryParse(idList[index], out uint tempID)) {
                    m_lastIDIndex = index;
                    id = tempID;
                    Controller.SetClipGroupID(id);
                    RefreshRepaint();
                    return true;
                }
            }
            if (SpaceWithButton(BtnAddClipGroupData))
                InputTextWindow.Open();
            if (id == 0)
                return true;
            if (SpaceWithButton(BtnDeleteClipGroupData))
                Controller.DeleteClipGroupData();
            return false;
        }

        private FrameData GetFrameData(int index) => LuaAnimClipModel.GetFrameData(index);
        private Vector2 m_lastFrameListScrollPos;
        private void FrameListUI() {
            FrameData[] frameList = LuaAnimClipModel.FrameList;
            if (frameList == null || frameList.Length == 0)
                return;
            m_lastFrameListScrollPos = BeginVerticalScrollView(m_lastFrameListScrollPos);
            for (int index = 0; index < frameList.Length; index++) {
                Space();
                if (HorizontalLayoutUI(FrameDataTitleUI, index))
                    break;
                HorizontalLayoutUI(FrameDataUI, index);
                FrameData data = GetFrameData(index);
                if (!data.hitFrameData.IsNullTable()) {
                    HorizontalLayoutUI(HitFrameDataTitleUI, index);
                    HitFrameDataUI(index);
                }
                if (!data.effectFrameData.IsNullTable()) {
                    HorizontalLayoutUI(EffectFrameDataTitleUI, index);
                    EffectFrameDataUI(index);
                }
                if (!data.grabFrameData.IsNullTable()) {
                    HorizontalLayoutUI(GrabFrameDataTitleUI, index);
                    GrabFrameDataUI(index);
                }
                if (!data.ungrabFrameData.IsNullTable()) {
                    HorizontalLayoutUI(UngrabFrameDataTitleUI, index);
                    HorizontalLayoutUI(UngrabFrameDataUI, index);
                }
                if (!data.blockStartFrameData.IsNullTable())
                    HorizontalLayoutUI(BlockStartFrameDataTitleUI, index);
                if (!data.blockEndFrameData.IsNullTable())
                    HorizontalLayoutUI(BlockEndFrameDataTitleUI, index);
                if (!data.blockFrameData.IsNullTable()) {
                    HorizontalLayoutUI(BlockFrameDataTitleUI, index);
                    BlockFrameDataUI(index);
                }
                if (!data.trackFrameData.IsNullTable())
                    HorizontalLayoutUI(TrackFrameDataTitleUI, index);
                if (!data.cacheFrameData.IsNullTable())
                    HorizontalLayoutUI(CacheFrameDataTitleUI, index);
                if (!data.sectionFrameData.IsNullTable())
                    HorizontalLayoutUI(SectionFrameDataTitleUI, index);
                if (!data.airBeginFrameData.IsNullTable())
                    HorizontalLayoutUI(AirBeginFrameDataTitleUI, index);
                if (!data.airEndFrameData.IsNullTable())
                    HorizontalLayoutUI(AirEndFrameDataTitleUI, index);
                if (!data.dodgeFrameData.IsNullTable())
                    HorizontalLayoutUI(DodgeFrameDataTitleUI, index);
                if (!data.cameraFrameData.IsNullTable()) {
                    HorizontalLayoutUI(CameraFrameDataTitleUI, index);
                    HorizontalLayoutUI(CameraFrameDataUI, index);
                }
                if (!data.trackChangeFrameData.IsNullTable()) {
                    HorizontalLayoutUI(TrackChangeFrameDataTitleUI, index);
                    HorizontalLayoutUI(TrackChangeFrameDataUI, index);
                }
                if (!data.buffFrameData.IsNullTable()) {
                    HorizontalLayoutUI(BuffFrameDataTitleUI, index);
                    BuffFrameDataUI(index);
                }
            }
            EndVerticalScrollView();
        }

        private bool FrameDataTitleUI(int index) {
            FrameData data = GetFrameData(index);
            SetTextColor(Color.yellow);
            SpaceWithLabel(LabelFrameData + (index + 1));
            SetTextColor(Color.white);
            if (SpaceWithButton(BtnAddEffect))
                Controller.AddNewCustomData(index, FrameType.PlayEffect);
            if (SpaceWithButton(BtnAddCube))
                Controller.AddNewCustomData(index, FrameType.Hit);
            if (SpaceWithButton(BtnAdd + LabelGrab))
                Controller.AddNewCustomData(index, FrameType.Grab);
            if (data.ungrabFrameData.IsNullTable() && SpaceWithButton(BtnAdd + LabelUngrab))
                Controller.AddUngrabFrameData(index);
            if (SpaceWithButton(BtnAdd + LabelBlockStart))
                Controller.AddPriorityFrameData(index, FrameType.BlockStart);
            if (SpaceWithButton(BtnAdd + LabelBlockEnd))
                Controller.AddPriorityFrameData(index, FrameType.BlockEnd);
            if (SpaceWithButton(BtnAdd + LabelBlock))
                Controller.AddNewCustomData(index, FrameType.Block);
            if (data.trackFrameData.IsNullTable() && SpaceWithButton(BtnAddTrack))
                Controller.AddPriorityFrameData(index, FrameType.Track);
            if (data.cacheFrameData.IsNullTable() && SpaceWithButton(BtnAdd + FrameType.CacheBegin))
                Controller.AddPriorityFrameData(index, FrameType.CacheBegin);
            if (data.sectionFrameData.IsNullTable() && SpaceWithButton(BtnAdd + FrameType.SectionOver))
                Controller.AddPriorityFrameData(index, FrameType.SectionOver);
            if (data.airBeginFrameData.IsNullTable() && SpaceWithButton(BtnAdd + FrameType.OverheadStart))
                Controller.AddPriorityFrameData(index, FrameType.OverheadStart);
            if (data.airEndFrameData.IsNullTable() && SpaceWithButton(BtnAdd + FrameType.OverheadBreak))
                Controller.AddPriorityFrameData(index, FrameType.OverheadBreak);
            if (data.dodgeFrameData.IsNullTable() && SpaceWithButton(BtnAdd + FrameType.DodgeBreak))
                Controller.AddPriorityFrameData(index, FrameType.DodgeBreak);
            if (data.cameraFrameData.IsNullTable() && SpaceWithButton(BtnAdd + FrameType.Camera))
                Controller.AddCameraFrameData(index);
            if (data.trackChangeFrameData.IsNullTable() && SpaceWithButton(BtnAddTrackChange))
                Controller.AddTrackChangeFrameData(index);
            if (SpaceWithButton(BtnAdd + FrameType.Buff))
                Controller.AddNewCustomData(index, FrameType.Buff);
            if (SpaceWithButton(BtnCopy))
                Controller.AddCopyFrameData(GetFrameData(index));
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
            SpaceWithLabel(LabelEndTime); 
            float endTime = TextField(data.endTime);
            if (endTime != data.endTime)
                Controller.SetFrameDataEndTime(index, endTime);
        }

        private void EffectFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.PlayEffect);
        private void EffectFrameDataUI(int frameIndex) => FrameDataListUI(frameIndex, FrameType.PlayEffect);
        private bool EffectDataUI(int frameIndex, object @object) {
            EffectData data = (EffectData)@object;
            SpaceWithLabel(LabelEffectType);
            data.type = (EffectType)EnumPopup(data.type);
            SpaceWithLabel(LabelEffectID);
            data.id = TextField(data.id);
            Lua.EffectConf.EffectData transformData = LuaEffectConfModel.GetEffectData(data.id);
            if (data.type == EffectType.Hit) {
                EffectRotationData rotationData = data.rotation;
                SpaceWithLabel(LabelX);
                rotationData.x = TextField(rotationData.x);
                SpaceWithLabel(LabelY);
                rotationData.y = TextField(rotationData.y);
                SpaceWithLabel(LabelZ);
                rotationData.z = TextField(rotationData.z);
                data.rotation = rotationData;
            }
            else if (!transformData.IsNullTable()) {
                if (!transformData.offset.IsNullTable())
                    EffectTransformDataUI(transformData.offset);
                if (!transformData.scale.IsNullTable())
                    EffectTransformDataUI(transformData.scale);
                if (!transformData.rotation.IsNullTable())
                    EffectTransformDataUI(transformData.rotation);
            }
            Controller.SetCustomeSubData(frameIndex, data, FrameType.PlayEffect);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCustomData(frameIndex, (int)data.index - 1, FrameType.PlayEffect);
            Space();
            return isDelete;
        }

        private void EffectTransformDataUI(Lua.EffectConf.EffectConfTransform data) => SpaceWithLabel(data.VectorString);

        private void GrabFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Grab);
        private void GrabFrameDataUI(int frameIndex) => FrameDataListUI(frameIndex, FrameType.Grab);
        private bool GrabDataUI(int frameIndex, object @object) {
            GrabData data = (GrabData)@object;
            CubeData cubeData = data.cubeData;
            CubeDataUI(ref cubeData);
            data.cubeData = cubeData;
            Controller.SetCustomeSubData(frameIndex, data, FrameType.Grab);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCustomData(frameIndex, (int)data.index - 1, FrameType.Grab);
            Space();
            return isDelete;
        }

        private void UngrabFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Ungrab);
        private void UngrabFrameDataUI(int frameIndex) {
            FrameData frameData = GetFrameData(frameIndex);
            UngrabFrameData ungrabFrameData = frameData.ungrabFrameData;
            UngrabData ungrabData = ungrabFrameData.ungrabData;
            SpaceWithLabel(LabelGravityAccelerate);
            ungrabData.gravityAccelerate = TextField(ungrabData.gravityAccelerate);
            SpaceWithLabel(LabelHorizontalSpeed);
            ungrabData.horizontalSpeed = TextField(ungrabData.horizontalSpeed);
            ungrabFrameData.ungrabData = ungrabData;
            Controller.SetUngrabFrameData(frameIndex, ungrabFrameData);
        }

        private void BlockFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Block);
        private void BlockFrameDataUI(int frameIndex) => FrameDataListUI(frameIndex, FrameType.Block);
        private bool BlockDataUI(int frameIndex, object @object) {
            BlockData data = (BlockData)@object;
            CubeData cubeData = data.cubeData;
            CubeDataUI(ref cubeData);
            data.cubeData = cubeData;
            Controller.SetCustomeSubData(frameIndex, data, FrameType.Block);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCustomData(frameIndex, (int)data.index - 1, FrameType.Block);
            Space();
            return isDelete;
        }

        private void HitFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Hit);
        private void HitFrameDataUI(int frameIndex) => FrameDataListUI(frameIndex, FrameType.Hit);
        private bool HitDataUI(int frameIndex, object @object) {
            HitData data = (HitData)@object;
            CubeData cubeData = data.cubeData;
            CubeDataUI(ref cubeData);
            data.cubeData = cubeData;
            SpaceWithLabel(LabelCrush);
            data.crush = Toggle(data.crush);
            Controller.SetCustomeSubData(frameIndex, data, FrameType.Hit);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCustomData(frameIndex, (int)data.index - 1, FrameType.Hit);
            Space();
            return isDelete;
        }
        private void CubeDataUI(ref CubeData cubeData) {
            SpaceWithLabel(LabelX);
            cubeData.x = TextField(cubeData.x);
            SpaceWithLabel(LabelY);
            cubeData.y = TextField(cubeData.y);
            SpaceWithLabel(LabelZ);
            cubeData.z = TextField(cubeData.z);
            SpaceWithLabel(LabelWidth);
            cubeData.width = TextField(cubeData.width);
            SpaceWithLabel(LabelHeight);
            cubeData.height = TextField(cubeData.height);
            SpaceWithLabel(LabelDepth);
            cubeData.depth = TextField(cubeData.depth);
        }

        private void BlockStartFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.BlockStart);
        private void BlockEndFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.BlockEnd);
        private void TrackFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Track);
        private void CacheFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.CacheBegin);
        private void SectionFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.SectionOver);
        private void AirBeginFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.OverheadStart);
        private void AirEndFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.OverheadBreak);
        private void DodgeFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.DodgeBreak);
        private void PriorityFrameDataTitleUI(int index, FrameType frameType) {
            switch (frameType) {
                case FrameType.PlayEffect:
                    SpaceWithLabel(LabelEffect);
                    break;
                case FrameType.Hit:
                    SpaceWithLabel(LabelCollision);
                    break;
                case FrameType.Grab:
                    SpaceWithLabel(LabelGrab);
                    break;
                case FrameType.Ungrab:
                    SpaceWithLabel(LabelUngrab);
                    break;
                case FrameType.BlockStart:
                    SpaceWithLabel(LabelBlockStart);
                    break;
                case FrameType.BlockEnd:
                    SpaceWithLabel(LabelBlockEnd);
                    break;
                case FrameType.Block:
                    SpaceWithLabel(LabelBlock);
                    break;
                case FrameType.TrackChange:
                    SpaceWithLabel(LabelTrack);
                    break;
                default:
                    SpaceWithLabel(frameType.ToString());
                    break;
            }
            SpaceWithLabel(LabelPriority);
            FrameData frameData = GetFrameData(index);
            IFieldValueTable table = (IFieldValueTable)frameData.GetFieldValueTableValue(frameType.ToString());
            ushort originPriority = (ushort)table.GetFieldValueTableValue(CommonFrameData.Key_Priority);
            ushort newPriority = (ushort)TextField(originPriority);
            if (newPriority != originPriority)
                Controller.SetFramePriorityData(index, frameType, newPriority);
            SpaceWithLabel(LabelLoop);
            ICommonFrameData commonFrameData = (ICommonFrameData)table;
            bool isLoop = Toggle(commonFrameData.GetLoop());
            Controller.SetFrameLoopData(index, frameType, isLoop);
            if (SpaceWithButton(BtnDelete))
                Controller.DeletePriorityFrameData(index, frameType);
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
                    uiFunction = HitDataUI;
                    break;
                case FrameType.Buff:
                    uiFunction = BuffDataUI;
                    break;
                case FrameType.Grab:
                    uiFunction = GrabDataUI;
                    break;
                case FrameType.Block:
                    uiFunction = BlockDataUI;
                    break;
            }
            if (dataList == null)
                return;
            for (int index = 0; index < dataList.Length; index++)
                if (HorizontalLayoutUI(uiFunction, frameData.index - 1, dataList.GetValue(index)))
                    break;
        }

        private void CameraFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Camera);
        private void CameraFrameDataUI(int frameIndex) {
            FrameData frameData = GetFrameData(frameIndex);
            CameraFrameData cameraFrameData = frameData.cameraFrameData;
            CameraData cameraData = cameraFrameData.cameraData;
            SpaceWithLabel(LabelEffectID);
            cameraData.id = TextField(cameraData.id);
            SpaceWithLabel(LabelCameraTriggerType);
            cameraData.triggerType = (CameraTriggerType)EnumPopup(cameraData.triggerType);
            SpaceWithLabel(LabelCameraFocusType);
            cameraData.focusType = (CameraFocusType)EnumPopup(cameraData.focusType);
            cameraFrameData.cameraData = cameraData;
            Controller.SetCameraFrameData(frameIndex, cameraFrameData);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCameraFrameData(frameIndex);
        }

        private void TrackChangeFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.TrackChange);
        private void TrackChangeFrameDataUI(int frameIndex) {
            FrameData frameData = GetFrameData(frameIndex);
            TrackChangeFrameData trackChangeFrameData = frameData.trackChangeFrameData;
            TrackChangeData trackChangeData = trackChangeFrameData.trackChangeData;
            SpaceWithLabel(LabelEffectID);
            trackChangeData.id = TextField(trackChangeData.id);
            SpaceWithLabel(LabelReplaceID);
            trackChangeData.replaceID = TextField(trackChangeData.replaceID);
            Controller.SetTrackChangeFrameData(frameIndex, trackChangeFrameData);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteTrackChangeFrameData(frameIndex);
        }

        private void BuffFrameDataTitleUI(int index) => PriorityFrameDataTitleUI(index, FrameType.Buff);
        private void BuffFrameDataUI(int frameIndex) => FrameDataListUI(frameIndex, FrameType.Buff);
        private bool BuffDataUI(int frameIndex, object @object) {
            BuffData data = (BuffData)@object;
            SpaceWithLabel(LabelEffectType);
            data.type = (BuffType)EnumPopup(data.type);
            SpaceWithLabel(LabelEffectID);
            data.id = TextField(data.id);
            Controller.SetCustomeSubData(frameIndex, data, FrameType.Buff);
            bool isDelete = SpaceWithButton(BtnDelete);
            if (isDelete)
                Controller.DeleteCustomData(frameIndex, (int)data.index - 1, FrameType.Buff);
            Space();
            return isDelete;
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
            float frameCount = time / Config.FramesPerSecond;
            SpaceWithLabel(string.Format(LabelFrameFormat, frameCount));
            Controller.SetAnimationPlayTime(time);
        }
    }
}