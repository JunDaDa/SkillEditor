﻿using UnityEngine;
using System.Text;
using System.Collections.Generic;
using Lua;
using Lua.AnimClipData;

namespace SkillEditor {

    internal static class LuaAnimClipModel {

        private static List<AnimClipData> m_listAnimClip = new List<AnimClipData>(Config.ModelCount);
        public static List<AnimClipData> AnimClipList => m_listAnimClip;
        private static int m_curModelIndex;

        public static void SetCurrentEditModelName(string modelName) {
            ModelName = modelName;
            if (m_curModelIndex == Config.ErrorIndex)
                AddNewAnimClipData(modelName);
        }

        public static string ModelName {
            private set {
                m_curModelIndex = Config.ErrorIndex;
                for (int index = 0; index < m_listAnimClip.Count; index++)
                    if (m_listAnimClip[index].modelName == value)
                        m_curModelIndex = index;
            }
            get {
                return GetAnimClipData().modelName;
            }
        }

        private static void AddNewAnimClipData(string modelName) {
            AnimClipData data = new AnimClipData();
            data.modelName = modelName;
            m_listAnimClip.Add(data);
            m_curModelIndex = m_listAnimClip.Count - 1;
        }

        private static AnimClipData GetAnimClipData() {
            if (m_curModelIndex == Config.ErrorIndex)
                Debug.LogError("AnimClipModel current index is error index");
            return m_listAnimClip[m_curModelIndex];
        }

        private static int m_curStateIndex;
        private static StateData m_curStateData;
        public static State ClipDataState {
            set {
                m_curStateData.state = value;
            }
            get => m_curStateData.state;
        }
        private static List<StateData> m_listState = new List<StateData>(2);

        private static int m_curClipIndex;
        private static ClipData m_curClipData;
        public static ClipData ClipData {
            set {
                m_curClipData = value;
                SetCollisionList();
            }
            get => m_curClipData;
        }
        public static List<ClipData> m_listClip = new List<ClipData>(16);

        public static void SetCurrentEditClipName(string clipName) {
            if (clipName != m_curClipData.clipName)
                SaveCurrentClipData();
            ResetClip();
            m_curClipData.clipName = clipName;
            StateData[] stateList = GetAnimClipData().stateList;
            if (stateList == null)
                return;
            for (int stateIndex = 0; stateIndex < stateList.Length; stateIndex++) {
                StateData stateData = stateList[stateIndex];
                m_listState.Add(stateData);
                if (stateData.clipList == null && stateData.clipList.Length == 0)
                    continue;
                for (int clipIndex = 0; clipIndex < stateData.clipList.Length; clipIndex++) {
                    ClipData clipData = stateData.clipList[clipIndex];
                    m_listClip.Add(clipData);
                    if (clipData.clipName == clipName) {
                        m_curStateIndex = stateIndex;
                        m_curStateData = stateData;
                        m_curClipIndex = clipIndex;
                        m_curClipData = clipData;
                        SetCollisionList();
                    }
                }
            }
        }

        private static void SaveCurrentClipData() {
            if (m_curStateData.state == State.None || m_curClipData.IsNullTable())
                return;
            bool isNewData = m_curStateIndex == Config.ErrorIndex && m_curClipIndex == Config.ErrorIndex;
            if (!isNewData) {
                m_curStateData.clipList[m_curClipIndex] = m_curClipData;
                m_listAnimClip[m_curModelIndex].stateList[m_curStateIndex] = m_curStateData;
                return;
            }
            m_listClip.Add(m_curClipData);
            m_curStateData.clipList = m_listClip.ToArray();
            int stateIndex = Config.ErrorIndex;
            for (int index = 0; index < m_listState.Count; index++) {
                if (m_listState[index].state == m_curStateData.state){
                    stateIndex = index;
                    break;
                }
            }
            if (stateIndex != Config.ErrorIndex) {
                m_listAnimClip[m_curModelIndex].stateList[stateIndex] = m_curStateData;
                return;
            }
            m_listState.Add(m_curStateData);
            m_listState.Sort(SortStateList);
            AnimClipData animClipData = GetAnimClipData();
            animClipData.stateList = m_listState.ToArray();
            m_listAnimClip[m_curModelIndex] = animClipData;
        }

        private static int SortStateList(StateData leftData, StateData rightData) {
            return leftData.state.CompareTo(rightData.state);
        }

        public static void AddFrameData() {
            FrameData[] array = ClipData.frameList;
            if (array == null) {
                array = new FrameData[] { new FrameData() };
                m_curClipData.frameList = array;
                return;
            }
            List<FrameData> list = new List<FrameData>(array);
            list.Add(new FrameData());
            m_curClipData.frameList = array;
        }

