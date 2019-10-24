﻿namespace SkillEditor {

    using SkillEditor.LuaStructure;

    internal class EditorWindow : BaseEditorWindow {

        private const string WindowName = "技能编辑器窗口";

        private const string BtnSelectPrefab = "Select Prefab";
        private const string LabelSelectTips = "Please select a model's prefab";
        private const string LabelNoClipTips = "Current prefab has no AnimationClip file";
        private const string LabelModelName = "模型 ";
        private const string LabelWeapon = "武器 ";
        private const string LabelModelClipTips = "动画 ";
        private const string LabelModelClipStateTips = "状态组 ";

        private const string LabelFrameGroupType = "帧类型组 ";
        private const string BtnAdd = "增加";
        private const string LabelFrameType = "帧类型 ";
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

            m_lastWeaponIndex = Config.ErrorIndex;
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
            if (IsNoSelectClip || LuaAnimClipModel.ClipDataState == State.None)
                return;
            Space();
            HorizontalLayoutUI(StateGroupUI);
            FrameListUI();
            Space();
            HorizontalLayoutUI(AnimationUI);
        }

        private void TitleUI() {
            string modelName = Config.TempModelName;
            SpaceWithLabel(Tool.GetCacheString(LabelModelName + modelName));
            WeaponUI(modelName);
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
                Controller.SetAnimationStateData(selectState);
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
            SpaceWithLabel(LabelFrameGroupType);
            m_lastFrameIndex = IntPopup(m_lastFrameIndex, FrameKeyNameArray, FrameKeyNameIndexArray);
            if (IsNoSelectFrameGroup)
                return;
            if (SpaceWithButton(BtnAdd)) {
                switch (FrameGroupKey) {
                    case ClipData.Key_KeyFrame:
                        Controller.AddNewKeyFrameData();
                        break;
                    case ClipData.Key_ProcessFrame:
                        Controller.AddNewProcessFrameData();
                        break;
                }
            }
        }

        private void FrameListUI() {
            if (IsNoSelectFrameGroup)
                return;
            switch (FrameGroupKey) {
                case ClipData.Key_KeyFrame:
                    KeyFrameData[] arrayKeyFrame = LuaAnimClipModel.ClipData.GetKeyFrameList();
                    if (arrayKeyFrame == null || arrayKeyFrame.Length == 0)
                        break;
                    for (int index = 0; index < arrayKeyFrame.Length; index++)
                        KeyFrameUI(index);
                    break;
                case ClipData.Key_ProcessFrame:
                    ProcessFrameData[] arrayProcessFrame = LuaAnimClipModel.ClipData.GetProcessFrameList();
                    if (arrayProcessFrame == null || arrayProcessFrame.Length == 0)
                        break;
                    for (int index = 0; index < arrayProcessFrame.Length; index++)
                        ProcessFrameUI(index);
                    break;
            }
        }

        private void KeyFrameUI(int index) {
            Space();
            KeyFrameData data = (KeyFrameData)HorizontalLayoutUI(KeyFrameDataUI, index);
            if (data.dataList != null)
                CustomDataListUI(data.dataList);
            Controller.SetKeyFrameData(index, data);
        }

        private void ProcessFrameUI(int index) {
            Space();
            ProcessFrameData data = (ProcessFrameData)HorizontalLayoutUI(ProcessFrameDataUI, index);
            if (data.dataList != null)
                CustomDataListUI(data.dataList);
            Controller.SetProcessFrameData(index, data);
        }

        private object KeyFrameDataUI(int index) {
            KeyFrameData data = LuaAnimClipModel.GetKeyFrameData(index);
            SpaceWithLabel(LabelFrameType);
            data.frameType = CheckFrameType((FrameType)EnumPopup(data.frameType), data.frameType);
            SpaceWithLabel(LabelTime);
            data.time = TextField(data.time);
            SpaceWithLabel(LabelPriority);
            data.priority = (short)TextField(data.priority);
            if ((data.frameType == FrameType.PlayEffect || data.frameType == FrameType.Hit) && 
                SpaceWithButton(BtnAdd)) {
                OnAddCustomDataButton(data.frameType, index);
                return LuaAnimClipModel.GetKeyFrameData(index);
            }
            return data;
        }

        private object ProcessFrameDataUI(int index) {
            ProcessFrameData data = LuaAnimClipModel.GetProcessFrameData(index);
            SpaceWithLabel(LabelFrameType);
            data.frameType = CheckFrameType((FrameType)EnumPopup(data.frameType), data.frameType);
            SpaceWithLabel(LabelTime);
            data.time = TextField(data.time);
            SpaceWithLabel(LabelPriority);
            data.priority = (short)TextField(data.priority);
            if ((data.frameType == FrameType.PlayEffect || data.frameType == FrameType.Hit) && 
                SpaceWithButton(BtnAdd)) {
                OnAddCustomDataButton(data.frameType, index);
                return LuaAnimClipModel.GetProcessFrameData(index);
            }
            return data;
        }

        private FrameType CheckFrameType(FrameType newType, FrameType originType) {
            switch (newType) {
                case FrameType.None:
                    return newType;
                case FrameType.Hit:
                    return FrameGroupKey == ClipData.Key_KeyFrame ? newType : originType;
                default:
                    return FrameGroupKey == ClipData.Key_KeyFrame ? originType : newType;
            }
        }

        private void OnAddCustomDataButton(FrameType frameType, int index) {
            switch (frameType) {
                case FrameType.PlayEffect:
                    Controller.AddNewEffectData(index);
                    break;
                case FrameType.Hit:
                    Controller.AddNewCubeData(index);
                    break;
            }
        }

        private void CustomDataListUI(CustomData[] array) {
            for (int index = 0; index < array.Length; index++) {
                CustomData customData = array[index];
                if (customData.data is EffectData) {
                    Space();
                    customData.data = HorizontalLayoutUI(EffectDataUI, customData.data);
                }
                else if (customData.data is CubeData[]) {
                    CubeData[] arrayCubeData = customData.data as CubeData[];
                    if (arrayCubeData.Length == 0)
                        continue;
                    for (int cubeDataIndex = 0; cubeDataIndex < arrayCubeData.Length; cubeDataIndex++) {
                        CubeData cubeData = arrayCubeData[cubeDataIndex];
                        Space();
                        arrayCubeData[cubeDataIndex] = (CubeData)HorizontalLayoutUI(CubeDataUI, cubeData);
                    }
                    customData.data = arrayCubeData;
                }
                array[index] = customData;
            }
        }

        private object EffectDataUI(object data) {
            EffectData effectData = (EffectData)data;
            SpaceWithLabel(LabelEffect);
            SpaceWithLabel(LabelEffectType);
            effectData.type = TextField(effectData.type);
            SpaceWithLabel(LabelEffectID);
            effectData.id = TextField(effectData.id);
            return effectData;
        }

        private object CubeDataUI(object data) {
            CubeData cubeData = (CubeData)data;
            SpaceWithLabel(LabelCollision);
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
            return cubeData;
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

        public static void RefreshRepaint() {
            GetWindow<EditorWindow>().Repaint();
        } 

        private void OnPlayButton() => Controller.Play();

        private void OnPauseButton() => Controller.Pause();

        private void OnStopButton() => Controller.Stop();
    }
}