        public static void SetFrameData(int index, FrameData data, bool isRefresHitFrame) {
            FrameData[] array = ClipData.frameList;
            array[index] = data;
            m_curClipData.frameList = array;
            if (isRefresHitFrame)
                SetCollisionList();
        }

        public static FrameData GetFrameData(int index) {
            FrameData[] list = ClipData.frameList;
            if (list == null && index >= 0 && index < list.Length)
                return default;
            return list[index];
        }

        public static void SetFrameDataTime(int index, float time) {
            FrameData data = GetFrameData(index);
            data.time = time;
            SetFrameData(index, data, false);
        }

        public static void AddNewEffectData(int index) {
            FrameData frameData = GetFrameData(index);
            EffectData[] dataList = frameData.effectFrameData.effectData.dataList;
            EffectData effectData = default;
            if (dataList == null) {
                effectData.index = 1;
                frameData.effectFrameData.priority = 1;
                frameData.effectFrameData.effectData.dataList = new EffectData[] { effectData };
            }
            else {
                List<EffectData> list = new List<EffectData>(dataList);
                effectData.index = (ushort)list.Count;
                list.Add(effectData);
                frameData.effectFrameData.effectData.dataList = list.ToArray();
            }
            SetFrameData(index, frameData, false);
        }

        public static void AddNewCubeData(int index) {
            FrameData frameData = GetFrameData(index);
            CubeData[] dataList = frameData.hitFrameData.cubeData.dataList;
            CubeData cubeData = default;
            if (dataList == null) {
                cubeData.index = 1;
                frameData.hitFrameData.priority = 1;
                frameData.hitFrameData.cubeData.dataList = new CubeData[] { cubeData };
            }
            else {
                List<CubeData> list = new List<CubeData>(dataList);
                cubeData.index = (ushort)list.Count;
                list.Add(cubeData);
                frameData.hitFrameData.cubeData.dataList = list.ToArray();
            }
            SetFrameData(index, frameData, false);
        }

        public static void AddNewCacheData(int index) => AddPriorityFrameData(index, FrameType.CacheBegin);
        public static void AddNewSectionData(int index) => AddPriorityFrameData(index, FrameType.SectionOver);
        private static void AddPriorityFrameData(int index, FrameType frameType) {
            FrameData frameData = GetFrameData(index);
            switch (frameType) {
                case FrameType.CacheBegin:
                    frameData.cacheFrameData.priority = 1;
                    frameData.cacheFrameData.frameType = FrameType.CacheBegin;
                    break;
                case FrameType.SectionOver:
                    frameData.sectionFrameData.priority = 1;
                    frameData.sectionFrameData.frameType = FrameType.SectionOver;
                    break;
            }
            SetFrameData(index, frameData, false);
        }

        private static List<KeyValuePair<float, CubeData[]>> m_listCollision = new List<KeyValuePair<float, CubeData[]>>();
        public static List<KeyValuePair<float, CubeData[]>> ListCollision => m_listCollision;

        private static void SetCollisionList() {
            m_listCollision.Clear();
            if (m_curClipData.frameList == null || m_curClipData.frameList.Length == 0)
                return;
            FrameData[] frameList = m_curClipData.frameList;
            for (int index = 0; index < frameList.Length; index++) {
                FrameData frameData = frameList[index];
                CubeData[] dataList = frameData.hitFrameData.cubeData.dataList;
                if (dataList == null || dataList.Length == 0)
                    continue;
                float time = frameData.time;
                KeyValuePair<float, CubeData[]> timeCubeData = new KeyValuePair<float, CubeData[]>(time, dataList);
                m_listCollision.Add(timeCubeData);
            }
            m_listCollision.Sort(SortCollisionList);
        }

        private static int SortCollisionList(KeyValuePair<float, CubeData[]> left, KeyValuePair<float, CubeData[]> right) {
            return left.Key.CompareTo(right.Key);
        }

        public static string GetWriteFileString(StringBuilder builder) {
            SaveCurrentClipData();
            builder.Append(LuaFormat.CurlyBracesPair.start);
            if (m_listAnimClip != null && m_listAnimClip.Count != 0) {
                builder.Append(LuaFormat.LineSymbol);
                for (int index = 0; index < m_listAnimClip.Count; index++)
                    builder.Append(m_listAnimClip[index].ToString());
            }
            builder.Append(LuaFormat.CurlyBracesPair.end);
            return builder.ToString();
        }

        public static void Reset() {
            ResetModel();
            ResetClip();
        }

        private static void ResetModel() {
            m_curModelIndex = Config.ErrorIndex;
            m_listAnimClip.Clear();
        }

        private static void ResetClip(){
            m_curStateIndex = Config.ErrorIndex;
            m_curStateData.Clear();
            m_listState.Clear();
            m_curClipIndex = Config.ErrorIndex;
            m_curClipData.Clear();
            m_listClip.Clear();
            m_listCollision.Clear();
        }
    }
}